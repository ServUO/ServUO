using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Mallew : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Mallew()
            : base("the wise")
        {
            Name = "Elder Mallew";
        }

        public Mallew(Serial serial)
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

            Hue = 0x876C;
            HairItemID = 0x2FD1;
            HairHue = 0x31E;
        }

        public override void InitOutfit()
        {
            SetWearable(new ElvenBoots(), 0x1BB, 1);
            SetWearable(new Circlet(), dropChance: 1);
            SetWearable(new Cloak(), 0x3B2, 1);
            SetWearable(new LeafChest(), 0x53E, 1);
            SetWearable(new LeafArms(), 0x53E, 1);
			SetWearable(new LeafTonlet(), 0x53E, 1);
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