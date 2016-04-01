using System;

namespace Server.Items
{
    public class ProtectoroftheBattleMage : LeatherChest
    {
        [Constructable]
        public ProtectoroftheBattleMage()
            : base()
        {
            this.Name = ("Protector of the Battle Mage");
		
            this.Hue = 1159;
			
            this.Attributes.LowerManaCost = 8;	
            this.Attributes.RegenMana = 2;
            this.Attributes.LowerRegCost = 10;
            this.Attributes.SpellDamage = 5;
            this.AbsorptionAttributes.CastingFocus = 3;
        }

        public ProtectoroftheBattleMage(Serial serial)
            : base(serial)
        {
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
                return 16;
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
                return 8;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 8;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}