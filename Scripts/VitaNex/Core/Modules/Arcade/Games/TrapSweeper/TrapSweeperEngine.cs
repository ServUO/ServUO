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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Server;
#endregion

namespace VitaNex.Modules.Games
{
	public class TrapSweeperEngine : GameEngine<TrapSweeper, TrapSweeperUI>
	{
		public const int MinWidth = 10;
		public const int MaxWidth = 22;

		public const int MinHeight = 10;
		public const int MaxHeight = 15;

		public const int MinDensity = 10;
		public const int MaxDensity = 30;

		public const int MinBonusDensity = 1;
		public const int MaxBonusDensity = 3;

		public const int MinPoints = 100;
		public const int MaxPoints = 500;

		private Grid<TrapSweeperTile> _Grid;

		public int Width => _Grid != null ? _Grid.Width : 0;
		public int Height => _Grid != null ? _Grid.Height : 0;
		public int Capacity => _Grid != null ? _Grid.Capacity : 0;
		public int Count => _Grid != null ? _Grid.Count : 0;

		public TrapSweeperState State { get; private set; }
		public TrapSweeperMode Mode { get; private set; }

		public DateTime Started { get; private set; }
		public DateTime Ended { get; private set; }

		public bool Mark { get; private set; }

		public int Blanks { get; private set; }
		public int Traps { get; private set; }
		public int Bonuses { get; private set; }

		public int Visible { get; private set; }
		public int Marked { get; private set; }

		public int Rewards { get; private set; }

		public TrapSweeperEngine(TrapSweeper game, Mobile user)
			: base(game, user)
		{
			_Grid = new Grid<TrapSweeperTile>();
		}

		public override void Reset()
		{
			base.Reset();

			Visible = 0;
			Marked = 0;

			Rewards = 0;

			Mark = false;
		}

		public IEnumerable<TrapSweeperTile> AllTiles()
		{
			return _Grid ?? _Grid.Ensure();
		}

		public IEnumerable<TrapSweeperTile> FindTiles(int x, int y, int w, int h)
		{
			return _Grid == null ? _Grid.Ensure() : _Grid.FindCells(x, y, w, h);
		}

		public IEnumerable<T> AllTiles<T>()
			where T : TrapSweeperTile
		{
			return (_Grid ?? _Grid.Ensure()).OfType<T>();
		}

		public IEnumerable<T> FindTiles<T>(int x, int y, int w, int h)
			where T : TrapSweeperTile
		{
			return (_Grid == null ? _Grid.Ensure() : _Grid.FindCells(x, y, w, h)).OfType<T>();
		}

		public void Generate(int w, int h, int d, int b, int p)
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			w = Math.Max(MinWidth, Math.Min(MaxWidth, w));
			h = Math.Max(MinHeight, Math.Min(MaxHeight, h));
			d = Math.Max(MinDensity, Math.Min(MaxDensity, d));
			b = Math.Max(MinBonusDensity, Math.Min(MaxBonusDensity, b));
			p = Math.Max(MinPoints, Math.Min(MaxPoints, p));

			if (_Grid == null)
			{
				_Grid = new Grid<TrapSweeperTile>(w, h);
			}
			else if (_Grid.Width != w || _Grid.Height != h)
			{
				_Grid.Resize(w, h);
			}

			_Grid.SetAllContent((x, y) => null);

			var q = new List<Point2D>(_Grid.Capacity);

			_Grid.ForEach((x, y, t) => q.Add(new Point2D(x, y)));

			q.Shuffle();

			var traps = (int)Math.Floor(q.Count * (d / 100.0));
			var bonus = (int)Math.Floor((q.Count - traps) * (b / 100.0));

			Parallel.ForEach(
				q,
				t =>
				{
					if (traps > 0)
					{
						_Grid[t.X, t.Y] = new TrapSweeperTileTrap(this, t.X, t.Y);
						Interlocked.Decrement(ref traps);
					}
					else if (bonus > 0)
					{
						_Grid[t.X, t.Y] = new TrapSweeperTileBonus(this, t.X, t.Y);
						Interlocked.Decrement(ref bonus);
					}
					else
					{
						_Grid[t.X, t.Y] = new TrapSweeperTileBlank(this, t.X, t.Y);
					}
				});

			q.Free(true);

			Blanks = Traps = Bonuses = 0;

			foreach (var t in AllTiles())
			{
				if (t is TrapSweeperTileBlank)
				{
					++Blanks;

					t.Points = p;
				}
				else if (t is TrapSweeperTileTrap)
				{
					++Traps;

					t.Points = 0;
				}
				else if (t is TrapSweeperTileBonus)
				{
					++Bonuses;

					t.Points = p * 2;
				}
			}

			Reset();
		}

