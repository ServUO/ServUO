using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class IRTeleporter : Item
    {
        private static readonly Point3D m_Dest = new Point3D(2263, 1572, -28);

        [Constructable]
        public IRTeleporter()
            : base(0x1BC3)
        {
            Visible = false;
            Movable = false;
        }

        public IRTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is PlayerMobile)
            {
                var neck = m.FindItemOnLayer(Layer.Neck);

                if (neck != null && neck.Hue == 1151 && (neck is FellowshipMedallion || neck is GargishFellowshipMedallion))
                {
                    BaseCreature.TeleportPets(m, m_Dest, Map.Ilshenar);
                    m.MoveToWorld(m_Dest, Map.Ilshenar);
                    return false;
                }
                else
                {
                    m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159385,
                        m.NetState); // * Your connection to the ethereal void is not honed, you cannot pass... *
                }
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
