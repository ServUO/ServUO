using Server.Engines.Points;
using Server.Items;
using Server.Mobiles;

using System;
using System.Collections.Generic;

namespace Server.Engines.SorcerersDungeon
{
    public class SorcerersDungeonData : PointsSystem
    {
        public override PointsType Loyalty => PointsType.SorcerersDungeon;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;
        public override bool ShowOnLoyaltyGump => false;

        private readonly TextDefinition m_Name = null;

        public SorcerersDungeonData()
        {
            DungeonPoints = new Dictionary<Mobile, int>();
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1157615, ((int)points).ToString()); // You have turned in ~1_COUNT~ artifacts of Enchanted Origin
        }

        public override void ProcessKill(Mobile victim, Mobile damager)
        {
            BaseCreature bc = victim as BaseCreature;

            if (bc == null)
                return;

            if (TOSDSpawner.Instance != null)
            {
                TOSDSpawner.Instance.OnCreatureDeath(bc);
            }

            if (!SorcerersDungeonEvent.Instance.Running || bc.Controlled || bc.Summoned || !damager.Alive)
                return;

            Region r = bc.Region;

            if (damager is PlayerMobile && r.IsPartOf("Sorcerer's Dungeon"))
            {
                if (!DungeonPoints.ContainsKey(damager))
                    DungeonPoints[damager] = 0;

                int fame = bc.Fame / 4;
                int luck = Math.Max(0, ((PlayerMobile)damager).RealLuck);

                DungeonPoints[damager] += (int)(fame * (1 + Math.Sqrt(luck) / 100));

                int x = DungeonPoints[damager];
                const double A = 0.000863316841;
                const double B = 0.00000425531915;

                double chance = A * Math.Pow(10, B * x);

                if (chance > Utility.RandomDouble())
                {
                    Item i = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(bc), LootPackEntry.IsMondain(bc), LootPackEntry.IsStygian(bc));

                    if (i != null)
                    {
                        RunicReforging.GenerateRandomItem(i, damager, Math.Max(100, RunicReforging.GetDifficultyFor(bc)), RunicReforging.GetLuckForKiller(bc), ReforgedPrefix.None, ReforgedSuffix.EnchantedOrigin);

                        damager.PlaySound(0x5B4);
                        damager.SendLocalizedMessage(1157613); // You notice some of your fallen foes' equipment to be of enchanted origin and decide it may be of some value...

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

        public Dictionary<Mobile, int> DungeonPoints { get; }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            if (TOSDSpawner.Instance != null)
            {
                writer.Write(0);

                TOSDSpawner.Instance.Serialize(writer);
            }
            else
            {
                writer.Write(1);
            }

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

            if (version == 0)
            {
                reader.ReadBool();
            }

            if (reader.ReadInt() == 0)
            {
                TOSDSpawner spawner = new TOSDSpawner();
                spawner.Deserialize(reader);
            }

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadMobile();
                int points = reader.ReadInt();

                if (m != null && points > 0)
                    DungeonPoints[m] = points;
            }
        }
    }
}
