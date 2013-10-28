using System;

namespace Server.Items
{
    public class LargePainting : Item
    {
        [Constructable]
        public LargePainting()
            : base(0x0EA0)
        {
            this.Movable = false;
        }

        public LargePainting(Serial serial)
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

    [FlipableAttribute(0x0E9F, 0x0EC8)] 
    public class WomanPortrait1 : Item
    {
        [Constructable]
        public WomanPortrait1()
            : base(0x0E9F)
        {
            this.Movable = false;
        }

        public WomanPortrait1(Serial serial)
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

    [FlipableAttribute(0x0EE7, 0x0EC9)] 
    public class WomanPortrait2 : Item
    {
        [Constructable]
        public WomanPortrait2()
            : base(0x0EE7)
        {
            this.Movable = false;
        }

        public WomanPortrait2(Serial serial)
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

    [FlipableAttribute(0x0EA2, 0x0EA1)] 
    public class ManPortrait1 : Item
    {
        [Constructable]
        public ManPortrait1()
            : base(0x0EA2)
        {
            this.Movable = false;
        }

        public ManPortrait1(Serial serial)
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

    [FlipableAttribute(0x0EA3, 0x0EA4)] 
    public class ManPortrait2 : Item
    {
        [Constructable]
        public ManPortrait2()
            : base(0x0EA3)
        {
            this.Movable = false;
        }

        public ManPortrait2(Serial serial)
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

    [FlipableAttribute(0x0EA6, 0x0EA5)] 
    public class LadyPortrait1 : Item
    {
        [Constructable]
        public LadyPortrait1()
            : base(0x0EA6)
        {
            this.Movable = false;
        }

        public LadyPortrait1(Serial serial)
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

    [FlipableAttribute(0x0EA7, 0x0EA8)] 
    public class LadyPortrait2 : Item
    {
        [Constructable]
        public LadyPortrait2()
            : base(0x0EA7)
        {
            this.Movable = false;
        }

        public LadyPortrait2(Serial serial)
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
}