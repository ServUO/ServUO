using System;
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
			this.m_Object = o;
		}

		protected override void OnTarget(Mobile from, object o)
		{
			IPoint3D p = o as IPoint3D;

			if (p != null)
			{
				if (!BaseCommand.IsAccessible(from, this.m_Object))
				{
					from.SendMessage("That is not accessible.");
					return;
				}

				if (p is Item)
					p = ((Item)p).GetWorldTop();

				CommandLogging.WriteLine(from, "{0} {1} moving {2} to {3}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(this.m_Object), new Point3D(p));

				if (this.m_Object is Item)
				{
					Item item = (Item)this.m_Object;

					if (!item.Deleted)
					{
						from.SendMessage("Item moved.");
						item.MoveToWorld(new Point3D(p), from.Map);
					}
					else
					{
						from.SendMessage("Invalid Item.");
					}
				}
				else if (this.m_Object is Mobile)
				{
					Mobile m = (Mobile)this.m_Object;

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
