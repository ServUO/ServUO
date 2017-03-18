using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VvV
{
    public class VvVWoodlandArms : WoodlandArms
	{
        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 6; } }
        public override int BaseColdResistance { get { return 17; } }
        public override int BasePoisonResistance { get { return 18; } }
        public override int BaseEnergyResistance { get { return 18; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public VvVWoodlandArms()
        {
            IsVvVItem = true;

            Attributes.BonusDex = 4;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 10;
            Attributes.RegenStam = 3;
        }

        public VvVWoodlandArms(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}

    public class VvVDragonArms : DragonArms
    {
        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 6; } }
        public override int BaseColdResistance { get { return 17; } }
        public override int BasePoisonResistance { get { return 18; } }
        public override int BaseEnergyResistance { get { return 18; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public VvVDragonArms()
        {
            IsVvVItem = true;

            Attributes.BonusDex = 4;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 10;
            Attributes.RegenStam = 3;
        }

        public VvVDragonArms(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VvVGargishPlateArms : GargishPlateArms
    {
        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 6; } }
        public override int BaseColdResistance { get { return 17; } }
        public override int BasePoisonResistance { get { return 18; } }
        public override int BaseEnergyResistance { get { return 18; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public VvVGargishPlateArms()
        {
            IsVvVItem = true;

            Attributes.BonusDex = 4;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 10;
            Attributes.RegenStam = 3;
        }

        public VvVGargishPlateArms(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VvVPlateArms : PlateArms
    {
        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 6; } }
        public override int BaseColdResistance { get { return 17; } }
        public override int BasePoisonResistance { get { return 18; } }
        public override int BaseEnergyResistance { get { return 18; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public VvVPlateArms()
        {
            IsVvVItem = true;

            Resource = CraftResource.None;

            Attributes.BonusDex = 4;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 10;
            Attributes.RegenStam = 3;
        }

        public VvVPlateArms(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}