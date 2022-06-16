using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Athialon : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Athialon()
            : base("the expeditionist")
        {
            Name = "Athialon";
        }

        public Athialon(Serial serial)
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

            Hue = 0x8382;
            HairItemID = 0x2FC0;
            HairHue = 0x35;
        }

        public override void InitOutfit()
        {
            SetWearable(new ElvenBoots(), 0x901, 1);
            SetWearable(new DiamondMace(), dropChance: 1);
            SetWearable(new WoodlandBelt(), dropChance: 1);
			SetWearable(new WoodlandLegs(), 0x3B2, 1);
			SetWearable(new WoodlandChest(), 0x3B2, 1);
			SetWearable(new WoodlandArms(), 0x3B2, 1); 
			SetWearable(new WingedHelm(), 0x3B2, 1); 
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