using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishMaceAndShieldGlasses))]
    public class MaceAndShieldGlasses : Glasses
	{
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public MaceAndShieldGlasses()
            : base()
        {
            Hue = 0x1DD;		
            Attributes.BonusStr = 10;
            Attributes.BonusDex = 5;		
            WeaponAttributes.HitLowerDefend = 30;
        }

        public MaceAndShieldGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073381;
            }
        }// Mace and Shield Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 25;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);			
            int version = reader.ReadInt();
        }
    }

    public class GargishMaceAndShieldGlasses : GargishGlasses
    {
        public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public GargishMaceAndShieldGlasses()
        {
            Hue = 0x1DD;
            Attributes.BonusStr = 10;
            Attributes.BonusDex = 5;
            WeaponAttributes.HitLowerDefend = 30;
        }

        public GargishMaceAndShieldGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073381;
            }
        }// Mace and Shield Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 25;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Alterable(typeof(DefTinkering), typeof(GargishGlassesOfTheArts))]
    public class GlassesOfTheArts : Glasses
    {
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public GlassesOfTheArts()
            : base()
        {
            Hue = 0x73;	
            Attributes.BonusInt = 5;
            Attributes.BonusStr = 5;
            Attributes.BonusHits = 15;
        }

        public GlassesOfTheArts(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073363;
            }
        }// Reading Glasses of the Arts
        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 4;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);			
            int version = reader.ReadInt();
        }
    }

    public class GargishGlassesOfTheArts : GargishGlasses
    {
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public GargishGlassesOfTheArts()
        {
            Hue = 0x73;
            Attributes.BonusInt = 5;
            Attributes.BonusStr = 5;
            Attributes.BonusHits = 15;
        }

        public GargishGlassesOfTheArts(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073363;
            }
        }// Reading Glasses of the Arts
        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 4;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Alterable(typeof(DefTinkering), typeof(GargishTradesGlasses))]
    public class TradesGlasses : Glasses
    {
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public TradesGlasses()
        {
            Attributes.BonusStr = 10;
            Attributes.BonusInt = 10;
        }

        public TradesGlasses(Serial serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073362;
            }
        }// Reading Glasses of the Trades
        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);		
            int version = reader.ReadInt();
        }
    }

    public class GargishTradesGlasses : GargishGlasses
    {
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public GargishTradesGlasses()
        {
            Attributes.BonusStr = 10;
            Attributes.BonusInt = 10;
        }

        public GargishTradesGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073362;
            }
        }// Reading Glasses of the Trades
        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Alterable(typeof(DefTinkering), typeof(GargishWizardsCrystalGlasses))]
    public class WizardsCrystalGlasses : Glasses
    {
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public WizardsCrystalGlasses()
        {
            Hue = 0x2B0;	
            Attributes.BonusMana = 10;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 15;
        }

        public WizardsCrystalGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073374;
            }
        }// Wizard's Crystal Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 5;
            }
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

    public class GargishWizardsCrystalGlasses : GargishGlasses
    {
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public GargishWizardsCrystalGlasses()
        {
            Hue = 0x2B0;
            Attributes.BonusMana = 10;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 15;
        }

        public GargishWizardsCrystalGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073374;
            }
        }// Wizard's Crystal Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 5;
            }
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

    [Alterable(typeof(DefTinkering), typeof(GargishTreasuresAndTrinketsGlasses))]
    public class TreasuresAndTrinketsGlasses : Glasses
    {
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public TreasuresAndTrinketsGlasses()
        {
            Hue = 0x5A6; // TODO check		
            Attributes.BonusInt = 10;
            Attributes.BonusHits = 5;
            Attributes.SpellDamage = 10;
        }

        public TreasuresAndTrinketsGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073373;
            }
        }// Treasures and Trinkets Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);		
            int version = reader.ReadInt();
        }
    }

    public class GargishTreasuresAndTrinketsGlasses : GargishGlasses
    {
		public override bool IsArtifact { get { return true; } }
		public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
		
        [Constructable]
        public GargishTreasuresAndTrinketsGlasses()
        {
            Hue = 0x5A6; // TODO check
            Attributes.BonusInt = 10;
            Attributes.BonusHits = 5;
            Attributes.SpellDamage = 10;
        }

        public GargishTreasuresAndTrinketsGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073373;
            }
        }// Treasures and Trinkets Reading Glasses
        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}