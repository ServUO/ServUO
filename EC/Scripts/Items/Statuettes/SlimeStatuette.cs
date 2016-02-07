using System;

namespace Server.Items
{
    public class SlimeStatuette : BaseStatuette
    {
        private static readonly int[] m_Sounds = new int[]
        {
            0x1C9, 0x1CA, 0x1CB, 0x1CC, 0x1CD
        };
        [Constructable]
        public SlimeStatuette()
            : base(0x20E8)
        {
            this.Hue = Utility.RandomList(0x899, 0x8A2, 0x8B0);
            this.Name = "Slime Statuette";
            this.Weight = 1.0;
        }

        public SlimeStatuette(Serial serial)
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