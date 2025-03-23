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

using Server;
using Server.Items;
#endregion

namespace VitaNex.Items
{
	public class StrobeLantern : Lantern
	{
		private static readonly short[] _Hues = { 11, 22, 33, 44, 55, 66, 77, 88, 99 };
		private static readonly LightType[] _Lights = { LightType.Circle150, LightType.Circle225, LightType.Circle300 };

		public static List<StrobeLantern> Instances { get; private set; }

		private static readonly PollTimer _Timer;

		static StrobeLantern()
		{
			Instances = new List<StrobeLantern>();

			_Timer = PollTimer.FromSeconds(1.0, CheckStrobe, Instances.Any);
		}

		private static void CheckStrobe()
		{
			Instances.ForEachReverse(o => o.Strobe());
		}

		private long _NextUpdate;
		private int _StrobeIndex;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool StrobeReverse { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan StrobeInterval { get; set; }

		[Hue, CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public override int Hue
		{
			get
			{
				if (CanStrobe() && Hues.InBounds(_StrobeIndex))
				{
					return Hues[_StrobeIndex];
				}

				return base.Hue;
			}
			set => base.Hue = value;
		}

		public virtual short[] Hues => _Hues;
		public virtual LightType[] Lights => _Lights;

		public virtual LightType DefaultLight => LightType.DarkCircle300;
		public virtual TimeSpan DefaultStrobeInterval => TimeSpan.FromSeconds(0.5);

		[Constructable]
		public StrobeLantern()
		{
			Name = "Strobe Lantern";
			Weight = 4;
			Hue = 0;

			Light = DefaultLight;
			StrobeInterval = DefaultStrobeInterval;

			Instances.Add(this);
		}

		public StrobeLantern(Serial serial)
			: base(serial)
		{
			Instances.Add(this);
		}

		public override void OnDelete()
		{
			base.OnDelete();

			Instances.Remove(this);
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			Instances.Remove(this);
		}

		public virtual bool CanStrobe()
		{
			return !Deleted && Burning && Map != null && Map != Map.Internal && !Hues.IsNullOrEmpty();
		}

		public void Strobe()
		{
			if (!CanStrobe() || VitaNexCore.Ticks < _NextUpdate)
			{
				return;
			}

			_NextUpdate = VitaNexCore.Ticks + (int)StrobeInterval.TotalMilliseconds;

			var index = ++_StrobeIndex % Hues.Length;

			if (StrobeReverse)
			{
				index = (Hues.Length - index) - 1;
			}

			_StrobeIndex = index;

			OnStrobe();
		}

		public override void Ignite()
		{
			var old = Burning;

			base.Ignite();

			if (!old && Burning)
			{
				OnStrobeBegin();
			}
			else if (old && !Burning)
			{
				OnStrobeEnd();
			}
		}

		public override void Douse()
		{
			var old = Burning;

			base.Douse();

			if (!old && Burning)
			{
				OnStrobeBegin();
			}
			else if (old && !Burning)
			{
				OnStrobeEnd();
			}
		}

		protected virtual void OnStrobe()
		{
			Light = Lights.GetRandom(Light);

			Update();
		}

		protected virtual void OnStrobeBegin()
		{
			Light = Lights.GetRandom(Light);

			Update();
		}

		protected virtual void OnStrobeEnd()
		{
			Light = DefaultLight;

			Update();
		}

		public virtual void Update()
		{
			ReleaseWorldPackets();
			Delta(ItemDelta.Update);
		}

		public override void OnDoubleClick(Mobile m)
		{
			var access = Protected ? AccessLevel.Counselor : AccessLevel.Player;

			if (this.CheckUse(m, true, false, 2, false, false, true, access))
			{
				base.OnDoubleClick(m);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var v = writer.SetVersion(2);

			switch (v)
			{
				case 2:
				{
					writer.Write(StrobeReverse);
					writer.Write(StrobeInterval);

					writer.Write(Burning);
				}
				break;
				// Old
				case 1:
					writer.Write(Burning);
					goto case 0;
				case 0:
				{
					writer.Write(StrobeInterval);
					writer.Write(_StrobeIndex);
					writer.Write(StrobeReverse);

					writer.WriteArray(Hues, (w, o) => w.Write(o));
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var v = reader.GetVersion();

			var burning = false;

			switch (v)
			{
				case 2:
				{
					StrobeReverse = reader.ReadBool();
					StrobeInterval = reader.ReadTimeSpan();

					burning = reader.ReadBool();
				}
				break;
				// Old
				case 1:
					burning = reader.ReadBool();
					goto case 0;
				case 0:
				{
					StrobeInterval = reader.ReadTimeSpan();
					_StrobeIndex = reader.ReadInt();
					StrobeReverse = reader.ReadBool();

					reader.ReadArray(r => r.ReadShort());
				}
				break;
			}

			if (burning)
			{
				Timer.DelayCall(Ignite);
			}
		}
	}
}