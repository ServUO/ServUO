#region References
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Reflection;

using Server.Commands;
using Server.Network;
#endregion

namespace Server.Gumps
{
	public class SetColorGump : Gump
	{
		private Color m_OldColor;
		private readonly PropertyInfo m_Property;
		private readonly Mobile m_Mobile;
		private readonly object m_Object;
		private readonly Stack m_Stack;
		private readonly int m_Page;
		private readonly ArrayList m_List;

		public static readonly bool OldStyle = PropsConfig.OldStyle;

		public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
		public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;

		public static readonly int TextHue = PropsConfig.TextHue;
		public static readonly int TextOffsetX = PropsConfig.TextOffsetX;

		public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
		public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
		public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
		public static readonly int BackGumpID = PropsConfig.BackGumpID;
		public static readonly int SetGumpID = PropsConfig.SetGumpID;

		public static readonly int SetWidth = PropsConfig.SetWidth;
		public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
		public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
		public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;

		public static readonly int PrevWidth = PropsConfig.PrevWidth;
		public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
		public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
		public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;

		public static readonly int NextWidth = PropsConfig.NextWidth;
		public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
		public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;

		public static readonly int OffsetSize = PropsConfig.OffsetSize;

		public static readonly int EntryHeight = PropsConfig.EntryHeight;
		public static readonly int BorderSize = PropsConfig.BorderSize;

		private static readonly int EntryWidth = 212;

		private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private static readonly int TotalHeight = OffsetSize + (6 * (EntryHeight + OffsetSize));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

		public SetColorGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list)
			: base(GumpOffsetX, GumpOffsetY)
		{
			m_Property = prop;
			m_Mobile = mobile;
			m_Object = o;
			m_Stack = stack;
			m_Page = page;
			m_List = list;

			m_OldColor = (Color)prop.GetValue(o, null);

			AddPage(0);

			AddBackground(0, 0, BackWidth, BackHeight, BackGumpID);
			AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), TotalHeight, OffsetGumpID);

			var name = m_OldColor.IsNamedColor ? m_OldColor.Name : m_OldColor.IsEmpty ? "Empty" : "";

			var rgb = "#" + (m_OldColor.ToArgb() & 0x00FFFFFF).ToString("X6");
			
			var val = String.Format("{0} ({1}) ({2},{3},{4})", name, rgb, m_OldColor.R, m_OldColor.G, m_OldColor.B);

			AddRect(0, prop.Name, 0, -1);
			AddRect(1, val, 0, -1);
			AddRect(2, "Name:", 1, 0);
			AddRect(3, "RGB:", 2, 1);
			AddRect(4, "Hex:", 3, 2);
			AddRect(5, "Empty", 4, -1);
		}

		private void AddRect(int index, string str, int button, int text)
		{
			var x = BorderSize + OffsetSize;
			var y = BorderSize + OffsetSize + (index * (EntryHeight + OffsetSize));

			AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
			AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, str);

			if (text != -1)
			{
				AddTextEntry(x + 40 + TextOffsetX, y, EntryWidth - TextOffsetX - 16, EntryHeight, TextHue, text, "");
			}

			x += EntryWidth + OffsetSize;

			if (SetGumpID != 0)
			{
				AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
			}

			if (button != 0)
			{
				AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, button, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			var toSet = Color.Empty;
			var shouldSet = false;

			var name = "";

			if (info.ButtonID == 1)
			{
				name = info.GetTextEntry(0).Text;
			}

			var rgb = "";

			if (info.ButtonID == 2)
			{
				rgb = info.GetTextEntry(1).Text;
			}

			var hex = "";

			if (info.ButtonID == 3)
			{
				hex = info.GetTextEntry(2).Text;
			}

			switch (info.ButtonID)
			{
				case 1: // Name
				{
					var toapply = name != string.Empty ? name : m_OldColor.IsNamedColor ? m_OldColor.Name : m_OldColor.IsEmpty ? "Empty" : "";

					toSet = Color.FromName(toapply);

					shouldSet = true;
				}
					break;
				case 2: // RGB
				{
					var toapply = rgb != string.Empty ? rgb : String.Format("{0},{1},{2}", m_OldColor.R, m_OldColor.G, m_OldColor.B);

					var args = toapply.Split(',');

					if (args.Length >= 3)
					{
						byte r, g, b;

						if (Byte.TryParse(args[0], out r) && Byte.TryParse(args[1], out g) && Byte.TryParse(args[2], out b))
						{
							toSet = Color.FromArgb(r, g, b);
							shouldSet = true;
						}
					}
				}
					break;
				case 3: // Hex
				{
					var toapply = hex != string.Empty ? hex : String.Format("#{0:X6}", m_OldColor.ToArgb() & 0x00FFFFFF);
					
					int val;

					if (Int32.TryParse(toapply.TrimStart('#'), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out val))
					{
						toSet = Color.FromArgb(val);
						shouldSet = true;
					}
				}
					break;
				case 4: // Empty
				{
					toSet = Color.Empty;
					shouldSet = true;
				}
					break;
			}

			if (shouldSet)
			{
				try
				{
					CommandLogging.LogChangeProperty(m_Mobile, m_Object, m_Property.Name, toSet.ToString());
					m_Property.SetValue(m_Object, toSet, null);
					PropertiesGump.OnValueChanged(m_Object, m_Property, m_Stack);
				}
				catch
				{
					m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
				}
			}

			m_Mobile.SendGump(new PropertiesGump(m_Mobile, m_Object, m_Stack, m_List, m_Page));
		}
	}
}