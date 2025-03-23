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

using Server;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.Games
{
	public abstract class GameEngine<TGame, TGump> : IGameEngine
		where TGame : class, IGame
		where TGump : SuperGump, IGameUI
	{
		private readonly Type _UIType = typeof(TGump);

		public Type UIType => _UIType;

		public bool IsDisposing { get; private set; }
		public bool IsDisposed { get; private set; }

		public Mobile User { get; private set; }

		public TGame Game { get; private set; }
		public TGump UI { get; private set; }

		public ArcadeProfile Profile => Arcade.EnsureProfile(User);

		public GameStatistics Statistics { get; private set; }

		public double PointsTotal
		{
			get => Statistics["Points Total"];
			private set => Statistics["Points Total"] = value;
		}

		public double PointsGained
		{
			get => Statistics["Points Gained"];
			private set => Statistics["Points Gained"] = value;
		}

		public double PointsLost
		{
			get => Statistics["Points Lost"];
			private set => Statistics["Points Lost"] = value;
		}

		public double Points { get; private set; }

		public GameEngine(TGame game, Mobile user)
		{
			Game = game;
			User = user;

			Statistics = new GameStatistics();
		}

		~GameEngine()
		{
			Dispose();
		}

		protected bool EnsureUI()
		{
			if (IsDisposed || IsDisposing)
			{
				return false;
			}

			if (User == null || User.Deleted)
			{
				if (UI != null)
				{
					UI.Close(true);
					UI = null;
				}

				return false;
			}

			if (UI != null && UI.User != User)
			{
				UI.Close(true);
				UI = null;
			}

			if (UI == null || UI.IsDisposed || UI.IsDisposing)
			{
				UI = UIType.CreateInstanceSafe<TGump>(this);
			}

			return UI != null && UI.Validate();
		}

		public bool Validate()
		{
			return !IsDisposed && !IsDisposing && UI != null && UI.Validate();
		}

		public virtual bool Open()
		{
			if (!EnsureUI() || !Validate())
			{
				Dispose();
				return false;
			}

			UI.Refresh(true);

			return true;
		}

		public virtual void Close()
		{
			if (UI != null && !UI.IsDisposed)
			{
				UI.Close(true);
			}
		}

		public virtual void Reset()
		{
			Points = 0;

			Statistics.Reset();
		}

		public void LogStatistics()
		{
			var p = Profile;

			foreach (var kv in Statistics)
			{
				Game.Log(kv.Key, kv.Value, true);

				if (p != null)
				{
					p.Log(Game, kv.Key, kv.Value, true);
				}
			}

			Statistics.Reset();
		}

		public void Log(string context, double value, bool offset)
		{
			if (offset)
			{
				if (value > 0)
				{
					LogIncrease(context, value);
				}
				else if (value < 0)
				{
					LogDecrease(context, value);
				}
			}
			else
			{
				Statistics[context] = value;
			}
		}

		public void LogIncrease(string context, double value)
		{
			value = Math.Abs(value);

			Statistics[context] += value;
		}

		public void LogDecrease(string context, double value)
		{
			value = Math.Abs(value);

			Statistics[context] -= value;
		}

		public void OffsetPoints(double value, bool log)
		{
			if (value > 0)
			{
				IncreasePoints(value, log);
			}
			else if (value < 0)
			{
				DecreasePoints(value, log);
			}
		}

		public void IncreasePoints(double value, bool log)
		{
			value = Math.Abs(value);

			Points += value;
			PointsTotal += value;

			if (log)
			{
				PointsGained += value;
			}
		}

		public void DecreasePoints(double value, bool log)
		{
			value = Math.Abs(value);

			Points -= value;
			PointsTotal -= value;

			if (log)
			{
				PointsLost += value;
			}
		}

		public void Dispose()
		{
			if (IsDisposed || IsDisposing)
			{
				return;
			}

			IsDisposing = true;

			LogStatistics();

			Close();

			if (UI == null)
			{
				return;
			}

			if (UI.IsOpen || UI.Hidden)
			{
				UI.Close(true);
			}

			UI = null;

			OnDispose();

			if (Game != null && User != null)
			{
				Game[User] = null;
			}

			Statistics.Clear();
			Statistics = null;

			Game = null;
			User = null;

			IsDisposed = true;

			OnDisposed();

			IsDisposing = false;
		}

		protected virtual void OnDispose()
		{ }

		protected virtual void OnDisposed()
		{ }

		public virtual void Serialize(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.Write(Points);

			Statistics.Serialize(writer);
		}

		public virtual void Deserialize(GenericReader reader)
		{
			reader.GetVersion();

			Points = reader.ReadDouble();

			Statistics.Deserialize(reader);
		}

		#region Explicit Impl
		IGame IGameEngine.Game => Game;
		IGameUI IGameEngine.UI => UI;
		#endregion
	}
}