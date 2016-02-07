using System;

namespace Server.Items
{
    public class DreadHornStatuette : BaseStatuette
    {
        private static readonly int[] m_Sounds = new int[]
        {
            0xA8, 0xA9, 0xAA, 0xAB, 0xAC
        };
        [Constructable]
        public DreadHornStatuette()
            : base(0x2D83)
        {
            this.Name = "Dread Horn";
            this.Weight = 5.0;
        }

        public DreadHornStatuette(Serial serial)
            : base(serial)
        {
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.TurnedOn && this.IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, this.Location, 2) && !Utility.InRange(oldLocation, this.Location, 2))
                Effects.PlaySound(this.Location, this.Map, m_Sounds[Utility.Random(m_Sounds.Length)]);

            base.OnMovement(m, oldLocation);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}