#region References
using Server.Network;
#endregion

namespace Server
{
	public class QuestArrow
	{
		public Mobile Mobile { get; }

		public IPoint3D Target { get; }

		public bool Running { get; private set; }

		public void Update()
		{
			Update(Target.X, Target.Y);
		}

		public void Update(int x, int y)
		{
			if (!Running)
			{
				return;
			}

			var ns = Mobile.NetState;

			if (ns != null)
			{
				IPoint2D target = Target;

				if (target == null)
				{
					target = new Point2D(x, y);
				}

				SetArrow.Send(ns, target);
			}
		}

		public void Stop()
		{
			Stop(Target.X, Target.Y);
		}

		public void Stop(int x, int y)
		{
			if (!Running)
			{
				return;
			}

			Mobile.ClearQuestArrow();

			var ns = Mobile.NetState;

			if (ns != null)
			{
				IPoint2D target = Target;

				if (target == null)
				{
					target = new Point2D(x, y);
				}

				CancelArrow.Send(ns, target);
			}

			Running = false;
			OnStop();
		}

		public virtual void OnStop()
		{ }

		public virtual void OnClick(bool rightClick)
		{ }

		public QuestArrow(Mobile m, IPoint3D t)
		{
			Running = true;
			Mobile = m;
			Target = t;
		}

		public QuestArrow(Mobile m, IPoint3D t, int x, int y)
			: this(m, t)
		{
			Update(x, y);
		}
	}
}
