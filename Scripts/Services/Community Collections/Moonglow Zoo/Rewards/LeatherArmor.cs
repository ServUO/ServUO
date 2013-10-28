using System;

namespace Server.Items
{
    public class RoyalZooLeatherLegs : LeatherLegs
    {
        [Constructable]
        public RoyalZooLeatherLegs()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusMana = 3;
            this.Attributes.RegenStam = 3;
            this.Attributes.ReflectPhysical = 10;
            this.Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherLegs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073222;
            }
        }// Leather Armor of the Britannia Royal Zoo
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

    public class RoyalZooLeatherGloves : LeatherGloves
    {
        [Constructable]
        public RoyalZooLeatherGloves()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusMana = 3;
            this.Attributes.RegenStam = 3;
            this.Attributes.ReflectPhysical = 10;
            this.Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherGloves(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073222;
            }
        }// Leather Armor of the Britannia Royal Zoo
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

    public class RoyalZooLeatherGorget : LeatherGorget
    {
        [Constructable]
        public RoyalZooLeatherGorget()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusMana = 3;
            this.Attributes.RegenStam = 3;
            this.Attributes.ReflectPhysical = 10;
            this.Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherGorget(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073222;
            }
        }// Leather Armor of the Britannia Royal Zoo
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

    public class RoyalZooLeatherArms : LeatherArms
    {
        [Constructable]
        public RoyalZooLeatherArms()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusMana = 3;
            this.Attributes.RegenStam = 3;
            this.Attributes.ReflectPhysical = 10;
            this.Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073222;
            }
        }// Leather Armor of the Britannia Royal Zoo
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

    public class RoyalZooLeatherChest : LeatherChest
    {
        [Constructable]
        public RoyalZooLeatherChest()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusMana = 3;
            this.Attributes.RegenStam = 3;
            this.Attributes.ReflectPhysical = 10;
            this.Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073222;
            }
        }// Leather Armor of the Britannia Royal Zoo
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

    public class RoyalZooLeatherFemaleChest : FemaleLeatherChest
    {
        [Constructable]
        public RoyalZooLeatherFemaleChest()
            : base()
        {
            this.Hue = 0x109;
		
            this.Attributes.BonusMana = 3;
            this.Attributes.RegenStam = 3;
            this.Attributes.ReflectPhysical = 10;
            this.Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherFemaleChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073222;
            }
        }// Leather Armor of the Britannia Royal Zoo
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