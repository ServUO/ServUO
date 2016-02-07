using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Thief : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Thief()
            : base("the thief")
        {
            this.SetSkill(SkillName.Camping, 55.0, 78.0);
            this.SetSkill(SkillName.DetectHidden, 65.0, 88.0);
            this.SetSkill(SkillName.Hiding, 45.0, 68.0);
            this.SetSkill(SkillName.Archery, 65.0, 88.0);
            this.SetSkill(SkillName.Tracking, 65.0, 88.0);
            this.SetSkill(SkillName.Veterinary, 60.0, 83.0);
        }

        public Thief(Serial serial)
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
            this.m_SBInfos.Add(new SBThief());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.Shirt(Utility.RandomNeutralHue()));
            this.AddItem(new Server.Items.LongPants(Utility.RandomNeutralHue()));
            this.AddItem(new Server.Items.Dagger());
            this.AddItem(new Server.Items.ThighBoots(Utility.RandomNeutralHue()));
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