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

using VitaNex.Network;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[CustomEnum(new[] { "None", "North", "Right", "East", "Down", "South", "Left", "West", "Up", "Random" })]
	public enum PvPBattleWeatherDirection : short
	{
		None = -0x01,
		North = Direction.North,
		Right = Direction.Right,
		East = Direction.East,
		Down = Direction.Down,
		South = Direction.South,
		Left = Direction.Left,
		West = Direction.West,
		Up = Direction.Up,
		Random = 0x80
	}

	public class PvPBattleWeather : PropertyObject
	{
		public static int DefaultEffectID = 14036;
		public static int DefaultImpactEffectID = 14000;
		public static int DefaultImpactSound = 549;

		private int _Density;
		private int _EffectHue;
		private int _EffectID;
		private int _EffectSpeed;
		private int _ImpactEffectDuration;
		private int _ImpactEffectHue;
		private int _ImpactEffectID;
		private int _ImpactEffectSound;
		private int _ImpactEffectSpeed;

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Enabled { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Force { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Impacts { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int Density { get => _Density; set => _Density = Math.Max(1, Math.Min(100, value)); }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleWeatherDirection Direction { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int EffectID { get => _EffectID; set => _EffectID = Math.Max(0, value); }

		[CommandProperty(AutoPvP.Access)]
		public virtual int EffectHue { get => _EffectHue; set => _EffectHue = Math.Max(0, Math.Min(3000, value)); }

		[CommandProperty(AutoPvP.Access)]
		public virtual int EffectSpeed
		{
			get => _EffectSpeed;
			set => _EffectSpeed = Math.Max(1, Math.Min(10, value));
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual EffectRender EffectRender { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int ImpactEffectID { get => _ImpactEffectID; set => _ImpactEffectID = Math.Max(0, value); }

		[CommandProperty(AutoPvP.Access)]
		public virtual int ImpactEffectHue
		{
			get => _ImpactEffectHue;
			set => _ImpactEffectHue = Math.Max(0, Math.Min(3000, value));
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual int ImpactEffectSpeed
		{
			get => _ImpactEffectSpeed;
			set => _ImpactEffectSpeed = Math.Max(1, Math.Min(10, value));
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual int ImpactEffectDuration
		{
			get => _ImpactEffectDuration;
			set => _ImpactEffectDuration = Math.Max(0, value);
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual EffectRender ImpactEffectRender { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int ImpactEffectSound
		{
			get => _ImpactEffectSound;
			set => _ImpactEffectSound = Math.Max(0, value);
		}

		public PvPBattleWeather()
		{
			Enabled = true;
			Force = false;
			Impacts = true;
			Density = 6;
			Direction = PvPBattleWeatherDirection.South;

			EffectID = DefaultEffectID;
			EffectHue = 0;
			EffectSpeed = 10;
			EffectRender = EffectRender.Normal;

			ImpactEffectID = DefaultImpactEffectID;
			ImpactEffectHue = 0;
			ImpactEffectSpeed = 10;
			ImpactEffectDuration = 30;
			ImpactEffectRender = EffectRender.Normal;
			ImpactEffectSound = DefaultImpactSound;
		}

		public PvPBattleWeather(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Battle Weather";
		}

		public override void Clear()
		{
			Enabled = false;
			Force = false;
			Impacts = false;
			Density = 0;
			Direction = PvPBattleWeatherDirection.None;

			EffectID = 0;
			EffectHue = 0;
			EffectSpeed = 1;
			EffectRender = EffectRender.Normal;

			ImpactEffectID = 0;
			ImpactEffectHue = 0;
			ImpactEffectSpeed = 1;
			ImpactEffectDuration = 0;
			ImpactEffectRender = EffectRender.Normal;
			ImpactEffectSound = 0;
		}

		public override void Reset()
		{
			Enabled = true;
			Force = false;
			Impacts = true;
			Density = 6;
			Direction = PvPBattleWeatherDirection.South;

			EffectID = DefaultEffectID;
			EffectHue = 0;
			EffectSpeed = 10;
			EffectRender = EffectRender.Normal;

			ImpactEffectID = DefaultImpactEffectID;
			ImpactEffectHue = 0;
			ImpactEffectSpeed = 10;
			ImpactEffectDuration = 30;
			ImpactEffectRender = EffectRender.Normal;
			ImpactEffectSound = DefaultImpactSound;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(Enabled);
					writer.Write(Force);

					writer.Write(Impacts);
					writer.Write(_Density);
					writer.WriteFlag(Direction);

					writer.Write(_EffectID);
					writer.Write(_EffectHue);
					writer.Write(_EffectSpeed);
					writer.WriteFlag(EffectRender);

					writer.Write(_ImpactEffectID);
					writer.Write(_ImpactEffectHue);
					writer.Write(_ImpactEffectSpeed);
					writer.Write(_ImpactEffectDuration);
					writer.WriteFlag(ImpactEffectRender);
					writer.Write(_ImpactEffectSound);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					Enabled = reader.ReadBool();
					Force = reader.ReadBool();

					Impacts = reader.ReadBool();
					_Density = reader.ReadInt();
					Direction = reader.ReadFlag<PvPBattleWeatherDirection>();

					_EffectID = reader.ReadInt();
					_EffectHue = reader.ReadInt();
					_EffectSpeed = reader.ReadInt();
					EffectRender = reader.ReadFlag<EffectRender>();

					_ImpactEffectID = reader.ReadInt();
					_ImpactEffectHue = reader.ReadInt();
					_ImpactEffectSpeed = reader.ReadInt();
					_ImpactEffectDuration = reader.ReadInt();
					ImpactEffectRender = reader.ReadFlag<EffectRender>();
					_ImpactEffectSound = reader.ReadInt();
				}
				break;
			}
		}
	}
}