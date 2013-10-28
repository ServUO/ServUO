using System;

namespace Server.Items
{
    public class EssenceSingularity : Item
    {
        [Constructable]
        public EssenceSingularity()
            : this(1)
        {
        }

        [Constructable]
        public EssenceSingularity(int amount)
            : base(0x571C)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Hue = 1109;
        }

        public EssenceSingularity(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113341;
            }
        }// essence of singularity
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