using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Rebinil : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Rebinil()
            : base("the healer")
        {
            Name = "Rebinil";
        }

        public Rebinil(Serial serial)
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

            Hue = 0x83E7;
            HairItemID = 0x2FD0;
            HairHue = 0x26B;
        }

        public override void InitOutfit()
        {
            AddItem(new Sandals(0x719));
            AddItem(new FemaleElvenRobe(0x757));
            AddItem(new RoyalCirclet());
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