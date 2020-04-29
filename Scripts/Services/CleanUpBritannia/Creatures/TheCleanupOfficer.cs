using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.CleanUpBritannia
{
    public class TheCleanupOfficer : BaseVendor
    {
        public override bool IsActiveVendor => false;
        public override bool IsInvulnerable => true;
        public override bool DisallowAllMoves => true;
        public override bool ClickTitle => true;
        public override bool CanTeach => false;

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo() { }

        [Constructable]
        public TheCleanupOfficer()
            : base("the Cleanup Officer")
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
            SetWearable(new Cloak(), 337);
            SetWearable(new ThighBoots());
            SetWearable(new LongPants(), 1409);
            SetWearable(new Doublet(), 50);
            SetWearable(new FancyShirt(), 1644);
            SetWearable(new Necklace());
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1151317); // Clean Up Britannia Reward Trader
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile && from.InRange(Location, 5))
                from.SendGump(new CleanUpBritanniaRewardGump(this, from as PlayerMobile));
        }

        public TheCleanupOfficer(Serial serial)
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
        }
    }
}
