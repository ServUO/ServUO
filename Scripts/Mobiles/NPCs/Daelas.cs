using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Daelas : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Daelas()
            : base("the aborist")
        {
            Name = "Daelas";
        }

        public Daelas(Serial serial)
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

            Hue = 0x84DE;
            HairItemID = 0x2FCF;
            HairHue = 0x8F;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x901));
            AddItem(new ElvenPants(0x8AB));

            Item item;

            item = new LeafGloves
            {
                Hue = 0x1BB
            };
            AddItem(item);

            item = new LeafChest
            {
                Hue = 0x8B0
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