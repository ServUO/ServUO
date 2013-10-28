using System;

namespace Server.Items
{
    public class FlaskOfOil : Item
    {
        [Constructable]
        public FlaskOfOil()
            : this(1)
        {
        }

        [Constructable]
        public FlaskOfOil(int amount)
            : base(0xEFF)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Amount = amount;
            this.Hue = 33;
        }

        public FlaskOfOil(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1027199;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}