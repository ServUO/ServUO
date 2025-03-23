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
using System.Drawing;
using System.Linq;

using Server;
using Server.Gumps;
using Server.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpPaperdoll : SuperGumpEntry, IGumpEntryVector
	{
		private const string _Format0 = "{{ gumphtml {0} {1} {2} {3} {4} 0 0 }}";
		private const string _Format1A = "{{ gumppic {0} {1} {2} }}";
		private const string _Format1B = "{{ gumppic {0} {1} {2} hue={3} }}";
		private const string _Format2 = "{{ tilepic {0} {1} {2} }}";
		private const string _Format3 = "{{ tilepichue {0} {1} {2} {3} }}";
		private const string _Format4 = "{{ croppedtext {0} {1} {2} {3} {4} {5} }}";
		private const string _Format5 = "{{ itemproperty {0} }}";

		private static readonly byte[] _Separator = Gump.StringToBuffer(" }{ ");
		private static readonly byte[] _Layout0 = Gump.StringToBuffer("itemproperty");
		private static readonly byte[] _Layout1 = Gump.StringToBuffer("resizepic");
		private static readonly byte[] _Layout2 = Gump.StringToBuffer(" }{ gumppic");
		private static readonly byte[] _Layout2Hue = Gump.StringToBuffer(" hue=");
		private static readonly byte[] _Layout3 = Gump.StringToBuffer(" }{ tilepic");
		private static readonly byte[] _Layout4 = Gump.StringToBuffer(" }{ tilepichue");
		private static readonly byte[] _Layout5 = Gump.StringToBuffer(" }{ croppedtext");
		private static readonly byte[] _Layout6 = Gump.StringToBuffer(" }{ itemproperty");

		private int _X, _Y;
		private bool _Properties;
		private List<Item> _Items;
		private Body _Body;
		private int _BodyHue;
		private int _SolidHue;
		private int _HairID, _HairHue;
		private int _FacialHairID, _FacialHairHue;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public bool Properties { get => _Properties; set => Delta(ref _Properties, value); }

		public List<Item> Items { get => _Items; set => Delta(ref _Items, value); }

		public Body Body { get => _Body; set => Delta(ref _Body, value); }
		public int BodyHue { get => _BodyHue; set => Delta(ref _BodyHue, value); }

		public int SolidHue { get => _SolidHue; set => Delta(ref _SolidHue, value); }

		public int HairID { get => _HairID; set => Delta(ref _HairID, value); }
		public int HairHue { get => _HairHue; set => Delta(ref _HairHue, value); }

		public int FacialHairID { get => _FacialHairID; set => Delta(ref _FacialHairID, value); }
		public int FacialHairHue { get => _FacialHairHue; set => Delta(ref _FacialHairHue, value); }

		public virtual int Width { get => 260; set { } }
		public virtual int Height { get => 237; set { } }

		public GumpPaperdoll(int x, int y, bool props, Mobile m)
			: this(
				x,
				y,
				props,
				m.Items,
				m.Body,
				m.Hue,
				m.SolidHueOverride,
				m.HairItemID,
				m.HairHue,
				m.FacialHairItemID,
				m.FacialHairHue)
		{ }

		public GumpPaperdoll(
			int x,
			int y,
			bool props,
			IEnumerable<Item> items,
			Body body,
			int bodyHue,
			int solidHue,
			int hairID,
			int hairHue,
			int facialHairID,
			int facialHairHue)
		{
			_X = x;
			_Y = y;
			_Properties = props;
			_Items = items.Ensure().ToList();
			_Body = body;
			_BodyHue = bodyHue;
			_SolidHue = solidHue;
			_HairID = hairID;
			_HairHue = hairHue;
			_FacialHairID = facialHairID;
			_FacialHairHue = facialHairHue;
		}

		public override string Compile()
		{
			var compiled = String.Format(_Format0, _X, _Y, Width, Height, " ".WrapUOHtmlBG(Color.Transparent));

			var hue = _BodyHue & 0x7FFF;

			if (_SolidHue >= 0)
			{
				hue = _SolidHue;
			}

			var gump = ArtworkSupport.LookupGump(_Body);

			if (gump <= 0)
			{
				gump = ShrinkTable.Lookup(_Body, 0);

				if (gump > 0)
				{
					if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
					{
						compiled += String.Format(_Format3, _X, _Y, gump, FixHue(hue));
					}
					else
					{
						compiled += String.Format(_Format2, _X, _Y, gump);
					}
				}

				return compiled;
			}

			if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
			{
				compiled += String.Format(_Format1B, _X, _Y, gump, FixHue(hue));
			}
			else
			{
				compiled += String.Format(_Format1A, _X, _Y, gump);
			}

			var hideHair = _Body.IsGhost;
			var hidePants = false;
			var props = String.Empty;

			compiled += Compile(ref props, ref hidePants, ref hideHair);

			if (!_Body.IsGhost && _FacialHairID > 0)
			{
				gump = ArtworkSupport.LookupGump(_FacialHairID, _Body.IsFemale);

				if (gump > 0)
				{
					hue = _SolidHue >= 0 ? _SolidHue : _FacialHairHue;

					if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
					{
						compiled += String.Format(_Format1B, _X, _Y, gump, FixHue(hue));
					}
					else
					{
						compiled += String.Format(_Format1A, _X, _Y, gump);
					}
				}
			}

			if (!hideHair && _HairID > 0)
			{
				gump = ArtworkSupport.LookupGump(_HairID, _Body.IsFemale);

				if (gump > 0)
				{
					hue = _SolidHue >= 0 ? _SolidHue : _HairHue;

					if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
					{
						compiled += String.Format(_Format1B, _X, _Y, gump, FixHue(hue));
					}
					else
					{
						compiled += String.Format(_Format1A, _X, _Y, gump);
					}
				}
			}

			return compiled + props;
		}

		public virtual string Compile(ref string props, ref bool hidePants, ref bool hideHair)
		{
			var compiled = String.Empty;

			if (_Items == null || _Items.Count == 0)
			{
				return compiled;
			}

			_Items.SortLayers();

			var noHue = FixHue(0);
			var noText = Parent.Intern(" ");

			foreach (var item in _Items.TakeWhile(i => i.Layer.IsOrdered())
									   .Where(i => !_Body.IsGhost || i.ItemID == 8270 || i.ItemID == 8271))
			{
				if (item.ItemID == 0x1411 || item.ItemID == 0x141A) // plate legs
				{
					hidePants = true;
				}
				else if (hidePants && item.Layer == Layer.Pants)
				{
					continue;
				}

				if (!hideHair && (item.ItemID == 8270 || item.ItemID == 8271 || item.Layer == Layer.Helm))
				{
					hideHair = true;
				}

				var gump = item.GetGumpID(_Body.IsFemale);

				if (gump <= 0)
				{
					continue;
				}

				var hue = _SolidHue >= 0 ? _SolidHue : item.Hue;

				if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
				{
					compiled += String.Format(_Format1B, _X, _Y, gump, FixHue(hue));
				}
				else
				{
					compiled += String.Format(_Format1A, _X, _Y, gump);
				}

				if (_Properties)
				{
					var tooltip = String.Format(_Format5, item.Serial.Value);

					foreach (var b in item.GetGumpBounds())
					{
						props += String.Format(_Format4, _X + b.X, _Y + b.Y, b.Width, b.Height, noHue, noText);
						props += tooltip;
					}
				}
			}

			return compiled;
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(_Layout0);
			disp.AppendLayout(-1);

			var hue = _BodyHue & 0x7FFF;

			if (_SolidHue >= 0)
			{
				hue = _SolidHue;
			}

			var gump = ArtworkSupport.LookupGump(_Body);

			if (gump <= 0)
			{
				gump = ShrinkTable.Lookup(_Body, 0);

				if (gump > 0)
				{
					if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
					{
						disp.AppendLayout(_Layout4);
					}
					else
					{
						disp.AppendLayout(_Layout3);
					}

					disp.AppendLayout(_X);
					disp.AppendLayout(_Y);
					disp.AppendLayout(gump);

					if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
					{
						disp.AppendLayout(FixHue(hue));
					}
				}

				return;
			}

			disp.AppendLayout(_Layout2);
			disp.AppendLayout(_X);
			disp.AppendLayout(_Y);
			disp.AppendLayout(gump);

			if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
			{
				disp.AppendLayout(_Layout2Hue);
				disp.AppendLayoutNS(FixHue(hue));
			}

			var hideHair = _Body.IsGhost;
			var hidePants = false;
			var props = String.Empty;

			AppendTo(disp, ref props, ref hidePants, ref hideHair);

			if (!_Body.IsGhost && _FacialHairID > 0)
			{
				gump = ArtworkSupport.LookupGump(_FacialHairID, _Body.IsFemale);

				if (gump > 0)
				{
					disp.AppendLayout(_Layout2);
					disp.AppendLayout(_X);
					disp.AppendLayout(_Y);
					disp.AppendLayout(gump);

					hue = _SolidHue >= 0 ? _SolidHue : _FacialHairHue;

					if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
					{
						disp.AppendLayout(_Layout2Hue);
						disp.AppendLayoutNS(FixHue(hue));
					}
				}
			}

			if (!hideHair && _HairID > 0)
			{
				gump = ArtworkSupport.LookupGump(_HairID, _Body.IsFemale);

				if (gump > 0)
				{
					disp.AppendLayout(_Layout2);
					disp.AppendLayout(_X);
					disp.AppendLayout(_Y);
					disp.AppendLayout(gump);

					hue = _SolidHue >= 0 ? _SolidHue : _HairHue;

					if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
					{
						disp.AppendLayout(_Layout2Hue);
						disp.AppendLayoutNS(FixHue(hue));
					}
				}
			}

			if (!_Properties || String.IsNullOrWhiteSpace(props))
			{
				return;
			}

			var noHue = FixHue(0);
			var noText = Parent.Intern(" ");

			props = props.Trim(',');

			foreach (var item in props.Split(',').Select(Int32.Parse).Select(s => _Items.Find(i => i.Serial.Value == s)))
			{
				foreach (var b in item.GetGumpBounds())
				{
					disp.AppendLayout(_Layout5);
					disp.AppendLayout(_X + b.X);
					disp.AppendLayout(_Y + b.Y);
					disp.AppendLayout(b.Width);
					disp.AppendLayout(b.Height);
					disp.AppendLayout(noHue);
					disp.AppendLayout(noText);

					disp.AppendLayout(_Layout6);
					disp.AppendLayout(item.Serial.Value);
				}
			}
		}

		public virtual void AppendTo(IGumpWriter disp, ref string props, ref bool hidePants, ref bool hideHair)
		{
			if (_Items == null || _Items.Count == 0)
			{
				return;
			}

			_Items.SortLayers();

			foreach (var item in _Items.TakeWhile(i => i.Layer.IsOrdered())
									   .Where(i => !_Body.IsGhost || i.ItemID == 8270 || i.ItemID == 8271))
			{
				if (item.ItemID == 0x1411 || item.ItemID == 0x141A) // plate legs
				{
					hidePants = true;
				}
				else if (hidePants && item.Layer == Layer.Pants)
				{
					continue;
				}

				if (!hideHair && (item.ItemID == 8270 || item.ItemID == 8271 || item.Layer == Layer.Helm))
				{
					hideHair = true;
				}

				var gump = ArtworkSupport.LookupGump(item.ItemID, _Body.IsFemale);

				if (gump <= 0)
				{
					continue;
				}

				disp.AppendLayout(_Layout2);
				disp.AppendLayout(_X);
				disp.AppendLayout(_Y);
				disp.AppendLayout(gump);

				var hue = _SolidHue >= 0 ? _SolidHue : item.Hue;

				if (hue > 0 || (_SolidHue >= 0 && hue == _SolidHue))
				{
					disp.AppendLayout(_Layout2Hue);
					disp.AppendLayoutNS(FixHue(hue));
				}

				if (_Properties)
				{
					props += item.Serial.Value + ",";
				}
			}
		}

		public override void Dispose()
		{
			if (_Items != null)
			{
				_Items.Free(true);
				_Items = null;
			}

			base.Dispose();
		}
	}
}