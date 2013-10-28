using System;

namespace Server.Items
{
    public class Taffy : CandyCane
    {
        [Constructable]
        public Taffy()
            : this(1)
        {
        }

        public Taffy(int amount)
            : base(0x469D)
        {
            this.Stackable = true;
        }

        public Taffy(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1096949;
            }
        }/* taffy */
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