using System;

namespace Server.Items
{
    public class Blacksleepingdog : Item
    {
        [Constructable]
        public Blacksleepingdog()
            : base(0x63EE)
        {
            this.Name = "A Puppy Raised By";
            this.Weight = 1.0;
        }

        public Blacksleepingdog(Serial serial)
            : base(serial)
        {
        }

        public void Flip()
        {
            switch (this.ItemID)
            {
                case 0x63EE:
                    this.ItemID = 0x63F1;
                    break;
                case 0x63EF:
                    this.ItemID = 0x63F2;
                    break;
                case 0x63F0:
                    this.ItemID = 0x63F3;
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