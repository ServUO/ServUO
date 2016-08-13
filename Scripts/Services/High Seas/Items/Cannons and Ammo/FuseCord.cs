using System;
using Server;

namespace Server.Items
{
    public class Fusecord : Item, ICommodity
    {
        public override int LabelNumber { get { return 1116305; } }

        int ICommodity.DescriptionNumber { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        [Constructable]
        public Fusecord() : this(1)
        {
        }

        [Constructable]
        public Fusecord(int amt) : base(5152)
        {
            Stackable = true;
            Hue = 782;
            Amount = amt;
        }

        public Fusecord(Serial serial) : base(serial) { }

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