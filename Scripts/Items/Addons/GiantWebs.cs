using System;

namespace Server.Items
{
    public class GiantWeb1 : BaseAddon
    {
        [Constructable]
        public GiantWeb1()
        {
            int itemID = 4280;
            int count = 5;
            bool leftToRight = false;

            for (int i = 0; i < count; ++i)
                this.AddComponent(new AddonComponent(itemID++), leftToRight ? i : count - 1 - i, -(leftToRight ? i : count - 1 - i), 0);
        }

        public GiantWeb1(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();
        }
    }

    public class GiantWeb2 : BaseAddon
    {
        [Constructable]
        public GiantWeb2()
        {
            int itemID = 4285;
            int count = 5;
            bool leftToRight = true;

            for (int i = 0; i < count; ++i)
                this.AddComponent(new AddonComponent(itemID++), leftToRight ? i : count - 1 - i, -(leftToRight ? i : count - 1 - i), 0);
        }

        public GiantWeb2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();
        }
    }

    public class GiantWeb3 : BaseAddon
    {
        [Constructable]
        public GiantWeb3()
        {
            int itemID = 4290;
            int count = 4;
            bool leftToRight = true;

            for (int i = 0; i < count; ++i)
                this.AddComponent(new AddonComponent(itemID++), leftToRight ? i : count - 1 - i, -(leftToRight ? i : count - 1 - i), 0);
        }

        public GiantWeb3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();
        }
    }

    public class GiantWeb4 : BaseAddon
    {
        [Constructable]
        public GiantWeb4()
        {
            int itemID = 4294;
            int count = 4;
            bool leftToRight = false;

            for (int i = 0; i < count; ++i)
                this.AddComponent(new AddonComponent(itemID++), leftToRight ? i : count - 1 - i, -(leftToRight ? i : count - 1 - i), 0);
        }

        public GiantWeb4(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();
        }
    }

    public class GiantWeb5 : BaseAddon
    {
        [Constructable]
        public GiantWeb5()
        {
            int itemID = 4298;
            int count = 4;
            bool leftToRight = true;

            for (int i = 0; i < count; ++i)
                this.AddComponent(new AddonComponent(itemID++), leftToRight ? i : count - 1 - i, -(leftToRight ? i : count - 1 - i), 0);
        }

        public GiantWeb5(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();
        }
    }

    public class GiantWeb6 : BaseAddon
    {
        [Constructable]
        public GiantWeb6()
        {
            int itemID = 4302;
            int count = 4;
            bool leftToRight = false;

            for (int i = 0; i < count; ++i)
                this.AddComponent(new AddonComponent(itemID++), leftToRight ? i : count - 1 - i, -(leftToRight ? i : count - 1 - i), 0);
        }

        public GiantWeb6(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadByte();
        }
    }
}