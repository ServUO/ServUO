namespace Server.Items
{

    public class ExperimentalRoomDoor : MetalDoor2
    {

        public override string DefaultName => "a door";

        private Room m_Room;

        [CommandProperty(AccessLevel.GameMaster)]
        public Room Room
        {
            get { return m_Room; }
            set { m_Room = value; }
        }

        [Constructable]
        public ExperimentalRoomDoor(Room room, DoorFacing facing) : base(facing)
        {
            m_Room = room;
        }

        public ExperimentalRoomDoor(Serial serial) : base(serial)
        {

        }

        public override void Use(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
            {
                from.SendMessage("You open the door with your godly powers.");
                base.Use(from);
                return;
            }

            Container pack = from.Backpack;
            bool hasGem = false;

            if (pack != null)
            {
                Item[] items = pack.FindItemsByType(typeof(ExperimentalGem));

                if (items != null && items.Length > 0)
                {
                    hasGem = true;

                    foreach (Item item in items)
                    {
                        ExperimentalGem gem = (ExperimentalGem)item;

                        if (gem.Active && (gem.CurrentRoom > m_Room || m_Room == Room.RoomZero))
                        {
                            base.Use(from);
                            return;
                        }
                    }
                }
                else
                    from.SendLocalizedMessage(1113410); // You must have an active Experimental Gem to enter that room.
            }

            if (hasGem)
                from.SendLocalizedMessage(1113411); // You have not yet earned access to that room!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
            writer.Write((int)m_Room);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Room = (Room)reader.ReadInt();
        }
    }
}

namespace Server.Items
{

    public class ExperimentalRoomBlocker : Item
    {
        private Room m_Room;

        [CommandProperty(AccessLevel.GameMaster)]
        public Room Room
        {
            get { return m_Room; }
            set { m_Room = value; }
        }

        [Constructable]
        public ExperimentalRoomBlocker(Room room) : base(7107)
        {
            m_Room = room;

            Visible = false;
            Movable = false;
        }

        public ExperimentalRoomBlocker(Serial serial) : base(serial)
        {

        }

        public override bool OnMoveOver(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            Container pack = from.Backpack;

            if (pack != null)
            {
                Item[] items = pack.FindItemsByType(typeof(ExperimentalGem));

                if (items != null)
                {
                    foreach (Item item in items)
                    {
                        ExperimentalGem gem = (ExperimentalGem)item;

                        if (gem.Active && (gem.CurrentRoom > m_Room || m_Room == Room.RoomZero))
                            return true;
                    }
                }
            }

            //TODO: Message?
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
            writer.Write((int)m_Room);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Room = (Room)reader.ReadInt();
        }
    }
}