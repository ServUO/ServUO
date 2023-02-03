using System;
using System.Reflection;
using System.Collections;
using Server;
using Server.Network;
using Server.Commands;
using System.CodeDom;
using System.Runtime.CompilerServices;

namespace Server.Gumps
{
	public class SetFlagListOptionGump : Gump
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

		private static bool PrevLabel = OldStyle, NextLabel = OldStyle;

		private static readonly int PrevLabelOffsetX = PrevWidth + 1;
		private static readonly int PrevLabelOffsetY = 0;

		private static readonly int NextLabelOffsetX = -29;
		private static readonly int NextLabelOffsetY = 0;

		protected string[] m_Names;
		protected object[] m_Values;
		protected TypeCode m_TypeCode;
		protected bool isSigned;

		public SetFlagListOptionGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, int propspage, ArrayList list, string[] names, object[] values) : base(GumpOffsetX, GumpOffsetY)
		{
			m_Property = prop;
			m_Mobile = mobile;
			m_Object = o;
			m_Stack = stack;
			m_Page = propspage;
			m_List = list;

			m_Names = names;
			m_Values = values;

			Enum propEnum = (Enum)prop.GetValue(m_Object);
			m_TypeCode = Type.GetTypeCode(prop.PropertyType);
			isSigned = (int)m_TypeCode % 2 == 1;

			bool injectedNone = false;
			bool injectedAll = false;
			long totalValue = 0;
			ulong totalValueU = 0;

			if (isSigned)
			{
				injectedNone = Convert.ToInt64(m_Values[0]) != 0;
				injectedAll = Convert.ToInt64(m_Values[m_Values.Length - 1]) != long.MaxValue;
			}
			else
			{
				injectedNone = Convert.ToUInt64(m_Values[0]) != 0;
				injectedAll = Convert.ToUInt64(m_Values[m_Values.Length - 1]) != ulong.MaxValue;
			}

			if (injectedNone || injectedAll)
			{
				int length = m_Values.Length + (injectedNone ? 1 : 0) + (injectedAll ? 1 : 0);
				string[] namesInjected = new string[length];
				object[] valuesInjected = new object[length];

				if (injectedNone)
				{
					namesInjected[0] = "None";
					valuesInjected[0] = 0;
				}

				Array.Copy(m_Names, 0, namesInjected, (injectedNone ? 1 : 0), m_Names.Length);
				Array.Copy(m_Values, 0, valuesInjected, (injectedNone ? 1 : 0), m_Values.Length);

				if (injectedAll)
				{
					namesInjected[length - 1] = "All";
					if (isSigned)
						valuesInjected[length - 1] = long.MaxValue;
					else
						valuesInjected[length - 1] = ulong.MaxValue;

					m_Names = namesInjected;
					m_Values = valuesInjected;
				}
			}

			if (isSigned)
				totalValue = Convert.ToInt64(propEnum);
			else
				totalValueU = Convert.ToUInt64(propEnum);

			int pages = (m_Names.Length + EntryCount - 1) / EntryCount;
			int index = 0;

			for (int page = 1; page <= pages; ++page)
			{
				AddPage(page);

				int start = (page - 1) * EntryCount;
				int count = m_Names.Length - start;

				if (count > EntryCount)
					count = EntryCount;

				int totalHeight = OffsetSize + ((count + 2) * (EntryHeight + OffsetSize));
				int backHeight = BorderSize + totalHeight + BorderSize;

				AddBackground(0, 0, BackWidth, backHeight, BackGumpID);
				AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);



				int x = BorderSize + OffsetSize;
				int y = BorderSize + OffsetSize;

				int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

				AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

				if (page > 1)
				{
					AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 0, GumpButtonType.Page, page - 1);

					if (PrevLabel)
						AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
				}

				x += PrevWidth + OffsetSize;

				if (!OldStyle)
					AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, HeaderGumpID);

				x += emptyWidth + OffsetSize;

				if (!OldStyle)
					AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);

				if (page < pages)
				{
					AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 0, GumpButtonType.Page, page + 1);

					if (NextLabel)
						AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
				}

				AddRect(0, prop.Name, 0);

				for (int i = 0; i < count; ++i)
				{
					bool f = false;
					if (isSigned)
					{
						long longvalue = Convert.ToInt64(m_Values[i + start]);

						if (longvalue != 0 && longvalue < long.MaxValue)
							f = propEnum.HasFlag(Enum.Parse(prop.PropertyType, m_Names[i + start]) as Enum);
						else
							f = longvalue == 0 && totalValue == 0 || longvalue > 0 && IsMaxValue(totalValue);
					}
					else
					{
						ulong longvalue = Convert.ToUInt64(m_Values[i + start]);

						if (longvalue != 0 && longvalue < ulong.MaxValue)
							f = propEnum.HasFlag(Enum.Parse(prop.PropertyType, m_Names[i + start]) as Enum);
						else
							f = longvalue == 0 && totalValueU == 0 || longvalue > 0 && IsMaxValue(totalValueU);
					}
					AddRect(i + 1, m_Names[index], ++index, f);
				}
			}
		}


		private void AddRect(int index, string str, int button, bool hasFlag = false)
		{
			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize + ((index + 1) * (EntryHeight + OffsetSize));

			AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
			AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, TextHue, str);

			x += EntryWidth + OffsetSize;

			if (SetGumpID != 0)
				AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

			if (button != 0)
			{
				AddButton(x + SetOffsetX - 1, y + SetOffsetY - 1, hasFlag ? 211 : 210, hasFlag ? 210 : 211, button, GumpButtonType.Reply, 0);
			}
		}

		private bool IsMaxValue(long value)
		{
			switch (m_TypeCode)
			{
				case TypeCode.SByte: return value == sbyte.MaxValue;
				case TypeCode.Int16: return value == short.MaxValue;
				case TypeCode.Int32: return value == int.MaxValue;
				case TypeCode.Int64: return value == long.MaxValue;
			}
			return false;
		}
		private bool IsMaxValue(ulong value)
		{
			switch (m_TypeCode)
			{
				case TypeCode.Byte: return value == byte.MaxValue;
				case TypeCode.UInt16: return value == ushort.MaxValue;
				case TypeCode.UInt32: return value == uint.MaxValue;
				case TypeCode.UInt64: return value == ulong.MaxValue;
			}
			return false;
		}

		private object SetBits(object a, object b)
		{
			if (isSigned)
			{
				long blong = Convert.ToInt64(b);
				if (blong == 0) return Convert.ChangeType(blong, m_TypeCode);
				if (blong == long.MaxValue)
					switch (m_TypeCode)
					{
						case TypeCode.SByte: return Convert.ChangeType(sbyte.MaxValue, m_TypeCode);
						case TypeCode.Int16: return Convert.ChangeType(short.MaxValue, m_TypeCode);
						case TypeCode.Int32: return Convert.ChangeType(int.MaxValue, m_TypeCode);
						case TypeCode.Int64: return Convert.ChangeType(blong, m_TypeCode);
					}

				return Convert.ChangeType(Convert.ToInt64(a) ^ blong, m_TypeCode);
			}

			ulong bulong = Convert.ToUInt64(b);
			if (bulong == 0) return Convert.ChangeType(bulong, m_TypeCode);
			if (bulong == ulong.MaxValue)
				switch (m_TypeCode)
				{
					case TypeCode.Byte: return Convert.ChangeType(byte.MaxValue, m_TypeCode);
					case TypeCode.UInt16: return Convert.ChangeType(ushort.MaxValue, m_TypeCode);
					case TypeCode.UInt32: return Convert.ChangeType(uint.MaxValue, m_TypeCode);
					case TypeCode.UInt64: return Convert.ChangeType(bulong, m_TypeCode);
				}

			return Convert.ChangeType(Convert.ToUInt64(a) ^ bulong, m_TypeCode);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			int index = info.ButtonID - 1;

			if (index >= 0 && index < m_Values.Length)
			{
				try
				{
					object toSet = SetBits(m_Property.GetValue(m_Object), m_Values[index]);

					string result = Properties.SetDirect(m_Mobile, m_Object, m_Object, m_Property, m_Property.Name, toSet, true);

					m_Mobile.SendMessage(result);

					if (result == "Property has been set.")
						PropertiesGump.OnValueChanged(m_Object, m_Property, m_Stack);
				}
				catch
				{
					m_Mobile.SendMessage("An exception was caught. The property may not have changed.");
				}
				m_Mobile.SendGump(new SetFlagListOptionGump(m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List, m_Names, m_Values));
				return;
			}

			m_Mobile.SendGump(new PropertiesGump(m_Mobile, m_Object, m_Stack, m_List, m_Page));
		}
	}
}