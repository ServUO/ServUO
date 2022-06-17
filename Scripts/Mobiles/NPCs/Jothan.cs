using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Jothan : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Jothan()
            : base("the wise")
        {
            Name = "Elder Jothan";
        }

        public Jothan(Serial serial)
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

            Hue = 0x8579;
            HairItemID = 0x2FC2;
            HairHue = 0x2C2;
        }

        public override void InitOutfit()
        {
            SetWearable(new ThighBoots(), dropChance: 1);
            SetWearable(new ElvenPants(), 0x57A, 1);
            SetWearable(new ElvenShirt(), 0x711, 1);
            SetWearable(new Cloak(), 0x21, 1);
            SetWearable(new Circlet(), dropChance: 1);
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