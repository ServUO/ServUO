using Server.Gumps;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class ArmorRefiner : BaseVendor
    {
        private RefinementCraftType m_RefineType;

        [CommandProperty(AccessLevel.GameMaster)]
        public RefinementCraftType RefineType { get { return m_RefineType; } set { m_RefineType = value; } }

        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override void InitSBInfo()
        {
        }

        [Constructable]
        public ArmorRefiner(RefinementCraftType type) : base("the armor refiner")
        {
            m_RefineType = type;

            SetSkill(SkillName.ArmsLore, 36.0, 68.0);
            AddItem(new HalfApron());

            switch (m_RefineType)
            {
                case RefinementCraftType.Blacksmith:
                    AddItem(new SmithHammer());
                    SetSkill(SkillName.Blacksmith, 65.0, 88.0);
                    break;
                case RefinementCraftType.Tailor:
                    SetSkill(SkillName.Tailoring, 60.0, 83.0);
                    break;
                case RefinementCraftType.Carpenter:
                    SetSkill(SkillName.Carpentry, 61.0, 93.0);
                    break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 10))
                from.SendGump(new RefinementHelpGump(m_RefineType));
        }

        public ArmorRefiner(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write((int)m_RefineType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
            m_RefineType = (RefinementCraftType)reader.ReadInt();
        }
    }
}