using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Vicaie : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Vicaie()
            : base("the wise")
        {
            this.Name = "Elder Vicaie";
        }

        public Vicaie(Serial serial)
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

            this.Hue = 0x8362;
            this.HairItemID = 0x2FCD;
            this.HairHue = 0x90;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots());
            this.AddItem(new Tunic(0x1FA1));

            Item item;

            item = new LeafLegs();
            item.Hue = 0x3B3;
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