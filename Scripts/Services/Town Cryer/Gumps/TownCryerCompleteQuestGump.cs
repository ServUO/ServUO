using System.Linq;

using Server.Engines.Quests;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Services.TownCryer
{
	public class TownCrierQuestCompleteGump : BaseGump
	{
		public object Title { get; set; }
		public object Body { get; set; }
		public int GumpID { get; set; }

		public TownCrierQuestCompleteGump(PlayerMobile pm, object title, object body, int id)
			: base(pm, 10, 100)
		{
			Title = title;
			Body = body;
			GumpID = id;
		}

		public TownCrierQuestCompleteGump(PlayerMobile pm, BaseQuest quest)
			: base(pm, 10, 100)
		{
			Title = quest.Title;
			Body = quest.Complete;

			var entry = TownCryerSystem.NewsEntries.FirstOrDefault(e => e.QuestType == quest.GetType());

			if (entry != null)
			{
				GumpID = entry.GumpImage;
			}
		}

		public override void AddGumpLayout()
		{
			AddBackground(0, 0, 454, 540, 9380);

			AddImage(62, 42, GumpID);

			if (Title is int tnum)
			{
				AddHtmlLocalized(0, 392, 454, 20, CenterLoc, $"#{tnum}", 0, false, false);
			}
			else if (Title is string tstr)
			{
				AddHtml(0, 392, 454, 20, Center(tstr), false, false);
			}

			if (Body is int bnum)
			{
				AddHtmlLocalized(27, 417, 390, 73, bnum, C32216(0x080808), false, true);
			}
			else if (Body is string bstr)
			{
				AddHtml(27, 417, 390, 73, bstr, false, true);
			}
		}
	}
}
