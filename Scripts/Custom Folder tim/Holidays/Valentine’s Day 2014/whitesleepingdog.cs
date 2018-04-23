using System;

namespace Server.Items
{
    public class Whitesleepingdog : Item
    {
        [Constructable]
        public Whitesleepingdog()
            : base(0x63E8)
        {
            this.Name = "A Puppy Raised By";
            this.Weight = 1.0;
        }

        public Whitesleepingdog(Serial serial)
            : base(serial)
        {
        }

        public void Flip()
        {
            switch (this.ItemID)
            {
                case 0x63E8:
                    this.ItemID = 0x63EB;
                    break;
                case 0x63E9:
                    this.ItemID = 0x63EC;
                    break;
                case 0x63EA:
                    this.ItemID = 0x63ED;
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