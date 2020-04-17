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
            this.Name = "Alelle";
        }

        public Alelle(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach => false;
        public override bool IsInvulnerable => true;
        protected override List<SBInfo> SBInfos => this.m_SBInfos;
        public override void InitSBInfo()
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = true;
            this.Race = Race.Elf;

            this.Hue = 0x8374;
            this.HairItemID = 0x2FCC;
            this.HairHue = 0x238;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x1BB));

            Item item;

            item = new LeafGloves();
            item.Hue = 0x1BB;
            this.AddItem(item);

            item = new LeafChest();
            item.Hue = 0x37;
            this.AddItem(item);

            item = new LeafLegs();
            item.Hue = 0x746;
            this.AddItem(item);
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