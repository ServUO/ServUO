using System;

namespace Server.Items
{
    public class HumanSignOfChaos : ChaosShield
    {
        [Constructable]
        public HumanSignOfChaos()
            : base()
        {
            this.Name = "Sign of Chaos";
            this.Hue = 2075;

            this.ArmorAttributes.SoulCharge = 20;
            this.Attributes.AttackChance = 5;
            this.Attributes.DefendChance = 10;
            this.Attributes.CastSpeed = 1;
        }

        public HumanSignOfChaos(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 3;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 2;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 2;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 2;
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