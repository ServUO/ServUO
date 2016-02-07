using System;

namespace Server.Items
{
    public class BarrelLid : Item
    {
        [Constructable]
        public BarrelLid()
            : base(0x1DB8)
        {
            this.Weight = 2;
        }

        public BarrelLid(Serial serial)
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

    [FlipableAttribute(0x1EB1, 0x1EB2, 0x1EB3, 0x1EB4)]
    public class BarrelStaves : Item
    {
        [Constructable]
        public BarrelStaves()
            : base(0x1EB1)
        {
            this.Weight = 1;
        }

        public BarrelStaves(Serial serial)
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

    public class BarrelHoops : Item
    {
        [Constructable]
        public BarrelHoops()
            : base(0x1DB7)
        {
            this.Weight = 5;
        }

        public BarrelHoops(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1011228;
            }
        }// Barrel hoops
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

    public class BarrelTap : Item
    {
        [Constructable]
        public BarrelTap()
            : base(0x1004)
        {
            this.Weight = 1;
        }

        public BarrelTap(Serial serial)
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