#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endregion

namespace Server
{
	[AttributeUsage(AttributeTargets.Property)]
	public class HueAttribute : Attribute
	{ }

	[AttributeUsage(AttributeTargets.Property)]
	public class BodyAttribute : Attribute
	{ }

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public class PropertyObjectAttribute : Attribute
	{ }

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class NoSortAttribute : Attribute
	{ }

	[AttributeUsage(AttributeTargets.Property)]
	public class NoDupeAttribute : Attribute
	{ }

	[AttributeUsage(AttributeTargets.Method)]
	public class CallPriorityAttribute : Attribute
	{
		public int Priority { get; set; }

		public CallPriorityAttribute(int priority)
		{
			Priority = priority;
		}
	}

	public class CallPriorityComparer : IComparer<MethodInfo>
	{
		public int Compare(MethodInfo x, MethodInfo y)
		{
			if (x == null && y == null)
			{
				return 0;
			}

			if (x == null)
			{
				return 1;
			}

			if (y == null)
			{
				return -1;
			}

			var xPriority = GetPriority(x);
			var yPriority = GetPriority(y);

			if (xPriority > yPriority)
				return 1;

			if (xPriority < yPriority)
				return -1;

			return 0;
		}

		private static int GetPriority(MethodInfo mi)
		{
			var objs = mi.GetCustomAttributes(typeof(CallPriorityAttribute), true);

			if (objs == null)
			{
				return 0;
			}

			if (objs.Length == 0)
			{
				return 0;
			}

			var attr = objs[0] as CallPriorityAttribute;

			if (attr == null)
			{
				return 0;
			}

			return attr.Priority;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class TypeAliasAttribute : Attribute
	{
		public string[] Aliases { get; }

		public TypeAliasAttribute(params string[] aliases)
		{
			Aliases = aliases;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class ParsableAttribute : Attribute
	{ }

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
	public class CustomEnumAttribute : Attribute
	{
		public string[] Names { get; }

		public CustomEnumAttribute(string[] names)
		{
			Names = names;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class DeleteConfirmAttribute : Attribute
	{
		public string Message { get; set; }

		public DeleteConfirmAttribute()
			: this("Are you sure you wish to delete this item?")
		{ }

		public DeleteConfirmAttribute(string message)
		{
			Message = message;
		}

		public static bool Find<T>(out string message) where T : class
		{
			return Find(typeof(T), out message);
		}

		public static bool Find(object o, out string message)
		{
			return Find(o.GetType(), out message);
		}

		public static bool Find(Type t, out string message)
		{
			var attrs = t.GetCustomAttributes(typeof(DeleteConfirmAttribute), true);

			if (attrs.Length > 0)
			{
				message = String.Join("\n", attrs.OfType<DeleteConfirmAttribute>().Select(a => a.Message));
				return true;
			}

			message = String.Empty;
			return false;
		}
	}

	[AttributeUsage(AttributeTargets.Constructor)]
	public class ConstructableAttribute : Attribute
	{
		public AccessLevel AccessLevel { get; set; }

		public ConstructableAttribute()
			: this(AccessLevel.Player) //Lowest accesslevel for current functionality (Level determined by access to [add)
		{ }

		public ConstructableAttribute(AccessLevel accessLevel)
		{
			AccessLevel = accessLevel;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class CommandPropertyAttribute : Attribute
	{
		public AccessLevel ReadLevel { get; }
		public AccessLevel WriteLevel { get; }

		public bool ReadOnly { get; }

		public CommandPropertyAttribute(AccessLevel level, bool readOnly)
		{
			ReadLevel = level;
			ReadOnly = readOnly;
		}

		public CommandPropertyAttribute(AccessLevel level)
			: this(level, level)
		{ }

		public CommandPropertyAttribute(AccessLevel readLevel, AccessLevel writeLevel)
		{
			ReadLevel = readLevel;
			WriteLevel = writeLevel;
		}
	}
}
