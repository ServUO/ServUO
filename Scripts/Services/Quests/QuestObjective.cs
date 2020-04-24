using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections;

namespace Server.Engines.Quests
{
    public abstract class QuestObjective
    {
        private QuestSystem m_System;
        private bool m_HasBeenRead;
        private int m_CurProgress;
        private bool m_HasCompleted;

        public abstract object Message { get; }
        public virtual int MaxProgress => 1;
        public virtual QuestItemInfo[] Info => null;
        public QuestSystem System
        {
            get
            {
                return m_System;
            }
            set
            {
                m_System = value;
            }
        }
        public bool HasBeenRead
        {
            get
            {
                return m_HasBeenRead;
            }
            set
            {
                m_HasBeenRead = value;
            }
        }
        public int CurProgress
        {
            get
            {
                return m_CurProgress;
            }
            set
            {
                m_CurProgress = value;
                CheckCompletionStatus();
            }
        }
        public bool HasCompleted
        {
            get
            {
                return m_HasCompleted;
            }
            set
            {
                m_HasCompleted = value;
            }
        }
        public virtual bool Completed => m_CurProgress >= MaxProgress;
        public bool IsSingleObjective => (MaxProgress == 1);
        public virtual void BaseDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    {
                        m_HasBeenRead = reader.ReadBool();
                        goto case 0;
                    }
                case 0:
                    {
                        m_CurProgress = reader.ReadEncodedInt();
                        m_HasCompleted = reader.ReadBool();

                        break;
                    }
            }

            ChildDeserialize(reader);
        }

        public virtual void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
        }

        public virtual void BaseSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(1); // version

            writer.Write(m_HasBeenRead);
            writer.WriteEncodedInt(m_CurProgress);
            writer.Write(m_HasCompleted);

            ChildSerialize(writer);
        }

        public virtual void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version
        }

        public virtual void Complete()
        {
            CurProgress = MaxProgress;
        }

        public virtual void RenderMessage(BaseQuestGump gump)
        {
            gump.AddHtmlObject(70, 130, 300, 100, Message, BaseQuestGump.Blue, false, false);
        }

        public virtual void RenderProgress(BaseQuestGump gump)
        {
            gump.AddHtmlObject(70, 260, 270, 100, Completed ? 1049077 : 1049078, BaseQuestGump.Blue, false, false);
        }

        public virtual void CheckCompletionStatus()
        {
            if (Completed && !HasCompleted)
            {
                HasCompleted = true;
                OnComplete();
            }
        }

        public virtual void OnRead()
        {
        }

        public virtual bool GetTimerEvent()
        {
            return !Completed;
        }

        public virtual void CheckProgress()
        {
        }

        public virtual void OnComplete()
        {
        }

        public virtual bool GetKillEvent(BaseCreature creature, Container corpse)
        {
            return !Completed;
        }

        public virtual void OnKill(BaseCreature creature, Container corpse)
        {
        }

        public virtual bool IgnoreYoungProtection(Mobile from)
        {
            return false;
        }
    }

    public class QuestLogUpdatedGump : BaseQuestGump
    {
        private readonly QuestSystem m_System;
        public QuestLogUpdatedGump(QuestSystem system)
            : base(3, 30)
        {
            m_System = system;

            AddPage(0);

            AddImage(20, 5, 1417);

            AddHtmlLocalized(0, 78, 120, 40, 1049079, White, false, false); // Quest Log Updated

            AddImageTiled(0, 78, 120, 40, 2624);
            AddAlphaRegion(0, 78, 120, 40);

            AddButton(30, 15, 5575, 5576, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
                m_System.ShowQuestLog();
        }
    }

    public class QuestObjectivesGump : BaseQuestGump
    {
        private readonly ArrayList m_Objectives;
        public QuestObjectivesGump(QuestObjective obj)
            : this(BuildList(obj))
        {
        }

        public QuestObjectivesGump(ArrayList objectives)
            : base(90, 50)
        {
            m_Objectives = objectives;

            Closable = false;

            AddPage(0);

            AddImage(0, 0, 3600);
            AddImageTiled(0, 14, 15, 375, 3603);
            AddImageTiled(380, 14, 14, 375, 3605);
            AddImage(0, 376, 3606);
            AddImageTiled(15, 376, 370, 16, 3607);
            AddImageTiled(15, 0, 370, 16, 3601);
            AddImage(380, 0, 3602);
            AddImage(380, 376, 3608);

            AddImageTiled(15, 15, 365, 365, 2624);
            AddAlphaRegion(15, 15, 365, 365);

            AddImage(20, 87, 1231);
            AddImage(75, 62, 9307);

            AddHtmlLocalized(117, 35, 230, 20, 1046026, Blue, false, false); // Quest Log

            AddImage(77, 33, 9781);
            AddImage(65, 110, 2104);

            AddHtmlLocalized(79, 106, 230, 20, 1049073, Blue, false, false); // Objective:

            AddImageTiled(68, 125, 120, 1, 9101);
            AddImage(65, 240, 2104);

            AddHtmlLocalized(79, 237, 230, 20, 1049076, Blue, false, false); // Progress details:

            AddImageTiled(68, 255, 120, 1, 9101);
            AddButton(175, 355, 2313, 2312, 1, GumpButtonType.Reply, 0);

            AddImage(341, 15, 10450);
            AddImage(341, 330, 10450);
            AddImage(15, 330, 10450);
            AddImage(15, 15, 10450);

            AddPage(1);

            for (int i = 0; i < objectives.Count; ++i)
            {
                QuestObjective obj = (QuestObjective)objectives[objectives.Count - 1 - i];

                if (i > 0)
                {
                    AddButton(55, 346, 9909, 9911, 0, GumpButtonType.Page, 1 + i);
                    AddHtmlLocalized(82, 347, 50, 20, 1043354, White, false, false); // Previous

                    AddPage(1 + i);
                }

                obj.RenderMessage(this);
                obj.RenderProgress(this);

                if (i > 0)
                {
                    AddButton(317, 346, 9903, 9905, 0, GumpButtonType.Page, i);
                    AddHtmlLocalized(278, 347, 50, 20, 1043353, White, false, false); // Next
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            for (int i = m_Objectives.Count - 1; i >= 0; --i)
            {
                QuestObjective obj = (QuestObjective)m_Objectives[i];

                if (!obj.HasBeenRead)
                {
                    obj.HasBeenRead = true;
                    obj.OnRead();
                }
            }
        }
    }
}
