using System;

namespace Server.Items
{
    public class InspectedKegOfTotalRefreshmentPotions : Item
    {
        public override int LabelNumber { get { return 1113025; } } // Inspected Keg of Total Refreshment Potions

        [Constructable]
        public InspectedKegOfTotalRefreshmentPotions()
            : base(0x1940)
        {
            Weight = 100.0;
        }

        public InspectedKegOfTotalRefreshmentPotions(Serial serial)
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