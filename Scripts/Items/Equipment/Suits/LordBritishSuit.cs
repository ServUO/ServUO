using System;

namespace Server.Items
{
    public class LordBritishSuit : BaseSuit
    {
        [Constructable]
        public LordBritishSuit()
            : base(AccessLevel.GameMaster, 0x0, 0x2042)
        {
        }

        public LordBritishSuit(Serial serial)
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