using System;

namespace Server.Items
{
    public class GladiatorsCollar : PlateGorget
    {
        [Constructable]
        public GladiatorsCollar()
        {
            this.Hue = 0x26d;

            this.Attributes.BonusHits = 10;
            this.Attributes.AttackChance = 10;

            this.ArmorAttributes.MageArmor = 1;
        }

        public GladiatorsCollar(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094917;
            }
        }// Gladiator's Collar [Replica]
        public override int BasePhysicalResistance
        {
            get
            {
                return 18;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 18;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 17;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 18;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 16;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
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