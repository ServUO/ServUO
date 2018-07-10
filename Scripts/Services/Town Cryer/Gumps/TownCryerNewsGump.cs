using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Gumps;

namespace Server.Services.TownCryer
{
    public class TownCryerNewsGump : BaseTownCryerGump
    {
        public TownCryerNewsEntry Entry { get; private set; }

        public TownCryerNewsGump(PlayerMobile pm, TownCrier cryer, TownCryerNewsEntry entry)
            : base(pm, cryer)
        {
            Entry = entry;
        }

        public override void AddGumpLayout()
        {
            base.AddGumpLayout();

            //AddPage(1);
            //AddImageTiled(58, 213, 397, 271, 0x24B2);

            if (Entry.Body.Number > 0)
            {
                AddHtmlLocalized(58, 213, 397, 271, Entry.Body.Number, C32216(0x080808), false, true);
            }
            else
            {
                AddHtml(58, 213, 397, 271, Entry.Body.String, false, true);
            }

            AddHtmlLocalized(0, 150, 854, 20, CenterLoc, Entry.Title.ToString(), 0, false, false);
            AddHtmlLocalized(0, 180, 854, 20, CenterLoc, "#1158084", 0, false, false); // The Town Cryer News Network

            AddImage(468, 213, Entry.GumpImage);
            AddImage(50, 532, 0x60C);

            if (!String.IsNullOrEmpty(Entry.InfoUrl))
            {
                AddButton(147, 600, 0x627, 0x628, 1, GumpButtonType.Reply, 0);
            }

            if (Entry.QuestType != null)
            {
                AddButton(545, 600, 0x629, 0x62A, 2, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                    var gump = new TownCryerGump(User, Cryer);
                    gump.Category = TownCryerGump.GumpCategory.News;
                    BaseGump.SendGump(gump);
                    break;
                case 1:
                    User.LaunchBrowser(Entry.InfoUrl);
                    Refresh();
                    break;
                case 2:
                    if (QuestHelper.HasQuest(User, Entry.QuestType))
                    {
                        Cryer.SayTo(User, 1080107, 1150); // I'm sorry, I have nothing for you at this time.
                    }
                    else
                    {
                        BaseQuest quest = QuestHelper.Construct(Entry.QuestType) as BaseQuest;

                        if (quest != null && (!QuestHelper.CheckDoneOnce(User, quest, Cryer, true) || User.AccessLevel > AccessLevel.Player))
                        {
                            quest.Owner = User;
                            quest.Quester = Cryer;

                            User.CloseGump(typeof(MondainQuestGump));
                            User.SendGump(new MondainQuestGump(quest));
                        }
                    }
                    break;
            }
        }
    }
}