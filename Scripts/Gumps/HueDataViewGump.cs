using System.Collections.Generic;

using Server.Network;

namespace Server.Gumps
{
	public class HueDataViewGump : BaseGridGump
	{
		private const int EntriesPerPage = 16;

		private static readonly System.Drawing.Color[] m_InvalidColors = new System.Drawing.Color[32];

		private readonly Mobile m_From;
		private readonly Dictionary<string, int> m_Columns;
		private readonly HueData.Hue m_Hue;
		private readonly System.Drawing.Color[] m_Colors;
		private readonly int m_Page;
		private readonly int m_ParentHue;
		private readonly int m_ParentPage;

		public HueDataViewGump(Mobile from, int page, int hue, int parent = -1)
			: base(30, 30)
		{
			m_From = from;

			m_Page = page;
			m_ParentHue = hue;
			m_ParentPage = parent;

			if (hue >= 0 && hue < HueData.List.Length)
			{
				m_Hue = HueData.List[hue];
			}

			m_Colors = m_Hue?.Colors ?? m_InvalidColors;

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

			if (m_ParentPage >= 0)
			{
				AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 0, ArrowLeftWidth, ArrowLeftHeight);
			}
			else
			{
				AddEntryHeader(20);
			}

			AddEntryHtml(40 + columnsWidth - 20 + ((m_Columns.Count - 2) * OffsetSize), Center($"Hue {m_ParentHue} {m_Hue?.Name}"));
			AddEntryHeader(20);

			AddNewLine();

			if (m_Page > 0)
			{
				AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 1, ArrowLeftWidth, ArrowLeftHeight);
			}
			else
			{
				AddEntryHeader(20);
			}

			AddEntryHtml(40 + columnsWidth - 20 + ((m_Columns.Count - 2) * OffsetSize), Center($"Page {m_Page + 1} of {(m_Colors.Length + EntriesPerPage - 1) / EntriesPerPage}"));

			if ((m_Page + 1) * EntriesPerPage < m_Colors.Length)
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

							AddEntryHtml(width, $"{hex:X6}");
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
						var prop = typeof(System.Drawing.Color).GetProperty(col.Key);

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
						_ = m_From.SendGump(new HueDataBrowserGump(m_From, m_ParentPage, m_ParentHue));
					}

					break;
				}
				case 1:
				{
					if (m_Page > 0)
					{
						_ = m_From.SendGump(new HueDataViewGump(m_From, m_Page - 1, m_ParentHue, m_ParentPage));
					}

					break;
				}
				case 2:
				{
					if ((m_Page + 1) * EntriesPerPage < m_Colors.Length)
					{
						_ = m_From.SendGump(new HueDataViewGump(m_From, m_Page + 1, m_ParentHue, m_ParentPage));
					}

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
