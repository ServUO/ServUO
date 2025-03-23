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
using Server.Gumps;
using Server.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpImageShadow : SuperGumpEntry, IGumpEntryPoint
	{
		private const string _Format1A = "{{ gumppic {0} {1} {2} }}";
		private const string _Format1B = "{{ gumppic {0} {1} {2} hue={3} }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("gumppic");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ gumppic");
		private static readonly byte[] _Layout1Hue = Gump.StringToBuffer(" hue=");

		private int _X, _Y;
		private int _ImageID, _ImageHue;
		private Angle _ShadowAngle;
		private int _ShadowOffset, _ShadowHue;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public int ImageID { get => _ImageID; set => Delta(ref _ImageID, value); }
		public int ImageHue { get => _ImageHue; set => Delta(ref _ImageHue, value); }

		public Angle ShadowAngle { get => _ShadowAngle; set => Delta(ref _ShadowAngle, value); }
		public int ShadowOffset { get => _ShadowOffset; set => Delta(ref _ShadowOffset, value); }
		public int ShadowHue { get => _ShadowHue; set => Delta(ref _ShadowHue, value); }

		public GumpImageShadow(int x, int y, int imageID)
			: this(x, y, imageID, 0)
		{ }

		public GumpImageShadow(int x, int y, int imageID, int imageHue)
			: this(x, y, imageID, imageHue, 45)
		{ }

		public GumpImageShadow(int x, int y, int imageID, int imageHue, Angle shadowAngle)
			: this(x, y, imageID, imageHue, shadowAngle, 5)
		{ }

		public GumpImageShadow(int x, int y, int imageID, int imageHue, Angle shadowAngle, int shadowOffset)
			: this(x, y, imageID, imageHue, shadowAngle, shadowOffset, 2999)
		{ }

		public GumpImageShadow(int x, int y, int imageID, int imageHue, Angle shadowAngle, int shadowOffset, int shadowHue)
		{
			_X = x;
			_Y = y;
			_ImageID = imageID;
			_ImageHue = imageHue;
			_ShadowAngle = shadowAngle;
			_ShadowOffset = shadowOffset;
			_ShadowHue = shadowHue;
		}

		public override string Compile()
		{
			var compiled = String.Empty;

			var s = _ShadowAngle.GetPoint2D(_X, _Y, _ShadowOffset);

			if (_ShadowHue <= 0)
			{
				compiled += String.Format(_Format1A, s.X, s.Y, _ImageID);
			}
			else
			{
				compiled += String.Format(_Format1B, s.X, s.Y, _ImageID, FixHue(_ShadowHue));
			}

			if (_ImageHue <= 0)
			{
				compiled += String.Format(_Format1A, _X, _Y, _ImageID);
			}
			else
			{
				compiled += String.Format(_Format1B, _X, _Y, _ImageID, FixHue(_ImageHue));
			}

			return compiled;
		}

		public override void AppendTo(IGumpWriter disp)
		{
			var s = _ShadowAngle.GetPoint2D(_X, _Y, _ShadowOffset);

			disp.AppendLayout(_Layout1A);
			disp.AppendLayout(s.X);
			disp.AppendLayout(s.Y);
			disp.AppendLayout(_ImageID);

			if (_ShadowHue > 0)
			{
				disp.AppendLayout(_Layout1Hue);
				disp.AppendLayoutNS(FixHue(_ShadowHue));
			}

			disp.AppendLayout(_Layout1B);
			disp.AppendLayout(_X);
			disp.AppendLayout(_Y);
			disp.AppendLayout(_ImageID);

			if (_ImageHue > 0)
			{
				disp.AppendLayout(_Layout1Hue);
				disp.AppendLayoutNS(FixHue(_ImageHue));
			}
		}
	}
}