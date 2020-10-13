namespace Server.Items
{
    public class ArcaneFocus : BaseDecayingItem
    {
        private int m_StrengthBonus;

        [CommandProperty(AccessLevel.GameMaster)]
        public int StrengthBonus
        {
            get { return m_StrengthBonus; }
            set { m_StrengthBonus = value; }
        }

        [Constructable]
        public ArcaneFocus()
            : this(3600, 1)
        {
        }

        public ArcaneFocus(int lifeSpan, int strengthBonus)
            : base(0x3155)
        {
            LootType = LootType.Blessed;
            m_StrengthBonus = strengthBonus;

            TimeLeft = lifeSpan;
        }

        public ArcaneFocus(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1032629;// Arcane Focus
        public override bool Nontransferable => true;

        public override void HandleInvalidTransfer(Mobile from)
        {
            from.SendLocalizedMessage(1073480); // Your arcane focus disappears.
            Delete();
        }

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
