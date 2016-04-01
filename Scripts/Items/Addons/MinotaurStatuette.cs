using System;

namespace Server.Items
{
    public class MinotaurStatuette : BaseStatuette
    {
        private static readonly int[] m_Sounds = new int[]
        {
            0x596, 0x597, 0x598, 0x599, 0x59A, 0x59B, 0x59C, 0x59D
        };
        [Constructable]
        public MinotaurStatuette()
            : base(0x2D89)
        {
            this.Name = "Minotaur Statuette";
            this.Weight = 1.0;
        }

        public MinotaurStatuette(Serial serial)
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