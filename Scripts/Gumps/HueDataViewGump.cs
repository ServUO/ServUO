using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

using Server.Commands;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Gumps
{
	public class HueDataViewGump : BaseGridGump
	{
		public static void Configure()
		{
			CommandSystem.Register("HueData", AccessLevel.Decorator, e =>
			{
				if (e.Arguments?.Length > 0)
				{
					DisplayTo(e.Mobile, e.GetInt32(0));
				}
				else
				{
					e.Mobile.SendMessage("Select an object to view its hue data...");

					e.Mobile.BeginTarget(-1, true, 0, (from, targeted) =>
					{
						if (targeted is StaticTarget st)
						{
							DisplayTo(e.Mobile, st.Hue);
						}
						else if (targeted is IEntity ent)
						{
							DisplayTo(e.Mobile, ent.Hue);
						}
						else
						{
							from.SendMessage($"Could not get hue for {targeted}");
						}
					});
				}
			});
		}

		public static void DisplayTo(Mobile user, int hue)
		{
			_ = user.SendGump(new HueDataViewGump(user, hue));
		}

		public const int EntriesPerPage = 16;

		public static HueData.Hue[] Hues => HueData.List;

		private static readonly Color[] m_InvalidColors;

		private static readonly int[] m_PreviewItems;

		static HueDataViewGump()
		{
			m_InvalidColors = new Color[32];

			var ver = new ComponentVerification();

			var arr = ver.ItemTable;

			var set = new ConcurrentBag<int>();

			Parallel.For(0, arr.Length, itemID =>
			{
				if (arr[itemID] >= 0)
				{
					set.Add(itemID);
				}
			});

			m_PreviewItems = set.ToArray();
		}

		private readonly Mobile m_From;

		private readonly Dictionary<string, int> m_Columns;

		private readonly HueData.Hue m_Hue;

		private readonly Color[] m_Colors;

		private readonly int m_Page;
		private readonly int m_Pages;

		private readonly int m_ParentHue;
		private readonly int m_ParentPage;

		public HueDataViewGump(Mobile from, int hue)
			: this(from, 0, hue)
		{ }

		public HueDataViewGump(Mobile from, int page, int hue, int parent = -1)
			: base(30, 30)
		{
			m_From = from;

			m_ParentHue = hue;
			m_ParentPage = parent;

			if (hue >= 0 && hue < Hues.Length)
			{
				m_Hue = Hues[hue];
			}

			m_Colors = m_Hue?.Colors ?? m_InvalidColors;

			m_Pages = Math.Max(1, (m_Colors.Length + EntriesPerPage - 1) / EntriesPerPage);
			m_Page = Math.Max(0, Math.Min(m_Pages - 1, page));

			m_Columns = new Dictionary<string, int>
			{
				["Index"] = 60,
				["Color"] = 60,
				["Name"] = 150,
				["Value"] = 80,
				["R"] = 40,
				["G"] = 40,
				["B"] = 40
			};

			Render();
		}

		public void Render()
		{
			var columnsWidth = 0;

			foreach (var cw in m_Columns.Values)
			{
				columnsWidth += cw;
			}

			AddNewPage();

			var hueName = m_Hue?.Name;

			if (!String.IsNullOrWhiteSpace(hueName))
			{
				hueName = hueName.Trim();
				hueName = hueName.Trim('"');

				hueName = hueName.Replace("->", "\u2192");
				hueName = hueName.Replace('<', '\u00AB');
				hueName = hueName.Replace('>', '\u00BB');

				hueName = $"\"{hueName}\"";
			}

			AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 1, ArrowLeftWidth, ArrowLeftHeight);
			AddEntryHtml(40 + columnsWidth - 20 + ((m_Columns.Count - 2) * OffsetSize), Center($"Hue {m_ParentHue} {hueName}"));
			AddEntryButton(20, ArrowRightID1, ArrowRightID2, 2, ArrowRightWidth, ArrowRightHeight);

			AddNewLine();

			if (m_Pages > 1)
			{
				AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 3, ArrowLeftWidth, ArrowLeftHeight);
				AddEntryHtml(40 + columnsWidth - 20 + ((m_Columns.Count - 2) * OffsetSize), Center($"Page {m_Page + 1} of {m_Pages}"));
				AddEntryButton(20, ArrowRightID1, ArrowRightID2, 4, ArrowRightWidth, ArrowRightHeight);

				AddNewLine();
			}

			AddBlankLine();

			var i = -1;

			foreach (var col in m_Columns)
			{
				AddEntryHtml(col.Value + (++i == 0 ? 40 : 0), col.Key);
			}

			AddEntryHeader(20);

			for (int index = m_Page * EntriesPerPage, line = 0; line < EntriesPerPage && index < m_Colors.Length; ++index, ++line)
			{
				AddNewLine();

				var color = m_Colors[index];

				var j = -1;

				foreach (var col in m_Columns)
				{
					var width = col.Value + (++j == 0 ? 40 : 0);

					if (col.Key == "Index")
					{
						var value = PropertiesGump.ValueToString(index);

						AddEntryHtml(width, value.Trim('"'));
					}
					else if (col.Key == "Color")
					{
						AddImageTiled(CurrentX, CurrentY, width, EntryHeight, EntryGumpID);

						if (!color.IsEmpty && color.A > 0)
						{
							var hex = color.ToArgb() & 0x00FFFFFF;

							if (hex == 0)
							{
								hex = 0x080808;
							}

							AddHtml(CurrentX, CurrentY, width, EntryHeight, $"<BODYBGCOLOR=#{hex:X6}> ", false, false);
						}

						IncreaseX(width);
					}
					else if (col.Key == "Value")
					{
						if (!color.IsEmpty && color.A > 0)
						{
							var hex = color.ToArgb() & 0x00FFFFFF;

							AddEntryHtml(width, $"#{hex:X6}");
						}
						else
						{
							AddEntryHtml(width, "---");
						}
					}
					else if (col.Key == "Name" && !color.IsNamedColor)
					{
						AddEntryHtml(width, "---");
					}
					else if (!color.IsEmpty && color.A > 0)
					{
						var prop = typeof(Color).GetProperty(col.Key);

						var value = PropertiesGump.ValueToString(color, prop);

						AddEntryHtml(width, value.Trim('"'));
					}
					else
					{
						AddEntryHtml(width, "---");
					}
				}

				AddEntryHeader(20);
			}

			if (m_PreviewItems.Length > 0)
			{
				AddNewLine();
				AddBlankLine();

				var footerWidth = m_Offset.Width - OffsetSize;
				var footerHeight = (EntryHeight * 8) + (OffsetSize * 7);

				AddImageTiled(CurrentX, CurrentY, footerWidth, footerHeight, HeaderGumpID);

				for (var r = 0; r < 5; r++)
				{
					AddNewLine();
				}

				var n = 0;
				var c = 0;

				while (c < footerWidth)
				{
					var itemID = Utility.RandomList(m_PreviewItems);
					var itemArt = ArtData.GetStatic(itemID);

					var itemWidth = itemArt.Width;
					var itemHeight = itemArt.Height;

					if (itemHeight > footerHeight - 40)
					{
						continue;
					}

					if (c + itemWidth > footerWidth)
					{
						var remain = (footerWidth - c) / 2;

						var l = Entries.Count;

						while (--l >= 0)
						{
							if (Entries[l] is GumpItem gi)
							{
								gi.X += remain;

								if (--n <= 0)
								{
									break;
								}
							}
						}

						break;
					}

					var offset = 0;

					if (itemHeight > 44)
					{
						offset -= itemHeight - 44;
					}
					else if (itemHeight < 44)
					{
						offset += 44 - itemHeight;
					}

					AddItem(CurrentX, CurrentY + offset, itemID, m_ParentHue);

					IncreaseX(itemWidth + 6 - OffsetSize);

					c += itemWidth + 6;

					++n;
				}

				for (var r = 0; r < 2; r++)
				{
					AddNewLine();
				}
			}

			FinishPage();
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			switch (info.ButtonID)
			{
				case 0:
				{
					if (m_ParentPage >= 0)
					{
						_ = m_From.CloseGump<HueDataBrowserGump>();
						_ = m_From.SendGump(new HueDataBrowserGump(m_From, m_ParentPage, m_ParentHue));
					}

					break;
				}
				case 1:
				{
					var parentHue = m_ParentHue;

					if (--parentHue < 0)
					{
						parentHue = Hues.Length - 1;
					}

					var parentPage = m_ParentPage;

					if (parentPage >= 0)
					{
						parentPage = parentHue / HueDataBrowserGump.EntriesPerPage;
					}

					_ = m_From.SendGump(new HueDataViewGump(m_From, m_Page, parentHue, parentPage));

					break;
				}
				case 2:
				{
					var parentHue = m_ParentHue;

					if (++parentHue >= Hues.Length)
					{
						parentHue = 0;
					}

					var parentPage = m_ParentPage;

					if (parentPage >= 0)
					{
						parentPage = parentHue / HueDataBrowserGump.EntriesPerPage;
					}

					_ = m_From.SendGump(new HueDataViewGump(m_From, m_Page, parentHue, parentPage));

					break;
				}
				case 3:
				{
					var page = m_Page;

					if (--page < 0)
					{
						page = m_Pages - 1;
					}

					_ = m_From.SendGump(new HueDataViewGump(m_From, page, m_ParentHue, m_ParentPage));

					break;
				}
				case 4:
				{
					var page = m_Page;

					if (++page >= m_Pages)
					{
						page = 0;
					}

					_ = m_From.SendGump(new HueDataViewGump(m_From, page, m_ParentHue, m_ParentPage));

					break;
				}
				default:
				{
					_ = m_From.SendGump(new HueDataViewGump(m_From, m_Page, m_ParentHue, m_ParentPage));

					break;
				}
			}
		}
	}
}
