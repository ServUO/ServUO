using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishFoldedSteelGlasses))]
    public class FoldedSteelGlasses : ElvenGlasses
	{
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public FoldedSteelGlasses()
        {
            Attributes.BonusStr = 8;
            Attributes.NightSight = 1;
            Attributes.DefendChance = 15;
            Hue = 0x47E;
        }

        public FoldedSteelGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073380;
            }
        }//Folded Steel Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 10;
            }
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GargishFoldedSteelGlasses : GargishGlasses
    {
        public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public GargishFoldedSteelGlasses()
        {
            Attributes.BonusStr = 8;
            Attributes.NightSight = 1;
            Attributes.DefendChance = 15;
            Hue = 0x47E;
        }

        public GargishFoldedSteelGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073380;
            }
        }//Folded Steel Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 10;
            }
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}