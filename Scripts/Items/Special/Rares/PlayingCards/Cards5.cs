using System;

namespace Server.Items
{
    public class DecoCards5 : Item
    {
        [Constructable]
        public DecoCards5()
            : base(0xE18)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoCards5(Serial serial)
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