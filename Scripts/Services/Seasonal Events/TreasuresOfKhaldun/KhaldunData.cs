using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.SeasonalEvents;

namespace Server.Engines.Points
{
    public class KhaldunData : PointsSystem
    {
        public override PointsType Loyalty { get { return PointsType.Khaldun; } }
        public override TextDefinition Name { get { return m_Name; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return double.MaxValue; } }
        public override bool ShowOnLoyaltyGump { get { return false; } }

        public bool InSeason { get { return SeasonalEventSystem.IsActive(EventType.TreasuresOfKhaldun); } }

        public bool Enabled { get; set; }
        public bool QuestContentGenerated { get; set; }

        private TextDefinition m_Name = null;

        public KhaldunData()
        {
            DungeonPoints = new Dictionary<Mobile, int>();
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1158674, ((int)points).ToString()); // You have turned in ~1_COUNT~ artifacts of the Cult         
        }

        public override void ProcessKill(Mobile victim, Mobile damager)
        {
            var bc = victim as BaseCreature;

            if (!InSeason || bc == null || bc.Controlled || bc.Summoned || !damager.Alive || damager.Deleted || !bc.IsChampionSpawn)
                return;

            Region r = bc.Region;

            if (damager is PlayerMobile && r.IsPartOf("Khaldun"))
            {
                if (!DungeonPoints.ContainsKey(damager))
                    DungeonPoints[damager] = 0;

                int fame = bc.Fame / 4;
                int luck = Math.Max(0, ((PlayerMobile)damager).RealLuck);

                DungeonPoints[damager] += (int)(fame * (1 + Math.Sqrt(luck) / 100)) * PotionOfGloriousFortune.GetBonus(damager);

                int x = DungeonPoints[damager];
                const double A = 0.000863316841;
                const double B = 0.00000425531915;

                double chance = A * Math.Pow(10, B * x);

                if (chance > Utility.RandomDouble())
                {
                    Item i = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(bc), LootPackEntry.IsMondain(bc), LootPackEntry.IsStygian(bc));

                    if (i != null)
                    {
                        RunicReforging.GenerateRandomItem(i, damager, Math.Max(100, RunicReforging.GetDifficultyFor(bc)), RunicReforging.GetLuckForKiller(bc), ReforgedPrefix.None, ReforgedSuffix.Khaldun);

                        damager.PlaySound(0x5B4);
                        damager.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.

                        if (!damager.PlaceInBackpack(i))
                        {
                            if (damager.BankBox != null && damager.BankBox.TryDropItem(damager, i, false))
                                damager.SendLocalizedMessage(1079730); // The item has been placed into your bank box.
                            else
                            {
                                damager.SendLocalizedMessage(1072523); // You find an artifact, but your backpack and bank are too full to hold it.
                                i.MoveToWorld(damager.Location, damager.Map);
                            }
                        }

                        DungeonPoints.Remove(damager);
                    }
                }
            }
        }

        public Dictionary<Mobile, int> DungeonPoints { get; set; }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);

            KhaldunTastyTreat.Save(writer);
            PotionOfGloriousFortune.Save(writer);

            writer.Write(Enabled);
            writer.Write(QuestContentGenerated);

            writer.Write(DungeonPoints.Count);
            foreach (KeyValuePair<Mobile, int> kvp in DungeonPoints)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    KhaldunTastyTreat.Load(reader);
                    PotionOfGloriousFortune.Load(reader);
                    goto case 1;
                case 1:
                    Enabled = reader.ReadBool();
                    QuestContentGenerated = reader.ReadBool();
                    goto case 0;
                case 0:
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Mobile m = reader.ReadMobile();
                        int points = reader.ReadInt();

                        if (m != null && points > 0)
                            DungeonPoints[m] = points;
                    }
                    break;
            }
        }
    }
}
