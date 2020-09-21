namespace Server.Items
{
    public abstract class BaseNecklace : BaseJewel
    {
        public BaseNecklace(int itemID)
            : base(itemID, Layer.Neck)
        {
        }

        public BaseNecklace(Serial serial)
            : base(serial)
        {
        }

        public override int BaseGemTypeNumber => 1044241;// star sapphire necklace
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

    public class Necklace : BaseNecklace
    {
        [Constructable]
        public Necklace()
            : base(0x1085)
        {
            Weight = 0.1;
        }

        public Necklace(Serial serial)
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

    public class GoldNecklace : BaseNecklace
    {
        [Constructable]
        public GoldNecklace()
            : base(0x1088)
        {
            Weight = 0.1;
        }

        public GoldNecklace(Serial serial)
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

    public class GoldBeadNecklace : BaseNecklace
    {
        [Constructable]
        public GoldBeadNecklace()
            : base(0x1089)
        {
            Weight = 0.1;
        }

        public GoldBeadNecklace(Serial serial)
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

    public class SilverNecklace : BaseNecklace
    {
        [Constructable]
        public SilverNecklace()
            : base(0x1F08)
        {
            Weight = 0.1;
        }

        public SilverNecklace(Serial serial)
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

    public class SilverBeadNecklace : BaseNecklace
    {
        [Constructable]
        public SilverBeadNecklace()
            : base(0x1F05)
        {
            Weight = 0.1;
        }

        public SilverBeadNecklace(Serial serial)
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

    public class OctopusNecklace : BaseNecklace
    {
        [Constructable]
        public OctopusNecklace()
            : base(0xA349)
        {
            AssignRandomGem();
        }

        private void AssignRandomGem()
        {
            int ran = Utility.RandomMinMax(1, 9);
            GemType = (GemType)ran;
        }

        public override void OnGemTypeChange(GemType old)
        {
            if (old == GemType)
                return;

            switch (GemType)
            {
                default:
                case GemType.None: Hue = 0; break;
                case GemType.StarSapphire: Hue = 1928; break;
                case GemType.Emerald: Hue = 1914; break;
                case GemType.Sapphire: Hue = 1926; break;
                case GemType.Ruby: Hue = 1911; break;
                case GemType.Citrine: Hue = 1955; break;
                case GemType.Amethyst: Hue = 1919; break;
                case GemType.Tourmaline: Hue = 1924; break;
                case GemType.Amber: Hue = 1923; break;
                case GemType.Diamond: Hue = 2067; break;
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (GemType != GemType.None)
            {
                list.Add(1159018, string.Format("#{0}", GemLocalization())); // ~1_type~ octopus necklace
            }
            else
            {
                list.Add(1125825); // octopus necklace
            }
        }

        public OctopusNecklace(Serial serial)
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
