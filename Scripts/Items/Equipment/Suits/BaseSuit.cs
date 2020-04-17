namespace Server.Items
{
    public abstract class BaseSuit : Item
    {
        private AccessLevel m_AccessLevel;
        public BaseSuit(AccessLevel level, int hue, int itemID)
            : base(itemID)
        {
            Hue = hue;
            Weight = 1.0;
            Movable = false;
            LootType = LootType.Newbied;
            Layer = Layer.OuterTorso;

            m_AccessLevel = level;
        }

        public BaseSuit(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Administrator)]
        public AccessLevel AccessLevel
        {
            get
            {
                return m_AccessLevel;
            }
            set
            {
                m_AccessLevel = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write((int)m_AccessLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_AccessLevel = (AccessLevel)reader.ReadInt();
                        break;
                    }
            }
        }

        public bool Validate()
        {
            object root = RootParent;

            if (root is Mobile && ((Mobile)root).AccessLevel < m_AccessLevel)
            {
                Delete();
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Validate())
                base.OnDoubleClick(from);
        }

        public override bool VerifyMove(Mobile from)
        {
            return (from.AccessLevel >= m_AccessLevel);
        }

        public override bool OnEquip(Mobile from)
        {
            if (from.AccessLevel < m_AccessLevel)
                from.SendMessage("You may not wear this.");

            return (from.AccessLevel >= m_AccessLevel);
        }
    }
}
