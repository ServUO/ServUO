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
#endregion

namespace VitaNex.SuperGumps
{
	public sealed class GumpAnimationState
	{
		private static readonly Dictionary<string, GumpAnimationState> _States =
			new Dictionary<string, GumpAnimationState>(0x400);

		private static readonly Queue<GumpAnimationState> _Pool = new Queue<GumpAnimationState>(0x200);

		private const double _Frequency = 1000.0;

		public static GumpAnimationState Acquire(string uid, long delay, long duration, bool repeat, bool wait)
		{
			GumpAnimationState state;

			lock (_States)
			{
				if (!_States.TryGetValue(uid, out state) || state == null)
				{
					lock (_Pool)
					{
						state = _Pool.Count > 0 ? _Pool.Dequeue() : new GumpAnimationState();
					}
				}

				_States[uid] = state;
			}

			state.Update(delay, duration, repeat, wait);

			return state;
		}

		public static void Free(string uid)
		{
			GumpAnimationState state;

			lock (_States)
			{
				if (_States.TryGetValue(uid, out state))
				{
					_States.Remove(uid);
				}
			}

			if (state != null)
			{
				state.Free();
			}
		}

		private long _Ticks;
		private double _Sample;
		private int _Count = -1;

		private long _Delay, _Duration;
		private bool _Repeat, _Wait;

		public long Start { get; private set; }
		public long Duration { get; private set; }
		public long End => Start + Duration;

		public bool Repeat { get; private set; }
		public bool Wait { get; private set; }

		public bool FirstUpdate { get; private set; }
		public bool FirstFrame { get; private set; }

		public int FrameRate { get; private set; }

		public double Slice
		{
			get
			{
				var ticks = VitaNexCore.Ticks;

				if (ticks < Start)
				{
					return 0.0;
				}

				if (Duration <= 0)
				{
					return (ticks / _Frequency) % 1.0;
				}

				if (ticks > End)
				{
					return 1.0;
				}

				return ((ticks - Start) / _Frequency) / (Duration / _Frequency);
			}
		}

		public int FrameCount
		{
			get
			{
				if (Duration <= 0)
				{
					return 1;
				}

				return (int)Math.Ceiling(FrameRate * (Duration / _Frequency));
			}
		}

		public int Frame => (int)Math.Ceiling(Slice * FrameCount);

		public bool Sequencing => VitaNexCore.Ticks < Start;
		public bool Animating => VitaNexCore.Ticks < End || Duration <= 0;
		public bool Waiting => Wait && !Sequencing && Animating;

		private GumpAnimationState()
		{ }

		private void Update(long delay, long duration, bool repeat, bool wait)
		{
			var ticks = VitaNexCore.Ticks;

			if (_Count++ > -1)
			{
				_Sample += ticks - _Ticks;

				if (_Sample >= _Frequency)
				{
					FrameRate = _Count;

					_Count = 0;
					_Sample -= _Frequency;
				}

				FirstUpdate = false;
			}
			else
			{
				FirstUpdate = true;
				FirstFrame = true;
				FrameRate = 10;
			}

			_Ticks = ticks;

			if (!FirstUpdate && _Ticks >= End && (Repeat || Duration <= 0))
			{
				FirstUpdate = true;
			}

			if (FirstUpdate)
			{
				Start = _Ticks + (_Delay = delay);
				Duration = _Duration = duration;
				Repeat = _Repeat = repeat;
				Wait = _Wait = wait;
			}
			else
			{
				if (_Delay != delay)
				{
					Start -= _Delay;
					_Delay = delay;
					Start += _Delay;
				}

				if (_Duration != duration)
				{
					Duration = _Duration = duration;
				}

				if (_Repeat != repeat)
				{
					Repeat = _Repeat = repeat;
				}

				if (_Wait != wait)
				{
					Wait = _Wait = wait;
				}
			}
		}

		public void Animated()
		{
			FirstFrame = false;
		}

		public void Reset()
		{
			_Count = -1;
		}

		private void Free()
		{
			_Ticks = 0;
			_Sample = 0;
			_Count = -1;

			_Delay = _Duration = 0;
			_Repeat = _Wait = false;

			Start = Duration = 0;
			Repeat = Wait = false;

			FirstUpdate = true;
			FirstFrame = true;
			FrameRate = 10;

			lock (_Pool)
			{
				if (_Pool.Count < 0x200)
				{
					_Pool.Enqueue(this);
				}
			}
		}
	}
}