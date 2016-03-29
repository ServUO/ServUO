using System;

namespace Server.Items
{
    public class AnimatedLegsoftheInsaneTinker : PlateLegs
    {
        [Constructable]
        public AnimatedLegsoftheInsaneTinker()
            : base()
        {
            this.Name = ("Animated Legs of the Insane Tinker");
            this.Hue = 2310;
            this.Attributes.BonusDex = 5;
            this.Attributes.RegenStam = 2;
            this.Attributes.WeaponDamage = 10;
            this.Attributes.WeaponSpeed = 10;
        }

        public AnimatedLegsoftheInsaneTinker(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 17;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 7;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 2;
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
        public override int AosStrReq
        {
            get
            {
                return 45;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 45;
            }
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }
    }
}