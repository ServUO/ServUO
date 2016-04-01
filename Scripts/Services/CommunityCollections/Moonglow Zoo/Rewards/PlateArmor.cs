using System;

namespace Server.Items
{
    public class RoyalZooPlateLegs : PlateLegs
    {
        [Constructable]
        public RoyalZooPlateLegs()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.Luck = 100;
            this.Attributes.DefendChance = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateLegs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073224;
            }
        }// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateGloves : PlateGloves
    {
        [Constructable]
        public RoyalZooPlateGloves()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.Luck = 100;
            this.Attributes.DefendChance = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateGloves(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073224;
            }
        }// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateGorget : PlateGorget
    {
        [Constructable]
        public RoyalZooPlateGorget()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.Luck = 100;
            this.Attributes.DefendChance = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateGorget(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073224;
            }
        }// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateArms : PlateArms
    {
        [Constructable]
        public RoyalZooPlateArms()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.Luck = 100;
            this.Attributes.DefendChance = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073224;
            }
        }// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateChest : PlateChest
    {
        [Constructable]
        public RoyalZooPlateChest()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.Luck = 100;
            this.Attributes.DefendChance = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073224;
            }
        }// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateFemaleChest : FemalePlateChest
    {
        [Constructable]
        public RoyalZooPlateFemaleChest()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.Luck = 100;
            this.Attributes.DefendChance = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateFemaleChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073224;
            }
        }// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateHelm : PlateHelm
    {
        [Constructable]
        public RoyalZooPlateHelm()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.Luck = 100;
            this.Attributes.DefendChance = 10;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073224;
            }
        }// Platemail Armor of the Britannia Royal Zoo
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