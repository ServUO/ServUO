using System;

namespace Server.Items
{
    public class DecoBottlesOfLiquor : Item
    {
        [Constructable]
        public DecoBottlesOfLiquor()
            : base(0x99E)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoBottlesOfLiquor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}