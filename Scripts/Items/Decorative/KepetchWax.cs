using System;

namespace Server.Items
{
    public class KepetchWax : Item
    {
        [Constructable]
        public KepetchWax()
            : base(0x5745)
        {
        }

        public KepetchWax(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112412;
            }
        }// kepetch wax
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