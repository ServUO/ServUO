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

#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Server;
using Server.Commands;
#endregion

namespace VitaNex
{
	public static class CommandUtility
	{
		private static readonly Type _TypeOfDescriptionAttribute = typeof(DescriptionAttribute);
		private static readonly Type _TypeOfUsageAttribute = typeof(UsageAttribute);

		public static CommandEntry Unregister(string value)
		{
			CommandEntry handler = null;

			if (!String.IsNullOrWhiteSpace(value) && CommandSystem.Entries.TryGetValue(value, out handler))
			{
				CommandSystem.Entries.Remove(value);
			}

			return handler;
		}

		public static bool Register(string value, AccessLevel access, CommandEventHandler handler)
		{
			return Register(value, access, handler, out var entry);
		}

		public static bool Register(string value, AccessLevel access, CommandEventHandler handler, out CommandEntry entry)
		{
			entry = null;

			if (String.IsNullOrWhiteSpace(value))
			{
				return false;
			}

			if (CommandSystem.Entries.ContainsKey(value))
			{
				return Replace(value, access, handler, value, out entry);
			}

			CommandSystem.Register(value, access, handler);

			return CommandSystem.Entries.TryGetValue(value, out entry);
		}

		public static bool RegisterAlias(string value, string alias)
		{
			return RegisterAlias(value, alias, out var entry);
		}

		public static bool RegisterAlias(string value, string alias, out CommandEntry entry)
		{
			entry = null;

			if (String.IsNullOrWhiteSpace(value) || String.IsNullOrWhiteSpace(alias))
			{
				return false;
			}

			if (!CommandSystem.Entries.TryGetValue(value, out entry) || entry == null)
			{
				return false;
			}

			return Register(alias, entry.AccessLevel, entry.Handler, out entry);
		}

		public static bool Replace(string value, AccessLevel access, CommandEventHandler handler, string newValue)
		{
			return Replace(value, access, handler, newValue, out var entry);
		}

		public static bool Replace(
			string value,
			AccessLevel access,
			CommandEventHandler handler,
			string newValue,
			out CommandEntry entry)
		{
			entry = null;

			if (String.IsNullOrWhiteSpace(value))
			{
				if (String.IsNullOrWhiteSpace(newValue))
				{
					return false;
				}

				value = newValue;
			}

			if (handler == null)
			{
				if (!CommandSystem.Entries.ContainsKey(value))
				{
					return false;
				}

				handler = CommandSystem.Entries[value].Handler;
			}

			if (value != newValue)
			{
				if (String.IsNullOrWhiteSpace(newValue))
				{
					Unregister(value);
					return true;
				}

				value = newValue;
			}

			Unregister(value);
			CommandSystem.Register(value, access, handler);

			return CommandSystem.Entries.TryGetValue(value, out entry);
		}

		public static bool SetAccess(string value, AccessLevel access)
		{
			if (!String.IsNullOrWhiteSpace(value))
			{
				if (CommandSystem.Entries.TryGetValue(value, out var handler))
				{
					return Register(value, access, handler.Handler);
				}
			}

			return false;
		}

		public static IEnumerable<CommandEntry> EnumerateCommands(AccessLevel level)
		{
			return CommandSystem.Entries.Values.Where(o => o != null && o.Handler != null)
								.Where(o => !String.IsNullOrWhiteSpace(o.Command))
								.Where(o => level >= o.AccessLevel);
		}

		public static IEnumerable<CommandEntry> EnumerateCommands()
		{
			return EnumerateCommands(0);
		}

		public static ILookup<AccessLevel, CommandEntry> LookupCommands(AccessLevel level)
		{
			return EnumerateCommands(level)
				.ToLookup(o => o.Handler.Method)
				.Select(o => o.Highest(e => e.Command.Length))
				.ToLookup(o => o.AccessLevel);
		}

		public static ILookup<AccessLevel, CommandEntry> LookupCommands()
		{
			return LookupCommands(0);
		}

		public static string GetDescription(this CommandEntry e)
		{
			return GetDescription(e.Handler);
		}

		public static string GetDescription(this CommandEventHandler o)
		{
			if (o == null)
			{
				return String.Empty;
			}

			return String.Join(
				"\n",
				o.Method.GetCustomAttributes(_TypeOfDescriptionAttribute, true)
				 .OfType<DescriptionAttribute>()
				 .Where(a => !String.IsNullOrWhiteSpace(a.Description))
				 .Select(a => a.Description.StripCRLF().StripExcessWhiteSpace())
				 .Select(v => Regex.Replace(v, @"[\<\{]", "("))
				 .Select(v => Regex.Replace(v, @"[\>\}]", ")")));
		}

		public static string GetUsage(this CommandEntry e)
		{
			return GetUsage(e.Handler);
		}

		public static string GetUsage(this CommandEventHandler o)
		{
			if (o == null)
			{
				return String.Empty;
			}

			var usage = String.Join(
				"\n",
				o.Method.GetCustomAttributes(_TypeOfUsageAttribute, true)
				 .OfType<UsageAttribute>()
				 .Where(a => !String.IsNullOrWhiteSpace(a.Usage))
				 .Select(a => a.Usage.StripCRLF().StripExcessWhiteSpace())
				 .Select(v => Regex.Replace(v, @"[\<\{]", "("))
				 .Select(v => Regex.Replace(v, @"[\>\}]", ")")));

			return usage;
		}
	}
}