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

#if ServUO58
#define ServUOX
#endif

#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Server;
using Server.Network;

using VitaNex.Collections;
using VitaNex.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		private Dictionary<string, Spoof> _TextTooltips;

#if ServUO
		public new void AddTooltip(string text)
#else
		public void AddTooltip(string text)
#endif
		{
			AddTooltip(text, Color.Empty);
		}

		public void AddTooltip(string text, Color color)
		{
			AddTooltip(String.Empty, text, Color.Empty, color);
		}

		public void AddTooltip(string title, string text)
		{
			AddTooltip(title, text, Color.Empty, Color.Empty);
		}

		public void AddTooltip(string title, string text, Color titleColor, Color textColor)
		{
			title = title ?? String.Empty;
			text = text ?? String.Empty;

			if (titleColor.IsEmpty || titleColor == Color.Transparent)
			{
				titleColor = Color.Yellow;
			}

			if (textColor.IsEmpty || textColor == Color.Transparent)
			{
				textColor = Color.White;
			}

			if (!String.IsNullOrWhiteSpace(title))
			{
				text = String.Concat(title.WrapUOHtmlColor(titleColor, false), '\n', text.WrapUOHtmlColor(textColor, false));
			}
			else
			{
				text = text.WrapUOHtmlColor(textColor, false);
			}

			if (!_TextTooltips.TryGetValue(text, out var spoof) || spoof == null || spoof.Deleted)
			{
				_TextTooltips[text] = spoof = Spoof.Acquire();
			}

			spoof.Text = text;

			AddProperties(spoof);
		}

		private sealed class Spoof : Entity
		{
			private static readonly char[] _Split = { '\n' };

			private static int _UID = Int32.MinValue;

			private static Serial NewUID
			{
				get
				{
					if (_UID >= -1)
					{
						_UID = Int32.MinValue;
					}

#if ServUOX
					return new Serial(++_UID);
#else
					return ++_UID;
#endif
				}
			}

			private static readonly ObjectPool<Spoof> _Pool = new ObjectPool<Spoof>();

			public static Spoof Acquire()
			{
				return _Pool.Acquire();
			}

			public static void Free(Spoof spoof)
			{
				_Pool.Free(spoof);
			}

			public static void Free(ref Spoof spoof)
			{
				_Pool.Free(ref spoof);
			}

			public int UID { get => Serial.Value; private set => this.SetPropertyValue("Serial", value); }

			private OPLInfo _OPLInfo;

			public Packet OPLPacket
			{
				get
				{
					if (_OPLInfo == null)
					{
						_OPLInfo = new OPLInfo(PropertyList);
						_OPLInfo.SetStatic();
					}

					return _OPLInfo;
				}
			}

			private ObjectPropertyList _PropertyList;

			public ObjectPropertyList PropertyList
			{
				get
				{
					if (_PropertyList == null)
					{
						_PropertyList = new ObjectPropertyList(this);

						if (Text != null)
						{
							var text = Text.StripHtmlBreaks(true);

							if (text.IndexOf('\n') >= 0)
							{
								var lines = text.Split(_Split, StringSplitOptions.RemoveEmptyEntries);

								_PropertyList.Add(lines[0]);

								ExtendedOPL.AddTo(_PropertyList, lines.Skip(1));
							}
							else
							{
								_PropertyList.Add(text);
							}
						}

						_PropertyList.Terminate();
						_PropertyList.SetStatic();
					}

					return _PropertyList;
				}
			}

			private string _Text = String.Empty;

			public string Text
			{
				get => _Text ?? String.Empty;
				set
				{
					if (_Text != value)
					{
						_Text = value;

						Packet.Release(ref _OPLInfo);
						Packet.Release(ref _PropertyList);
					}
				}
			}

			public Spoof()
				: base(NewUID, Point3D.Zero, Map.Internal)
			{ }
		}
	}
}
