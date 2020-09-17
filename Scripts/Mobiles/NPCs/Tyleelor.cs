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
            AddItem(new ElvenBoots(0x1BB));

            Item item;

            item = new WoodlandLegs
            {
                Hue = 0x236
            };
            AddItem(item);

            item = new WoodlandChest
            {
                Hue = 0x236
            };
            AddItem(item);

            item = new WoodlandArms
            {
                Hue = 0x236
            };
            AddItem(item);

            item = new WoodlandBelt
            {
                Hue = 0x237
            };
            AddItem(item);

            item = new VultureHelm
            {
                Hue = 0x236
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