using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class BoatPaint : Item
    {
        public override int LabelNumber => 1116236;
        public override double DefaultWeight => 10.0;

        private bool m_Permanent;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Permanent { get { return m_Permanent; } set { m_Permanent = value; } }

        public BoatPaint(object hue) : this((int)hue)
        {
        }

        [Constructable]
        public BoatPaint(int hue) : base(4011)
        {
            Hue = hue;
            m_Permanent = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(from, this);
                from.SendLocalizedMessage(1116613); //Select the main mast of the ship you wish to dye.
            }
        }

        private class InternalTarget : Target
        {
            private readonly BoatPaint m_Paint;
            private readonly Mobile m_From;

            public InternalTarget(Mobile from, BoatPaint paint) : base(5, false, TargetFlags.None)
            {
                m_From = from;
                m_Paint = paint;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D)
                {
                    IPoint3D pnt = (IPoint3D)targeted;
                    BaseGalleon galleon = BaseGalleon.FindGalleonAt(pnt, from.Map);

                    if (galleon == null || !galleon.Contains(from))
                        return;

                    if (galleon.GetSecurityLevel(from) < SecurityLevel.Captain)
                        from.SendMessage("You must be the captain to paint this ship!");

                    else if (galleon.Contains(pnt)/*&& boat.X == pnt.X && boat.Y == pnt.Y*/)
                    {
                        if (m_Paint.Permanent)
                        {
                            if (galleon.TryPermanentPaintBoat(from, m_Paint.Hue))
                                m_Paint.Delete();
                        }
                        else
                        {
                            if (galleon.TryPaintBoat(from, m_Paint.Hue))
                                m_Paint.Delete();
                        }
                    }
                    else
                        from.SendLocalizedMessage(1116612); //You must target the main mast of the ship you wish to dye.

                }
            }
        }

        public BoatPaint(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PermanentBoatPaint : BoatPaint
    {
        public override int LabelNumber => 1116768;

        [Constructable]
        public PermanentBoatPaint()
            : this(Utility.RandomMinMax(1954, 1997))
        {
        }

        [Constructable]
        public PermanentBoatPaint(int hue) : base(hue)
        {
            Permanent = true;
        }

        public static PermanentBoatPaint DropRandom()
        {
            return new PermanentBoatPaint(Utility.RandomMinMax(1954, 1997));
        }

        public PermanentBoatPaint(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}