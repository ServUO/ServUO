using System;

using Server.Gumps;
using Server.Mobiles;

namespace Server.Services.TownCryer
{
	public class TownCryerGuildGump : BaseTownCryerGump
	{
		public TownCryerGuildEntry Entry { get; private set; }

		public TownCryerGuildGump(PlayerMobile pm, TownCrier cryer, TownCryerGuildEntry entry)
			: base(pm, cryer)
		{
			Entry = entry;
		}

		public override void AddGumpLayout()
		{
			base.AddGumpLayout();

			AddHtml(57, 155, 724, 20, Entry?.Title ?? "Untitled", false, false);
			AddHtmlLocalized(57, 180, 724, 20, 1158066, $"{Entry?.Author ?? "Unknown"}, {Entry?.Guild?.Name}", 0, false, false); // Posted By: ~1_NAME~

			AddHtmlLocalized(57, 215, 50, 20, 1158067, false, false); // When:
			AddHtmlLocalized(57, 235, 50, 20, 1158068, false, false); // Where:

			var time = Entry?.EventTime.Hour ?? 0;

			time = time == 0 ? 12 : time > 12 ? time - 12 : time;

			var ampm = Entry?.EventTime.Hour >= 12 ? "pm" : "am";

			AddLabel(102, 215, 0, $"{Entry?.EventTime.Month}-{Entry?.EventTime.Day}-{Entry?.EventTime.Year} {time}{ampm} {TimeZone.CurrentTimeZone.StandardName}");

			AddLabel(102, 235, 0, Entry?.EventLocation ?? "");

			AddHtml(57, 270, 724, 205, Entry?.Body ?? "", false, false);

			AddImage(85, 425, 0x5EF);
		}

		public override void OnResponse(RelayInfo info)
		{
			if (info.ButtonID == 0)
			{
				var gump = new TownCryerGump(User, Cryer)
				{
					Category = TownCryerGump.GumpCategory.Guild
				};
				_ = SendGump(gump);
			}
		}
	}
}
