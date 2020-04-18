namespace Server.Items
{
    public class DreadSpiderStatuette : BaseStatuette
    {
        private static readonly int[] m_Sounds = new int[]
        {
            0x493, 0x494, 0x495, 0x496, 0x497
        };
        [Constructable]
        public DreadSpiderStatuette()
            : base(0x25C4)
        {
            Name = "Dread Spider";
            Weight = 5.0;
        }

        public DreadSpiderStatuette(Serial serial)
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