using System;

namespace Server.Items
{
    [FlipableAttribute(0x14F3, 0x14F4)]
    public class ToyBoat : Item
    {
        [Constructable]
        public ToyBoat()
            : base(0x14F4)
        {
        }

        public ToyBoat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074363;
            }
        }// A toy boat
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
        }
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1073634); // An aquarium decoration
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