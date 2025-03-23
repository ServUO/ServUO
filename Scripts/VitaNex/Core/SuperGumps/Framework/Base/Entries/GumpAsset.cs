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
using System.Drawing;
using System.IO;

using Server.Gumps;
using Server.Network;

using VitaNex.IO;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpAsset : SuperGumpEntry, IGumpEntryVector
	{
		private const string _Format1 = "{{ htmlgump {0} {1} {2} {3} {4} 0 0 }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("htmlgump");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ htmlgump");

		private int _X, _Y;
		private VirtualAsset _Asset;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public VirtualAsset Asset { get => _Asset; set => Delta(ref _Asset, value); }

		public virtual int Width
		{
			get => _Asset != null ? _Asset.Width : 0;
			set
			{
				if (_Asset != null)
				{
					_Asset.Width = value;
				}
			}
		}

		public virtual int Height
		{
			get => _Asset != null ? _Asset.Height : 0;
			set
			{
				if (_Asset != null)
				{
					_Asset.Height = value;
				}
			}
		}

		public GumpAsset(int x, int y, string path)
			: this(x, y, VirtualAsset.LoadAsset(path))
		{ }

		public GumpAsset(int x, int y, Uri url)
			: this(x, y, VirtualAsset.LoadAsset(url))
		{ }

		public GumpAsset(int x, int y, FileInfo file)
			: this(x, y, VirtualAsset.LoadAsset(file))
		{ }

		public GumpAsset(int x, int y, VirtualAsset asset)
		{
			_X = x;
			_Y = y;
			_Asset = asset;
		}

		public override string Compile()
		{
			if (IsEnhancedClient)
			{
				return String.Empty;
			}

			var compiled = String.Empty;

			if (!VirtualAsset.IsNullOrEmpty(_Asset))
			{
				_Asset.ForEach(
					(x, y, c) =>
					{
						if (!c.IsEmpty && c != Color.Transparent)
						{
							compiled += Compile(x, y, 1, 1, c);
						}
					});
			}

			if (String.IsNullOrWhiteSpace(compiled))
			{
				compiled = Compile(_X, _Y, Width, Height, Color.Transparent);
			}

			return compiled;
		}

		public virtual string Compile(int x, int y, int w, int h, Color c)
		{
			return String.Format(_Format1, _X + x, _Y + y, w, h, Parent.Intern(" ".WrapUOHtmlBG(c)));
		}

		public override void AppendTo(IGumpWriter disp)
		{
			if (IsEnhancedClient)
			{
				AppendEmptyLayout(disp);
				return;
			}

			var first = true;

			if (!VirtualAsset.IsNullOrEmpty(_Asset))
			{
				_Asset.ForEach(
					(x, y, c) =>
					{
						if (!c.IsEmpty && c != Color.Transparent)
						{
							AppendTo(disp, ref first, x, y, 1, 1, c);
						}
					});
			}

			if (first)
			{
				AppendTo(disp, ref first, _X, _Y, Width, Height, Color.Transparent);
			}
		}

		public virtual void AppendTo(IGumpWriter disp, ref bool first, int x, int y, int w, int h, Color c)
		{
			disp.AppendLayout(first ? _Layout1A : _Layout1B);
			disp.AppendLayout(_X + x);
			disp.AppendLayout(_Y + y);
			disp.AppendLayout(1);
			disp.AppendLayout(1);
			disp.AppendLayout(Parent.Intern(" ".WrapUOHtmlBG(c)));
			disp.AppendLayout(false);
			disp.AppendLayout(false);

			first = false;
		}

		public override void Dispose()
		{
			_Asset = null;

			base.Dispose();
		}
	}
}