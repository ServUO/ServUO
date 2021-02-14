using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Glassblower : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Glassblower()
            : base("the alchemist")
        {
            SetSkill(SkillName.Alchemy, 85.0, 100.0);
            SetSkill(SkillName.TasteID, 85.0, 100.0);
        }

        public Glassblower(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild => NpcGuild.MagesGuild;
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBGlassblower());
            m_SBInfos.Add(new SBAlchemist(this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
