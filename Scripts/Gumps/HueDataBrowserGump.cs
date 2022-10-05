using System.Collections.Generic;

using Server.Commands;
using Server.Network;

using static System.Net.Mime.MediaTypeNames;

namespace Server.Gumps
{
	public class HueDataBrowserGump : BaseGridGump
	{
		public static void Configure()
		{
			CommandSystem.Register("HueBrowser", AccessLevel.Decorator, e=> DisplayTo(e.Mobile));
		}

		public static void DisplayTo(Mobile user)
		{
			user.CloseGump<HueDataBrowserGump>();
			user.SendGump(new HueDataBrowserGump(user));
		}

		private const int EntriesPerPage = 20;

		private readonly Mobile m_From;
		private readonly Dictionary<string, int> m_Columns;
		private readonly HueData.Hue[] m_List;
		private readonly int m_Page;
		private readonly int m_Hue;

		public HueDataBrowserGump(Mobile from, int page = 0, int hue = -1)
			: base(30, 30)
		{
			m_From = from;

			m_Page = page;
			m_Hue = hue;

			m_List = HueData.List;

			m_Columns = new Dictionary<string, int>
			{
				["Index"] = 60,
				["Name"] = 150,
				["Colors"] = 32 * EntryHeight
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

			if (m_Page > 0)
			{
				AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 1, ArrowLeftWidth, ArrowLeftHeight);
			}
			else
			{
				AddEntryHeader(20);
			}

			AddEntryHtml(40 + columnsWidth - 20 + ((m_Columns.Count - 2) * OffsetSize), Center($"Page {m_Page + 1} of {(m_List.Length + EntriesPerPage - 1) / EntriesPerPage}"));

			if ((m_Page + 1) * EntriesPerPage < m_List.Length)
			{
				AddEntryButton(20, ArrowRightID1, ArrowRightID2, 2, ArrowRightWidth, ArrowRightHeight);
			}
			else
			{
				AddEntryHeader(20);
			}

			AddNewLine();

			var i = -1;

			foreach (var col in m_Columns)
			{
				AddEntryHtml(col.Value + (++i == 0 ? 40 : 0), col.Key);
			}

			AddEntryHeader(20);

			for (int index = m_Page * EntriesPerPage, line = 0; line < EntriesPerPage && index < m_List.Length; ++index, ++line)
			{
				AddNewLine();

				var hue = m_List[index];

				if (hue != null)
				{
					var j = -1;

					foreach (var col in m_Columns)
					{
						var width = col.Value + (++j == 0 ? 40 : 0);

						if (col.Key == "Colors")
						{
							AddImageTiled(CurrentX, CurrentY, width, EntryHeight, EntryGumpID);

							width /= 32;

							foreach (var color in hue.Colors)
							{
								if (!color.IsEmpty && color.A > 0)
								{
									var hex = color.ToArgb() & 0x00FFFFFF;

									if (hex == 0)
									{
										hex = 0x080808;
									}

									AddHtml(CurrentX, CurrentY, width, EntryHeight, $"<BODYBGCOLOR=#{hex:X6}> ", false, false);
								}

								IncreaseX(width - OffsetSize);
							}
						}
						else
						{
							var prop = typeof(HueData.Hue).GetProperty(col.Key);

							var value = PropertiesGump.ValueToString(hue, prop);

							AddEntryHtml(width, value.Trim('"'));
						}
					}

					var isSelected = hue.Index == m_Hue;

					AddEntryButton(20, isSelected ? 9762 : ArrowRightID1, isSelected ? 9763 : ArrowRightID2, 3 + index, ArrowRightWidth, ArrowRightHeight);
				}
				else
				{
					var j = -1;

					foreach (var col in m_Columns)
					{
						var width = col.Value + (++j == 0 ? 40 : 0);

						if (col.Key == "Colors")
						{
							AddEntryHtml(width, "");
						}
						else
						{
							AddEntryHtml(width, "---");
						}
					}

					AddEntryHeader(20);
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
					break;
				}
				case 1:
				{
					if (m_Page > 0)
					{
						_ = m_From.SendGump(new HueDataBrowserGump(m_From, m_Page - 1, m_Hue));
					}

					break;
				}
				case 2:
				{
					if ((m_Page + 1) * EntriesPerPage < m_List.Length)
					{
						_ = m_From.SendGump(new HueDataBrowserGump(m_From, m_Page + 1, m_Hue));
					}

					break;
				}
				default:
				{
					var v = info.ButtonID - 3;

					if (v < 0 || v >= m_List.Length)
					{
						_ = m_From.SendGump(new HueDataBrowserGump(m_From, m_Page, m_Hue));
					}
					else if (m_List[v] != null)
					{
						_ = m_From.SendGump(new HueDataViewGump(m_From, 0, v, m_Page));
					}
					else
					{
						m_From.SendMessage("That is not accessible.");

						_ = m_From.SendGump(new HueDataBrowserGump(m_From, m_Page, m_Hue));
					}

					break;
				}
			}
		}
	}
}
