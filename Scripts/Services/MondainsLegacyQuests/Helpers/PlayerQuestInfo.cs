
using Server.Gumps;
using Server.Commands;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public enum QuestPage
    {
        Quests,
        Restart
    }

    public class PlayerQuestInfoGump : BaseGump
    {
        public static void Initialize()
        {
            CommandSystem.Register("Quests", AccessLevel.GameMaster, CheckQuests);
        }

        [Usage("Quests")]
        [Description("Displays the staff member the quests and status.")]
        public static void CheckQuests(CommandEventArgs e)
        {
            var pm = e.Mobile as PlayerMobile;

            if (pm != null)
            {
                pm.BeginTarget(-1, false, TargetFlags.None, (m, targeted) =>
                {
                    if (targeted is PlayerMobile subject)
                    {
                        var quests = subject.Quests;
                        var chains = subject.Chains;
                        var done = subject.DoneQuests;

                        SendGump(new PlayerQuestInfoGump(pm, subject));
                    }
                });
            }
        }

        public PlayerMobile Subject { get; private set; }
        public QuestPage Page { get; private set; }

        public PlayerQuestInfoGump(PlayerMobile user, PlayerMobile subject)
            : base(user, 100, 20)
        {
            Subject = subject;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 600, 400, 3000);
            AddPage(0);
            AddHtml(0, 10, 600, 20, Center(string.Format("Quest Info for {0}", Subject.Name)), false, false);

            AddButton(10, 20, Page == QuestPage.Quests ? 4006 : 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(43, 20, 0, "Active Quests");

            AddButton(10, 42, Page == QuestPage.Restart ? 4006 : 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(43, 42, 0, "Restart Info");

            switch (Page)
            {
                case QuestPage.Quests:
                    BuildQuests();
                    break;
                case QuestPage.Restart:
                    BuildRestart();
                    break;
            }
        }

        private void BuildQuests()
        {
            var y = 70;

            var quests = Subject.Quests;

            AddLabel(10, y, 0, "Quest");
            AddLabel(210, y, 0, "Status");
            AddLabel(300, y, 0, "Chain");
            AddLabel(400, y, 0, "Remove");

            y += 22;

            if (quests != null)
            {
                for (int i = 0; i < quests.Count; i++)
                {
                    var quest = quests[i];

                    if (quest == null)
                    {
                        continue;
                    }

                    if (quest.Title is int)
                    {
                        AddHtmlLocalized(10, y, 200, 20, (int)quest.Title, false, false);
                    }
                    else if (quest.Title is string)
                    {
                        AddHtml(10, y, 200, 20, (string)quest.Title, false, false);
                    }

                    AddLabel(210, y, 0, string.Format("{0}/{1}", ObjectivesComplete(quest).ToString(), quest.Objectives.Count));
                    AddLabelCropped(300, y, 100, 20, 0, string.Format("{0}", quest.ChainID != QuestChain.None ? quest.ChainID.ToString() : "N/A"));
                    AddButton(400, y, 0xFB1, 0xFB3, i + 100, GumpButtonType.Reply, 0);

                    y += 20;
                }
            }
        }

        private void BuildRestart()
        {
            var done = Subject.DoneQuests;
            var y = 70;

            AddLabel(10, y, 0, "Quest Type");
            AddLabel(250, y, 0, "Next Restart");
            AddLabel(500, y, 0, "Remove Delay");

            y += 20;

            if (done != null)
            {
                for (int i = 0; i < done.Count; i++)
                {
                    var info = done[i];

                    if (info != null)
                    {
                        var quest = QuestHelper.Construct(info.QuestType);

                        if (quest != null)
                        {
                            if (quest.Title is int)
                            {
                                AddHtmlLocalized(10, y, 200, 20, (int)quest.Title, false, false);
                            }
                            else if (quest.Title is string)
                            {
                                AddHtml(10, y, 200, 20, (string)quest.Title, false, false);
                            }
                        }
                        else
                        {
                            AddLabel(10, y, 0, info.QuestType.Name);
                        }

                        AddLabel(250, y, 0, quest != null && quest.DoneOnce ? "Never" : info.RestartTime.ToString());

                        AddButton(500, y, 0xFB1, 0xFB3, i + 1000, GumpButtonType.Reply, 0);

                        y += 20;
                    }
                }
            }
        }

        private int ObjectivesComplete(BaseQuest q)
        {
            int count = 0;

            for (int i = 0; i < q.Objectives.Count; i++)
            {
                if (q.Objectives[i].Completed)
                {
                    count++;
                }
            }

            return count;
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 1: Page = QuestPage.Quests; Refresh(); break;
                case 2: Page = QuestPage.Restart; Refresh(); break;
                default:
                    if (info.ButtonID >= 1000)
                    {
                        var done = Subject.DoneQuests;
                        var id = info.ButtonID - 1000;

                        if (id >= 0 && id < done.Count)
                        {
                            var restart = done[id];

                            if (restart != null)
                            {
                                SendGump(new ConfirmCallbackGump(
                                    User,
                                    "Remove Delayed Restart",
                                    string.Format("By selecting yes, you will remove restart delay or restart restriction for this quest. This player will be able to restart this quest immediately thereafter.", restart.RestartTime.ToString()),
                                    null,
                                    null,
                                    (m, o) =>
                                    {
                                        Subject.DoneQuests.Remove(restart);
                                        User.SendMessage("Restart info removed.");
                                    }));
                            }
                        }
                    }
                    else if (info.ButtonID >= 100)
                    {
                        var quests = Subject.Quests;
                        var id = info.ButtonID - 100;

                        if (id >= 0 && id < quests.Count)
                        {
                            var quest = quests[id];

                            if (quests != null)
                            {
                                User.SendGump(new MondainResignGump(quest));
                            }
                        }
                    }

                    break;
            }
        }
    }
}
