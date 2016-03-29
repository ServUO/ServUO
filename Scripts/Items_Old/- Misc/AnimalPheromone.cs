using System;

namespace Server.Items
{
    public class AnimalPheromone : Item
    {
        [Constructable]
        public AnimalPheromone()
            : base(0x182F)
        {
        }

        public AnimalPheromone(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1071200;
            }
        }//  animal pheromone
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