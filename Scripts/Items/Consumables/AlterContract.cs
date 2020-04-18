using Server.Engines.Craft;

namespace Server.Items
{
    public class AlterContract : Item
    {
        private RepairSkillType m_Type;
        private string m_CrafterName;

        [CommandProperty(AccessLevel.GameMaster)]
        public RepairSkillType Type
        {
            get { return m_Type; }

            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string CrafterName
        {
            get { return m_CrafterName; }

            set
            {
                m_CrafterName = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public AlterContract(RepairSkillType type, Mobile crafter)
            : base(0x14F0)
        {
            m_CrafterName = crafter.Name;
            Type = type;

            Hue = 0x1BC;
            Weight = 1.0;
        }

        public AlterContract(Serial serial)
            : base(serial)
        {
        }

        public string GetTitle()
        {
            if (m_Type == RepairSkillType.Smithing)
                return "Blacksmithing";
            else if (m_Type == RepairSkillType.Carpentry)
                return "Carpentry";
            else if (m_Type == RepairSkillType.Tailoring)
                return "Tailoring";
            else if (m_Type == RepairSkillType.Tinkering)
                return "Tinkering";
            else
                return null;
        }

        public CraftSystem GetCraftSystem()
        {
            if (m_Type == RepairSkillType.Smithing)
                return DefBlacksmithy.CraftSystem;
            else if (m_Type == RepairSkillType.Carpentry)
                return DefCarpentry.CraftSystem;
            else if (m_Type == RepairSkillType.Tailoring)
                return DefTailoring.CraftSystem;
            else if (m_Type == RepairSkillType.Tinkering)
                return DefTinkering.CraftSystem;
            else
                return null;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1050043, m_CrafterName); // crafted by ~1_NAME~
            list.Add(1060636); // exceptional
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1094795, GetTitle());
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                // The contract must be in your backpack to use it.
                from.SendLocalizedMessage(1047012);
            }
            else
            {
                CraftSystem cs = GetCraftSystem();

                AlterItem.BeginTarget(from, cs, this);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version 

            writer.Write((int)m_Type);
            writer.Write(m_CrafterName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Type = (RepairSkillType)reader.ReadInt();
            m_CrafterName = reader.ReadString();
        }
    }
}
