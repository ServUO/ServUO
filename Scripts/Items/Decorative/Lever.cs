// By Nerun

namespace Server.Items
{
    public class Lever : Item
    {
        [Constructable]
        public Lever()
            : base(Utility.RandomList(0x108C, 0x108D, 0x108E, 0x1093, 0x1094, 0x1095, 0x108F, 0x1090, 0x1091, 0x1092))
        {
            Movable = false;
        }

        public Lever(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(500357 + Utility.Random(5));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}