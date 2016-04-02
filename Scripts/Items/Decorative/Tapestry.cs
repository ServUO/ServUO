using System;

namespace Server.Items
{
    public class Tapestry1N : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry1N()
            : base(0xEAA)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry1N(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X + 1, this.Y , this.Z);
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
            private Tapestry1N m_Item;
            public InternalItem(Tapestry1N item)
                : base(0xEAB)
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
                    this.m_Item.Location = new Point3D(this.X - 1, this.Y , this.Z);
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

                this.m_Item = reader.ReadItem() as Tapestry1N;
            }
        }
    }

    public class Tapestry2N : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry2N()
            : base(0xEAC)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry2N(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X + 1, this.Y , this.Z);
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
            private Tapestry2N m_Item;
            public InternalItem(Tapestry2N item)
                : base(0xEAD)
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
                    this.m_Item.Location = new Point3D(this.X - 1, this.Y , this.Z);
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

                this.m_Item = reader.ReadItem() as Tapestry2N;
            }
        }
    }

    public class Tapestry2W : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry2W()
            : base(0xEAE)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry2W(Serial serial)
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
            private Tapestry2W m_Item;
            public InternalItem(Tapestry2W item)
                : base(0xEAF)
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
                    this.m_Item.Location = new Point3D(this.X , this.Y + 1 , this.Z);
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

                this.m_Item = reader.ReadItem() as Tapestry2W;
            }
        }
    }

    public class Tapestry3N : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry3N()
            : base(0xFD6)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry3N(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X - 2, this.Y , this.Z);
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
            private Tapestry3N m_Item;
            public InternalItem(Tapestry3N item)
                : base(0xFD5)
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
                    this.m_Item.Location = new Point3D(this.X + 2, this.Y , this.Z);
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

                this.m_Item = reader.ReadItem() as Tapestry3N;
            }
        }
    }

    public class Tapestry3W : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry3W()
            : base(0xFD7)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry3W(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X , this.Y - 2 , this.Z);
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
            private Tapestry3W m_Item;
            public InternalItem(Tapestry3W item)
                : base(0xFD8)
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
                    this.m_Item.Location = new Point3D(this.X , this.Y + 2 , this.Z);
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

                this.m_Item = reader.ReadItem() as Tapestry3W;
            }
        }
    }

    public class Tapestry4N : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry4N()
            : base(0xFDA)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry4N(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X - 1, this.Y , this.Z);
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
            private Tapestry4N m_Item;
            public InternalItem(Tapestry4N item)
                : base(0xFD9)
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
                    this.m_Item.Location = new Point3D(this.X + 1, this.Y , this.Z);
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

                this.m_Item = reader.ReadItem() as Tapestry4N;
            }
        }
    }

    public class Tapestry4W : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry4W()
            : base(0xFDB)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry4W(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X , this.Y - 1, this.Z);
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
            private Tapestry4W m_Item;
            public InternalItem(Tapestry4W item)
                : base(0xFDC)
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

                this.m_Item = reader.ReadItem() as Tapestry4W;
            }
        }
    }

    public class Tapestry5N : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry5N()
            : base(0xFDE)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry5N(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X - 1, this.Y , this.Z);
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
            private Tapestry5N m_Item;
            public InternalItem(Tapestry5N item)
                : base(0xFDD)
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
                    this.m_Item.Location = new Point3D(this.X + 1, this.Y , this.Z);
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

                this.m_Item = reader.ReadItem() as Tapestry5N;
            }
        }
    }

    public class Tapestry5W : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry5W()
            : base(0xFDF)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry5W(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X , this.Y - 1, this.Z);
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
            private Tapestry5W m_Item;
            public InternalItem(Tapestry5W item)
                : base(0xFE0)
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

                this.m_Item = reader.ReadItem() as Tapestry5W;
            }
        }
    }

    public class Tapestry6N : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry6N()
            : base(0xFE2)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry6N(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X - 1, this.Y , this.Z);
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
            private Tapestry6N m_Item;
            public InternalItem(Tapestry6N item)
                : base(0xFE1)
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
                    this.m_Item.Location = new Point3D(this.X + 1, this.Y , this.Z);
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

                this.m_Item = reader.ReadItem() as Tapestry6N;
            }
        }
    }

    public class Tapestry6W : Item
    {
        private InternalItem m_Item;
        [Constructable]
        public Tapestry6W()
            : base(0xFE3)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
        }

        public Tapestry6W(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X , this.Y - 1, this.Z);
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
            private Tapestry6W m_Item;
            public InternalItem(Tapestry6W item)
                : base(0xFE4)
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

                this.m_Item = reader.ReadItem() as Tapestry6W;
            }
        }
    }
}