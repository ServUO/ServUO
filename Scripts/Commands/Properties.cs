#region Header
// **********
// ServUO - Properties.cs
// **********
#endregion

#region References
using System;
using System.Reflection;

using CustomsFramework;

using Server.Commands;
using Server.Commands.Generic;
using Server.Gumps;
using Server.Targeting;

using CPA = Server.CommandPropertyAttribute;
#endregion

namespace Server.Commands
{
	[Flags]
	public enum PropertyAccess
	{
		Read = 0x01,
		Write = 0x02,
		ReadWrite = Read | Write
	}

	public class Properties
	{
		private static readonly Type _TypeOfCPA = typeof(CPA);
		private static readonly Type _TypeOfSerial = typeof(Serial);
		private static readonly Type _TypeOfCustomSerial = typeof(CustomSerial);
		private static readonly Type _TypeOfType = typeof(Type);
		private static readonly Type _TypeOfChar = typeof(Char);
		private static readonly Type _TypeOfString = typeof(String);
		private static readonly Type _TypeOfIDynamicEnum = typeof(IDynamicEnum);
		private static readonly Type _TypeOfText = typeof(TextDefinition);
		private static readonly Type _TypeOfTimeSpan = typeof(TimeSpan);
		private static readonly Type _TypeOfParsable = typeof(ParsableAttribute);

		private static readonly Type[] _ParseTypes = new[] {typeof(string)};
		private static readonly object[] _ParseParams = new object[1];

		private static readonly Type[] _NumericTypes = new[]
		{
			typeof(Byte), typeof(SByte), typeof(Int16), typeof(UInt16), typeof(Int32), typeof(UInt32), typeof(Int64),
			typeof(UInt64)
		};

		public static void Initialize()
		{
			CommandSystem.Register("Props", AccessLevel.Counselor, Props_OnCommand);
		}

		public static CPA GetCPA(PropertyInfo p)
		{
			var attrs = p.GetCustomAttributes(_TypeOfCPA, false);

			return attrs.Length == 0 ? null : attrs[0] as CPA;
		}

		public static PropertyInfo[] GetPropertyInfoChain(
			Mobile m, Type type, string propertyString, PropertyAccess endAccess, ref string failReason)
		{
			var split = propertyString.Split('.');

			if (split.Length == 0)
			{
				return null;
			}

			var info = new PropertyInfo[split.Length];

			for (int i = 0; i < info.Length; ++i)
			{
				string propertyName = split[i];

				if (CIEqual(propertyName, "current"))
				{
					continue;
				}

				var props = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

				bool isFinal = i == info.Length - 1;

				PropertyAccess access = endAccess;

				if (!isFinal)
				{
					access |= PropertyAccess.Read;
				}

				foreach (PropertyInfo p in props)
				{
					if (!CIEqual(p.Name, propertyName))
					{
						continue;
					}

					CPA attr = GetCPA(p);

					if (attr == null)
					{
						failReason = String.Format("Property '{0}' not found.", propertyName);
						return null;
					}

					if ((access & PropertyAccess.Read) != 0 && m.AccessLevel < attr.ReadLevel)
					{
						failReason = String.Format(
							"You must be at least {0} to get the property '{1}'.", Mobile.GetAccessLevelName(attr.ReadLevel), propertyName);

						return null;
					}

					if ((access & PropertyAccess.Write) != 0 && m.AccessLevel < attr.WriteLevel)
					{
						failReason = String.Format(
							"You must be at least {0} to set the property '{1}'.", Mobile.GetAccessLevelName(attr.WriteLevel), propertyName);

						return null;
					}

					if ((access & PropertyAccess.Read) != 0 && !p.CanRead)
					{
						failReason = String.Format("Property '{0}' is write only.", propertyName);
						return null;
					}

					if ((access & PropertyAccess.Write) != 0 && (!p.CanWrite || attr.ReadOnly) && isFinal)
					{
						failReason = String.Format("Property '{0}' is read only.", propertyName);
						return null;
					}

					info[i] = p;
					type = p.PropertyType;
					break;
				}

				if (info[i] != null)
				{
					continue;
				}

				failReason = String.Format("Property '{0}' not found.", propertyName);
				return null;
			}

			return info;
		}

		public static PropertyInfo GetPropertyInfo(
			Mobile m, ref object obj, string propertyName, PropertyAccess access, ref string failReason)
		{
			var chain = GetPropertyInfoChain(m, obj.GetType(), propertyName, access, ref failReason);

			return chain == null ? null : GetPropertyInfo(ref obj, chain, ref failReason);
		}

