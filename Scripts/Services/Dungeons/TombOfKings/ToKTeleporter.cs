using Server.Mobiles;

namespace Server.Items
{
    public class ToKTeleporter : Item
    {
        private static readonly Point3D m_Dest = new Point3D(35, 215, -5);

        [Constructable]
        public ToKTeleporter()
            : base(0x1BC3)
        {
            Name = "Tomb of Kings Antechamber Teleporter";

            Visible = false;
            Movable = false;
        }

        public ToKTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.IsPlayer())
            {
                BaseCreature.TeleportPets(m, m_Dest, Map.TerMur);
                m.MoveToWorld(m_Dest, Map.TerMur);

                m.PlaySound(0x1FE);
                Effects.SendLocationParticles(m, 0x3728, 10, 10, 0x139F);

                m.SendLocalizedMessage(1112182); // You have been returned to the antechamber of the tomb, directly in front of the Serpent's Breath.

                return false;
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