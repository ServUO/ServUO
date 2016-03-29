using System;

namespace Server.Items
{
    public class BronzedArmorValkyrie : FemaleLeatherChest
    {
        [Constructable]
        public BronzedArmorValkyrie()
        {
            this.Attributes.BonusHits = 5;
			this.Attributes.BonusStr = 5;
			this.Attributes.BonusDex = 5;
			this.Attributes.BonusStam = 5;
			this.Attributes.RegenStam = 3;
			this.Attributes.LowerManaCost = 10;
			this.Hue = 1863; // Hue not exact
			this.Name = ("Bronzed Armor of the Valkyrie");
        }

        public BronzedArmorValkyrie(Serial serial)
            : base(serial)
        {
        }

 public override int BasePhysicalResistance
        {
            get
            {
                return 11;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 14;
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
                return 11;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 8;
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