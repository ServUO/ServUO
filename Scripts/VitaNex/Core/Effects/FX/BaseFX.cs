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
using System.Threading.Tasks;

using Server;
#endregion

namespace VitaNex.FX
{
	public static class EffectUtility
	{
		public static readonly Point3D[][] EmptyPoints = new Point3D[0][];
	}

	public interface IEffect
	{
		IPoint3D Start { get; set; }
		Map Map { get; set; }
		Action Callback { get; set; }

		bool Sending { get; }

		void Send();
	}

	public abstract class BaseEffect<TQueue, TEffectInfo> : List<TQueue>, IEffect
		where TQueue : EffectQueue<TEffectInfo>
		where TEffectInfo : EffectInfo
	{
		public bool Processing { get; protected set; }
		public int CurrentProcess { get; protected set; }

		public TQueue CurrentQueue { get; private set; }

		public bool Sending { get; protected set; }

		public virtual IPoint3D Start { get; set; }
		public virtual Map Map { get; set; }
		public virtual int Repeat { get; set; }
		public virtual TimeSpan Interval { get; set; }
		public virtual Action<TEffectInfo> EffectHandler { get; set; }
		public virtual Action<TEffectInfo> EffectMutator { get; set; }
		public virtual Action Callback { get; set; }

		public virtual bool EnableMutate { get; set; }
		public virtual bool Reversed { get; set; }

		public abstract TEffectInfo[] Effects { get; }

		public BaseEffect(
			IPoint3D start,
			Map map,
			int repeat = 0,
			TimeSpan? interval = null,
			Action<TEffectInfo> effectHandler = null,
			Action callback = null)
		{
			Start = start;
			Map = map;
			Repeat = Math.Max(0, repeat);
			Interval = interval ?? TimeSpan.FromMilliseconds(100);
			EffectHandler = effectHandler;
			Callback = callback;
		}

		public abstract TQueue CreateEffectQueue(IEnumerable<TEffectInfo> queue);
		public abstract TEffectInfo CloneEffectInfo(TEffectInfo src);

		public void Update()
		{
			this.Free(true);

			if (Effects == null || Effects.Length == 0)
			{
				return;
			}

			var points = GetTargetPoints(CurrentProcess);

			if (points == null || points.Length == 0)
			{
				return;
			}

			Capacity = points.Length;

			this.SetAll(i => null);

			for (var i = 0; i < points.Length; i++)
			{
				var list = points[i];

				if (list == null || list.Length == 0)
				{
					continue;
				}

				var fx = new TEffectInfo[list.Length][];

				fx.SetAll(fxi => new TEffectInfo[Effects.Length]);

				Parallel.For(
					0,
					list.Length,
					index =>
					{
						var p = list[index];

						var pIndex = 0;

						for (var ei = 0; ei < Effects.Length; ei++)
						{
							var e = CloneEffectInfo(Effects[ei]);

							if (e == null || e.IsDisposed)
							{
								continue;
							}

							e.QueueIndex = index;
							e.ProcessIndex = pIndex++;

							e.Source = new Entity(Serial.Zero, p, Map);
							e.Map = Map;

							if (EnableMutate)
							{
								MutateEffect(e);
							}

							if (!e.IsDisposed)
							{
								fx[index][ei] = e;
							}
						}
					});

				var q = CreateEffectQueue(fx.Combine());

				if (q.Mutator == null && EffectMutator != null)
				{
					q.Mutator = EffectMutator;
				}

				if (q.Handler == null && EffectHandler != null)
				{
					q.Handler = EffectHandler;
				}

				this[i] = q;
			}

			RemoveAll(l => l == null);

			this.Free(false);

			if (Reversed)
			{
				Reverse();
			}

			var idx = 0;

			foreach (var cur in this)
			{
				if (++idx >= Count)
				{
					cur.Callback = InternalCallback;
					break;
				}

				var next = this[idx];

				cur.Callback = () => InternalCallback(next);
			}

			OnUpdated();
		}

		public virtual Point3D[][] GetTargetPoints(int dist)
		{
			return Start == null ? EffectUtility.EmptyPoints : new[] { new[] { Start.Clone3D() } };
		}

		protected virtual void OnUpdated()
		{ }

		public virtual void MutateEffect(TEffectInfo e)
		{ }

		public void Send()
		{
			if (Sending)
			{
				return;
			}

			Sending = true;

			VitaNexCore.TryCatch(InternalSend, VitaNexCore.ToConsole);
		}

		public virtual void OnSend()
		{ }

		private void InternalSend()
		{
			Update();

			if (Count == 0 || this[0] == null)
			{
				return;
			}

			Processing = true;
			InternalMoveNext(this[0]);
			OnSend();
		}

		private void InternalMoveNext(TQueue next)
		{
			CurrentQueue = next;
			CurrentQueue.Process();
		}

		private void InternalCallback()
		{
			Sending = false;

			if (Callback != null)
			{
				Callback();
			}

			if (++CurrentProcess <= Repeat)
			{
				if (Interval <= TimeSpan.Zero)
				{
					Send();
					return;
				}

				Timer.DelayCall(Interval, Send);
				return;
			}

			CurrentProcess = 0;
			Processing = false;

			this.Free(true);
		}

		private void InternalCallback(TQueue next)
		{
			Processing = true;

			if (Interval <= TimeSpan.Zero)
			{
				InternalMoveNext(next);
				return;
			}

			Timer.DelayCall(Interval, InternalMoveNext, next);
		}
	}