		public void GenerateEasy()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			Generate(MinWidth, MinHeight, MinDensity, MinBonusDensity, MinPoints);
		}

		public void GenerateNormal()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			const int w = MinWidth + ((MaxWidth - MinWidth) / 2);
			const int h = MinHeight + ((MaxHeight - MinHeight) / 2);
			const int d = MinDensity + ((MaxDensity - MinDensity) / 2);
			const int b = MinBonusDensity + ((MaxBonusDensity - MinBonusDensity) / 2);
			const int p = MinPoints + ((MaxPoints - MinPoints) / 2);

			Generate(w, h, d, b, p);
		}

		public void GenerateHard()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			Generate(MaxWidth, MaxHeight, MaxDensity, MaxBonusDensity, MaxPoints);
		}

		public void GenerateRandom()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			Generate(
				Utility.RandomMinMax(MinWidth, MaxWidth),
				Utility.RandomMinMax(MinHeight, MaxHeight),
				Utility.RandomMinMax(MinDensity, MaxDensity),
				Utility.RandomMinMax(MinBonusDensity, MaxBonusDensity),
				Utility.RandomMinMax(MinPoints, MaxPoints));
		}

		public void DoCollect()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			// TODO: Give Rewards

			DoMenu();
		}

		public void DoMenu()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			UI.CanDispose = true;

			if (State == TrapSweeperState.Play)
			{
				State = TrapSweeperState.Menu;
				DoEnd(false);
			}
			else
			{
				State = TrapSweeperState.Menu;
				UI.Refresh(true);
			}
		}

		public void DoMode(TrapSweeperMode opt)
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			Mode = opt;

			UI.Refresh(true);
		}

		public void DoMark()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			Mark = !Mark;

			UI.Refresh(true);
		}

		public void DoPlay()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			UI.CanDispose = false;

			switch (Mode)
			{
				case TrapSweeperMode.Easy:
					GenerateEasy();
					break;
				case TrapSweeperMode.Normal:
					GenerateNormal();
					break;
				case TrapSweeperMode.Hard:
					GenerateHard();
					break;
				case TrapSweeperMode.Random:
					GenerateRandom();
					break;
			}

			DoStart();
		}

		public void DoQuit()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			UI.CanDispose = true;

			if (State == TrapSweeperState.Play)
			{
				State = TrapSweeperState.Menu;
				DoEnd(false);
			}
			else if (State != TrapSweeperState.Menu)
			{
				State = TrapSweeperState.Menu;
				UI.Refresh(true);
			}
			else
			{
				State = TrapSweeperState.Menu;
				UI.Close(true);
			}
		}

		public void DoStart()
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			UI.CanDispose = false;

			State = TrapSweeperState.Play;
			Started = DateTime.UtcNow;

			UI.Refresh(true);
		}

		public void DoEnd(bool win)
		{
			if (!Validate())
			{
				Dispose();
				return;
			}

			UI.CanDispose = true;

			State = win ? TrapSweeperState.Win : TrapSweeperState.Lose;
			Ended = DateTime.UtcNow;

			if (win)
			{
				var factor = 1.0;

				var time = Ended - Started;

				if (time.TotalMinutes > 0)
				{
					double threshold, multiplier;

					switch (Mode)
					{
						case TrapSweeperMode.Easy:
						{
							threshold = 10.0;
							multiplier = 0.33;
						}
						break;
						case TrapSweeperMode.Normal:
						{
							threshold = 10.0;
							multiplier = 0.66;
						}
						break;
						case TrapSweeperMode.Hard:
						{
							threshold = 10.0;
							multiplier = 1.00;
						}
						break;
						default:
						{
							threshold = Utility.RandomMinMax(10.0, 30.0);
							multiplier = Utility.RandomMinMax(0.33, 1.00);
						}
						break;
					}

					if (time.TotalMinutes <= threshold)
					{
						factor += (1.0 - (time.TotalMinutes / threshold)) * multiplier;
					}
				}

				OffsetPoints(Points * factor, true);

				Log("Victories", 1, true);
			}
			else
			{
				OffsetPoints(-Points, true);

				Log("Defeats", 1, true);
			}

			UI.Refresh(true);
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			if (_Grid != null)
			{
				_Grid.ForEach(
					(x, y, t) =>
					{
						if (t != null)
						{
							t.Dispose();
						}
					});

				_Grid.Free(true);
				_Grid = null;
			}
		}

		public abstract class TrapSweeperTile : IDisposable
		{
			public virtual int HiddenID => 9026;
			public virtual int MarkID => 9026;
			public virtual int ClickID => 9021;
			public virtual int DisplayID => 9021;

			public virtual int Hue => 0;

			public bool IsDisposed { get; private set; }

			public TrapSweeperEngine Engine { get; private set; }

			public int X { get; private set; }
			public int Y { get; private set; }

			public int Points { get; set; }

			private bool _Marked;

			public bool Marked
			{
				get => _Marked;
				set
				{
					if (!Validate())
					{
						Dispose();
						return;
					}

					if (!_Marked && value)
					{
						if (Engine.Marked < Engine.Traps)
						{
							_Marked = true;
							OnMarked();
						}
					}
					else if (_Marked && !value)
					{
						_Marked = false;
						OnUnmarked();
					}
				}
			}

			private bool _Visible;

			public bool Visible
			{
				get => _Visible;
				set
				{
					if (!Validate())
					{
						Dispose();
						return;
					}

					if (!_Visible && value)
					{
						_Visible = true;
						OnReveal();
					}
					else if (_Visible && !value)
					{
						_Visible = false;
						OnHide();
					}
				}
			}

			public TrapSweeperTile(TrapSweeperEngine g, int x, int y)
			{
				Engine = g;

				X = x;
				Y = y;
			}

			~TrapSweeperTile()
			{
				Dispose();
			}

			public bool Validate()
			{
				return !IsDisposed && Engine != null && Engine.Validate();
			}

			public void Click()
			{
				if (!Validate())
				{
					Dispose();
					return;
				}

				if (Engine.Mark)
				{
					ToggleMark();
				}
				else
				{
					ToggleVisibility();
				}

				Engine.UI.Refresh(true);
			}

			protected void ToggleMark()
			{
				if (!Validate())
				{
					Dispose();
					return;
				}

				if (!Visible)
				{
					Marked = !Marked;
				}
			}

			protected void ToggleVisibility()
			{
				if (!Validate())
				{
					Dispose();
					return;
				}

				Marked = false;
				Visible = !Visible;
			}

			public void Dispose()
			{
				if (IsDisposed)
				{
					return;
				}

				IsDisposed = true;

				OnDispose();

				if (Engine != null)
				{
					if (Engine._Grid != null)
					{
						Engine._Grid[X, Y] = null;
					}

					Engine = null;
				}

				X = Y = -1;
			}

			protected virtual void OnHide()
			{
				--Engine.Visible;

				Engine.OffsetPoints(-Points, true);
			}

			protected virtual void OnReveal()
			{
				++Engine.Visible;

				Engine.OffsetPoints(Points, true);
			}

			protected virtual void OnMarked()
			{
				++Engine.Marked;
			}

			protected virtual void OnUnmarked()
			{
				--Engine.Marked;
			}

			protected virtual void OnDispose()
			{
				if (Engine == null || Engine.IsDisposed)
				{
					_Visible = _Marked = false;
					return;
				}

				if (_Visible)
				{
					--Engine.Visible;
				}

				if (_Marked)
				{
					--Engine.Marked;
				}

				_Visible = _Marked = false;
			}
		}

		public sealed class TrapSweeperTileBlank : TrapSweeperTile
		{
			private int _Traps = -1;

			public int Traps
			{
				get
				{
					if (!Validate())
					{
						Dispose();
						return 0;
					}

					if (_Traps < 0)
					{
						_Traps = Engine.FindTiles<TrapSweeperTileTrap>(X - 1, Y - 1, 3, 3).Count();
					}

					return _Traps;
				}
			}

			public override int DisplayID => Traps > 0 ? 2225 + (Traps - 1) : 9021;

			public TrapSweeperTileBlank(TrapSweeperEngine g, int x, int y)
				: base(g, x, y)
			{ }

			protected override void OnReveal()
			{
				base.OnReveal();

				if (Engine.AllTiles<TrapSweeperTileBlank>().All(t => t.Visible))
				{
					Engine.DoEnd(true);
					return;
				}

				Engine.IncreasePoints(Points, true);

				if (Traps > 0)
				{
					return;
				}

				var p = 0.0;

				foreach (var t in Engine.FindTiles<TrapSweeperTileBlank>(X - 1, Y - 1, 3, 3).Where(t => t != this && !t.Visible))
				{
					t.Visible = true;
					p += t.Points;
				}

				Engine.OffsetPoints(p, true);
			}
		}

		public sealed class TrapSweeperTileTrap : TrapSweeperTile
		{
			public override int DisplayID => 9020;
			public override int Hue => 34;

			public TrapSweeperTileTrap(TrapSweeperEngine g, int x, int y)
				: base(g, x, y)
			{ }

			/*protected override void OnMarked()
			{
				base.OnMarked();

				if (!Engine.AllTiles<SweeperTileTrap>().All(t => t.Marked))
				{
					return;
				}

				Engine.Rewards += Engine.AllTiles<SweeperTileBonus>().Count(t => !t.Visible);

				Engine.DoEnd(true);
			}*/

			protected override void OnReveal()
			{
				base.OnReveal();

				Engine.DecreasePoints(Points, true);

				Engine.DoEnd(false);
			}
		}

		public sealed class TrapSweeperTileBonus : TrapSweeperTile
		{
			public override int DisplayID => 9027;
			public override int Hue => 85;

			public TrapSweeperTileBonus(TrapSweeperEngine g, int x, int y)
				: base(g, x, y)
			{ }

			protected override void OnReveal()
			{
				base.OnReveal();

				++Engine.Rewards;
			}

			protected override void OnHide()
			{
				base.OnHide();

				--Engine.Rewards;
			}
		}
	}
}