		public static PropertyInfo GetPropertyInfo(ref object obj, PropertyInfo[] chain, ref string failReason)
		{
			if (chain == null || chain.Length == 0)
			{
				failReason = "Property chain is empty.";
				return null;
			}

			for (int i = 0; i < chain.Length - 1; ++i)
			{
				if (chain[i] == null)
				{
					continue;
				}

				obj = chain[i].GetValue(obj, null);

				if (obj != null)
				{
					continue;
				}

				failReason = String.Format("Property '{0}' is null.", chain[i]);
				return null;
			}

			return chain[chain.Length - 1];
		}

		public static string GetValue(Mobile from, object o, string name)
		{
			string failReason = "";

			var chain = GetPropertyInfoChain(from, o.GetType(), name, PropertyAccess.Read, ref failReason);

			if (chain == null || chain.Length == 0)
			{
				return failReason;
			}

			PropertyInfo p = GetPropertyInfo(ref o, chain, ref failReason);

			return p == null ? failReason : InternalGetValue(o, p, chain);
		}

		public static string IncreaseValue(Mobile m, object o, string[] args)
		{
			int len = args.Length / 2;

			var realObjs = new object[len];
			var realProps = new PropertyInfo[len];
			var realValues = new int[len];

			bool positive = false, negative = false;

			for (int i = 0; i < realProps.Length; ++i)
			{
				int idx = i * 2;
				string name = args[idx];

				try
				{
					string valueString = args[1 + idx];

					if (valueString.StartsWith("0x"))
					{
						realValues[i] = Convert.ToInt32(valueString.Substring(2), 16);
					}
					else
					{
						realValues[i] = Convert.ToInt32(valueString);
					}
				}
				catch
				{
					return "Offset value could not be parsed.";
				}

				if (realValues[i] > 0)
				{
					positive = true;
				}
				else if (realValues[i] < 0)
				{
					negative = true;
				}
				else
				{
					return "Zero is not a valid value to offset.";
				}

				string failReason = null;
				
				realObjs[i] = o;
				realProps[i] = GetPropertyInfo(m, ref realObjs[i], name, PropertyAccess.ReadWrite, ref failReason);

				if (failReason != null)
				{
					return failReason;
				}

				if (realProps[i] == null)
				{
					return "Property not found.";
				}
			}

			for (int i = 0; i < realProps.Length; ++i)
			{
				object obj = realProps[i].GetValue(realObjs[i], null);

				if (!(obj is IConvertible))
				{
					return "Property is not IConvertable.";
				}

				try
				{
					long v = Convert.ToInt64(obj) + realValues[i];
					object toSet = Convert.ChangeType(v, realProps[i].PropertyType);
					realProps[i].SetValue(realObjs[i], toSet, null);

					EventSink.InvokeOnPropertyChanged(new OnPropertyChangedEventArgs(m, realObjs[i], realProps[i], obj, toSet));
				}
				catch
				{
					return "Value could not be converted";
				}
			}

			if (realProps.Length == 1)
			{
				return String.Format("The property has been {0}.", positive ? "increased." : "decreased");
			}

			if (positive && negative)
			{
				return "The properties have been changed.";
			}

			return String.Format("The properties have been {0}.", positive ? "increased." : "decreased");
		}

		public static string SetValue(Mobile m, object o, string name, string value)
		{
			object logObject = o;
			string failReason = "";
			PropertyInfo p = GetPropertyInfo(m, ref o, name, PropertyAccess.Write, ref failReason);

			return p == null ? failReason : InternalSetValue(m, logObject, o, p, name, value, true);
		}

		public static string ConstructFromString(Type type, object obj, string value, ref object constructed)
		{
			object toSet;
			bool isSerial = IsSerial(type);
			bool isCustomSerial = IsCustomSerial(type);

			if (isSerial || isCustomSerial) // mutate into int32
			{
				type = _NumericTypes[4];
			}

			if (value == "(-null-)" && !type.IsValueType)
			{
				value = null;
			}

			if (IsEnum(type))
			{
				try
				{
					toSet = Enum.Parse(type, value ?? String.Empty, true);
				}
				catch
				{
					return "That is not a valid enumeration member.";
				}
			}
			else if (IsType(type))
			{
				try
				{
					toSet = ScriptCompiler.FindTypeByName(value);

					if (toSet == null)
					{
						return "No type with that name was found.";
					}
				}
				catch
				{
					return "No type with that name was found.";
				}
			}
			else if (IsParsable(type))
			{
				try
				{
					toSet = Parse(obj, type, value);
				}
				catch
				{
					return "That is not properly formatted.";
				}
			}
			else if (value == null)
			{
				toSet = null;
			}
			else if (value.StartsWith("0x") && IsNumeric(type))
			{
				try
				{
					toSet = Convert.ChangeType(Convert.ToUInt64(value.Substring(2), 16), type);
				}
				catch
				{
					return "That is not properly formatted.";
				}
			}
			else
			{
				try
				{
					toSet = Convert.ChangeType(value, type);
				}
				catch
				{
					return "That is not properly formatted.";
				}
			}

			if (isSerial) // mutate back
			{
				toSet = (Serial)Convert.ToInt32(toSet);
			}
			else if (isCustomSerial)
			{
				toSet = (CustomSerial)Convert.ToInt32(toSet);
			}

			constructed = toSet;
			return null;
		}

