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

        public override bool HandlesOnMovement => true;

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

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
