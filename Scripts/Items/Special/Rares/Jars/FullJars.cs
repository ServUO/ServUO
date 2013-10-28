using System;

namespace Server.Items
{
    public class DecoFullJar : Item
    {
        [Constructable]
        public DecoFullJar()
            : base(0x1006)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoFullJar(Serial serial)
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

    public class DecoFullJars3 : Item
    {
        [Constructable]
        public DecoFullJars3()
            : base(0xE4a)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoFullJars3(Serial serial)
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

    public class DecoFullJars4 : Item
    {
        [Constructable]
        public DecoFullJars4()
            : base(0xE4b)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoFullJars4(Serial serial)
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