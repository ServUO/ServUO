using System;

namespace Server.Items
{
    public class LavaSerpenCrust : Item
    {
        [Constructable]
        public LavaSerpenCrust()
            : this(1)
        {
        }

        [Constructable]
        public LavaSerpenCrust(int amount)
            : base(0x572D)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public LavaSerpenCrust(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113336;
            }
        }// lava serpent crust
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