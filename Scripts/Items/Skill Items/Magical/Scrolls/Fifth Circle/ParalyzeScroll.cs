using System;

namespace Server.Items
{
    public class ParalyzeScroll : SpellScroll
    {
        [Constructable]
        public ParalyzeScroll()
            : this(1)
        {
        }

        [Constructable]
        public ParalyzeScroll(int amount)
            : base(37, 0x1F52, amount)
        {
        }

        public ParalyzeScroll(Serial serial)
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