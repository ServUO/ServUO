using System;
using Server;

namespace Server.Items
{
    public class SnakeSkinBoots : Boots
    {
        public override int LabelNumber { get { return 1080122; } } // Snake Skin Boots

        [Constructable]
        public SnakeSkinBoots()
        {
            Hue = 0x7D9;
            Resistances.Poison = 2;
        }

        public SnakeSkinBoots(Serial serial)
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