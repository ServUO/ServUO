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
            SetSkill(SkillName.Mining, 65.0, 88.0);
        }

        public Miner(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBMiner());
        }

        public override void InitOutfit()
        {
            AddItem(new Items.FancyShirt(0x3E4));
            AddItem(new Items.LongPants(0x192));
            AddItem(new Items.Pickaxe());
            AddItem(new Items.ThighBoots(0x283));

            base.InitOutfit();
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