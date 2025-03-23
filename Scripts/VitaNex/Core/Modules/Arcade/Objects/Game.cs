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
using System.Collections;
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex.Modules.Games
{
	public abstract class Game<TEngine> : IGame, IEnumerable<TEngine>
		where TEngine : class, IGameEngine
	{
		private readonly Type _EngineType = typeof(TEngine);

		[CommandProperty(Arcade.Access)]
		public Type EngineType => _EngineType;

		[CommandProperty(Arcade.Access)]
		public abstract IconDefinition Icon { get; }

		[CommandProperty(Arcade.Access)]
		public abstract string Name { get; }

		[CommandProperty(Arcade.Access)]
		public abstract string Desc { get; }

		[CommandProperty(Arcade.Access)]
		public abstract string Help { get; }

		private bool _Enabled;

		[CommandProperty(Arcade.Access)]
		public bool Enabled
		{
			get => _Enabled;
			set
			{
				if (_Enabled && !value)
				{
					_Enabled = false;

					OnDisabled();
					Flush();
				}
				else if (!_Enabled && value)
				{
					_Enabled = true;

					OnEnabled();
				}
			}
		}

		private List<TEngine> _Sessions;

		public IEnumerable<TEngine> Sessions
		{
			get
			{
				var c = _Sessions.Count;

				while (--c >= 0)
				{
					if (!_Sessions.InBounds(c))
					{
						continue;
					}

					var e = _Sessions[c];

					if (e != null && !e.IsDisposed && !e.IsDisposing)
					{
						yield return e;
					}
				}
			}
		}

		[CommandProperty(Arcade.Access)]
		public int SessionCount => _Sessions.Count;

		public TEngine this[Mobile user]
		{
			get => _Sessions.Find(e => e != null && e.User == user);
			private set
			{
				if (user == null)
				{
					return;
				}

				var e = this[user];

				if (value != null)
				{
					if (e == value)
					{
						return;
					}

					if (e != null)
					{
						e.Dispose();

						_Sessions.Remove(e);
					}

					_Sessions.Update(value);
				}
				else if (e != null)
				{
					e.Dispose();

					_Sessions.Remove(e);
				}
			}
		}

		[CommandProperty(Arcade.Access)]
		public GameStatistics Statistics { get; private set; }

		public Game()
		{
			_Sessions = new List<TEngine>();

			Statistics = new GameStatistics();
		}

		public void Flush()
		{
			foreach (var e in Sessions)
			{
				e.Dispose();
			}

			_Sessions.Clear();
		}

		public void Enable()
		{
			Enabled = true;
		}

		public void Disable()
		{
			Enabled = false;
		}

		protected virtual void OnEnabled()
		{ }

		protected virtual void OnDisabled()
		{ }

		protected TEngine EnsureSession(Mobile user)
		{
			if (user == null)
			{
				return null;
			}

			var e = this[user];

			if (e == null || !e.IsDisposing || e.IsDisposed)
			{
				if (user.Deleted || !user.IsOnline())
				{
					return null;
				}

				this[user] = e = EngineType.CreateInstanceSafe<TEngine>(this, user);
			}
			else if (user.Deleted || !user.IsOnline())
			{
				this[user] = null;
			}

			return e;
		}

		public bool Validate(Mobile user)
		{
			var e = EnsureSession(user);

			return e != null && e.Validate();
		}

		public bool Open(Mobile user)
		{
			var e = EnsureSession(user);

			if (e == null)
			{
				return false;
			}

			if (!e.Open() || !e.Validate())
			{
				e.Dispose();
				return false;
			}

			return true;
		}

		public void Close(Mobile user)
		{
			this[user] = null;
		}

		public void Reset(Mobile user)
		{
			var e = EnsureSession(user);

			if (e != null && e.Validate())
			{
				e.Reset();
			}
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

		public IEnumerator<TEngine> GetEnumerator()
		{
			return _Sessions.GetEnumerator();
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(_Enabled);

					Statistics.Serialize(writer);

					writer.WriteBlockList(
						_Sessions,
						(w, e) =>
						{
							w.Write(e.User);
							e.Serialize(w);
						});
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					_Enabled = reader.ReadBool();

					Statistics.Deserialize(reader);

					_Sessions = reader.ReadBlockList(
						r =>
						{
							var e = EnsureSession(r.ReadMobile());

							e.Deserialize(r);

							return e;
						},
						_Sessions);
				}
				break;
			}
		}

		#region Explicit Impl
		IEnumerable<IGameEngine> IGame.Sessions => Sessions;

		IGameEngine IGame.this[Mobile user]
		{
			get => this[user];
			set
			{
				if (value == null || value is TEngine)
				{
					this[user] = (TEngine)value;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion
	}
}