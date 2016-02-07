using System;

namespace Server.Items
{
    public class OphidianKnightStatuette : BaseStatuette
    {
        private static readonly int[] m_Sounds = new int[]
        {
            0x27B, 0x27C, 0x27D, 0x27E, 0x27F
        };
        [Constructable]
        public OphidianKnightStatuette()
            : base(0x25AA)
        {
            this.Name = "Ophidian Knight";
            this.Weight = 5.0;
        }

        public OphidianKnightStatuette(Serial serial)
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