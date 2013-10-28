using System;

namespace Server.Items
{
    public abstract class BaseScales : Item, ICommodity
    {
        protected virtual CraftResource DefaultResource { get { return CraftResource.RedScales; } }

        private CraftResource m_Resource;
        public BaseScales(CraftResource resource)
            : this(resource, 1)
        {
        }

        public BaseScales(CraftResource resource, int amount)
            : base(0x26B4)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Hue = CraftResources.GetHue(resource);

            this.m_Resource = resource;
        }

        public BaseScales(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1053139;
            }
        }// dragon scales
        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return this.m_Resource;
            }
            set
            {
                this.m_Resource = value;
                this.InvalidateProperties();
            }
        }
        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
        }
        int ICommodity.DescriptionNumber
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1: // Reset from Resource System
                    this.m_Resource = this.DefaultResource;
                    reader.ReadString();
                    break;
                case 0:
                    {
                        this.m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }
        }
    }

    public class RedScales : BaseScales
    {
        [Constructable]
        public RedScales()
            : this(1)
        {
        }

        [Constructable]
        public RedScales(int amount)
            : base(CraftResource.RedScales, amount)
        {
        }

        public RedScales(Serial serial)
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

    public class YellowScales : BaseScales
    {
        protected override CraftResource DefaultResource { get { return CraftResource.YellowScales; } }

        [Constructable]
        public YellowScales()
            : this(1)
        {
        }

        [Constructable]
        public YellowScales(int amount)
            : base(CraftResource.YellowScales, amount)
        {
        }

        public YellowScales(Serial serial)
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

    public class BlackScales : BaseScales
    {
        protected override CraftResource DefaultResource { get { return CraftResource.BlackScales; } }

        [Constructable]
        public BlackScales()
            : this(1)
        {
        }

        [Constructable]
        public BlackScales(int amount)
            : base(CraftResource.BlackScales, amount)
        {
        }

        public BlackScales(Serial serial)
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

    public class GreenScales : BaseScales
    {
        protected override CraftResource DefaultResource { get { return CraftResource.GreenScales; } }

        [Constructable]
        public GreenScales()
            : this(1)
        {
        }

        [Constructable]
        public GreenScales(int amount)
            : base(CraftResource.GreenScales, amount)
        {
        }

        public GreenScales(Serial serial)
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

    public class WhiteScales : BaseScales
    {
        protected override CraftResource DefaultResource { get { return CraftResource.WhiteScales; } }

        [Constructable]
        public WhiteScales()
            : this(1)
        {
        }

        [Constructable]
        public WhiteScales(int amount)
            : base(CraftResource.WhiteScales, amount)
        {
        }

        public WhiteScales(Serial serial)
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

    public class BlueScales : BaseScales
    {
        protected override CraftResource DefaultResource { get { return CraftResource.BlueScales; } }

        [Constructable]
        public BlueScales()
            : this(1)
        {
        }

        [Constructable]
        public BlueScales(int amount)
            : base(CraftResource.BlueScales, amount)
        {
        }

        public BlueScales(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1053140;
            }
        }// sea serpent scales
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