using System;

namespace Server.Items
{
    public class ParoxysmusDinner : Item
    {
        [Constructable]
        public ParoxysmusDinner()
            : base(0x1E95)
        {
        }

        public ParoxysmusDinner(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072086;
            }
        }// Paroxysmus' Dinner
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