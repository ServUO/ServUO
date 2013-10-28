using System;

namespace Server.Items
{
    public class UnicornRibs : Item
    {
        [Constructable]
        public UnicornRibs()
            : base(0x9F1)
        {
            this.LootType = LootType.Blessed;			
            this.Weight = 1;
            this.Hue = 0x14B;
        }

        public UnicornRibs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074611;
            }
        }// Unicorn Ribs
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