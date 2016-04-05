using System;

namespace Server.Items
{
    public class RoyalZooStuddedLegs : StuddedLegs
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RoyalZooStuddedLegs()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusHits = 2;
            this.Attributes.BonusMana = 3;
            this.Attributes.LowerManaCost = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedLegs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073223;
            }
        }// Studded Armor of the Britannia Royal Zoo
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

    public class RoyalZooStuddedGloves : StuddedGloves
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RoyalZooStuddedGloves()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusHits = 2;
            this.Attributes.BonusMana = 3;
            this.Attributes.LowerManaCost = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedGloves(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073223;
            }
        }// Studded Armor of the Britannia Royal Zoo
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

    public class RoyalZooStuddedGorget : StuddedGorget
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RoyalZooStuddedGorget()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusHits = 2;
            this.Attributes.BonusMana = 3;
            this.Attributes.LowerManaCost = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedGorget(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073223;
            }
        }// Studded Armor of the Britannia Royal Zoo
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

    public class RoyalZooStuddedArms : StuddedArms
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RoyalZooStuddedArms()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusHits = 2;
            this.Attributes.BonusMana = 3;
            this.Attributes.LowerManaCost = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073223;
            }
        }// Studded Armor of the Britannia Royal Zoo
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

    public class RoyalZooStuddedChest : StuddedChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RoyalZooStuddedChest()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusHits = 2;
            this.Attributes.BonusMana = 3;
            this.Attributes.LowerManaCost = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073223;
            }
        }// Studded Armor of the Britannia Royal Zoo
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

    public class RoyalZooStuddedFemaleChest : FemaleStuddedChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RoyalZooStuddedFemaleChest()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusHits = 2;
            this.Attributes.BonusMana = 3;
            this.Attributes.LowerManaCost = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedFemaleChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073223;
            }
        }// Studded Armor of the Britannia Royal Zoo
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
}