	public abstract class BaseRangedEffect<TQueue, TEffectInfo> : BaseEffect<TQueue, TEffectInfo>
		where TQueue : EffectQueue<TEffectInfo>
		where TEffectInfo : EffectInfo
	{
		public virtual int Range { get; set; }
		public virtual bool AverageZ { get; set; }
		public virtual bool LOSCheck { get; set; }

		public BaseRangedEffect(
			IPoint3D start,
			Map map,
			int range = 5,
			int repeat = 0,
			TimeSpan? interval = null,
			Action<TEffectInfo> effectHandler = null,
			Action callback = null)
			: base(start, map, repeat, interval, effectHandler, callback)
		{
			Range = range;
			AverageZ = true;
			LOSCheck = false;
		}

		public override Point3D[][] GetTargetPoints(int count)
		{
			return Start.ScanRangeGet(Map, Range, ComputePoint, AverageZ);
		}

		protected virtual bool ComputePoint(ScanRangeResult r)
		{
			if (!r.Excluded && ExcludePoint(r.Current, r.Distance, Utility.GetDirection(Start, r.Current)))
			{
				r.Exclude();
			}

			return false;
		}

		protected virtual bool ExcludePoint(Point3D p, int range, Direction fromCenter)
		{
			return LOSCheck && !Map.LineOfSight(p, Start);
		}
	}

	public abstract class BaseBoundsEffect<TQueue, TEffectInfo> : BaseEffect<TQueue, TEffectInfo>
		where TQueue : EffectQueue<TEffectInfo>
		where TEffectInfo : EffectInfo
	{
		public virtual Rectangle2D Bounds { get; set; }
		public virtual bool AverageZ { get; set; }

		public BaseBoundsEffect(
			IPoint3D start,
			Map map,
			Rectangle2D bounds,
			int repeat = 0,
			TimeSpan? interval = null,
			Action<TEffectInfo> effectHandler = null,
			Action callback = null)
			: base(start, map, repeat, interval, effectHandler, callback)
		{
			Bounds = bounds;
			AverageZ = true;
		}

		public override Point3D[][] GetTargetPoints(int count)
		{
			var points = new List<Point3D>[Math.Max(Bounds.Width, Bounds.Height)];

			Bounds.ForEach(
				p2d =>
				{
					var distance = (int)Math.Floor(Start.GetDistance(p2d));

					points[distance].Add(p2d.ToPoint3D(AverageZ ? p2d.GetAverageZ(Map) : Start.Z));
				});

			var arr = points.ToMultiArray();

			points.Free(true);

			return arr;
		}
	}
}