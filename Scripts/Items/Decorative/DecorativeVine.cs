using System;

namespace Server.Items
{
    public class DecorativeVines : Item
    {
        [Constructable]
        public DecorativeVines()
            : this(Utility.Random(4))
        {
        }

        [Constructable]
        public DecorativeVines(int v)
            : base(0x2CF9)
        {
            if (v < 0 || v > 3)
                v = 0;

            this.ItemID += v;
            this.Weight = 1.0;
        }

        public DecorativeVines(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
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