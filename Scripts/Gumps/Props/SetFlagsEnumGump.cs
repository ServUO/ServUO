using System;
using System.Collections;
using System.Reflection;

using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
	public class SetFlagsEnumGump : Gump
	{
		protected PropertyInfo m_Property;
		protected Mobile m_Mobile;
		protected object m_Object;
		protected Stack m_Stack;
		protected int m_Page;
		protected ArrayList m_List;

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
		private static readonly int EntryCount = 13;

		private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;

		private static readonly bool PrevLabel = OldStyle, NextLabel = OldStyle;

		private static readonly int PrevLabelOffsetX = PrevWidth + 1;
		private static readonly int PrevLabelOffsetY = 0;

		private static readonly int NextLabelOffsetX = -29;
		private static readonly int NextLabelOffsetY = 0;

		protected string[] m_Names;
		protected Enum[] m_Values;
		protected TypeCode m_TypeCode;
		protected bool m_Signed;
		protected Type m_Type;

		private readonly Enum m_Value, m_None, m_All;

		public SetFlagsEnumGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, int propspage, ArrayList list)
			: base(GumpOffsetX, GumpOffsetY)
		{
			m_Property = prop;
			m_Mobile = mobile;
			m_Object = o;
			m_Stack = stack;
			m_Page = propspage;
			m_List = list;

			m_Type = prop.PropertyType;
			m_TypeCode = Type.GetTypeCode(m_Type);
			m_Signed = (int)m_TypeCode % 2 == 1;

			m_Names = Enum.GetNames(m_Type);
			m_Values = PropertiesGump.GetObjects<Enum>(Enum.GetValues(m_Type));

			m_Value = (Enum)prop.GetValue(m_Object);

			if (m_Signed)
			{
				m_None = (Enum)Enum.ToObject(m_Type, 0L);
				m_All = (Enum)Enum.ToObject(m_Type, ~0L);
			}
			else
			{
				m_None = (Enum)Enum.ToObject(m_Type, 0UL);
				m_All = (Enum)Enum.ToObject(m_Type, ~0UL);
			}

			var injectNone = Array.IndexOf(m_Values, m_None) < 0;
			var injectAll = Array.IndexOf(m_Values, m_All) < 0;

			if (injectNone || injectAll)
			{
				var length = m_Values.Length + (injectNone ? 1 : 0) + (injectAll ? 1 : 0);

				var names = new string[length];
				var values = new Enum[length];

				if (injectNone)
				{
					names[0] = "None";
					values[0] = m_None;
				}

				Array.Copy(m_Names, 0, names, injectNone ? 1 : 0, m_Names.Length);
				Array.Copy(m_Values, 0, values, injectNone ? 1 : 0, m_Values.Length);

				if (injectAll)
				{
					names[length - 1] = "All";
					values[length - 1] = m_All;
				}

				m_Names = names;
				m_Values = values;
			}

			var pages = (m_Names.Length + EntryCount - 1) / EntryCount;
			var button = 0;

			for (var page = 1; page <= pages; ++page)
			{
				AddPage(page);

				var start = (page - 1) * EntryCount;
				var count = Math.Min(EntryCount, m_Names.Length - start);

				var totalHeight = OffsetSize + ((count + 2) * (EntryHeight + OffsetSize));
				var backHeight = BorderSize + totalHeight + BorderSize;

				AddBackground(0, 0, BackWidth, backHeight, BackGumpID);
				AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

				var x = BorderSize + OffsetSize;
				var y = BorderSize + OffsetSize;

				var emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

				AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

				if (page > 1)
				{
					AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 0, GumpButtonType.Page, page - 1);

					if (PrevLabel)
					{
						AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
					}
				}

				x += PrevWidth + OffsetSize;

				if (!OldStyle)
				{
					AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, HeaderGumpID);
				}

				x += emptyWidth + OffsetSize;

				if (!OldStyle)
				{
					AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);
				}

				if (page < pages)
				{
					AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 0, GumpButtonType.Page, page + 1);

					if (NextLabel)
					{
						AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
					}
				}

				AddRect(0, prop.Name, null, 0);

				for (var i = 0; i < count; i++)
				{
					AddRect(i + 1, m_Names[start + i], m_Values[start + i], ++button);
				}
			}
		}

		private void AddRect(int index, string name, Enum value, int button)
		{
			var x = BorderSize + OffsetSize;
			var y = BorderSize + OffsetSize + ((index + 1) * (EntryHeight + OffsetSize));

			AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
			AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, name);

			x += EntryWidth + OffsetSize;

			if (SetGumpID != 0)
			{
				AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);
			}

			if (button != 0 && value != null)
			{
				var hasFlag = value == m_None ? m_Value == m_None : m_Value.HasFlag(value);

				AddButton(x + SetOffsetX - 1, y + SetOffsetY - 1, hasFlag ? 211 : 210, hasFlag ? 210 : 211, button, GumpButtonType.Reply, 0);
			}
		}

		private object SetBits(Enum seed, Enum swap)
		{
			if (swap == m_None)
			{
				return m_None;
			}

			if (swap == m_All)
			{
				return m_All;
			}

			object value;

			if (m_Signed)
			{
				value = Enum.ToObject(m_Type, Convert.ToInt64(seed) ^ Convert.ToInt64(swap));
			}
			else
			{
				value = Enum.ToObject(m_Type, Convert.ToUInt64(seed) ^ Convert.ToUInt64(swap));
			}

			return value;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			var index = info.ButtonID - 1;

			if (index >= 0 && index < m_Values.Length)
			{
				try
				{
					var toSet = SetBits(m_Value, m_Values[index]);

					var result = Properties.SetDirect(m_Mobile, m_Object, m_Object, m_Property, m_Property.Name, toSet, true);

					m_Mobile.SendMessage(result);

					if (result == "Property has been set.")
					{
						PropertiesGump.OnValueChanged(m_Object, m_Property, m_Stack);
					}
				}
				catch
				{
					m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
				}

				_ = m_Mobile.SendGump(new SetFlagsEnumGump(m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List));
				return;
			}

			_ = m_Mobile.SendGump(new PropertiesGump(m_Mobile, m_Object, m_Stack, m_List, m_Page));
		}
	}
}
