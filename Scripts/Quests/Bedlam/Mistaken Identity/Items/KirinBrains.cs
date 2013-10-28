using System;

namespace Server.Items
{
    public class KirinBrains : Item
    {
        [Constructable]
        public KirinBrains()
            : base(0x1CF0)
        {
            this.LootType = LootType.Blessed;			
            this.Weight = 1;
            this.Hue = 0xD7;
        }

        public KirinBrains(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074612;
            }
        }// Ki-Rin Brains
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