		public static string SetDirect(
			Mobile m, object logObject, object obj, PropertyInfo prop, string givenName, object toSet, bool shouldLog)
		{
			try
			{
				if (toSet is AccessLevel)
				{
					AccessLevel newLevel = (AccessLevel)toSet;
					AccessLevel reqLevel = AccessLevel.Administrator;

					if (newLevel == AccessLevel.Administrator)
					{
						reqLevel = AccessLevel.Developer;
					}
					else if (newLevel >= AccessLevel.Developer)
					{
						reqLevel = AccessLevel.Owner;
					}

					if (m.AccessLevel < reqLevel)
					{
						return "You do not have access to that level.";
					}
				}

				if (shouldLog)
				{
					CommandLogging.LogChangeProperty(m, logObject, givenName, toSet == null ? "(-null-)" : toSet.ToString());
				}

				object oldValue = prop.GetValue(obj, null);
				prop.SetValue(obj, toSet, null);

				EventSink.InvokeOnPropertyChanged(new OnPropertyChangedEventArgs(m, obj, prop, oldValue, toSet));
				
				return "Property has been set.";
			}
			catch(Exception e)
			{
                Console.WriteLine(e.ToString());
				return "An exception was caught, the property may not be set.";
			}
		}

		public static string SetDirect(object obj, PropertyInfo prop, object toSet)
		{
			try
			{
				if (toSet is AccessLevel)
				{
					return "You do not have access to that level.";
				}

				object oldValue = prop.GetValue(obj, null);
				prop.SetValue(obj, toSet, null);

				EventSink.InvokeOnPropertyChanged(new OnPropertyChangedEventArgs(null, obj, prop, oldValue, toSet));

				return "Property has been set.";
			}
            catch (Exception e)
			{
                Console.WriteLine(e.ToString());
				return "An exception was caught, the property may not be set.";
			}
		}

		public static string InternalSetValue(
			Mobile m, object logobj, object o, PropertyInfo p, string pname, string value, bool shouldLog)
		{
			IDynamicEnum toSet;
			object toSetObj = null;
			string result = null;

			if (IsIDynamicEnum(p.PropertyType))
			{
				toSetObj = toSet = p.GetValue(o, null) as IDynamicEnum;

				if (toSet != null)
				{
					toSet.Value = value;

					if (!toSet.IsValid)
					{
						result = "No type with that name was found.";
					}
				}
				else
				{
					result = "That is not properly formatted.";
				}
			}
			else
			{
				result = ConstructFromString(p.PropertyType, o, value, ref toSetObj);
			}

			return result ?? SetDirect(m, logobj, o, p, pname, toSetObj, shouldLog);
		}

		public static string InternalSetValue(object o, PropertyInfo p, string value)
		{
			IDynamicEnum toSet;
			object toSetObj = null;
			string result = null;

			if (IsIDynamicEnum(p.PropertyType))
			{
				toSetObj = toSet = p.GetValue(o, null) as IDynamicEnum;

				if (toSet != null)
				{
					toSet.Value = value;

					if (!toSet.IsValid)
					{
						result = "No type with that name was found.";
					}
				}
				else
				{
					result = "That is not properly formatted.";
				}
			}
			else
			{
				result = ConstructFromString(p.PropertyType, o, value, ref toSetObj);
			}

			return result ?? SetDirect(o, p, toSetObj);
		}

		[Usage("Props [serial]")]
		[Description("Opens a menu where you can view and edit all properties of a targeted (or specified) object.")]
		private static void Props_OnCommand(CommandEventArgs e)
		{
			if (e.Length == 1)
			{
				IEntity ent = World.FindEntity(e.GetInt32(0));

				if (ent == null)
				{
					e.Mobile.SendMessage("No object with that serial was found.");
				}
				else if (!BaseCommand.IsAccessible(e.Mobile, ent))
				{
					e.Mobile.SendMessage("That is not accessible.");
				}
				else
				{
					e.Mobile.SendGump(new PropertiesGump(e.Mobile, ent));
				}
			}
			else
			{
				e.Mobile.Target = new PropsTarget();
			}
		}

