using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Alelle : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Alelle()
            : base("the aborist")
        {
            Name = "Alelle";
        }

        public Alelle(Serial serial)
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

            Female = true;
            Race = Race.Elf;

            Hue = 0x8374;
            HairItemID = 0x2FCC;
            HairHue = 0x238;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x1BB));

            Item item;

            item = new LeafGloves
            {
                Hue = 0x1BB
            };
            AddItem(item);

            item = new LeafChest
            {
                Hue = 0x37
            };
            AddItem(item);

            item = new LeafLegs
            {
                Hue = 0x746
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