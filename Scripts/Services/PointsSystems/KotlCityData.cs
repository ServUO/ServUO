using System;
using Server;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Engines.TreasuresOfKotlCity;

namespace Server.Engines.Points
{
    public class KotlCityData : PointsSystem
    {
        public override PointsType Loyalty { get { return PointsType.TreasuresOfKotlCity; } }
        public override TextDefinition Name { get { return m_Name; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return double.MaxValue; } }
        public override bool ShowOnLoyaltyGump { get { return false; } }

        private TextDefinition m_Name = null;

        public bool Enabled { get; set; }

        public KotlCityData()
        {
            DungeonPoints = new Dictionary<Mobile, int>();
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1156902, ((int)points).ToString()); // You have turned in ~1_COUNT~ artifacts of the Kotl
        }

        public override void ProcessKill(BaseCreature victim, Mobile damager, int index)
        {
            if (!Enabled || victim.Controlled || victim.Summoned)
                return;
                
            Region r = victim.Region;

            if (damager is PlayerMobile && r.IsPartOf("KotlCity"))
            {
                if (!DungeonPoints.ContainsKey(damager))
                    DungeonPoints[damager] = 0;

                int fame = victim.Fame / 2;
                int luck = Math.Max(0, ((PlayerMobile)damager).RealLuck);

                if (victim.Spawner is KotlBattleSimulator)
                {
                    fame *= 2;
                }

                DungeonPoints[damager] += (int)(fame * (1 + Math.Sqrt(luck) / 100));

                int x = DungeonPoints[damager];
                const double A = 0.000863316841;
                const double B = 0.00000425531915;

                double chance = A * Math.Pow(10, B * x);

                if (chance > Utility.RandomDouble())
                {
                    Item i = Loot.RandomArmorOrShieldOrWeaponOrJewelry(LootPackEntry.IsInTokuno(victim), LootPackEntry.IsMondain(victim), LootPackEntry.IsStygian(victim));

                    if (i != null)
                    {
                        RunicReforging.GenerateRandomItem(i, damager, Math.Max(100, RunicReforging.GetDifficultyFor(victim)), RunicReforging.GetLuckForKiller(victim), ReforgedPrefix.None, ReforgedSuffix.Kotl);

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
            writer.Write(0);

            writer.Write(Enabled);

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

            Enabled = reader.ReadBool();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadMobile();
                int points = reader.ReadInt();

                if (m != null && points > 0)
                    DungeonPoints[m] = points;
            }
        }

        public static void Initialize()
        {
            if (Core.TOL)
            {
                CommandSystem.Register("EnableTKC", AccessLevel.Administrator, PointsSystem.TreasuresOfKotlCity.Enable);
                CommandSystem.Register("DisableTKC", AccessLevel.Administrator, PointsSystem.TreasuresOfKotlCity.Disable);
                CommandSystem.Register("ToggleTKC", AccessLevel.Administrator, PointsSystem.TreasuresOfKotlCity.Toggle);
            }
        }

        public void Enable(CommandEventArgs e)
        {
            if (!Enabled)
            {
                Enabled = true;
                e.Mobile.SendMessage("Treasures of Kotl City enabled!");
            }
            else
            {
                e.Mobile.SendMessage("Treasures of Kotl City is already enabled!");
            }
        }

        public void Disable(CommandEventArgs e)
        {
            if (Enabled)
            {
                Enabled = false;
                e.Mobile.SendMessage("Treasures of Kotl City disabled!");
            }
            else
            {
                e.Mobile.SendMessage("Treasures of Kotl City is already disabled!");
            }
        }

        public void Toggle(CommandEventArgs e)
        {
            if (Enabled)
            {
                Enabled = false;
                e.Mobile.SendMessage("Treasures of Kotl City disabled!");
            }
            else
            {
                Enabled = true;
                e.Mobile.SendMessage("Treasures of Kotl City enabled!");
            }
        }
    }
}
