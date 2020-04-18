namespace Server.Items
{
    public class PaintedCavesTele : Teleporter
    {
        [Constructable]
        public PaintedCavesTele()
            : base(new Point3D(6308, 892, 0), Map.Trammel)
        {
        }

        public PaintedCavesTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (!MondainsLegacy.PaintedCaves && (int)m.AccessLevel < (int)AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1042753, "Painted Caves"); // ~1_SOMETHING~ has been temporarily disabled.
                return true;
            }

            return base.OnMoveOver(m);
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