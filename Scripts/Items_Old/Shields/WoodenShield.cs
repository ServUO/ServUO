using System;

namespace Server.Items
{
    public class WoodenShield : BaseShield
    {
        [Constructable]
        public WoodenShield()
            : base(0x1B7A)
        {
            this.Weight = 5.0;
        }

        public WoodenShield(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 1;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 20;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 25;
            }
        }
        public override int AosStrReq
        {
            get
            {
                return 20;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 8;
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

            writer.Write((int)0);//version
        }
    }
}