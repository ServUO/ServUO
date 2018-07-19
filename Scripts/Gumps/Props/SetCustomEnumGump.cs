#region References
using System;
using System.Collections;
using System.Reflection;

using Server.Commands;
using Server.Network;
#endregion

namespace Server.Gumps
{
	public class SetCustomEnumGump : SetListOptionGump
	{
		private static readonly Type typeofIDynamicEnum = typeof(IDynamicEnum);

		private readonly string[] _Names;

		public SetCustomEnumGump(
			PropertyInfo prop,
			Mobile mobile,
			object o,
			Stack stack,
			int propspage,
			ArrayList list,
			string[] names)
			: base(prop, mobile, o, stack, propspage, list, names, null)
		{
			_Names = names;
		}

		public override void OnResponse(NetState sender, RelayInfo relayInfo)
		{
			var index = relayInfo.ButtonID - 1;

			if (index >= 0 && index < _Names.Length)
			{
				try
				{
					var info = m_Property.PropertyType.GetMethod("Parse", new[] {typeof(string)});

					var result = "";

					if (info != null)
					{
						result = Properties.SetDirect(
							m_Mobile,
							m_Object,
							m_Object,
							m_Property,
							m_Property.Name,
							info.Invoke(null, new object[] {_Names[index]}),
							true);
					}
					else if (m_Property.PropertyType == typeof(Enum) || m_Property.PropertyType.IsSubclassOf(typeof(Enum)))
					{
						result = Properties.SetDirect(
							m_Mobile,
							m_Object,
							m_Object,
							m_Property,
							m_Property.Name,
							Enum.Parse(m_Property.PropertyType, _Names[index], false),
							true);
					}
					else if (typeofIDynamicEnum.IsAssignableFrom(m_Property.PropertyType))
					{
						var ienum = (IDynamicEnum)m_Property.GetValue(m_Object, null);

						if (ienum != null)
						{
							ienum.Value = _Names[index];
						}

						result = Properties.SetDirect(m_Mobile, m_Object, m_Object, m_Property, m_Property.Name, ienum, true);
					}

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
			}

			m_Mobile.SendGump(new PropertiesGump(m_Mobile, m_Object, m_Stack, m_List, m_Page));
		}
	}
}