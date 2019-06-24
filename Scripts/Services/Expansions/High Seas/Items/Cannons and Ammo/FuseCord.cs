using System;
using Server;

namespace Server.Items
{
    [TypeAlias("Server.Items.Fusecord")]
    public class FuseCord : Item, ICommodity
    {
        public override int LabelNumber { get { return 1116305; } }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        [Constructable]
        public FuseCord() : this(1)
        {
        }

        [Constructable]
        public FuseCord(int amt) : base(5152)
        {
            Stackable = true;
            Hue = 782;
            Amount = amt;
        }

        public FuseCord(Serial serial) : base(serial) { }

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
