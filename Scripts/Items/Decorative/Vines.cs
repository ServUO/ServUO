using System;

namespace Server.Items
{
    public class Vines : Item
    {
        [Constructable]
        public Vines()
            : this(Utility.Random(8))
        {
        }

        [Constructable]
        public Vines(int v)
            : base(0xCEB)
        {
            if (v < 0 || v > 7)
                v = 0;

            this.ItemID += v;
            this.Weight = 1.0;
        }

        public Vines(Serial serial)
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