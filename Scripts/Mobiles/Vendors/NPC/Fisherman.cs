using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Fisherman : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Fisherman()
            : base("the fisher")
        {
            this.SetSkill(SkillName.Fishing, 75.0, 98.0);
        }

        public Fisherman(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.FishermensGuild;
            }
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
            this.m_SBInfos.Add(new SBFisherman());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.FishingPole());
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