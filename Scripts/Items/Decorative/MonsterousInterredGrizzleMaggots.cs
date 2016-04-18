using System;

namespace Server.Items
{
    public class MonsterousInterredGrizzleMaggots : Item
    {
        [Constructable]
        public MonsterousInterredGrizzleMaggots()
            : base(0x2633)
        {
        }

        public MonsterousInterredGrizzleMaggots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075090;
            }
        }// Monsterous Interred Grizzle Maggots
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