using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.LavaSerpenCrust")]
    public class LavaSerpentCrust : Item
    {
        [Constructable]
        public LavaSerpentCrust()
            : this(1)
        {
        }

        [Constructable]
        public LavaSerpentCrust(int amount)
            : base(0x572D)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public LavaSerpentCrust(Serial serial)
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