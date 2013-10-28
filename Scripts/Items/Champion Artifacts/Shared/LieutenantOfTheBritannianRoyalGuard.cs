using System;

namespace Server.Items
{
    public class LieutenantOfTheBritannianRoyalGuard : BodySash
    {
        [Constructable]
        public LieutenantOfTheBritannianRoyalGuard()
        {
            this.Hue = 0xe8;

            this.Attributes.BonusInt = 5;
            this.Attributes.RegenMana = 2;
            this.Attributes.LowerRegCost = 10;
        }

        public LieutenantOfTheBritannianRoyalGuard(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094910;
            }
        }// Lieutenant of the Britannian Royal Guard [Replica]
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