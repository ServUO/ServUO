using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using System.Collections.Generic;

namespace Server.Engines.VvV
{
    public class SilverTrader : BaseVendor
    {
        public override bool IsActiveVendor { get { return false; } }
        public override bool DisallowAllMoves { get { return true; } }
        public override bool ClickTitle { get { return true; } }
        public override bool CanTeach { get { return false; } }

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return this.m_SBInfos; } }
        public override void InitSBInfo() { }

        [Constructable]
        public SilverTrader() : base("the Silver Trader")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
        }

        public override void InitOutfit()
        {
            Robe robe = new Robe();
            robe.ItemID = 0x2684;
            robe.Name = "a robe";

            SetWearable(robe, 1109);

            Timer.DelayCall(TimeSpan.FromSeconds(10), StockInventory);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1155513); // Vice vs Virtue Reward Vendor
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && InRange(m.Location, 3))
            {
                if (ViceVsVirtueSystem.IsVvV(m))
                {
                    m.SendGump(new VvVRewardGump(this, (PlayerMobile)m));
                }
                else
                {
                    SayTo(m, 1155585); // You have no silver to trade with. Join Vice vs Virtue and return to me.
                }
            }
        }

        public void StockInventory()
        {
            if (Backpack == null)
                AddItem(new Backpack());

            foreach (CollectionItem item in VvVRewards.Rewards)
            {
                if (item.Tooltip == 0)
                {
                    if (Backpack.GetAmount(item.Type) > 0)
                    {
                        Item itm = Backpack.FindItemByType(item.Type);

                        if (itm is IVvVItem)
                            ((IVvVItem)itm).IsVvVItem = true;

                        continue;
                    }

                    Item i = Activator.CreateInstance(item.Type) as Item;

                    if (i != null)
                    {
                        Backpack.DropItem(i);

                        if (i is IOwnerRestricted)
                            ((IOwnerRestricted)i).OwnerName = "Your Player Name";

                        if (i is IVvVItem)
                            ((IVvVItem)i).IsVvVItem = true;

                        if (i is VesperOrderShield)
                        {
                            i.Name = "Order Shield";
                            ((VesperOrderShield)i).Attributes.CastSpeed = 0;
                        }

                        NegativeAttributes neg = RunicReforging.GetNegativeAttributes(i);

                        if (neg != null)
                        {
                            neg.Antique = 1;
                        }
                    }
                }
            }
        }

        public SilverTrader(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Timer.DelayCall(TimeSpan.FromSeconds(5), StockInventory);
        }
    }
}