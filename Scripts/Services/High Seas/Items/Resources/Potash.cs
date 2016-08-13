using System;
using Server;

namespace Server.Items
{
    public class Potash : Item, ICommodity
    {
        public override int LabelNumber { get { return 1116319; } }

        int ICommodity.DescriptionNumber { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        [Constructable]
        public Potash() : this(1)
        {
        }

        [Constructable]
        public Potash(int amount) : base(16954)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1905;
        }

        public Potash(Serial serial) : base(serial) { }

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