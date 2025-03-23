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

using Server.Accounting;
#endregion

namespace Server
{
	public static class AccountExtUtility
	{
		public static bool IsOnline(this IAccount acc)
		{
			return FindMobiles(acc, m => m.IsOnline()).Any();
		}

		public static Mobile GetOnlineMobile(this IAccount acc)
		{
			return FindMobiles(acc, m => m.IsOnline()).FirstOrDefault();
		}

		public static TMob GetOnlineMobile<TMob>(this IAccount acc)
			where TMob : Mobile
		{
			return FindMobiles<TMob>(acc, m => m.IsOnline()).FirstOrDefault();
		}

		public static Mobile[] GetMobiles(this IAccount acc)
		{
			return GetMobiles(acc, null);
		}

		public static Mobile[] GetMobiles(this IAccount acc, Func<Mobile, bool> predicate)
		{
			return FindMobiles(acc, predicate).ToArray();
		}

		public static IEnumerable<Mobile> FindMobiles(this IAccount acc)
		{
			return FindMobiles(acc, null);
		}

		public static IEnumerable<Mobile> FindMobiles(this IAccount acc, Func<Mobile, bool> predicate)
		{
			if (acc == null)
			{
				yield break;
			}

			for (var i = 0; i < acc.Length; i++)
			{
				if (acc[i] != null && (predicate == null || predicate(acc[i])))
				{
					yield return acc[i];
				}
			}
		}

		public static TMob[] GetMobiles<TMob>(this IAccount acc)
			where TMob : Mobile
		{
			return GetMobiles<TMob>(acc, null);
		}

		public static TMob[] GetMobiles<TMob>(this IAccount acc, Func<TMob, bool> predicate)
			where TMob : Mobile
		{
			return FindMobiles(acc, predicate).ToArray();
		}

		public static IEnumerable<TMob> FindMobiles<TMob>(this IAccount acc)
			where TMob : Mobile
		{
			return FindMobiles<TMob>(acc, null);
		}

		public static IEnumerable<TMob> FindMobiles<TMob>(this IAccount acc, Func<TMob, bool> predicate)
			where TMob : Mobile
		{
			if (acc == null)
			{
				yield break;
			}

			for (var i = 0; i < acc.Length; i++)
			{
				if (acc[i] is TMob && (predicate == null || predicate((TMob)acc[i])))
				{
					yield return (TMob)acc[i];
				}
			}
		}

		public static Account[] GetSharedAccounts(this IAccount acc)
		{
			return GetSharedAccounts(acc as Account);
		}

		public static Account[] GetSharedAccounts(this Account acc)
		{
			return FindSharedAccounts(acc).ToArray();
		}

		public static IEnumerable<Account> FindSharedAccounts(this IAccount acc)
		{
			return FindSharedAccounts(acc as Account);
		}

		public static IEnumerable<Account> FindSharedAccounts(this Account acc)
		{
			if (acc == null)
			{
				yield break;
			}

			foreach (var a in Accounts.GetAccounts().AsParallel().OfType<Account>().Where(a => IsSharedWith(acc, a)))
			{
				yield return a;
			}
		}

		public static bool IsSharedWith(this IAccount acc, IAccount a)
		{
			return IsSharedWith(acc as Account, a as Account);
		}

		public static bool IsSharedWith(this Account acc, Account a)
		{
			return acc != null && a != null && (acc == a || acc.LoginIPs.Any(a.LoginIPs.Contains));
		}

		public static bool CheckAccount(this Mobile a, Mobile b)
		{
			return a != null && b != null && (a == b || a.Account == b.Account);
		}

		public static bool CheckAccount(this Mobile a, IAccount b)
		{
			return a != null && b != null && a.Account == b;
		}

		public static bool CheckAccount(this IAccount a, Mobile b)
		{
			return a != null && b != null && a == b.Account;
		}
	}
}