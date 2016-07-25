using System;

namespace Server.Items
{
    public class InspectedKegOfGreaterPoisonPotions : Item
    {
        public override int LabelNumber { get { return 1113026; } } // Inspected Keg of Greater Poison Potion

        [Constructable]
        public InspectedKegOfGreaterPoisonPotions()
            : base(0x1940)
        {
            Weight = 100.0;
        }

        public InspectedKegOfGreaterPoisonPotions(Serial serial)
            : base(serial)
        {
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