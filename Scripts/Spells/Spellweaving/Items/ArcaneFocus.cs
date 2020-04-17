using System;

namespace Server.Items
{
    public class ArcaneFocus : TransientItem
    {
        private int m_StrengthBonus;
        [Constructable]
        public ArcaneFocus()
            : this(TimeSpan.FromHours(1), 1)
        {
        }

        [Constructable]
        public ArcaneFocus(int lifeSpan, int strengthBonus)
            : this(TimeSpan.FromSeconds(lifeSpan), strengthBonus)
        {
        }

        public ArcaneFocus(TimeSpan lifeSpan, int strengthBonus)
            : base(0x3155, lifeSpan)
        {
            LootType = LootType.Blessed;
            m_StrengthBonus = strengthBonus;
        }

        public ArcaneFocus(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1032629;// Arcane Focus
        [CommandProperty(AccessLevel.GameMaster)]
        public int StrengthBonus
        {
            get
            {
                return m_StrengthBonus;
            }
            set
            {
                m_StrengthBonus = value;
            }
        }
        public override TextDefinition InvalidTransferMessage => 1073480;// Your arcane focus disappears.
        public override bool Nontransferable => true;
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060485, m_StrengthBonus.ToString()); // strength bonus ~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_StrengthBonus);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_StrengthBonus = reader.ReadInt();
        }
    }
}