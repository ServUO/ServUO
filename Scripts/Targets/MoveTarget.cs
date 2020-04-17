using Server.Commands;
using Server.Commands.Generic;
using Server.Targeting;

namespace Server.Targets
{
    public class MoveTarget : Target
    {
        private readonly object m_Object;
        public MoveTarget(object o)
            : base(-1, true, TargetFlags.None)
        {
            m_Object = o;
        }

        protected override void OnTarget(Mobile from, object o)
        {
            IPoint3D p = o as IPoint3D;

            if (p != null)
            {
                if (!BaseCommand.IsAccessible(from, m_Object))
                {
                    from.SendLocalizedMessage(500447); // That is not accessible.
                    return;
                }

                if (p is Item)
                    p = ((Item)p).GetWorldTop();

                CommandLogging.WriteLine(from, "{0} {1} moving {2} to {3}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(m_Object), new Point3D(p));

                if (m_Object is Item)
                {
                    Item item = (Item)m_Object;

                    if (!item.Deleted)
                    {
                        from.SendMessage("Item moved.");
                        item.MoveToWorld(new Point3D(p), from.Map);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1154965); // Invalid item.
                    }
                }
                else if (m_Object is Mobile)
                {
                    Mobile m = (Mobile)m_Object;

                    if (!m.Deleted)
                    {
                        from.SendMessage("Mobile moved.");
                        m.MoveToWorld(new Point3D(p), from.Map);
                    }
                    else
                    {
                        from.SendMessage("Invalid Mobile.");
                    }
                }
            }
        }
    }
}
