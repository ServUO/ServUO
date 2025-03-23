#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;

using Server;
using Server.Commands;

using VitaNex.Network;

using Geo = Server.Misc.Geometry;
#endregion

namespace VitaNex.FX
{
	public enum SpecialFX
	{
		None = 0,
		Random,

		FirePentagram
		//-FireSpiral
	}

	public static class SpecialEffects
	{
		public static void Initialize()
		{
			CommandSystem.Register(
				"SpecialFXHide",
				AccessLevel.GameMaster,
				ce =>
				{
					if (ce == null || ce.Mobile == null)
					{
						return;
					}

					var m = ce.Mobile;

					if (m.Hidden)
					{
						m.Hidden = false;
						CommandSystem.Entries["SpecialFX"].Handler(ce);
					}
					else
					{
						CommandSystem.Entries["SpecialFX"].Handler(ce);
						m.Hidden = true;
					}
				});

			CommandSystem.Register(
				"SpecialFX",
				AccessLevel.GameMaster,
				ce =>
				{
					if (ce == null || ce.Mobile == null)
					{
						return;
					}

					var m = ce.Mobile;

					if (ce.Arguments.Length < 1 || !Enum.TryParse(ce.Arguments[0], true, out SpecialFX effect))
					{
						effect = SpecialFX.None;
					}

					if (ce.Arguments.Length < 2 || !Int32.TryParse(ce.Arguments[1], out var range))
					{
						range = 5;
					}

					if (ce.Arguments.Length < 3 || !Int32.TryParse(ce.Arguments[2], out var speed))
					{
						speed = 10;
					}

					if (ce.Arguments.Length < 4 || !Int32.TryParse(ce.Arguments[3], out var repeat))
					{
						repeat = 0;
					}

					if (ce.Arguments.Length < 5 || !Int32.TryParse(ce.Arguments[4], out var reverse))
					{
						reverse = 0;
					}

					range = Math.Max(0, Math.Min(100, range));
					speed = Math.Max(1, Math.Min(10, speed));
					repeat = Math.Max(0, Math.Min(100, repeat));
					reverse = Math.Max(0, Math.Min(1, reverse));

					var e = effect.CreateInstance(
						m.Location,
						m.Map,
						range,
						repeat,
						TimeSpan.FromMilliseconds(1000 - ((speed - 1) * 100)));

					if (e != null)
					{
						e.Reversed = (reverse > 0);
						e.Send();
					}
					else
					{
						m.SendMessage(0x55, "Usage: <effect> <range> <speed> <repeat> <reverse>");
					}
				});
		}

		public static BaseSpecialEffect CreateInstance(
			this SpecialFX type,
			IPoint3D start,
			Map map,
			int range = 5,
			int repeat = 0,
			TimeSpan? interval = null,
			Action<EffectInfo> effectHandler = null,
			Action callback = null)
		{
			switch (type)
			{
				case SpecialFX.None:
					return null;
				case SpecialFX.FirePentagram:
					return new FirePentagramEffect(start, map, range, repeat, interval, effectHandler, callback);
				/*case SpecialFX.FireSpiral:
					return new FireSpiralEffect(start, map, range, repeat, interval, effectHandler, callback);*/
				default:
				{
					var rfx = (SpecialFX[])Enum.GetValues(typeof(SpecialFX));

					do
					{
						type = rfx.GetRandom();
					}
					while (type == SpecialFX.Random || type == SpecialFX.None);

					return CreateInstance(type, start, map, range, repeat, interval, effectHandler, callback);
				}
			}
		}
	}
}

namespace VitaNex.FX
{
	public abstract class BaseSpecialEffect : BaseRangedEffect<EffectQueue, EffectInfo>
	{
		public BaseSpecialEffect(
			IPoint3D start,
			Map map,
			int range = 2,
			int repeat = 0,
			TimeSpan? interval = null,
			Action<EffectInfo> effectHandler = null,
			Action callback = null)
			: base(start, map, range, repeat, interval, effectHandler, callback)
		{ }

