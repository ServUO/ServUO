namespace Server.Items
{
    public class OphidianWarriorStatuette : BaseStatuette
    {
        private static readonly int[] m_Sounds = new int[]
        {
            0x27B, 0x27C, 0x27D, 0x27E, 0x27F
        };
        [Constructable]
        public OphidianWarriorStatuette()
            : base(0x25AD)
        {
            Name = "Ophidian Warrior";
            Weight = 5.0;
        }

        public OphidianWarriorStatuette(Serial serial)
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