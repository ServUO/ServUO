using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishBrightsightLenses))]
    public class BrightsightLenses : ElvenGlasses
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BrightsightLenses()
        {
            this.Hue = 0x501;

            this.Attributes.NightSight = 1;
            this.Attributes.RegenMana = 3;

            this.ArmorAttributes.SelfRepair = 3;
        }

        public BrightsightLenses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075039;
            }
        }// Brightsight Lenses
        public override int BasePhysicalResistance
        {
            get
            {
                return 9;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 29;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 7;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                this.WeaponAttributes.SelfRepair = 0;
                this.ArmorAttributes.SelfRepair = 3;
            }
        }
    }

    [Alterable(typeof(DefTinkering), typeof(GargishBrightsightLenses))]
    public class GargishBrightsightLenses : GargishGlasses
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishBrightsightLenses()
        {
            this.Hue = 0x501;

            this.Attributes.NightSight = 1;
            this.Attributes.RegenMana = 3;

            this.ArmorAttributes.SelfRepair = 3;
        }

        public GargishBrightsightLenses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075039;
            }
        }// Brightsight Lenses
        public override int BasePhysicalResistance
        {
            get
            {
                return 9;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 29;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 7;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}