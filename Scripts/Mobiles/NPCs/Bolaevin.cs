using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Bolaevin : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Bolaevin()
            : base("the arcanist")
        {
            Name = "Bolaevin";
        }

        public Bolaevin(Serial serial)
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
            HairItemID = 0x2FC0;
            HairHue = 0x36;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x3B3));
            AddItem(new RoyalCirclet());
            AddItem(new LeafChest());
            AddItem(new LeafArms());

            Item item;

            item = new LeafLegs
            {
                Hue = 0x1BB
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