using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class KeeperOfChivalry : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public KeeperOfChivalry()
            : base("the Keeper of Chivalry")
        {
            this.SetSkill(SkillName.Fencing, 75.0, 85.0);
            this.SetSkill(SkillName.Macing, 75.0, 85.0);
            this.SetSkill(SkillName.Swords, 75.0, 85.0);
            this.SetSkill(SkillName.Chivalry, 100.0);
        }

        public KeeperOfChivalry(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo()
        {
            this.m_SBInfos.Add(new SBKeeperOfChivalry());
        }

        public override void InitOutfit()
        {
            this.AddItem(new PlateArms());
            this.AddItem(new PlateChest());
            this.AddItem(new PlateGloves());
            this.AddItem(new StuddedGorget());
            this.AddItem(new PlateLegs());

            switch ( Utility.Random(4) )
            {
                case 0:
                    this.AddItem(new PlateHelm());
                    break;
                case 1:
                    this.AddItem(new NorseHelm());
                    break;
                case 2:
                    this.AddItem(new CloseHelm());
                    break;
                case 3:
                    this.AddItem(new Helmet());
                    break;
            }

            switch ( Utility.Random(3) )
            {
                case 0:
                    this.AddItem(new BodySash(0x482));
                    break;
                case 1:
                    this.AddItem(new Doublet(0x482));
                    break;
                case 2:
                    this.AddItem(new Tunic(0x482));
                    break;
            }

            this.AddItem(new Broadsword());

            Item shield = new MetalKiteShield();

            shield.Hue = Utility.RandomNondyedHue();

            this.AddItem(shield);

            switch ( Utility.Random(2) )
            {
                case 0:
                    this.AddItem(new Boots());
                    break;
                case 1:
                    this.AddItem(new ThighBoots());
                    break;
            }

            this.PackGold(100, 200);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}