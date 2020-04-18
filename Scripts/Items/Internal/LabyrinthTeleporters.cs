using Server.Mobiles;

namespace Server.Items
{
    public class LabyrinthIslandTele : Item
    {
        [Constructable]
        public LabyrinthIslandTele()
            : base(0x2FD4)
        {
            Movable = false;
        }

        public LabyrinthIslandTele(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (MondainsLegacy.Labyrinth && from.InRange(Location, 2))
                from.MoveToWorld(new Point3D(1731, 978, -80), Map);
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

    public class LabyrinthTele : Item
    {
        [Constructable]
        public LabyrinthTele()
            : base(0x248B)
        {
            Movable = false;
        }

        public LabyrinthTele(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!MondainsLegacy.Labyrinth && (int)from.AccessLevel < (int)AccessLevel.GameMaster)
            {
                from.SendLocalizedMessage(1042753, "Labyrinth"); // ~1_SOMETHING~ has been temporarily disabled.
                return;
            }

            if (from.InRange(Location, 2))
            {
                Point3D p = new Point3D(330, 1973, 0);

                BaseCreature.TeleportPets(from, p, Map);
                from.MoveToWorld(p, Map);
            }
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