		private static bool CIEqual(string l, string r)
		{
			return Insensitive.Equals(l, r);
		}

		private static string InternalGetValue(object o, PropertyInfo p, PropertyInfo[] chain = null)
		{
			Type type = p.PropertyType;
			object value = p.GetValue(o, null);
			string toString;

			if (value == null)
			{
				toString = "null";
			}
			else if (IsNumeric(type))
			{
				toString = String.Format("{0} (0x{0:X})", value);
			}
			else if (IsChar(type))
			{
				toString = String.Format("'{0}' ({1} [0x{1:X}])", value, (int)value);
			}
			else if (IsString(type))
			{
				toString = (string)value == "null" ? @"@""null""" : String.Format("\"{0}\"", value);
			}
			else if (IsIDynamicEnum(type))
			{
				toString = ((IDynamicEnum)value).Value == "null"
							   ? @"@""null"""
							   : String.Format("\"{0}\"", ((IDynamicEnum)value).Value);
			}
			else if (IsText(type))
			{
				toString = ((TextDefinition)value).Format(false);
			}
			else
			{
				toString = value.ToString();
			}

			if (chain == null)
			{
				return String.Format("{0} = {1}", p.Name, toString);
			}

			var concat = new string[chain.Length * 2 + 1];

			for (int i = 0; i < chain.Length; ++i)
			{
				int idx = i * 2;

				concat[idx] = chain[i].Name;
				concat[idx + 1] = i < chain.Length - 1 ? "." : " = ";
			}

			concat[concat.Length - 1] = toString;

			return String.Concat(concat);
		}

		private static bool IsSerial(Type t)
		{
			return t == _TypeOfSerial;
		}

		private static bool IsCustomSerial(Type t)
		{
			return t == _TypeOfCustomSerial;
		}

		private static bool IsType(Type t)
		{
			return t == _TypeOfType;
		}

		private static bool IsChar(Type t)
		{
			return t == _TypeOfChar;
		}

		private static bool IsString(Type t)
		{
			return t == _TypeOfString;
		}

		private static bool IsIDynamicEnum(Type t)
		{
			return _TypeOfIDynamicEnum.IsAssignableFrom(t);
		}

		private static bool IsText(Type t)
		{
			return t == _TypeOfText;
		}

		private static bool IsEnum(Type t)
		{
			return t.IsEnum;
		}

		private static bool IsParsable(Type t)
		{
			return t == _TypeOfTimeSpan || t.IsDefined(_TypeOfParsable, false);
		}

		private static object Parse(object o, Type t, string value)
		{
			MethodInfo method = t.GetMethod("Parse", _ParseTypes);

			_ParseParams[0] = value;

			return method.Invoke(o, _ParseParams);
		}

		private static bool IsNumeric(Type t)
		{
			return Array.IndexOf(_NumericTypes, t) >= 0;
		}

		private class PropsTarget : Target
		{
			public PropsTarget()
				: base(-1, true, TargetFlags.None)
			{ }

			protected override void OnTarget(Mobile from, object o)
			{
				if (!BaseCommand.IsAccessible(from, o))
				{
					from.SendMessage("That is not accessible.");
				}
				else
				{
					from.SendGump(new PropertiesGump(from, o));
				}
			}
		}
	}
}

namespace Server
{
	public abstract class PropertyException : ApplicationException
	{
		public Property Property { get; protected set; }

		public PropertyException(Property property, string message)
			: base(message)
		{
			Property = property;
		}
	}

	public abstract class BindingException : PropertyException
	{
		public BindingException(Property property, string message)
			: base(property, message)
		{ }
	}

	public sealed class NotYetBoundException : BindingException
	{
		public NotYetBoundException(Property property)
			: base(property, String.Format("Property has not yet been bound."))
		{ }
	}

	public sealed class AlreadyBoundException : BindingException
	{
		public AlreadyBoundException(Property property)
			: base(property, String.Format("Property has already been bound."))
		{ }
	}

	public sealed class UnknownPropertyException : BindingException
	{
		public UnknownPropertyException(Property property, string current)
			: base(property, String.Format("Property '{0}' not found.", current))
		{ }
	}

	public sealed class ReadOnlyException : BindingException
	{
		public ReadOnlyException(Property property)
			: base(property, "Property is read-only.")
		{ }
	}

	public sealed class WriteOnlyException : BindingException
	{
		public WriteOnlyException(Property property)
			: base(property, "Property is write-only.")
		{ }
	}

