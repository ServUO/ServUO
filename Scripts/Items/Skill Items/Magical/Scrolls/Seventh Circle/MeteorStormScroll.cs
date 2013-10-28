using System;

namespace Server.Items
{
    public class MeteorSwarmScroll : SpellScroll
    {
        [Constructable]
        public MeteorSwarmScroll()
            : this(1)
        {
        }

        [Constructable]
        public MeteorSwarmScroll(int amount)
            : base(54, 0x1F63, amount)
        {
        }

        public MeteorSwarmScroll(Serial serial)
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