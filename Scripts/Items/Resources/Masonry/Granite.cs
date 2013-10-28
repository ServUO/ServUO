using System;

namespace Server.Items
{
    public abstract class BaseGranite : Item
    {
        private CraftResource m_Resource;
        public BaseGranite(CraftResource resource)
            : base(0x1779)
        {
            this.Hue = CraftResources.GetHue(resource);
            this.Stackable = Core.ML;

            this.m_Resource = resource;
        }

        public BaseGranite(Serial serial)
            : base(serial)
        {
        }

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
                return Core.ML ? 1.0 : 10.0;
            }// Pub 57
        }
        public override int LabelNumber
        {
            get
            {
                return 1044607;
            }
        }// high quality granite
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        this.m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }
			
            if (version < 1)
                this.Stackable = Core.ML;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(this.m_Resource))
            {
                int num = CraftResources.GetLocalizationNumber(this.m_Resource);

                if (num > 0)
                    list.Add(num);
                else
                    list.Add(CraftResources.GetName(this.m_Resource));
            }
        }
    }

    public class Granite : BaseGranite
    {
        [Constructable]
        public Granite()
            : base(CraftResource.Iron)
        {
        }

        public Granite(Serial serial)
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

    public class DullCopperGranite : BaseGranite
    {
        [Constructable]
        public DullCopperGranite()
            : base(CraftResource.DullCopper)
        {
        }

        public DullCopperGranite(Serial serial)
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

    public class ShadowIronGranite : BaseGranite
    {
        [Constructable]
        public ShadowIronGranite()
            : base(CraftResource.ShadowIron)
        {
        }

        public ShadowIronGranite(Serial serial)
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

    public class CopperGranite : BaseGranite
    {
        [Constructable]
        public CopperGranite()
            : base(CraftResource.Copper)
        {
        }

        public CopperGranite(Serial serial)
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

    public class BronzeGranite : BaseGranite
    {
        [Constructable]
        public BronzeGranite()
            : base(CraftResource.Bronze)
        {
        }

        public BronzeGranite(Serial serial)
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

    public class GoldGranite : BaseGranite
    {
        [Constructable]
        public GoldGranite()
            : base(CraftResource.Gold)
        {
        }

        public GoldGranite(Serial serial)
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

    public class AgapiteGranite : BaseGranite
    {
        [Constructable]
        public AgapiteGranite()
            : base(CraftResource.Agapite)
        {
        }

        public AgapiteGranite(Serial serial)
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

    public class VeriteGranite : BaseGranite
    {
        [Constructable]
        public VeriteGranite()
            : base(CraftResource.Verite)
        {
        }

        public VeriteGranite(Serial serial)
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

    public class ValoriteGranite : BaseGranite
    {
        [Constructable]
        public ValoriteGranite()
            : base(CraftResource.Valorite)
        {
        }

        public ValoriteGranite(Serial serial)
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