using System;

namespace Server.Items
{
    public class EmptyJar : Item
    {
        [Constructable]
        public EmptyJar()
            : base(0x1005)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public EmptyJar(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EmptyJars : Item
    {
        [Constructable]
        public EmptyJars()
            : base(0xe44)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public EmptyJars(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EmptyJars2 : Item
    {
        [Constructable]
        public EmptyJars2()
            : base(0xe45)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public EmptyJars2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EmptyJars3 : Item
    {
        [Constructable]
        public EmptyJars3()
            : base(0xe46)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public EmptyJars3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EmptyJars4 : Item
    {
        [Constructable]
        public EmptyJars4()
            : base(0xe47)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public EmptyJars4(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}