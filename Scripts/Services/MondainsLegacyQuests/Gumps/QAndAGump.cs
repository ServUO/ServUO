using System;
using Server.Commands;
using Server.Network;
using Server.Engines.Quests;
using System.Collections.Generic;
using Server.Items;

namespace Server.Gumps
{
    public class QAndAGump : Gump
    {
        private const int FontColor = 0xFFFFFF;

        private Mobile m_From;
        private QuestionAndAnswerObjective m_Objective;
        private BaseQuest m_Quest;
        private int m_Index;

        public QAndAGump(Mobile owner, BaseQuest quest) : base(50, 50)
        {
            m_From = owner;
            m_Quest = quest;
            this.Closable = false;

            foreach (BaseObjective objective in quest.Objectives)
            {
                if (objective is QuestionAndAnswerObjective)
                {
                    m_Objective = (QuestionAndAnswerObjective)objective;
                    break;
                }
            }

            if (m_Objective == null)
                return;

            QuestionAndAnswerEntry entry = m_Objective.GetRandomQandA();

            if (entry == null)
                return;

            AddPage(0);

            int question = entry.Question;
            object answer = entry.Answers[Utility.Random(entry.Answers.Length)];

            List<object> selections = new List<object>(entry.WrongAnswers);
            m_Index = Utility.Random(selections.Count); //Gets correct answer
            selections.Insert(m_Index, answer);

            AddBackground(0, 0, 305, 145 + (selections.Count * 35), 9270);
            AddHtmlLocalized(104, 25, 200, 26, (int)m_Quest.Title, FontColor, false, false); //La Insep Om
            AddHtmlLocalized(20, 55, 250, 36, question, FontColor, false, false); //question

            int y = 105;

            for (int i = 0; i < selections.Count; i++)
            {
                //AddButton(82, y, 2152, 2153, i + 1, GumpButtonType.Reply, 0);
                AddRadio(20, y, 2152, 2153, false, i + 1);

                object selection = selections[i];

                if (selection is int)
                    AddHtmlLocalized(70, y - 2, 200, 18, (int)selection, FontColor, false, false);
                else
                    AddHtml(70, y - 2, 200, 18, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", FontColor, selections[i]), false, false);

                y += 35;
            }

            AddButton(129, y, 2074, 2076, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID > 0 && info.IsSwitched(m_Index + 1)) //correct answer
            {
                m_Objective.Update(null);

                if (m_Quest.Completed)
                {
                    m_Quest.OnCompleted();
                    m_From.SendGump(new MondainQuestGump(m_Quest, MondainQuestGump.Section.Complete, false, true));
                }
                else
                {
                    m_From.SendMessage("Correct!");
                    m_From.SendGump(new QAndAGump(m_From, m_Quest));
                }
            }
            else
            {
                m_From.SendGump(new MondainQuestGump(m_Quest, MondainQuestGump.Section.Failed, false, true));
                m_Quest.OnResign(false);
            }
        }
    }
}