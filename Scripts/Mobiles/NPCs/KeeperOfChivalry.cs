using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class KeeperOfChivalry : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public KeeperOfChivalry()
            : base("the Keeper of Chivalry")
        {
            SetSkill(SkillName.Fencing, 75.0, 85.0);
            SetSkill(SkillName.Macing, 75.0, 85.0);
            SetSkill(SkillName.Swords, 75.0, 85.0);
            SetSkill(SkillName.Chivalry, 100.0);
        }

        public KeeperOfChivalry(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBKeeperOfChivalry());
        }

        public override void InitOutfit()
        {
            SetWearable(new PlateArms(), dropChance: 1);
            SetWearable(new PlateChest(), dropChance: 1);
            SetWearable(new PlateGloves(), dropChance: 1);
            SetWearable(new StuddedGorget(), dropChance: 1);
			SetWearable(new PlateLegs(), dropChance: 1);

            switch (Utility.Random(4))
            {
                case 0:
					SetWearable(new PlateHelm(), dropChance: 1);
                    break;
                case 1:
					SetWearable(new NorseHelm(), dropChance: 1);
                    break;
                case 2:
					SetWearable(new CloseHelm(), dropChance: 1);
                    break;
                case 3:
					SetWearable(new Helmet(), dropChance: 1);
                    break;
            }

            switch (Utility.Random(3))
            {
                case 0:
					SetWearable(new BodySash(), 0x482, 1);
                    break;
                case 1:
					SetWearable(new Doublet(), 0x482, 1);
                    break;
                case 2:
					SetWearable(new Tunic(), 0x482, 1);
                    break;
            }

			SetWearable(new Broadsword(), dropChance: 1);
			SetWearable(new MetalKiteShield(), Utility.RandomNondyedHue(), 1);

            switch (Utility.Random(2))
            {
                case 0:
					SetWearable(new Boots(), dropChance: 1);
                    break;
                case 1:
					SetWearable(new ThighBoots(), dropChance: 1);
                    break;
            }
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
