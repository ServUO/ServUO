using System;

namespace Server.Items
{
    public class HalfEmptyJar : Item
    {
        [Constructable]
        public HalfEmptyJar()
            : base(0x1007)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public HalfEmptyJar(Serial serial)
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

    public class HalfEmptyJars : Item
    {
        [Constructable]
        public HalfEmptyJars()
            : base(0xe4c)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public HalfEmptyJars(Serial serial)
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

    public class Jars2 : Item
    {
        [Constructable]
        public Jars2()
            : base(0xE4d)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public Jars2(Serial serial)
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

    public class Jars3 : Item
    {
        [Constructable]
        public Jars3()
            : base(0xE4e)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public Jars3(Serial serial)
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

    public class Jars4 : Item
    {
        [Constructable]
        public Jars4()
            : base(0xE4f)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public Jars4(Serial serial)
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