		public override EffectQueue CreateEffectQueue(IEnumerable<EffectInfo> queue)
		{
			return new EffectQueue(queue, null, EffectHandler, false);
		}

		public override EffectInfo CloneEffectInfo(EffectInfo src)
		{
			return new EffectInfo(null, null, src.EffectID, src.Hue, src.Speed, src.Duration, src.Render, src.Delay);
		}
	}

	public class FirePentagramEffect : BaseSpecialEffect
	{
		public static EffectInfo[] Info => new[]
				{
					new EffectInfo(null, null, 14089, 0, 10, 20, EffectRender.SemiTransparent),
					new EffectInfo(null, null, 13401, 0, 10, 20, EffectRender.Normal, TimeSpan.FromMilliseconds(200))
				};

		private readonly EffectInfo[] _Effects = Info;

		public override EffectInfo[] Effects => _Effects;

		public FirePentagramEffect(
			IPoint3D start,
			Map map,
			int range = 5,
			int repeat = 0,
			TimeSpan? interval = null,
			Action<EffectInfo> effectHandler = null,
			Action callback = null)
			: base(start, map, range, repeat, interval, effectHandler, callback)
		{ }

		private const double Section = 72;

		private static readonly double[][] _Lines =
		{
			new[] {0, Section * 2}, new[] {Section * 2, Section * 4}, new[] {Section * 4, Section}, new[] {Section, Section * 3},
			new[] {Section * 3, 0}
		};

		public override Point3D[][] GetTargetPoints(int count)
		{
			var points = new List<Point3D>[_Lines.Length];

			points.SetAll(i => new List<Point3D>());

			_Lines.For(
				(i, list) =>
				{
					var start = Start.Clone3D(
						(int)Math.Round(Range * Math.Sin(Geo.DegreesToRadians(list[0]))),
						(int)Math.Round(Range * Math.Cos(Geo.DegreesToRadians(list[0]))));

					var end = Start.Clone3D(
						(int)Math.Round(Range * Math.Sin(Geo.DegreesToRadians(list[1]))),
						(int)Math.Round(Range * Math.Cos(Geo.DegreesToRadians(list[1]))));

					if (AverageZ)
					{
						start = start.GetWorldTop(Map);
						end = end.GetWorldTop(Map);
					}

					points[i].AddRange(start.GetLine3D(end, Map));
				});

			return points.ToMultiArray();
		}
	}

	/*public class FireSpiralEffect : BaseSpecialEffect
	{
		public static EffectInfo[] Info
		{
			get
			{
				return new[]
				{
					new EffectInfo(null, null, 14089, 0, 10, 20, EffectRender.SemiTransparent),
					new EffectInfo(null, null, 13401, 0, 10, 20, EffectRender.Normal, TimeSpan.FromMilliseconds(200))
				};
			}
		}

		private readonly EffectInfo[] _Effects = Info;

		public override EffectInfo[] Effects { get { return _Effects; } }

		public FireSpiralEffect(
			IPoint3D start,
			Map map,
			int range = 5,
			int repeat = 0,
			TimeSpan? interval = null,
			Action<EffectInfo> effectHandler = null,
			Action callback = null)
			: base(start, map, range, repeat, interval, effectHandler, callback)
		{ }

		public override Point3D[][] GetTargetPoints(int count)
		{
			List<List<Point3D>> points = new List<List<Point3D>>(Range + 1);

			points.SetAll(i => new List<Point3D>());

			for (int r = 0; r <= Range; r++)
			{
				int bound = r * r;
				int area = (int)(Math.PI * bound);

				int x, y;
				
				for (int t = 0; t <= area; t++)
				{
					x = (int)(r * Math.Cos(t)) + Start.X;
					y = (int)(r * Math.Sin(t)) + Start.Y;
			
					if (x * x + y * y <= bound)
					{
						points[r].Add(new Point3D(x, y, AverageZ ? Map.GetAverageZ(x, y) : Start.Z));
					}
				}
			}

			return points.ToMultiArray();
		}
	}*/
}