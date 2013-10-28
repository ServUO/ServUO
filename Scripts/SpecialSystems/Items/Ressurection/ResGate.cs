using System;
using Server.Gumps;

namespace Server.Items
{
    public class ResGate : Item
    {
        [Constructable]
        public ResGate()
            : base(0xF6C)
        {
            this.Movable = false;
            this.Hue = 0x2D1;
            this.Light = LightType.Circle300;
        }

        public ResGate(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "a resurrection gate";
            }
        }
        public override bool OnMoveOver(Mobile m)
        {
            if (!m.Alive && m.Map != null && m.Map.CanFit(m.Location, 16, false, false))
            {
                m.PlaySound(0x214);
                m.FixedEffect(0x376A, 10, 16);

                m.CloseGump(typeof(ResurrectGump));
                m.SendGump(new ResurrectGump(m));
            }
            else
            {
                m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
            }

            return false;
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