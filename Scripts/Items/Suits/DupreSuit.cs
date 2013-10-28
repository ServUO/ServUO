using System;

namespace Server.Items
{
    public class DupreSuit : BaseSuit
    {
        [Constructable]
        public DupreSuit()
            : base(AccessLevel.GameMaster, 0x0, 0x2050)
        {
        }

        public DupreSuit(Serial serial)
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