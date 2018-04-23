using System;

namespace Server.Items
{
    public class Orangesleepingcat : Item
    {
        [Constructable]
        public Orangesleepingcat()
            : base(0x63E2)
        {
            this.Name = "A Kitten Raised By";
            this.Weight = 1.0;
        }

        public Orangesleepingcat(Serial serial)
            : base(serial)
        {
        }

        public void Flip()
        {
            switch (this.ItemID)
            {
                case 0x63E2:
                    this.ItemID = 0x63E5;
                    break;
                case 0x63E3:
                    this.ItemID = 0x63E6;
                    break;
                case 0x63E4:
                    this.ItemID = 0x63E7;
                    break;
            }
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