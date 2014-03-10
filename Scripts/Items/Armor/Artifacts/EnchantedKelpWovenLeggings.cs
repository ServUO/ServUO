using System;

namespace Server.Items
{
    public class EnchantedKelpWovenLeggings : LeatherLegs
    {
        [Constructable]
        public EnchantedKelpWovenLeggings()
        {
            this.Attributes.BonusHits = 5;
			this.Attributes.BonusMana = 8;
			this.Attributes.RegenMana = 2;
			this.Attributes.SpellDamage = 8;
			this.Attributes.LowerRegCost = 15;
			this.Hue = 1267; // Hue probably not exact
			this.AbsorptionAttributes.CastingFocus = 4;
			this.Name = ("Enchanted Kelp Woven Leggings");
        }

        public EnchantedKelpWovenLeggings(Serial serial)
            : base(serial)
        {
        }

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
                return 13;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 12;
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
                return 14;
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}