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
using System.Drawing;

using Server;
#endregion

namespace VitaNex.Modules.CastBars
{
	public sealed class CastBarState : PropertyObject
	{
		public static bool DefEnabled = false;
		public static Point DefOffset = new Point(480, 460);

		[CommandProperty(SpellCastBars.Access)]
		public bool Enabled { get; set; }

		[CommandProperty(SpellCastBars.Access)]
		public Point Offset { get; set; }

		public CastBarState()
		{
			SetDefaults();
		}

		public CastBarState(bool enabled)
			: this(enabled, DefOffset)
		{ }

		public CastBarState(Point offset)
			: this(DefEnabled, offset)
		{ }

		public CastBarState(bool enabled, Point offset)
		{
			Enabled = enabled;
			Offset = offset;
		}

		public CastBarState(GenericReader reader)
			: base(reader)
		{ }

		public void SetDefaults()
		{
			Enabled = DefEnabled;
			Offset = DefOffset;
		}

		public override void Clear()
		{
			SetDefaults();
		}

		public override void Reset()
		{
			SetDefaults();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Enabled);

			writer.Write(Offset.X);
			writer.Write(Offset.Y);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Enabled = reader.ReadBool();

			Offset = new Point(reader.ReadInt(), reader.ReadInt());
		}
	}
}