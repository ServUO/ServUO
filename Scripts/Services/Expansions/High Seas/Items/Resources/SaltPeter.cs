using System;
using Server;

namespace Server.Items
{
    public class Saltpeter : Item, ICommodity
    {
        public override int LabelNumber { get { return 1116302; } }

        int ICommodity.DescriptionNumber { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        [Constructable]
        public Saltpeter() : this(1)
        {
        }

        [Constructable]
        public Saltpeter(int count) : base(16954)
        {
            Stackable = true;
            Amount = count;
            Hue = 1150;
        }

        public Saltpeter(Serial serial) : base(serial) { }

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