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
            AddItem(new PlateArms());
            AddItem(new PlateChest());
            AddItem(new PlateGloves());
            AddItem(new StuddedGorget());
            AddItem(new PlateLegs());

            switch (Utility.Random(4))
            {
                case 0:
                    AddItem(new PlateHelm());
                    break;
                case 1:
                    AddItem(new NorseHelm());
                    break;
                case 2:
                    AddItem(new CloseHelm());
                    break;
                case 3:
                    AddItem(new Helmet());
                    break;
            }

            switch (Utility.Random(3))
            {
                case 0:
                    AddItem(new BodySash(0x482));
                    break;
                case 1:
                    AddItem(new Doublet(0x482));
                    break;
                case 2:
                    AddItem(new Tunic(0x482));
                    break;
            }

            AddItem(new Broadsword());

            Item shield = new MetalKiteShield
            {
                Hue = Utility.RandomNondyedHue()
            };

            AddItem(shield);

            switch (Utility.Random(2))
            {
                case 0:
                    AddItem(new Boots());
                    break;
                case 1:
                    AddItem(new ThighBoots());
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
