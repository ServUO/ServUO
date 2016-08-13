using Server;
using System;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class BoatPaintRemover : Item
    {
        public override int LabelNumber { get { return 1116766; } }
        public override double DefaultWeight { get { return 10.0; } }

        [Constructable]
        public BoatPaintRemover() : base(4011)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if(IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private BoatPaintRemover m_PaintRemover;

            public InternalTarget(BoatPaintRemover paintremover)
                : base(5, false, TargetFlags.None)
            {
                m_PaintRemover = paintremover;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D)
                {
                    IPoint3D pnt = (IPoint3D)targeted;
                    BaseBoat boat = BaseBoat.FindBoatAt(pnt, from.Map);

                    if (boat is BaseGalleon && boat.Contains(from))
                    {
                        if (((BaseGalleon)boat).RemovePaint())
                            m_PaintRemover.Delete();
                    }

                    else
                        from.SendLocalizedMessage(1116612); //You must target the main mast of the ship you wish to dye.

                }
            }
        }

        public BoatPaintRemover(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}