namespace Server.Items
{
    public abstract class BaseShirt : BaseClothing
    {
        public BaseShirt(int itemID)
            : this(itemID, 0)
        {
        }

        public BaseShirt(int itemID, int hue)
            : base(itemID, Layer.Shirt, hue)
        {
        }

        public BaseShirt(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x1efd, 0x1efe)]
    public class FancyShirt : BaseShirt
    {
        [Constructable]
        public FancyShirt()
            : this(0)
        {
        }

        [Constructable]
        public FancyShirt(int hue)
            : base(0x1EFD, hue)
        {
            Weight = 2.0;
        }

        public FancyShirt(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x1517, 0x1518)]
    public class Shirt : BaseShirt
    {
        [Constructable]
        public Shirt()
            : this(0)
        {
        }

        [Constructable]
        public Shirt(int hue)
            : base(0x1517, hue)
        {
            Weight = 1.0;
        }

        public Shirt(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x2794, 0x27DF)]
    public class ClothNinjaJacket : BaseShirt
    {
        [Constructable]
        public ClothNinjaJacket()
            : this(0)
        {
        }

        [Constructable]
        public ClothNinjaJacket(int hue)
            : base(0x2794, hue)
        {
            Weight = 5.0;
            Layer = Layer.InnerTorso;
        }

        public ClothNinjaJacket(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ElvenShirt : BaseShirt
    {
        [Constructable]
        public ElvenShirt()
            : this(0)
        {
        }

        [Constructable]
        public ElvenShirt(int hue)
            : base(0x3175, hue)
        {
            Weight = 2.0;
        }

        public ElvenShirt(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class ElvenDarkShirt : BaseShirt
    {
        [Constructable]
        public ElvenDarkShirt()
            : this(0)
        {
        }

        [Constructable]
        public ElvenDarkShirt(int hue)
            : base(0x3176, hue)
        {
            Weight = 2.0;
        }

        public ElvenDarkShirt(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class GargishClothChest : BaseClothing
    {
        [Constructable]
        public GargishClothChest()
            : this(0)
        {
        }

        [Constructable]
        public GargishClothChest(int hue)
            : base(0x0406, Layer.InnerTorso, hue)
        {
            Weight = 2.0;
        }

        public GargishClothChest(Serial serial)
            : base(serial)
        {
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
            {
                if (((Mobile)parent).Female)
                    ItemID = 0x0405;
                else
                    ItemID = 0x0406;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FemaleGargishClothChest : BaseClothing
    {
        [Constructable]
        public FemaleGargishClothChest()
            : this(0)
        {
        }

        [Constructable]
        public FemaleGargishClothChest(int hue)
            : base(0x0405, Layer.InnerTorso, hue)
        {
            Weight = 2.0;
        }

        public FemaleGargishClothChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class MaleGargishClothChest : BaseClothing
    {
        [Constructable]
        public MaleGargishClothChest()
            : this(0)
        {
        }

        [Constructable]
        public MaleGargishClothChest(int hue)
            : base(0x0406, Layer.InnerTorso, hue)
        {
            Weight = 2.0;
        }

        public MaleGargishClothChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
