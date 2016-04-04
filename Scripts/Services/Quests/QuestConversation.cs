using System;
using System.Collections;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Quests
{
    public abstract class QuestConversation
    {
        private QuestSystem m_System;
        private bool m_HasBeenRead;
        public QuestConversation()
        {
        }

        public abstract object Message { get; }
        public virtual QuestItemInfo[] Info
        {
            get
            {
                return null;
            }
        }
        public virtual bool Logged
        {
            get
            {
                return true;
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
        public virtual void BaseDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_HasBeenRead = reader.ReadBool();

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
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_HasBeenRead);

            this.ChildSerialize(writer);
        }

        public virtual void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version
        }

        public virtual void OnRead()
        {
        }
    }

    public class QuestConversationsGump : BaseQuestGump
    {
        private readonly ArrayList m_Conversations;
        public QuestConversationsGump(QuestConversation conv)
            : this(BuildList(conv))
        {
        }

        public QuestConversationsGump(ArrayList conversations)
            : base(30, 50)
        {
            this.m_Conversations = conversations;

            this.Closable = false;

            this.AddPage(0);

            this.AddImage(349, 10, 9392);
            this.AddImageTiled(349, 130, 100, 120, 9395);
            this.AddImageTiled(149, 10, 200, 140, 9391);
            this.AddImageTiled(149, 250, 200, 140, 9397);
            this.AddImage(349, 250, 9398);
            this.AddImage(35, 10, 9390);
            this.AddImageTiled(35, 150, 120, 100, 9393);
            this.AddImage(35, 250, 9396);

            this.AddHtmlLocalized(110, 60, 200, 20, 1049069, White, false, false); // <STRONG>Conversation Event</STRONG>

            this.AddImage(65, 14, 10102);
            this.AddImageTiled(81, 14, 349, 17, 10101);
            this.AddImage(426, 14, 10104);

            this.AddImageTiled(55, 40, 388, 323, 2624);
            this.AddAlphaRegion(55, 40, 388, 323);

            this.AddImageTiled(75, 90, 200, 1, 9101);
            this.AddImage(75, 58, 9781);
            this.AddImage(380, 45, 223);

            this.AddButton(220, 335, 2313, 2312, 1, GumpButtonType.Reply, 0);
            this.AddImage(0, 0, 10440);

            this.AddPage(1);

            for (int i = 0; i < conversations.Count; ++i)
            {
                QuestConversation conv = (QuestConversation)conversations[conversations.Count - 1 - i];

                if (i > 0)
                {
                    this.AddButton(65, 366, 9909, 9911, 0, GumpButtonType.Page, 1 + i);
                    this.AddHtmlLocalized(90, 367, 50, 20, 1043354, Black, false, false); // Previous

                    this.AddPage(1 + i);
                }

                this.AddHtmlObject(70, 110, 365, 220, conv.Message, LightGreen, false, true);

                if (i > 0)
                {
                    this.AddButton(420, 366, 9903, 9905, 0, GumpButtonType.Page, i);
                    this.AddHtmlLocalized(370, 367, 50, 20, 1043353, Black, false, false); // Next
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            for (int i = this.m_Conversations.Count - 1; i >= 0; --i)
            {
                QuestConversation qc = (QuestConversation)this.m_Conversations[i];

                if (!qc.HasBeenRead)
                {
                    qc.HasBeenRead = true;
                    qc.OnRead();
                }
            }
        }
    }
}