using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Olaeni : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Olaeni()
            : base("the thaumaturgist")
        {
            Name = "Olaeni";
        }

        public Olaeni(Serial serial)
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

            Hue = 0x851D;
            HairItemID = 0x2FCF;
            HairHue = 0x322;
        }

        public override void InitOutfit()
        {
            AddItem(new Shoes(0x736));
            AddItem(new FemaleElvenRobe(0x1C));
            AddItem(new GemmedCirclet());
            AddItem(new Item(0xDF2));
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