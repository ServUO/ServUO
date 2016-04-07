using System;

namespace Server.Items
{
    public class TreefellowWood : Item
    {
        [Constructable]
        public TreefellowWood()
            : base(0x1BDD)
        {
            this.Name = "Treefellow Wood";  

            this.Hue = 2425;
            this.Movable = true;
        }

        public TreefellowWood(Serial serial)
            : base(serial)
        {
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