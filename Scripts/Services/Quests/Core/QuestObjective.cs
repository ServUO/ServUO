using System;
using System.Collections;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests
{
    public abstract class QuestObjective
    {
        private QuestSystem m_System;
        private bool m_HasBeenRead;
        private int m_CurProgress;
        private bool m_HasCompleted;
        public QuestObjective()
        {
        }

        public abstract object Message { get; }
        public virtual int MaxProgress
        {
            get
            {
                return 1;
            }
        }
        public virtual QuestItemInfo[] Info
        {
            get
            {
                return null;
            }
        }
        public QuestSystem System
        {
            get
            {
                return this.m_System;
            }
            set
            {
                this.m_System = value;
            }
        }
        public bool HasBeenRead
        {
            get
            {
                return this.m_HasBeenRead;
            }
            set
            {
                this.m_HasBeenRead = value;
            }
        }
        public int CurProgress
        {
            get
            {
                return this.m_CurProgress;
            }
            set
            {
                this.m_CurProgress = value;
                this.CheckCompletionStatus();
            }
        }
        public bool HasCompleted
        {
            get
            {
                return this.m_HasCompleted;
            }
            set
            {
                this.m_HasCompleted = value;
            }
        }
        public virtual bool Completed
        {
            get
            {
                return this.m_CurProgress >= this.MaxProgress;
            }
        }
        public bool IsSingleObjective
        {
            get
            {
                return (this.MaxProgress == 1);
            }
        }
        public virtual void BaseDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_HasBeenRead = reader.ReadBool();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_CurProgress = reader.ReadEncodedInt();
                        this.m_HasCompleted = reader.ReadBool();

                        break;
                    }
            }

            this.ChildDeserialize(reader);
        }

        public virtual void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
        }

        public virtual void BaseSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)1); // version

            writer.Write((bool)this.m_HasBeenRead);
            writer.WriteEncodedInt((int)this.m_CurProgress);
            writer.Write((bool)this.m_HasCompleted);

            this.ChildSerialize(writer);
        }

        public virtual void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version
        }

        public virtual void Complete()
        {
            this.CurProgress = this.MaxProgress;
        }

        public virtual void RenderMessage(BaseQuestGump gump)
        {
            gump.AddHtmlObject(70, 130, 300, 100, this.Message, BaseQuestGump.Blue, false, false);
        }

        public virtual void RenderProgress(BaseQuestGump gump)
        {
            gump.AddHtmlObject(70, 260, 270, 100, this.Completed ? 1049077 : 1049078, BaseQuestGump.Blue, false, false);
        }

        public virtual void CheckCompletionStatus()
        {
            if (this.Completed && !this.HasCompleted)
            {
                this.HasCompleted = true;
                this.OnComplete();
            }
        }

        public virtual void OnRead()
        {
        }

        public virtual bool GetTimerEvent()
        {
            return !this.Completed;
        }

        public virtual void CheckProgress()
        {
        }

        public virtual void OnComplete()
        {
        }

        public virtual bool GetKillEvent(BaseCreature creature, Container corpse)
        {
            return !this.Completed;
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
            this.m_System = system;

            this.AddPage(0);

            this.AddImage(20, 5, 1417);

            this.AddHtmlLocalized(0, 78, 120, 40, 1049079, White, false, false); // Quest Log Updated

            this.AddImageTiled(0, 78, 120, 40, 2624);
            this.AddAlphaRegion(0, 78, 120, 40);

            this.AddButton(30, 15, 5575, 5576, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
                this.m_System.ShowQuestLog();
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
            this.m_Objectives = objectives;

            this.Closable = false;

            this.AddPage(0);

            this.AddImage(0, 0, 3600);
            this.AddImageTiled(0, 14, 15, 375, 3603);
            this.AddImageTiled(380, 14, 14, 375, 3605);
            this.AddImage(0, 376, 3606);
            this.AddImageTiled(15, 376, 370, 16, 3607);
            this.AddImageTiled(15, 0, 370, 16, 3601);
            this.AddImage(380, 0, 3602);
            this.AddImage(380, 376, 3608);

            this.AddImageTiled(15, 15, 365, 365, 2624);
            this.AddAlphaRegion(15, 15, 365, 365);

            this.AddImage(20, 87, 1231);
            this.AddImage(75, 62, 9307);

            this.AddHtmlLocalized(117, 35, 230, 20, 1046026, Blue, false, false); // Quest Log

            this.AddImage(77, 33, 9781);
            this.AddImage(65, 110, 2104);

            this.AddHtmlLocalized(79, 106, 230, 20, 1049073, Blue, false, false); // Objective:

            this.AddImageTiled(68, 125, 120, 1, 9101);
            this.AddImage(65, 240, 2104);

            this.AddHtmlLocalized(79, 237, 230, 20, 1049076, Blue, false, false); // Progress details:

            this.AddImageTiled(68, 255, 120, 1, 9101);
            this.AddButton(175, 355, 2313, 2312, 1, GumpButtonType.Reply, 0);

            this.AddImage(341, 15, 10450);
            this.AddImage(341, 330, 10450);
            this.AddImage(15, 330, 10450);
            this.AddImage(15, 15, 10450);

            this.AddPage(1);

            for (int i = 0; i < objectives.Count; ++i)
            {
                QuestObjective obj = (QuestObjective)objectives[objectives.Count - 1 - i];

                if (i > 0)
                {
                    this.AddButton(55, 346, 9909, 9911, 0, GumpButtonType.Page, 1 + i);
                    this.AddHtmlLocalized(82, 347, 50, 20, 1043354, White, false, false); // Previous

                    this.AddPage(1 + i);
                }

                obj.RenderMessage(this);
                obj.RenderProgress(this);

                if (i > 0)
                {
                    this.AddButton(317, 346, 9903, 9905, 0, GumpButtonType.Page, i);
                    this.AddHtmlLocalized(278, 347, 50, 20, 1043353, White, false, false); // Next
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            for (int i = this.m_Objectives.Count - 1; i >= 0; --i)
            {
                QuestObjective obj = (QuestObjective)this.m_Objectives[i];

                if (!obj.HasBeenRead)
                {
                    obj.HasBeenRead = true;
                    obj.OnRead();
                }
            }
        }
    }
}