#region References
using System;

using Server.Commands;
using Server.PathAlgorithms;
using Server.PathAlgorithms.FastAStar;
using Server.PathAlgorithms.SlowAStar;
using Server.Targeting;
#endregion

namespace Server
{
	public sealed class MovementPath
	{
		public static PathAlgorithm OverrideAlgorithm;

		public Map Map { get; }
		public Point3D Start { get; }
		public Point3D Goal { get; }
		public Direction[] Directions { get; }

		public bool Success => Directions?.Length > 0;

		public MovementPath(Mobile m, Point3D goal)
			: this(m, goal, m.Map)
		{ }

		public MovementPath(IPoint3D p, Point3D goal, Map map)
		{
			var start = new Point3D(p);

			Map = map;
			Start = start;
			Goal = goal;

			if (map == null || map == Map.Internal)
				return;

			if (Utility.InRange(start, goal, 1))
				return;

			try
			{
				var alg = OverrideAlgorithm;

				if (alg == null)
					alg = FastAStarAlgorithm.Instance;

				if (alg != null && alg.CheckCondition(p, map, start, goal))
					Directions = alg.Find(p, map, start, goal);
			}
			catch (Exception e)
			{
				Console.WriteLine("Warning: {0}: Pathing error from {1} to {2}", e.GetType().Name, start, goal);

				Diagnostics.ExceptionLogging.LogException(e);
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register("Path", AccessLevel.GameMaster, Path_OnCommand);
		}

		public static void Path_OnCommand(CommandEventArgs e)
		{
			e.Mobile.BeginTarget(-1, true, TargetFlags.None, Path_OnTarget);
			e.Mobile.SendMessage("Target a location and a path will be drawn there.");
		}

		public static void Path_OnTarget(Mobile from, object obj)
		{
			if (obj is IPoint3D p)
			{
				Spells.SpellHelper.GetSurfaceTop(ref p);

				Path(from, p, FastAStarAlgorithm.Instance, "Fast", 0);
				Path(from, p, SlowAStarAlgorithm.Instance, "Slow", 2);

				OverrideAlgorithm = null;
			}
		}

		public static void Pathfind(Mobile from, Direction d)
		{
			try
			{
				from.Direction = d;
				from.NetState.BlockAllPackets = true;
				from.Move(d);
				from.NetState.BlockAllPackets = false;
				from.ProcessDelta();
			}
			catch (Exception e)
			{
				Diagnostics.ExceptionLogging.LogException(e);
			}
		}

		private static void Path(Mobile from, IPoint3D p, PathAlgorithm alg, string name, int zOffset)
		{
			OverrideAlgorithm = alg;

			var start = DateTime.UtcNow.Ticks;
			var path = new MovementPath(from, new Point3D(p));
			var end = DateTime.UtcNow.Ticks;
			var len = Math.Round((end - start) / 10000.0, 2);

			if (!path.Success)
			{
				from.SendMessage("{0} path failed: {1}ms", name, len);
				return;
			}

			from.SendMessage("{0} path success: {1}ms", name, len);

			var x = from.X;
			var y = from.Y;
			var z = from.Z;

			for (var i = 0; i < path.Directions.Length; ++i)
			{
				Movement.Movement.Offset(path.Directions[i], ref x, ref y);

				new Items.RecallRune().MoveToWorld(new Point3D(x, y, z + zOffset), from.Map);
			}
		}
	}
}
