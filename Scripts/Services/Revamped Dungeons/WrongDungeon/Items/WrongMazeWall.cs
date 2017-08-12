using System;
using Server.Network;

namespace Server.Items
{
    public class WrongMazeWall : BaseWall
    {
        [Constructable]
        public WrongMazeWall()
            : base(578)
        {
            Movable = false;
            Visible = false;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.Name != null)
                from.Send(new AsciiMessage(Serial, ItemID, MessageType.Label, 0, 3, "", this.Name));
            else
                from.Send(new AsciiMessage(Serial, ItemID, MessageType.Label, 0, 3, "", "a dungeon wall"));
        }

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            IPooledEnumerable eable = GetMobilesInRange(2);

            foreach (object o in eable)
            {
                if (o is Mobile && ((Mobile)o).InRange(Location, 1))
                {
                    eable.Free();
                    Visible = true;
                    return;
                }
            }
            eable.Free();
            Visible = false;
        }

        public WrongMazeWall(Serial serial)
            : base(serial)
        {
        }

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