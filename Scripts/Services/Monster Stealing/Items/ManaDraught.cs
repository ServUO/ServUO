using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    [TypeAlias("drNO.ThieveItems.ManaDraught")]
    public class ManaDraught : Item
    {
        private static Dictionary<PlayerMobile, DateTime> DaughtUsageList = new Dictionary<PlayerMobile, DateTime>();
        private static TimeSpan Cooldown = TimeSpan.FromMinutes(10);

        public override int LabelNumber { get { return 1094938; } } // Mana Draught

        [Constructable] 
        public ManaDraught()
            : base(0xFFB)
        {
            Hue = 0x48A;
            Weight = 1.0;
        }

        public static void DoCleanup()
        {
            List<PlayerMobile> toRemove = new List<PlayerMobile>();

            foreach (PlayerMobile pm in DaughtUsageList.Keys)
            {
                if (DaughtUsageList[pm] != null)
                {
                    if (DaughtUsageList[pm] < DateTime.Now + Cooldown)
                    {
                        toRemove.Add(pm);
                    }
                }
            }

            foreach (PlayerMobile pm in toRemove)
            {
                DaughtUsageList.Remove(pm);
            }

            toRemove.Clear();
        }

        private bool CheckUse(PlayerMobile pm)
        {
            if (DaughtUsageList.ContainsKey(pm))
            {
                if (DaughtUsageList[pm] + Cooldown >= DateTime.Now)
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
                if (DaughtUsageList[by] != null)
                {
                    by.SendLocalizedMessage(1079263, ((int)((DaughtUsageList[by] + Cooldown)-DateTime.Now).TotalSeconds).ToString());
                }
            }
        }

        private void DoHeal(PlayerMobile pm)
        {
            int toHeal = Utility.RandomMinMax(25, 40);

            int diff = pm.ManaMax - pm.Mana;
            if (diff == 0)
            {
                pm.SendLocalizedMessage(1095127); //You are already at full mana 
                return;
            }
            toHeal = Math.Min(toHeal, diff);

            pm.Mana += toHeal;
            this.Consume();
            if (!DaughtUsageList.ContainsKey(pm))
            {
                DaughtUsageList.Add(pm, DateTime.Now);
            }
            else
            {
                DaughtUsageList[pm] = DateTime.Now;
            }
            
            pm.SendLocalizedMessage(1095128);//The sour draught instantly restores some of your mana!
        }

        public ManaDraught(Serial serial)
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
