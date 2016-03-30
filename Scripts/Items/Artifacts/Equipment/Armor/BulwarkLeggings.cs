using System;

namespace Server.Items
{
    public class BulwarkLeggings : RingmailLegs
    {
        [Constructable]
        public BulwarkLeggings()
        {
            this.LootType = LootType.Blessed;

            this.Attributes.RegenStam = 1;
            this.Attributes.RegenMana = 1;
        }

        public BulwarkLeggings(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1077727;
            }
        }// Bulwark Leggings
        public override int BasePhysicalResistance
        {
            get
            {
                return 9;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 3;
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