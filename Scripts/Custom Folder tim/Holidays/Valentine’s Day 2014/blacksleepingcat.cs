using System;

namespace Server.Items
{
    public class Blacksleepingcat : Item
    {
        [Constructable]
        public Blacksleepingcat()
            : base(0x63DC)
        {
            this.Name = "A Kitten Raised By";
            this.Weight = 1.0;
        }

        public Blacksleepingcat(Serial serial)
            : base(serial)
        {
        }

        public void Flip()
        {
            switch (this.ItemID)
            {
                case 0x63DC:
                    this.ItemID = 0x63DF;
                    break;
                case 0x63DD:
                    this.ItemID = 0x63E0;
                    break;
                case 0x63DE:
                    this.ItemID = 0x63E1;
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