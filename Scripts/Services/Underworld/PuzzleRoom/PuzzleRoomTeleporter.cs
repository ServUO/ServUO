namespace Server.Items
{
    public class PuzzleRoomTeleporter : Teleporter
    {
        [Constructable]
        public PuzzleRoomTeleporter()
        {
        }

        public override bool CanTeleport(Mobile m)
        {
            if (m.Backpack == null)
                return false;

            Item item = m.Backpack.FindItemByType(typeof(MagicKey));

            if (item == null)
            {
                m.SendLocalizedMessage(1113393); // You must carry a magic key to enter this room.
                return false;
            }

            return base.CanTeleport(m);
        }

        public PuzzleRoomTeleporter(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}