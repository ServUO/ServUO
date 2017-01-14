using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.GargoyleAlchemist")]
    public class Glassblower : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Glassblower()
            : base("the alchemist")
        {
            this.SetSkill(SkillName.Alchemy, 85.0, 100.0);
            this.SetSkill(SkillName.TasteID, 85.0, 100.0);
        }

        public Glassblower(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.MagesGuild;
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
            this.m_SBInfos.Add(new SBGlassblower());
            this.m_SBInfos.Add(new SBAlchemist());
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

            if (this.Body == 0x2F2)
                this.Body = 0x2F6;
        }
    }
}