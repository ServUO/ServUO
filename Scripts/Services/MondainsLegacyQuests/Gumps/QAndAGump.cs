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
        private const int FontColor = 0x000008;

        private Mobile m_From;
        private QuestionAndAnswerObjective m_Objective;
        private BaseQuest m_Quest;
        private int m_Index;

        public QAndAGump(Mobile owner, BaseQuest quest) : base(0, 0)
        {
            m_From = owner;
            m_Quest = quest;
            Closable = false;
            Disposable = false;

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
            AddImage(0, 0, 1228);
            AddImage(40, 78, 95);
            AddImageTiled(49, 87, 301, 3, 96);
            AddImage(350, 78, 97);

            object answer = entry.Answers[Utility.Random(entry.Answers.Length)];

            List<object> selections = new List<object>(entry.WrongAnswers);
            m_Index = Utility.Random(selections.Count); //Gets correct answer
            selections.Insert(m_Index, answer);
            
            AddHtmlLocalized(40, 40, 320, 40, entry.Question, FontColor, false, false); //question

            for (int i = 0; i < selections.Count; i++)
            {
                object selection = selections[i];

                AddButton(49, 104 + (i * 40), 2224, 2224, selection == answer ? 1 : 0, GumpButtonType.Reply, 0);                

                if (selection is int)
                    AddHtmlLocalized(80, 102 + (i * 40), 200, 18, (int)selection, 0x0, false, false);
                else
                    AddHtml(80, 102 + (i * 40), 200, 18, String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", FontColor, selection), false, false);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 1) //correct answer
            {
                m_Objective.Update(null);

                if (m_Quest.Completed)
                {
                    m_Quest.OnCompleted();
                    m_From.SendGump(new MondainQuestGump(m_Quest, MondainQuestGump.Section.Complete, false, true));
                }
                else
                {
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