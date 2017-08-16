using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    [TypeAlias("drNO.ThieveItems.SeedOflife")]
    public class SeedOfLife : Item
    {
        private static Dictionary<PlayerMobile, DateTime> SeedUsageList = new Dictionary<PlayerMobile, DateTime>();
        private static TimeSpan Cooldown = TimeSpan.FromMinutes(10);

        public static void Initialize()
        {
            EventSink.AfterWorldSave += CheckCleanup;
        }

        public override int LabelNumber { get { return 1094937; } } // seed of life

        [Constructable]
        public SeedOfLife()
            : base(0x1727)
        {
            Hue = 0x491;
            Weight = 1.0;
            Stackable = true;
            LootType = LootType.Cursed; 
        }

        public static void CheckCleanup(AfterWorldSaveEventArgs e)
        {
            DoCleanup();
            ManaDraught.DoCleanup();
            GemOfSalvation.DoCleanup();
        }

        public static void DoCleanup()
        {
            List<PlayerMobile> toRemove = new List<PlayerMobile>();

            foreach (PlayerMobile pm in SeedUsageList.Keys)
            {
                if (SeedUsageList[pm] != null)
                {
                    if (SeedUsageList[pm] < DateTime.Now + Cooldown)
                    {
                        toRemove.Add(pm);
                    }
                }
            }

            foreach (PlayerMobile pm in toRemove)
            {
                SeedUsageList.Remove(pm);
            }

            toRemove.Clear();

        }

        private bool CheckUse(PlayerMobile pm)
        {
            if (SeedUsageList.ContainsKey(pm))
            {
                if (SeedUsageList[pm] + Cooldown >= DateTime.Now)
                    return false;
                else
                    return true;
            }
            else
            {
                return true;
            }
        }

        private void OnUsed(PlayerMobile by)
        {
            if (CheckUse(by))
            {
                DoHeal(by);
            }
            else
            {
                if (SeedUsageList[by] != null)
                {
                    by.SendLocalizedMessage(1079263,((int)(((SeedUsageList[by] + Cooldown)-DateTime.Now).TotalSeconds)).ToString()); 
                }
            }
        }

        private void DoHeal(PlayerMobile pm)
        {
            int toHeal = Utility.RandomMinMax(25, 40);

            int diff = pm.HitsMax - pm.Hits;
            if (diff == 0)
            {
                pm.SendLocalizedMessage(1049547); //You are already at full health 
                return; 
            }
            toHeal = Math.Min(toHeal, diff);
            
            pm.Hits += toHeal;
            this.Consume();

            if (!SeedUsageList.ContainsKey(pm))
            {
                SeedUsageList.Add(pm, DateTime.Now);
            }
            else
            {
                SeedUsageList[pm] = DateTime.Now; 
            }
            pm.SendLocalizedMessage(1095126);//The bitter seed instantly restores some of your health!
        }

        public SeedOfLife(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
