using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Alethanian : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Alethanian()
            : base("the wise")
        {
            Name = "Elder Alethanian";
        }

        public Alethanian(Serial serial)
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

            Hue = 0x876C;
            HairItemID = 0x2FC2;
            HairHue = 0x368;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new GemmedCirclet());
            AddItem(new HidePants());
            AddItem(new HideFemaleChest());
            AddItem(new HidePauldrons());
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