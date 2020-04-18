namespace Server.Items
{
    public class ClimbingVine : Item
    {
        public override int LabelNumber => 1023307;  // vines

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D ClimbLocation { get; set; }

        [Constructable]
        public ClimbingVine()
            : this(Point3D.Zero)
        {
        }

        [Constructable]
        public ClimbingVine(Point3D p)
            : base(0x1AA1)
        {
            ClimbLocation = p;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SayTo(from, 1156290, 1153); // *The vines looks as though they may be strong enough to support climbing...*

            if (ClimbLocation != Point3D.Zero && from.InRange(GetWorldLocation(), 2) && Z >= from.Z)
            {
                from.MoveToWorld(ClimbLocation, Map);
            }
        }

        public ClimbingVine(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(ClimbLocation);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            ClimbLocation = reader.ReadPoint3D();
        }

        public static ClimbingVine Vine1 { get; set; }
        public static ClimbingVine Vine2 { get; set; }

        public static void Initialize()
        {
            Vine1 = Map.TerMur.FindItem<ClimbingVine>(new Point3D(687, 1759, 40));
            Vine2 = Map.TerMur.FindItem<ClimbingVine>(new Point3D(687, 1759, 60));

            if (Vine1 == null)
            {
                Vine1 = new ClimbingVine(new Point3D(679, 1757, 100));
                Vine1.MoveToWorld(new Point3D(678, 1759, 40), Map.TerMur);
            }

            if (Vine2 == null)
            {
                Vine2 = new ClimbingVine(new Point3D(679, 1757, 100));
                Vine2.MoveToWorld(new Point3D(678, 1759, 60), Map.TerMur);
            }
        }
    }
}
