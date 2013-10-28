using System;

namespace Server.Items
{
    public class CrossbowBolts : Item
    {
        [Constructable]
        public CrossbowBolts()
            : base(0x1BFC)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public CrossbowBolts(Serial serial)
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