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
using System.Drawing;

using Server;
using Server.Mobiles;
#endregion

namespace VitaNex.Notify
{
	public abstract class WorldNotifyGump : NotifyGump
	{
		private static void InitSettings(NotifySettings o)
		{
			o.CanIgnore = true;
			o.Desc = "General broadcasts from the staff and server.";
		}

		public WorldNotifyGump(Mobile user, string html)
			: this(user, html, false)
		{ }

		public WorldNotifyGump(Mobile user, string html, bool autoClose)
			: base(user, html)
		{
			AnimDuration = TimeSpan.FromSeconds(1.0);
			PauseDuration = TimeSpan.FromSeconds(10.0);
			HtmlColor = Color.Yellow;
			AutoClose = autoClose;
		}

		private class Sub0 : WorldNotifyGump
		{
			public Sub0(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub0(Mobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}

		private class Sub1 : WorldNotifyGump
		{
			public Sub1(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub1(Mobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}

		private class Sub2 : WorldNotifyGump
		{
			public Sub2(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub2(Mobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}

		private class Sub3 : WorldNotifyGump
		{
			public Sub3(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub3(PlayerMobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}

		private class Sub4 : WorldNotifyGump
		{
			public Sub4(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub4(Mobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}

		private class Sub5 : WorldNotifyGump
		{
			public Sub5(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub5(Mobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}

		private class Sub6 : WorldNotifyGump
		{
			public Sub6(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub6(Mobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}

		private class Sub7 : WorldNotifyGump
		{
			public Sub7(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub7(Mobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}

		private class Sub8 : WorldNotifyGump
		{
			public Sub8(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub8(Mobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}

		private class Sub9 : WorldNotifyGump
		{
			public Sub9(Mobile user, string html)
				: base(user, html)
			{ }

			public Sub9(Mobile user, string html, bool autoClose)
				: base(user, html, autoClose)
			{ }
		}
	}
}