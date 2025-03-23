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
using System.Threading.Tasks;

using Server;
using Server.Network;

using VitaNex.FX;
using VitaNex.Network;
#endregion

namespace VitaNex.Items
{
	public class GhostMulti : Item
	{
		private PollTimer _Timer;
		private Point3D _Center;
		private List<EffectInfo> _Effects;

		private int _MultiID;
		private EffectRender _Render;
		private TimeSpan _Interval;
		private int _Duration;
		private int _Speed;

		[Hue, CommandProperty(AccessLevel.GameMaster)]
		public override int Hue
		{
			get => base.Hue;
			set
			{
				value = Math.Max(0, Math.Min(3000, value));

				base.Hue = value;

				if (_Effects == null)
				{
					return;
				}

				foreach (var e in _Effects)
				{
					e.Hue = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int MultiID
		{
			get => _MultiID;
			set
			{
				if (_MultiID == value)
				{
					return;
				}

				_MultiID = value;

				Stop();
				Start();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public EffectRender Render
		{
			get => _Render;
			set
			{
				_Render = value;

				if (_Effects == null)
				{
					return;
				}

				foreach (var e in _Effects)
				{
					e.Render = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan Interval
		{
			get => _Interval;
			set
			{
				_Interval = value;

				if (_Timer != null)
				{
					_Timer.Interval = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Duration
		{
			get => _Duration;
			set
			{
				_Duration = value;

				if (_Effects == null)
				{
					return;
				}

				foreach (var e in _Effects)
				{
					e.Duration = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Speed
		{
			get => _Speed;
			set
			{
				_Speed = value;

				if (_Effects == null)
				{
					return;
				}

				foreach (var e in _Effects)
				{
					e.Speed = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active { get; set; }

		public override bool Decays => false;
		public override bool IsVirtualItem => true;
		public override bool HandlesOnMovement => true;

		public virtual bool PlayerRangeSensitive => true;
		public virtual TimeSpan ActivityInterval => TimeSpan.FromSeconds(30.0);

		private PollTimer _ActivityTimer;
		private DateTime _LastActivity;

		[Constructable]
		public GhostMulti()
			: this(1)
		{ }

		[Constructable]
		public GhostMulti(int multiID)
			: this(14284, multiID)
		{ }

		[Constructable]
		public GhostMulti(int itemID, int multiID)
			: base(itemID)
		{
			_MultiID = multiID;
			_Interval = TimeSpan.FromSeconds(3.55);
			_Render = EffectRender.Normal;
			_Duration = 73;
			_Speed = 10;

			Name = "Ghostly Structure";
			Light = LightType.DarkCircle300;
			Weight = 0;
			Visible = false;
			Active = true;
		}

		public GhostMulti(Serial serial)
			: base(serial)
		{ }

		public override void OnDelete()
		{
			base.OnDelete();

			Stop();
		}

		public override void OnLocationChange(Point3D oldLocation)
		{
			base.OnLocationChange(oldLocation);

			Start();
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			Start();
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			base.OnMovement(m, oldLocation);

			if (m == null || m.Deleted || !m.Player || m.NetState == null)
			{
				return;
			}

			_LastActivity = DateTime.UtcNow;

			Start();
		}

		public void Start()
		{
			if (Deleted || !Active || Map == null || Map == Map.Internal || Parent != null)
			{
				Stop();
				return;
			}

			if (_Effects == null || _Center != GetWorldLocation())
			{
				ClearEffects();

				_Effects = new List<EffectInfo>(GetEffects());
			}

			if (_Timer == null)
			{
				_Timer = PollTimer.CreateInstance(
					Interval,
					() =>
					{
						if (!Active)
						{
							Stop();
							return;
						}

						SendEffects();
					},
					() => !Deleted && Active && Map != null && Map != Map.Internal && Parent == null);
			}
			else
			{
				_Timer.Running = true;
			}

			if (!PlayerRangeSensitive)
			{
				if (_ActivityTimer != null)
				{
					_ActivityTimer.Running = false;
					_ActivityTimer = null;
				}

				return;
			}

			if (_ActivityTimer != null)
			{
				_ActivityTimer.Running = true;
				return;
			}

			_ActivityTimer = PollTimer.CreateInstance(
				ActivityInterval,
				() =>
				{
					if (DateTime.UtcNow - _LastActivity < ActivityInterval)
					{
						return;
					}

					var clients = GetClientsInRange(Core.GlobalMaxUpdateRange);

					if (clients.Any())
					{
						clients.Free();
						return;
					}

					clients.Free();

					Stop();
				},
				() => !Deleted && Map != null && Map != Map.Internal && Parent == null);
		}

		public void Stop()
		{
			if (_ActivityTimer != null)
			{
				_ActivityTimer.Running = false;
				_ActivityTimer = null;
			}

			ClearEffects();

			if (_Timer == null)
			{
				return;
			}

			_Timer.Running = false;
			_Timer = null;
		}

		protected EffectInfo[] GetEffects()
		{
			var l = GetWorldLocation();
			var c = MultiData.GetComponents(MultiID);

			_Center = l.Clone3D(c.Center.X, c.Center.Y, l.Z);

			var list = new EffectInfo[c.List.Length];

			Parallel.For(
				0,
				list.Length,
				index =>
				{
					var t = c.List[index];
					var p = l.Clone3D(t.m_OffsetX, t.m_OffsetY, t.m_OffsetZ);

					list[index] = new EffectInfo(p, Map, t.m_ItemID, Hue, Speed, Duration, Render);
				});

			return list;
		}

		protected virtual void ClearEffects()
		{
			if (_Effects == null)
			{
				return;
			}

			foreach (var e in _Effects)
			{
				e.Dispose();
			}

			_Effects.Free(true);
			_Effects = null;
		}

		protected virtual void SendEffects()
		{
			if (_Effects == null)
			{
				return;
			}

			foreach (var e in _Effects)
			{
				e.Send();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.Write(Active);
					goto case 0;
				case 0:
				{
					writer.Write(_MultiID);
					writer.Write(_Center);
					writer.WriteFlag(_Render);
					writer.Write(_Duration);
					writer.Write(_Speed);
					writer.Write(_Interval);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 1:
					Active = reader.ReadBool();
					goto case 0;
				case 0:
				{
					_MultiID = reader.ReadInt();
					_Center = reader.ReadPoint3D();
					_Render = reader.ReadFlag<EffectRender>();
					_Duration = reader.ReadInt();
					_Speed = reader.ReadInt();
					_Interval = reader.ReadTimeSpan();
				}
				break;
			}

			Start();
		}
	}
}