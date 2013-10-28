using System;

namespace Server.Items
{
    public class AlchemistsBandage : Item
    {
        [Constructable]
        public AlchemistsBandage()
            : base(0xE21)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
            this.Hue = 0x482;
        }

        public AlchemistsBandage(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075452;
            }
        }// Alchemist's Bandage
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