using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Tyleelor : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Tyleelor()
            : base("the expeditionist")
        {
            Name = "Tyeelor";
        }

        public Tyleelor(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach => false;
        public override bool IsInvulnerable => true;
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x8367;
            HairItemID = 0x2FC1;
            HairHue = 0x38;
        }

        public override void InitOutfit()
        {
			SetWearable(new ElvenBoots(), 0x1BB, 1);
			SetWearable(new WoodlandLegs(), 0x236, 1);
			SetWearable(new WoodlandChest(), 0x236, 1);
			SetWearable(new WoodlandArms(), 0x236, 1);
			SetWearable(new WoodlandBelt(), 0x237, 1);
			SetWearable(new VultureHelm(), 0x236, 1);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}