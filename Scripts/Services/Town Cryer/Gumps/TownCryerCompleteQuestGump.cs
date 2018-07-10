using Server;
using System;
using System.Linq;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.Quests;

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

            if (Title is int)
            {
                AddHtmlLocalized(0, 392, 454, 20, CenterLoc, String.Format("#{0}", (int)Title), 0, false, false);
            }
            else if (Title is string)
            {
                AddHtml(0, 392, 454, 20, Center((string)Title), false, false);
            }

            if (Body is int)
            {
                AddHtmlLocalized(27, 417, 390, 73, (int)Body, C32216(0x080808), false, true);
            }
            else if (Body is string)
            {
                AddHtml(27, 417, 390, 73, (string)Body, false, true);
            }
        }
    }
}