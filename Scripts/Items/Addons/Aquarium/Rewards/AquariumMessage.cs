using System;

namespace Server.Items
{
    public class AquariumMessage : MessageInABottle
    {
        [Constructable]
        public AquariumMessage()
            : base()
        {
        }

        public AquariumMessage(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073894;
            }
        }// Message in a Bottle
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