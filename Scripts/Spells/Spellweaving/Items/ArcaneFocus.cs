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
            this.LootType = LootType.Blessed;
            this.m_StrengthBonus = strengthBonus;
        }

        public ArcaneFocus(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1032629;
            }
        }// Arcane Focus
        [CommandProperty(AccessLevel.GameMaster)]
        public int StrengthBonus
        {
            get
            {
                return this.m_StrengthBonus;
            }
            set
            {
                this.m_StrengthBonus = value;
            }
        }
        public override TextDefinition InvalidTransferMessage
        {
            get
            {
                return 1073480;
            }
        }// Your arcane focus disappears.
        public override bool Nontransferable
        {
            get
            {
                return true;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060485, this.m_StrengthBonus.ToString()); // strength bonus ~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(this.m_StrengthBonus);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_StrengthBonus = reader.ReadInt();
        }
    }
}