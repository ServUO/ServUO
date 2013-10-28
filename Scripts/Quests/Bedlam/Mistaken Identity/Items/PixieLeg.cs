using System;

namespace Server.Items
{
    public class PixieLeg : Item
    {
        [Constructable]
        public PixieLeg()
            : this(1)
        {
        }

        [Constructable]
        public PixieLeg(int amount)
            : base(0x1608)
        {
            this.LootType = LootType.Blessed;			
            this.Weight = 1;
            this.Hue = 0x1C2;
			
            this.Stackable = true;
            this.Amount = amount;
        }

        public PixieLeg(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074613;
            }
        }// Pixie Leg
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