using Server.Gumps;
using Server.Network;
using System.Collections;

namespace Server.Engines.Quests
{
    public abstract class QuestConversation
    {
        private QuestSystem m_System;
        private bool m_HasBeenRead;

        public abstract object Message { get; }
        public virtual QuestItemInfo[] Info => null;
        public virtual bool Logged => true;

        public QuestSystem System
        {
            get { return m_System; }
            set { m_System = value; }
        }
        public bool HasBeenRead
        {
            get { return m_HasBeenRead; }
            set { m_HasBeenRead = value; }
        }

        public virtual void BaseDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    {
                        m_HasBeenRead = reader.ReadBool();

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
            writer.WriteEncodedInt(0); // version

            writer.Write(m_HasBeenRead);

            ChildSerialize(writer);
        }

        public virtual void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version
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
            m_Conversations = conversations;

            Closable = false;

            AddPage(0);

            AddImage(349, 10, 9392);
            AddImageTiled(149, 10, 200, 260, 9394);
            AddImageTiled(349, 130, 114, 120, 9395);
            AddImageTiled(149, 10, 200, 140, 9391);
            AddImageTiled(149, 250, 200, 140, 9397);
            AddImage(349, 250, 9398);
            AddImage(35, 10, 9390);
            AddImageTiled(35, 130, 114, 120, 9393);
            AddImage(35, 250, 9396);

            AddHtmlLocalized(110, 60, 200, 20, 1049069, White, false, false); // <STRONG>Conversation Event</STRONG>

            AddImage(65, 14, 10102);
            AddImageTiled(81, 14, 349, 17, 10101);
            AddImage(426, 14, 10104);

            AddImageTiled(55, 40, 388, 323, 2624);
            AddAlphaRegion(55, 40, 388, 323);

            AddImageTiled(75, 90, 200, 1, 9101);
            AddImage(75, 58, 9781);
            AddImage(380, 45, 223);

            AddButton(220, 335, 2313, 2312, 1, GumpButtonType.Reply, 0);
            AddImage(0, 0, 10440);

            AddPage(1);

            for (int i = 0; i < conversations.Count; ++i)
            {
                QuestConversation conv = (QuestConversation)conversations[conversations.Count - 1 - i];

                if (i > 0)
                {
                    AddButton(65, 366, 9909, 9911, 0, GumpButtonType.Page, 1 + i);
                    AddHtmlLocalized(90, 367, 50, 20, 1043354, Black, false, false); // Previous

                    AddPage(1 + i);
                }

                AddHtmlObject(70, 110, 365, 220, conv.Message, LightGreen, false, true);

                if (i > 0)
                {
                    AddButton(420, 366, 9903, 9905, 0, GumpButtonType.Page, i);
                    AddHtmlLocalized(370, 367, 50, 20, 1043353, Black, false, false); // Next
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            for (int i = m_Conversations.Count - 1; i >= 0; --i)
            {
                QuestConversation qc = (QuestConversation)m_Conversations[i];

                if (!qc.HasBeenRead)
                {
                    qc.HasBeenRead = true;
                    qc.OnRead();
                }
            }
        }
    }
}
