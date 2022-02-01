#region References
using System;

using CalcMoves = Server.Movement.Movement;
#endregion

namespace Server
{
	public class PathFollower
	{
		// Should we use pathfinding? 'false' for not
		private static readonly bool Enabled = true;

		private static readonly TimeSpan RepathDelay = TimeSpan.FromSeconds(2.0);

		private readonly Mobile m_From;

		private MovementPath m_Path;

		private int m_Index;

		private Point3D m_Next, m_LastGoalLoc;

		private DateTime m_LastPathTime;

		public MoveMethod Mover { get; set; }

		public IPoint3D Goal { get; }

		public PathFollower(Mobile from, IPoint3D goal)
		{
			m_From = from;
			Goal = goal;
		}

		public MoveResult Move(Direction d)
		{
			if (Mover == null)
				return m_From.Move(d) ? MoveResult.Success : MoveResult.Blocked;

			return Mover(d);
		}

		public Point3D GetGoalLocation()
		{
			if (Goal is Item o)
				return o.GetWorldLocation();

			return new Point3D(Goal);
		}

		public void Advance(ref Point3D p, int index)
		{
			if (m_Path != null && m_Path.Success)
			{
				var dirs = m_Path.Directions;

				if (index >= 0 && index < dirs.Length)
				{
					int x = p.X, y = p.Y;

					CalcMoves.Offset(dirs[index], ref x, ref y);

					p.X = x;
					p.Y = y;
				}
			}
		}

		public void ForceRepath()
		{
			m_Path = null;
		}

		public bool CheckPath()
		{
			if (!Enabled)
				return false;

			var repath = false;

			var goal = GetGoalLocation();

			if (m_Path == null)
				repath = true;
			else if ((!m_Path.Success || goal != m_LastGoalLoc) && m_LastPathTime + RepathDelay <= DateTime.UtcNow)
				repath = true;
			else if (m_Path.Success && Check(m_From.Location, m_LastGoalLoc, 0))
				repath = true;

			if (!repath)
				return false;

			m_LastPathTime = DateTime.UtcNow;
			m_LastGoalLoc = goal;

			m_Path = new MovementPath(m_From, goal);

			m_Index = 0;
			m_Next = m_From.Location;

			Advance(ref m_Next, m_Index);

			return true;
		}

		public bool Check(Point3D loc, Point3D goal, int range)
		{
			if (!Utility.InRange(loc, goal, range))
				return false;

			if (range <= 1 && Math.Abs(loc.Z - goal.Z) >= 16)
				return false;

			return true;
		}

		public bool Follow(bool run, int range)
		{
			var goal = GetGoalLocation();

			if (Check(m_From.Location, goal, range))
				return true;

			var repathed = CheckPath();

			Direction d;

			if (!Enabled || !m_Path.Success)
			{
				d = m_From.GetDirectionTo(goal);

				if (run)
					d |= Direction.Running;

				m_From.SetDirection(d);

				Move(d);

				return Check(m_From.Location, goal, range);
			}

			d = m_From.GetDirectionTo(m_Next);

			if (run)
				d |= Direction.Running;

			m_From.SetDirection(d);

			var res = Move(d);

			if (res == MoveResult.Blocked)
			{
				if (repathed)
					return false;

				m_Path = null;

				CheckPath();

				if (!m_Path.Success)
				{
					d = m_From.GetDirectionTo(goal);

					if (run)
						d |= Direction.Running;

					m_From.SetDirection(d);

					Move(d);

					return Check(m_From.Location, goal, range);
				}

				d = m_From.GetDirectionTo(m_Next);

				if (run)
					d |= Direction.Running;

				m_From.SetDirection(d);

				res = Move(d);

				if (res == MoveResult.Blocked)
					return false;
			}

			if (m_From.X == m_Next.X && m_From.Y == m_Next.Y)
			{
				if (m_From.Z == m_Next.Z)
					Advance(ref m_Next, ++m_Index);
				else
									m_Path = null;
							}

			return Check(m_From.Location, goal, range);
		}
	}
}
