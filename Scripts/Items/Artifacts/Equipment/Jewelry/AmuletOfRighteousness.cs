using Server.Targeting;

namespace Server.Items
{
    public class AmuletOfRighteousness : SilverNecklace, IUsesRemaining
    {
        public override bool IsArtifact => true;
        private int m_UsesRemaining;
        [Constructable]
        public AmuletOfRighteousness()
            : this(100)
        {
        }

        [Constructable]
        public AmuletOfRighteousness(int uses)
            : base()
        {
            LootType = LootType.Blessed;
            Weight = 1.0;

            m_UsesRemaining = uses;
        }

        public AmuletOfRighteousness(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075313;// Amulet of Righteousness
        public virtual bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return m_UsesRemaining;
            }
            set
            {
                m_UsesRemaining = value;
                InvalidateProperties();
            }
        }
        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (IsChildOf(from.Backpack) || Parent == from)
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_UsesRemaining = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly AmuletOfRighteousness m_Amulet;
            public InternalTarget(AmuletOfRighteousness amulet)
                : base(12, false, TargetFlags.None)
            {
                m_Amulet = amulet;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Amulet == null || m_Amulet.Deleted)
                    return;

                if (targeted is Mobile)
                {
                    Mobile target = (Mobile)targeted;

                    if (m_Amulet.UsesRemaining <= 0)
                    {
                        from.SendLocalizedMessage(1042544); // This item is out of charges.
                        return;
                    }

                    target.BoltEffect(0);
                    m_Amulet.UsesRemaining -= 1;
                    m_Amulet.InvalidateProperties();
                }
            }
        }
    }
}
