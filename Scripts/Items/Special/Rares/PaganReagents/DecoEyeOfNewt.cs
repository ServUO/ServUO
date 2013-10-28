using System;

namespace Server.Items
{
    public class DecoEyeOfNewt : Item
    {
        [Constructable]
        public DecoEyeOfNewt()
            : base(0xF87)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoEyeOfNewt(Serial serial)
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