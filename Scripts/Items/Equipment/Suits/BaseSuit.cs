using System;

namespace Server.Items
{
    public abstract class BaseSuit : Item
    {
        private AccessLevel m_AccessLevel;
        public BaseSuit(AccessLevel level, int hue, int itemID)
            : base(itemID)
        {
            this.Hue = hue;
            this.Weight = 1.0;
            this.Movable = false;
            this.LootType = LootType.Newbied;
            this.Layer = Layer.OuterTorso;

            this.m_AccessLevel = level;
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
                return this.m_AccessLevel;
            }
            set
            {
                this.m_AccessLevel = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_AccessLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_AccessLevel = (AccessLevel)reader.ReadInt();
                        break;
                    }
            }
        }

        public bool Validate()
        {
            object root = this.RootParent;

            if (root is Mobile && ((Mobile)root).AccessLevel < this.m_AccessLevel)
            {
                this.Delete();
                return false;
            }

            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.Validate())
                base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.Validate())
                base.OnDoubleClick(from);
        }

        public override bool VerifyMove(Mobile from)
        {
            return (from.AccessLevel >= this.m_AccessLevel);
        }

        public override bool OnEquip(Mobile from)
        {
            if (from.AccessLevel < this.m_AccessLevel)
                from.SendMessage("You may not wear this.");

            return (from.AccessLevel >= this.m_AccessLevel);
        }
    }
}