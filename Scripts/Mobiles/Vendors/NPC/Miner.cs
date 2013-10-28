using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Miner : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Miner()
            : base("the miner")
        {
            this.SetSkill(SkillName.Mining, 65.0, 88.0);
        }

        public Miner(Serial serial)
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
            this.m_SBInfos.Add(new SBMiner());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.FancyShirt(0x3E4));
            this.AddItem(new Server.Items.LongPants(0x192));
            this.AddItem(new Server.Items.Pickaxe());
            this.AddItem(new Server.Items.ThighBoots(0x283));
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