namespace Server.Items
{
    public class RedDeathStatuette : BaseStatuette
    {
        private static readonly int[] m_Sounds = new int[]
        {
            0xE5, 0xE6, 0xE7, 0xE8, 0xE9
        };
        [Constructable]
        public RedDeathStatuette()
            : base(0x2617)
        {
            Name = "Red Death";
            Weight = 5.0;
        }

        public RedDeathStatuette(Serial serial)
            : base(serial)
        {
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (TurnedOn && IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, Location, 2) && !Utility.InRange(oldLocation, Location, 2))
                Effects.PlaySound(Location, Map, m_Sounds[Utility.Random(m_Sounds.Length)]);

            base.OnMovement(m, oldLocation);
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