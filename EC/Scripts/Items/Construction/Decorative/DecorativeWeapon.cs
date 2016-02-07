using System;

namespace Server.Items
{
    [FlipableAttribute(0x155E, 0x155F, 0x155C, 0x155D)] 
    public class DecorativeBowWest : Item
    {
        [Constructable]
        public DecorativeBowWest()
            : base(Utility.Random(0x155E, 2))
        {
            this.Movable = false;
        }

        public DecorativeBowWest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [FlipableAttribute(0x155C, 0x155D, 0x155E, 0x155F)] 
    public class DecorativeBowNorth : Item
    {
        [Constructable]
        public DecorativeBowNorth()
            : base(Utility.Random(0x155C, 2))
        {
            this.Movable = false;
        }

        public DecorativeBowNorth(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [FlipableAttribute(0x1560, 0x1561, 0x1562, 0x1563)] 
    public class DecorativeAxeNorth : Item
    {
        [Constructable]
        public DecorativeAxeNorth()
            : base(Utility.Random(0x1560, 2))
        {
            this.Movable = false;
        }

        public DecorativeAxeNorth(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [FlipableAttribute(0x1562, 0x1563, 0x1560, 0x1561)] 
    public class DecorativeAxeWest : Item
    {
        [Constructable]
        public DecorativeAxeWest()
            : base(Utility.Random(0x1562, 2))
        {
            this.Movable = false;
        }

        public DecorativeAxeWest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DecorativeSwordNorth : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public DecorativeSwordNorth()
            : base(0x1565)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public DecorativeSwordNorth(Serial serial)
            : base(serial)
        {
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
            private DecorativeSwordNorth m_Item;
            public InternalItem(DecorativeSwordNorth item)
                : base(0x1564)
            {
                this.Movable = true;

                this.m_Item = item;
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
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

                this.m_Item = reader.ReadItem() as DecorativeSwordNorth;
            }
        }
    }

    public class DecorativeSwordWest : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public DecorativeSwordWest()
            : base(0x1566)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public DecorativeSwordWest(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X , this.Y - 1 , this.Z);
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
            private DecorativeSwordWest m_Item;
            public InternalItem(DecorativeSwordWest item)
                : base(0x1567)
            {
                this.Movable = true;

                this.m_Item = item;
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override void OnLocationChange(Point3D oldLocation)
            {
                if (this.m_Item != null)
                    this.m_Item.Location = new Point3D(this.X , this.Y + 1, this.Z);
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

                this.m_Item = reader.ReadItem() as DecorativeSwordWest;
            }
        }
    }

    public class DecorativeDAxeNorth : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public DecorativeDAxeNorth()
            : base(0x1569)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public DecorativeDAxeNorth(Serial serial)
            : base(serial)
        {
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
            private DecorativeDAxeNorth m_Item;
            public InternalItem(DecorativeDAxeNorth item)
                : base(0x1568)
            {
                this.Movable = true;

                this.m_Item = item;
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
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

                this.m_Item = reader.ReadItem() as DecorativeDAxeNorth;
            }
        }
    }

    public class DecorativeDAxeWest : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public DecorativeDAxeWest()
            : base(0x156A)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public DecorativeDAxeWest(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X , this.Y - 1 , this.Z);
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
            private DecorativeDAxeWest m_Item;
            public InternalItem(DecorativeDAxeWest item)
                : base(0x156B)
            {
                this.Movable = true;

                this.m_Item = item;
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override void OnLocationChange(Point3D oldLocation)
            {
                if (this.m_Item != null)
                    this.m_Item.Location = new Point3D(this.X , this.Y + 1, this.Z);
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

                this.m_Item = reader.ReadItem() as DecorativeDAxeWest;
            }
        }
    }
}