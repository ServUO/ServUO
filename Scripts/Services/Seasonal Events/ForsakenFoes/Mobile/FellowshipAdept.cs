using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Fellowship
{
    public class FellowshipAdept : BaseVendor
    {
        public override bool IsActiveVendor => false;
        public override bool IsInvulnerable => true;
        public override bool DisallowAllMoves => true;
        public override bool ClickTitle => true;
        public override bool CanTeach => false;

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo() { }

        public static FellowshipAdept InstanceTram { get; set; }
        public static FellowshipAdept InstanceFel { get; set; }

        [Constructable]
        public FellowshipAdept()
            : base("the Fellowship Adept")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            Hue = Utility.RandomSkinHue();
            Body = 0x190;
            HairItemID = 0x2044;
            HairHue = 1644;
            FacialHairItemID = 0x203F;
            FacialHairHue = 1644;
        }

        public override void InitOutfit()
        {
            SetWearable(new Kamishimo());
            SetWearable(new Sandals());
            SetWearable(new GoldRing());

            if (Backpack == null)
            {
                Item backpack = new Backpack
                {
                    Movable = false
                };

                AddItem(backpack);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1159182); // Fellowship Shop
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {

            if (from.Alive)
            {
                list.Add(new BrowseShopEntry(from, this));
            }

            base.AddCustomContextEntries(from, list);
        }

        private class BrowseShopEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly BaseVendor m_Vendor;

            public BrowseShopEntry(Mobile from, BaseVendor vendor)
                : base(1159181, 2) // Browse the Fellowship Shop
            {
                Enabled = vendor.CheckVendorAccess(from);

                m_From = from;
                m_Vendor = vendor;
            }

            public override void OnClick()
            {
                if (!m_From.InRange(m_Vendor.Location, 5) || !(m_From is PlayerMobile))
                    return;

                m_From.SendGump(new FellowshipRewardGump(m_Vendor, m_From as PlayerMobile));
            }
        }

        public FellowshipAdept(Serial serial)
            : base(serial)
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

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
