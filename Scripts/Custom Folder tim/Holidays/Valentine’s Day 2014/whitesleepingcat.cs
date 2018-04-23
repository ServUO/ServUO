using System;

namespace Server.Items
{
    public class Whitesleepingcat : Item
    {
        [Constructable]
        public Whitesleepingcat()
            : base(0x63D6)
        {
            this.Name = "A Kitten Raised By";
            this.Weight = 1.0;
        }

        public Whitesleepingcat(Serial serial)
            : base(serial)
        {
        }

        public void Flip()
        {
            switch (this.ItemID)
            {
                case 0x63D6:
                    this.ItemID = 0x63D9;
                    break;
                case 0x63D7:
                    this.ItemID = 0x63DA;
                    break;
                case 0x63D8:
                    this.ItemID = 0x63DB;
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