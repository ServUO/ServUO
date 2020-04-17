using Server.Engines.Plants;
using Server.Targeting;

namespace Server.Items
{
    public class PoppiesDust : Item, IUsesRemaining
    {
        public override int LabelNumber => 1095223;  // poppies dust

        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public virtual bool ShowUsesRemaining
        {
            get { return true; }
            set { }
        }

        [Constructable]
        public PoppiesDust()
            : this(8)
        {
        }

        [Constructable]
        public PoppiesDust(int charges)
            : base(0xC4D)
        {
            Weight = 1.0;
            Stackable = false;
            m_UsesRemaining = charges;
        }

        public PoppiesDust(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_UsesRemaining = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            list.Add(1060741, m_UsesRemaining.ToString()); // charges: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                return;
            }

            from.Target = new InternalTarget(this);
            from.SendLocalizedMessage(500343); // What do you wish to appraise and identify?
        }

        private class InternalTarget : Target
        {
            private readonly PoppiesDust m_Dust;

            public InternalTarget(PoppiesDust dust)
                : base(3, false, TargetFlags.None)
            {
                m_Dust = dust;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Dust.Deleted || m_Dust.UsesRemaining <= 0)
                {
                    return;
                }

                if (!m_Dust.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                    return;
                }
                else if (targeted is PoppiesDust && m_Dust != targeted)
                {
                    PoppiesDust pd = targeted as PoppiesDust;
                    pd.UsesRemaining += m_Dust.UsesRemaining;
                    m_Dust.Delete();

                    from.SendLocalizedMessage(1158448); // You combine the charges on the items.
                }
                else if (targeted is Seed)
                {
                    Seed m_Seed = (Seed)targeted;

                    if (m_Seed.ShowType)
                    {
                        from.SendLocalizedMessage(1114369, "", 946); // This seed has already been identified.
                        return;
                    }

                    m_Seed.ShowType = true;

                    --m_Dust.UsesRemaining;
                }
            }
        }
    }
}
