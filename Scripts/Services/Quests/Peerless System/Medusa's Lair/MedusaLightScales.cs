using System;

namespace Server.Items
{
    public class MedusaLightScales : Item
    {
        [Constructable]
        public MedusaLightScales()
            : this(1)
        {
        }

        [Constructable]
        public MedusaLightScales(int amount)
            : base(9908)
        {
            this.Hue = 1266;
            this.Stackable = true;
            this.Amount = amount;
        }

        public MedusaLightScales(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112626;
            }
        }// Medusa Scales
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