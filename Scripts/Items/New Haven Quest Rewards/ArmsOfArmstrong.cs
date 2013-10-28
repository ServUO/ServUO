using System;

namespace Server.Items
{
    public class ArmsOfArmstrong : LeatherArms
    {
        [Constructable]
        public ArmsOfArmstrong()
        {
            this.LootType = LootType.Blessed;

            this.Attributes.BonusStr = 3;
            this.Attributes.RegenHits = 1;
        }

        public ArmsOfArmstrong(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1077675;
            }
        }// Arms of Armstrong
        public override int BasePhysicalResistance
        {
            get
            {
                return 6;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 6;
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
                return 5;
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