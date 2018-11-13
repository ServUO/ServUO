using System;

namespace Server.Items
{
    public class ProtectoroftheBattleMage : LeatherChest
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113761; } } // Protector of the Battle Mage
		
        [Constructable]
        public ProtectoroftheBattleMage()
            : base()
        {
            Hue = 1159;		
            Attributes.LowerManaCost = 8;	
            Attributes.RegenMana = 2;
            Attributes.LowerRegCost = 10;
            Attributes.SpellDamage = 5;
            AbsorptionAttributes.CastingFocus = 3;
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