using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public class Ankhs
    {
        public const int ResurrectRange = 2;
        public const int TitheRange = 2;
        public const int LockRange = 2;
        public static void GetContextMenuEntries(Mobile from, Item item, List<ContextMenuEntry> list)
        {
            if (from is PlayerMobile)
                list.Add(new LockKarmaEntry((PlayerMobile)from));

            list.Add(new ResurrectEntry(from, item));

            if (Core.AOS)
                list.Add(new TitheEntry(from));
        }

        public static void Resurrect(Mobile m, Item item)
        {
            if (m.Alive)
                return;

            if (!m.InRange(item.GetWorldLocation(), ResurrectRange))
                m.SendLocalizedMessage(500446); // That is too far away.
            else if (m.Map != null && m.Map.CanFit(m.Location, 16, false, false))
            {
                m.CloseGump(typeof(ResurrectGump));
                m.SendGump(new ResurrectGump(m, ResurrectMessage.VirtueShrine));
            }
            else
                m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
        }

        private class ResurrectEntry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            private readonly Item m_Item;
            public ResurrectEntry(Mobile mobile, Item item)
                : base(6195, ResurrectRange)
            {
                this.m_Mobile = mobile;
                this.m_Item = item;

                this.Enabled = !this.m_Mobile.Alive;
            }

            public override void OnClick()
            {
                Resurrect(this.m_Mobile, this.m_Item);
            }
        }

        private class LockKarmaEntry : ContextMenuEntry
        {
            private readonly PlayerMobile m_Mobile;
            public LockKarmaEntry(PlayerMobile mobile)
                : base(mobile.KarmaLocked ? 6197 : 6196, LockRange)
            {
                this.m_Mobile = mobile;
            }

            public override void OnClick()
            {
                this.m_Mobile.KarmaLocked = !this.m_Mobile.KarmaLocked;

                if (this.m_Mobile.KarmaLocked)
                    this.m_Mobile.SendLocalizedMessage(1060192); // Your karma has been locked. Your karma can no longer be raised.
                else
                    this.m_Mobile.SendLocalizedMessage(1060191); // Your karma has been unlocked. Your karma can be raised again.
            }
        }

        private class TitheEntry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            public TitheEntry(Mobile mobile)
                : base(6198, TitheRange)
            {
                this.m_Mobile = mobile;

                this.Enabled = this.m_Mobile.Alive;
            }

            public override void OnClick()
            {
                if (this.m_Mobile.CheckAlive())
                    this.m_Mobile.SendGump(new TithingGump(this.m_Mobile, 0));
            }
        }
    }

    public class AnkhWest : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public AnkhWest()
            : this(false)
        {
        }

        [Constructable]
        public AnkhWest(bool bloodied)
            : base(bloodied ? 0x1D98 : 0x3)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(bloodied, this);
        }

        public AnkhWest(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }// Tell the core that we implement OnMovement
        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get
            {
                return base.Hue;
            }
            set
            {
                base.Hue = value;
                if (this.m_Item.Hue != value)
                    this.m_Item.Hue = value;
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.Parent == null && Utility.InRange(this.Location, m.Location, 1) && !Utility.InRange(this.Location, oldLocation, 1))
                Ankhs.Resurrect(m, this);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            Ankhs.GetContextMenuEntries(from, this, list);
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            Ankhs.Resurrect(m, this);
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X, this.Y + 1, this.Z);
        }

        public override void OnMapChange()
        {
            if (this.m_Item != null)
                this.m_Item.Map = this.Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Item != null)
                this.m_Item.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Item);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Item = reader.ReadItem() as InternalItem;
        }

        private class InternalItem : Item
        {
            private AnkhWest m_Item;
            public InternalItem(bool bloodied, AnkhWest item)
                : base(bloodied ? 0x1D97 : 0x2)
            {
                this.Movable = false;

                this.m_Item = item;
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override bool HandlesOnMovement
            {
                get
                {
                    return true;
                }
            }// Tell the core that we implement OnMovement
            [Hue, CommandProperty(AccessLevel.GameMaster)]
            public override int Hue
            {
                get
                {
                    return base.Hue;
                }
                set
                {
                    base.Hue = value;
                    if (this.m_Item.Hue != value)
                        this.m_Item.Hue = value;
                }
            }
            public override void OnLocationChange(Point3D oldLocation)
            {
                if (this.m_Item != null)
                    this.m_Item.Location = new Point3D(this.X, this.Y - 1, this.Z);
            }

            public override void OnMapChange()
            {
                if (this.m_Item != null)
                    this.m_Item.Map = this.Map;
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (this.m_Item != null)
                    this.m_Item.Delete();
            }

            public override void OnMovement(Mobile m, Point3D oldLocation)
            {
                if (this.Parent == null && Utility.InRange(this.Location, m.Location, 1) && !Utility.InRange(this.Location, oldLocation, 1))
                    Ankhs.Resurrect(m, this);
            }

            public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
            {
                base.GetContextMenuEntries(from, list);
                Ankhs.GetContextMenuEntries(from, this, list);
            }

            public override void OnDoubleClickDead(Mobile m)
            {
                Ankhs.Resurrect(m, this);
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write(this.m_Item);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                this.m_Item = reader.ReadItem() as AnkhWest;
            }
        }
    }

    [TypeAlias("Server.Items.AnkhEast")]
    public class AnkhNorth : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public AnkhNorth()
            : this(false)
        {
        }

        [Constructable]
        public AnkhNorth(bool bloodied)
            : base(bloodied ? 0x1E5D : 0x4)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(bloodied, this);
        }

        public AnkhNorth(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }// Tell the core that we implement OnMovement
        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get
            {
                return base.Hue;
            }
            set
            {
                base.Hue = value;
                if (this.m_Item.Hue != value)
                    this.m_Item.Hue = value;
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.Parent == null && Utility.InRange(this.Location, m.Location, 1) && !Utility.InRange(this.Location, oldLocation, 1))
                Ankhs.Resurrect(m, this);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            Ankhs.GetContextMenuEntries(from, this, list);
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            Ankhs.Resurrect(m, this);
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X + 1, this.Y, this.Z);
        }

        public override void OnMapChange()
        {
            if (this.m_Item != null)
                this.m_Item.Map = this.Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Item != null)
                this.m_Item.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Item);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Item = reader.ReadItem() as InternalItem;
        }

        [TypeAlias("Server.Items.AnkhEast+InternalItem")]
        private class InternalItem : Item
        {
            private AnkhNorth m_Item;
            public InternalItem(bool bloodied, AnkhNorth item)
                : base(bloodied ? 0x1E5C : 0x5)
            {
                this.Movable = false;

                this.m_Item = item;
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override bool HandlesOnMovement
            {
                get
                {
                    return true;
                }
            }// Tell the core that we implement OnMovement
            [Hue, CommandProperty(AccessLevel.GameMaster)]
            public override int Hue
            {
                get
                {
                    return base.Hue;
                }
                set
                {
                    base.Hue = value;
                    if (this.m_Item.Hue != value)
                        this.m_Item.Hue = value;
                }
            }
            public override void OnLocationChange(Point3D oldLocation)
            {
                if (this.m_Item != null)
                    this.m_Item.Location = new Point3D(this.X - 1, this.Y, this.Z);
            }

            public override void OnMapChange()
            {
                if (this.m_Item != null)
                    this.m_Item.Map = this.Map;
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (this.m_Item != null)
                    this.m_Item.Delete();
            }

            public override void OnMovement(Mobile m, Point3D oldLocation)
            {
                if (this.Parent == null && Utility.InRange(this.Location, m.Location, 1) && !Utility.InRange(this.Location, oldLocation, 1))
                    Ankhs.Resurrect(m, this);
            }

            public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
            {
                base.GetContextMenuEntries(from, list);
                Ankhs.GetContextMenuEntries(from, this, list);
            }

            public override void OnDoubleClickDead(Mobile m)
            {
                Ankhs.Resurrect(m, this);
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write(this.m_Item);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                this.m_Item = reader.ReadItem() as AnkhNorth;
            }
        }
    }
}