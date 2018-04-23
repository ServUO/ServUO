using System;

namespace Server.Items
{
    public class Blackwhitesleepingdog : Item
    {
        [Constructable]
        public Blackwhitesleepingdog()
            : base(0x63F4)
        {
            this.Name = "A Puppy Raised By";
            this.Weight = 1.0;
        }

        public Blackwhitesleepingdog(Serial serial)
            : base(serial)
        {
        }

        public void Flip()
        {
            switch (this.ItemID)
            {
                case 0x63F4:
                    this.ItemID = 0x63F7;
                    break;
                case 0x63F5:
                    this.ItemID = 0x63F8;
                    break;
                case 0x63F6:
                    this.ItemID = 0x63F9;
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