	public abstract class AccessException : PropertyException
	{
		public AccessException(Property property, string message)
			: base(property, message)
		{ }
	}

	public sealed class InternalAccessException : AccessException
	{
		public InternalAccessException(Property property)
			: base(property, "Property is internal.")
		{ }
	}

	public abstract class ClearanceException : AccessException
	{
		public AccessLevel PlayerAccess { get; protected set; }
		public AccessLevel NeededAccess { get; protected set; }

		public ClearanceException(Property property, AccessLevel playerAccess, AccessLevel neededAccess, string accessType)
			: base(
				property,
				string.Format(
					"You must be at least {0} to {1} this property, you are currently {2}.",
					Mobile.GetAccessLevelName(neededAccess),
					accessType,
					Mobile.GetAccessLevelName(playerAccess)))
		{ }
	}

	public sealed class ReadAccessException : ClearanceException
	{
		public ReadAccessException(Property property, AccessLevel playerAccess, AccessLevel neededAccess)
			: base(property, playerAccess, neededAccess, "read")
		{ }
	}

	public sealed class WriteAccessException : ClearanceException
	{
		public WriteAccessException(Property property, AccessLevel playerAccess, AccessLevel neededAccess)
			: base(property, playerAccess, neededAccess, "write")
		{ }
	}

	public sealed class Property
	{
		private PropertyInfo[] _Chain;

		public PropertyAccess Access { get; private set; }
		public string Binding { get; private set; }

		public bool IsBound { get { return _Chain != null; } }

		public PropertyInfo[] Chain
		{
			get
			{
				if (!IsBound)
				{
					throw new NotYetBoundException(this);
				}

				return _Chain;
			}
		}

		public Type Type
		{
			get
			{
				if (!IsBound)
				{
					throw new NotYetBoundException(this);
				}

				return _Chain[_Chain.Length - 1].PropertyType;
			}
		}

		public Property(string binding)
		{
			Binding = binding;
		}

		public Property(PropertyInfo[] chain)
		{
			_Chain = chain;
		}

		public static Property Parse(Type type, string binding, PropertyAccess access)
		{
			Property prop = new Property(binding);

			prop.BindTo(type, access);

			return prop;
		}

		public bool CheckAccess(Mobile from)
		{
			if (!IsBound)
			{
				throw new NotYetBoundException(this);
			}

			for (int i = 0; i < _Chain.Length; ++i)
			{
				PropertyAccess access = Access;
				PropertyInfo prop = _Chain[i];
				bool isFinal = i == _Chain.Length - 1;

				if (!isFinal)
				{
					access |= PropertyAccess.Read;
				}

				CPA security = Properties.GetCPA(prop);

				if (security == null)
				{
					throw new InternalAccessException(this);
				}

				if ((access & PropertyAccess.Read) != 0 && from.AccessLevel < security.ReadLevel)
				{
					throw new ReadAccessException(this, from.AccessLevel, security.ReadLevel);
				}

				if ((access & PropertyAccess.Write) != 0 && (from.AccessLevel < security.WriteLevel || security.ReadOnly))
				{
					throw new WriteAccessException(this, from.AccessLevel, security.ReadLevel);
				}
			}

			return true;
		}

		public void BindTo(Type objectType, PropertyAccess desiredAccess)
		{
			if (IsBound)
			{
				throw new AlreadyBoundException(this);
			}

			var split = Binding.Split('.');
			var chain = new PropertyInfo[split.Length];

			for (int i = 0; i < split.Length; ++i)
			{
				bool isFinal = i == chain.Length - 1;

				chain[i] = objectType.GetProperty(split[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

				if (chain[i] == null)
				{
					throw new UnknownPropertyException(this, split[i]);
				}

				objectType = chain[i].PropertyType;

				PropertyAccess access = desiredAccess;

				if (!isFinal)
				{
					access |= PropertyAccess.Read;
				}

				if ((access & PropertyAccess.Read) != 0 && !chain[i].CanRead)
				{
					throw new WriteOnlyException(this);
				}

				if ((access & PropertyAccess.Write) != 0 && !chain[i].CanWrite)
				{
					throw new ReadOnlyException(this);
				}
			}

			Access = desiredAccess;
			_Chain = chain;
		}

		public override string ToString()
		{
			if (!IsBound)
			{
				return Binding;
			}

			var toJoin = new string[_Chain.Length];

			for (int i = 0; i < toJoin.Length; ++i)
			{
				toJoin[i] = _Chain[i].Name;
			}

			return string.Join(".", toJoin);
		}
	}
}