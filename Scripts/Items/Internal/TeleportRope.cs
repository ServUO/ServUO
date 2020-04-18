namespace Server.Items
{
    public class TeleportRope : Static
    {
        public virtual bool AllowDead => true;

        [CommandProperty(AccessLevel.Administrator)]
        public Point3D ToLocation { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public Map ToMap { get; set; }

        [Constructable]
        public TeleportRope()
            : base(0x14FA)
        {
            ToLocation = Point3D.Zero;
            ToMap = Map.Internal;
        }

        public TeleportRope(Serial serial)
            : base(serial)
        { }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (ToLocation == Point3D.Zero ||
                ToMap == Map.Internal)
                return;

            from.MoveToWorld(ToLocation, ToMap);
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            if (AllowDead)
            {
                OnDoubleClick(from);
            }
            else
            {
                base.OnDoubleClickDead(from);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // Version
            writer.Write(ToLocation);
            writer.Write(ToMap);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    ToLocation = reader.ReadPoint3D();
                    ToMap = reader.ReadMap();
                    break;
            }
        }
    }
}
