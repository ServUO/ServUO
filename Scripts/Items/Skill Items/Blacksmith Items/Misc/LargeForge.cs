using System;

namespace Server.Items
{
    [Server.Engines.Craft.Forge]
    public class LargeForgeWest : Item
    {
        private InternalItem m_Item;
        private InternalItem2 m_Item2;
        [Constructable]
        public LargeForgeWest()
            : base(0x199A)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
            this.m_Item2 = new InternalItem2(this);
        }

        public LargeForgeWest(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X, this.Y + 1, this.Z);
            if (this.m_Item2 != null)
                this.m_Item2.Location = new Point3D(this.X, this.Y + 2, this.Z);
        }

        public override void OnMapChange()
        {
            if (this.m_Item != null)
                this.m_Item.Map = this.Map;
            if (this.m_Item2 != null)
                this.m_Item2.Map = this.Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Item != null)
                this.m_Item.Delete();
            if (this.m_Item2 != null)
                this.m_Item2.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Item);
            writer.Write(this.m_Item2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Item = reader.ReadItem() as InternalItem;
            this.m_Item2 = reader.ReadItem() as InternalItem2;
        }

        [Server.Engines.Craft.Forge]
        private class InternalItem : Item
        {
            private LargeForgeWest m_Item;
            public InternalItem(LargeForgeWest item)
                : base(0x1996)
            {
                this.Movable = false;

                this.m_Item = item;
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
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

                this.m_Item = reader.ReadItem() as LargeForgeWest;
            }
        }

        [Server.Engines.Craft.Forge]
        private class InternalItem2 : Item
        {
            private LargeForgeWest m_Item;
            public InternalItem2(LargeForgeWest item)
                : base(0x1992)
            {
                this.Movable = false;

                this.m_Item = item;
            }

            public InternalItem2(Serial serial)
                : base(serial)
            {
            }

            public override void OnLocationChange(Point3D oldLocation)
            {
                if (this.m_Item != null)
                    this.m_Item.Location = new Point3D(this.X, this.Y - 2, this.Z);
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

                this.m_Item = reader.ReadItem() as LargeForgeWest;
            }
        }
    }

    [Server.Engines.Craft.Forge]
    public class LargeForgeEast : Item
    {
        private InternalItem m_Item;
        private InternalItem2 m_Item2;
        [Constructable]
        public LargeForgeEast()
            : base(0x197A)
        {
            this.Movable = false;

            this.m_Item = new InternalItem(this);
            this.m_Item2 = new InternalItem2(this);
        }

        public LargeForgeEast(Serial serial)
            : base(serial)
        {
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            if (this.m_Item != null)
                this.m_Item.Location = new Point3D(this.X + 1, this.Y, this.Z);
            if (this.m_Item2 != null)
                this.m_Item2.Location = new Point3D(this.X + 2, this.Y, this.Z);
        }

        public override void OnMapChange()
        {
            if (this.m_Item != null)
                this.m_Item.Map = this.Map;
            if (this.m_Item2 != null)
                this.m_Item2.Map = this.Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Item != null)
                this.m_Item.Delete();
            if (this.m_Item2 != null)
                this.m_Item2.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Item);
            writer.Write(this.m_Item2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Item = reader.ReadItem() as InternalItem;
            this.m_Item2 = reader.ReadItem() as InternalItem2;
        }

        [Server.Engines.Craft.Forge]
        private class InternalItem : Item
        {
            private LargeForgeEast m_Item;
            public InternalItem(LargeForgeEast item)
                : base(0x197E)
            {
                this.Movable = false;

                this.m_Item = item;
            }

            public InternalItem(Serial serial)
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

                this.m_Item = reader.ReadItem() as LargeForgeEast;
            }
        }

        [Server.Engines.Craft.Forge]
        private class InternalItem2 : Item
        {
            private LargeForgeEast m_Item;
            public InternalItem2(LargeForgeEast item)
                : base(0x1982)
            {
                this.Movable = false;

                this.m_Item = item;
            }

            public InternalItem2(Serial serial)
                : base(serial)
            {
            }

            public override void OnLocationChange(Point3D oldLocation)
            {
                if (this.m_Item != null)
                    this.m_Item.Location = new Point3D(this.X - 2, this.Y, this.Z);
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

                this.m_Item = reader.ReadItem() as LargeForgeEast;
            }
        }
    }
}