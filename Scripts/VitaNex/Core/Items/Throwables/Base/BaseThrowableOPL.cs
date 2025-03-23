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
using System.Drawing;

using Server;

using VitaNex.Network;
#endregion

namespace VitaNex.Items
{
	public static class BaseThrowableOPL
	{
		public static void Initialize()
		{
			ExtendedOPL.OnItemOPLRequest += OnItemOPLRequest;
		}

		private static void OnItemOPLRequest(Item item, Mobile viewer, ExtendedOPL list)
		{
			if (item == null || list == null)
			{
				return;
			}

			if (viewer == null && item.Parent is Mobile)
			{
				viewer = (Mobile)item.Parent;
			}

			if (viewer == null)
			{
				return;
			}

			var throwable = item as IBaseThrowable;

			if (throwable == null)
			{
				return;
			}

			GetProperties(throwable, viewer, list);
		}

		public static void GetProperties(IBaseThrowable throwable, Mobile viewer, ExtendedOPL list)
		{
			if (throwable == null || list == null)
			{
				return;
			}

			var lines = new List<string>();

			if (throwable.Consumable)
			{
				lines.Add("Consumable");
			}

			if (throwable.ClearHands)
			{
				lines.Add("Clears Hands");
			}

			if (!throwable.AllowCombat)
			{
				lines.Add("Non-Combat");
			}

			if (lines.Count > 0)
			{
				list.Add(String.Join(", ", lines).WrapUOHtmlColor(Color.Orange));
				lines.Clear();
			}

			if (throwable.RequiredSkillValue > 0)
			{
				list.Add("Required Skill: {0} - {1:F2}%", throwable.RequiredSkill, throwable.RequiredSkillValue);
			}

			DateTime now = DateTime.UtcNow, readyWhen = (throwable.ThrownLast + throwable.ThrowRecovery);
			var diff = TimeSpan.Zero;

			if (readyWhen > now)
			{
				diff = readyWhen - now;
			}

			if (diff > TimeSpan.Zero)
			{
				list.Add("Use: {0:D2}:{1:D2}:{2:D2}".WrapUOHtmlColor(Color.LimeGreen), diff.Hours, diff.Minutes, diff.Seconds);
			}
			else if (!String.IsNullOrWhiteSpace(throwable.Usage))
			{
				list.Add("Use: {0}".WrapUOHtmlColor(Color.Cyan), throwable.Usage);
			}

			if (!String.IsNullOrWhiteSpace(throwable.Token))
			{
				list.Add("\"{0}\"".WrapUOHtmlColor(Color.Gold), throwable.Token);
			}
		}
	}
}