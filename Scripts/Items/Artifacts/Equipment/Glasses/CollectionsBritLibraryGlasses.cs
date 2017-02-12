using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishMaceAndShieldGlasses))]
    public class MaceAndShieldGlasses : Glasses
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MaceAndShieldGlasses()
            : base()
        {
            this.Hue = 0x1DD;
		
            this.Attributes.BonusStr = 10;
            this.Attributes.BonusDex = 5;
			
            this.WeaponAttributes.HitLowerDefend = 30;
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
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    #region Gargish Mace and Shield
    public class GargishMaceAndShieldGlasses : GargishGlasses
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GargishMaceAndShieldGlasses()
        {
            this.Hue = 0x1DD;

            this.Attributes.BonusStr = 10;
            this.Attributes.BonusDex = 5;

            this.WeaponAttributes.HitLowerDefend = 30;
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
    #endregion

    [Alterable(typeof(DefTinkering), typeof(GargishGlassesOfTheArts))]
    public class GlassesOfTheArts : Glasses
    {
        [Constructable]
        public GlassesOfTheArts()
            : base()
        {
            this.Hue = 0x73;
		
            this.Attributes.BonusInt = 5;
            this.Attributes.BonusStr = 5;
            this.Attributes.BonusHits = 15;
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
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    #region Gargish Glasses of the Arts
    public class GargishGlassesOfTheArts : GargishGlasses
    {
        [Constructable]
        public GargishGlassesOfTheArts()
        {
            this.Hue = 0x73;

            this.Attributes.BonusInt = 5;
            this.Attributes.BonusStr = 5;
            this.Attributes.BonusHits = 15;
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
    #endregion

    [Alterable(typeof(DefTinkering), typeof(GargishTradesGlasses))]
    public class TradesGlasses : Glasses
    {
        [Constructable]
        public TradesGlasses()
        {
            this.Attributes.BonusStr = 10;
            this.Attributes.BonusInt = 10;
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
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    #region Gargish Trades Glasses
    public class GargishTradesGlasses : GargishGlasses
    {
        [Constructable]
        public GargishTradesGlasses()
        {
            this.Attributes.BonusStr = 10;
            this.Attributes.BonusInt = 10;
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
    #endregion

    [Alterable(typeof(DefTinkering), typeof(GargishWizardsCrystalGlasses))]
    public class WizardsCrystalGlasses : Glasses
    {
        [Constructable]
        public WizardsCrystalGlasses()
        {
            this.Hue = 0x2B0;
		
            this.Attributes.BonusMana = 10;
            this.Attributes.RegenMana = 3;
            this.Attributes.SpellDamage = 15;
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
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    #region Gargish Wizards Crystal Glasses
    public class GargishWizardsCrystalGlasses : GargishGlasses
    {
        [Constructable]
        public GargishWizardsCrystalGlasses()
        {
            this.Hue = 0x2B0;

            this.Attributes.BonusMana = 10;
            this.Attributes.RegenMana = 3;
            this.Attributes.SpellDamage = 15;
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
    #endregion

    [Alterable(typeof(DefTinkering), typeof(GargishTreasuresAndTrinketsGlasses))]
    public class TreasuresAndTrinketsGlasses : Glasses
    {
        [Constructable]
        public TreasuresAndTrinketsGlasses()
        {
            this.Hue = 0x5A6; // TODO check
		
            this.Attributes.BonusInt = 10;
            this.Attributes.BonusHits = 5;
            this.Attributes.SpellDamage = 10;
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
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    #region gargish Treasures and Trinkets
    public class GargishTreasuresAndTrinketsGlasses : GargishGlasses
    {
        [Constructable]
        public GargishTreasuresAndTrinketsGlasses()
        {
            this.Hue = 0x5A6; // TODO check

            this.Attributes.BonusInt = 10;
            this.Attributes.BonusHits = 5;
            this.Attributes.SpellDamage = 10;
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
    #endregion
}