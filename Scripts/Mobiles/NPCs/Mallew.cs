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
            AddItem(new ElvenBoots(0x1BB));
            AddItem(new Circlet());
            AddItem(new Cloak(0x3B2));

            Item item;

            item = new LeafChest
            {
                Hue = 0x53E
            };
            AddItem(item);

            item = new LeafArms
            {
                Hue = 0x53E
            };
            AddItem(item);

            item = new LeafTonlet
            {
                Hue = 0x53E
            };
            AddItem(item);
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