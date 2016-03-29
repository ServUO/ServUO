using System;

namespace Server.Items
{
    public class HumanSignOfOrder : OrderShield
    {
        [Constructable]
        public HumanSignOfOrder()
            : base()
        {
            this.Name = "Sign of Order";

            this.SkillBonuses.SetValues(0, SkillName.Chivalry, 10.0);
            this.Attributes.AttackChance = 5;
            this.Attributes.DefendChance = 10;
            this.Attributes.CastSpeed = 1;
            this.Attributes.CastRecovery = 1;
        }

        public HumanSignOfOrder(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 1;
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
                return 0;
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
            writer.Write((int)0); //version
        }
    }
}