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
            Name = "Elder Vicaie";
        }

        public Vicaie(Serial serial)
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

            Hue = 0x8362;
            HairItemID = 0x2FCD;
            HairHue = 0x90;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new Tunic(0x1FA1));

            Item item;

            item = new LeafLegs
            {
                Hue = 0x3B3
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