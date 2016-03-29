using System;

namespace Server.Items
{
    public class IronwoodCrown : RavenHelm
    {
        [Constructable]
        public IronwoodCrown()
        {
            this.Hue = 0x1;

            this.ArmorAttributes.SelfRepair = 3;

            this.Attributes.BonusStr = 5;
            this.Attributes.BonusDex = 5;
            this.Attributes.BonusInt = 5;
        }

        public IronwoodCrown(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072924;
            }
        }// Ironwood Crown
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
                return 6;
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
                return 7;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 10;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}