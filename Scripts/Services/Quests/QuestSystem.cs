using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public delegate void QuestCallback();

    public abstract class QuestSystem
    {
        public static void Configure()
        {
            EventSink.OnKilledBy += OnKilledBy;
            EventSink.Login += OnLogin;
        }

        private PlayerMobile m_From;
        private ArrayList m_Objectives;
        private ArrayList m_Conversations;

        public QuestSystem(PlayerMobile from)
        {
            m_From = from;
            m_Objectives = new ArrayList();
            m_Conversations = new ArrayList();
        }

        public QuestSystem()
        {
        }

        public abstract object Name { get; }
        public abstract object OfferMessage { get; }
        public abstract int Picture { get; }
        public abstract bool IsTutorial { get; }
        public abstract TimeSpan RestartDelay { get; }

        public PlayerMobile From
        {
            get
            {
                return m_From;
            }
            set
            {
                m_From = value;
            }
        }
        public ArrayList Objectives
        {
            get
            {
                return m_Objectives;
            }
            set
            {
                m_Objectives = value;
            }
        }
        public ArrayList Conversations
        {
            get
            {
                return m_Conversations;
            }
            set
            {
                m_Conversations = value;
            }
        }

        private static readonly string _TimerID = "QuestTimer";

        public static bool CanOfferQuest(Mobile check, Type questType)
        {
            bool inRestartPeriod;

            return CanOfferQuest(check, questType, out inRestartPeriod);
        }

        public static bool CanOfferQuest(Mobile check, Type questType, out bool inRestartPeriod)
        {
            inRestartPeriod = false;

            PlayerMobile pm = check as PlayerMobile;

            if (pm == null)
                return false;

            if (pm.HasGump(typeof(QuestOfferGump)))
                return false;

            List<QuestRestartInfo> doneQuests = pm.DoneQuests;

            if (doneQuests != null)
            {
                for (int i = 0; i < doneQuests.Count; ++i)
                {
                    QuestRestartInfo restartInfo = doneQuests[i];

                    if (restartInfo.QuestType == questType)
                    {
                        DateTime endTime = restartInfo.RestartTime;

                        if (DateTime.UtcNow < endTime)
                        {
                            inRestartPeriod = true;
                            return false;
                        }

                        doneQuests.RemoveAt(i--);
                        return true;
                    }
                }
            }

            return true;
        }

        public static void FocusTo(Mobile who, Mobile to)
        {
            if (Utility.RandomBool())
            {
                who.Animate(17, 7, 1, true, false, 0);
            }
            else
            {
                switch (Utility.Random(3))
                {
                    case 0:
                        who.Animate(32, 7, 1, true, false, 0);
                        break;
                    case 1:
                        who.Animate(33, 7, 1, true, false, 0);
                        break;
                    case 2:
                        who.Animate(34, 7, 1, true, false, 0);
                        break;
                }
            }

            who.Direction = who.GetDirectionTo(to);
        }

        public static int RandomBrightHue()
        {
            if (0.1 > Utility.RandomDouble())
                return Utility.RandomList(0x62, 0x71);

            return Utility.RandomList(0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59);
        }

        public virtual void StartTimer()
        {
            TimerRegistry.Register<QuestSystem>(_TimerID, this, TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), false, q => q.Slice());
        }

        public virtual void StopTimer()
        {
            TimerRegistry.RemoveFromRegistry<QuestSystem>(_TimerID, this);
        }

        public virtual void Slice()
        {
            for (int i = m_Objectives.Count - 1; i >= 0; --i)
            {
                QuestObjective obj = (QuestObjective)m_Objectives[i];

                if (obj.GetTimerEvent())
                    obj.CheckProgress();
            }
        }

        public static void OnKilledBy(OnKilledByEventArgs e)
        {
            if (e.KilledBy is PlayerMobile pm && e.Killed is BaseCreature bc)
            {
                QuestSystem qs = pm.Quest;

                if (qs != null)
                {
                    qs.OnKill(bc, bc.Corpse);
                }
            }
        }

        public static void OnLogin(LoginEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
            {
                if (pm.Quest != null)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
                    {
                        pm.Quest.ShowQuestLog();
                    });
                }
            }
        }

        public virtual void OnKill(BaseCreature creature, Container corpse)
        {
            for (int i = m_Objectives.Count - 1; i >= 0; --i)
            {
                QuestObjective obj = (QuestObjective)m_Objectives[i];

                if (obj.GetKillEvent(creature, corpse))
                    obj.OnKill(creature, corpse);
            }
        }

        public virtual bool IgnoreYoungProtection(Mobile from)
        {
            for (int i = m_Objectives.Count - 1; i >= 0; --i)
            {
                QuestObjective obj = (QuestObjective)m_Objectives[i];

                if (obj.IgnoreYoungProtection(from))
                    return true;
            }

            return false;
        }

        public virtual void BaseDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    {
                        int count = reader.ReadEncodedInt();

                        m_Objectives = new ArrayList(count);

                        for (int i = 0; i < count; ++i)
                        {
                            QuestObjective obj = QuestSerializer.DeserializeObjective(this, reader);

                            if (obj != null)
                            {
                                obj.System = this;
                                m_Objectives.Add(obj);
                            }
                        }

                        count = reader.ReadEncodedInt();

                        m_Conversations = new ArrayList(count);

                        for (int i = 0; i < count; ++i)
                        {
                            QuestConversation conv = QuestSerializer.DeserializeConversation(this, reader);

                            if (conv != null)
                            {
                                conv.System = this;
                                m_Conversations.Add(conv);
                            }
                        }

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

            writer.WriteEncodedInt(m_Objectives.Count);

            for (int i = 0; i < m_Objectives.Count; ++i)
                QuestSerializer.Serialize(this, (QuestObjective)m_Objectives[i], writer);

            writer.WriteEncodedInt(m_Conversations.Count);

            for (int i = 0; i < m_Conversations.Count; ++i)
                QuestSerializer.Serialize(this, (QuestConversation)m_Conversations[i], writer);

            ChildSerialize(writer);
        }

        public virtual void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version
        }

        public bool IsObjectiveInProgress(Type type)
        {
            QuestObjective obj = FindObjective(type);

            return (obj != null && !obj.Completed);
        }

        public QuestObjective FindObjective(Type type)
        {
            for (int i = m_Objectives.Count - 1; i >= 0; --i)
            {
                QuestObjective obj = (QuestObjective)m_Objectives[i];

                if (obj.GetType() == type)
                    return obj;
            }

            return null;
        }

        public virtual void SendOffer()
        {
            m_From.SendGump(new QuestOfferGump(this));
        }

        public virtual void GetContextMenuEntries(List<ContextMenuEntry> list)
        {
            if (m_Objectives.Count > 0)
                list.Add(new QuestCallbackEntry(6154, ShowQuestLog)); // View Quest Log

            if (m_Conversations.Count > 0)
                list.Add(new QuestCallbackEntry(6156, ShowQuestConversation)); // Quest Conversation

            list.Add(new QuestCallbackEntry(6155, BeginCancelQuest)); // Cancel Quest
        }

        public virtual void ShowQuestLogUpdated()
        {
            m_From.CloseGump(typeof(QuestLogUpdatedGump));
            m_From.SendGump(new QuestLogUpdatedGump(this));
        }

        public virtual void ShowQuestLog()
        {
            if (m_Objectives.Count > 0)
            {
                m_From.CloseGump(typeof(QuestItemInfoGump));
                m_From.CloseGump(typeof(QuestLogUpdatedGump));
                m_From.CloseGump(typeof(QuestObjectivesGump));
                m_From.CloseGump(typeof(QuestConversationsGump));

                m_From.SendGump(new QuestObjectivesGump(m_Objectives));

                QuestObjective last = (QuestObjective)m_Objectives[m_Objectives.Count - 1];

                if (last.Info != null)
                    m_From.SendGump(new QuestItemInfoGump(last.Info));
            }
        }

        public virtual void ShowQuestConversation()
        {
            if (m_Conversations.Count > 0)
            {
                m_From.CloseGump(typeof(QuestItemInfoGump));
                m_From.CloseGump(typeof(QuestObjectivesGump));
                m_From.CloseGump(typeof(QuestConversationsGump));

                m_From.SendGump(new QuestConversationsGump(m_Conversations));

                QuestConversation last = (QuestConversation)m_Conversations[m_Conversations.Count - 1];

                if (last.Info != null)
                    m_From.SendGump(new QuestItemInfoGump(last.Info));
            }
        }

        public virtual void BeginCancelQuest()
        {
            m_From.SendGump(new QuestCancelGump(this));
        }

        public virtual void EndCancelQuest(bool shouldCancel)
        {
            if (m_From.Quest != this)
                return;

            if (shouldCancel)
            {
                m_From.SendLocalizedMessage(1049015); // You have canceled your quest.
                Cancel();
            }
            else
            {
                m_From.SendLocalizedMessage(1049014); // You have chosen not to cancel your quest.
            }
        }

        public virtual void Cancel()
        {
            ClearQuest(false);
        }

        public virtual void Complete()
        {
            EventSink.InvokeQuestComplete(new QuestCompleteEventArgs(From, GetType()));

            ClearQuest(true);
        }

        public virtual void ClearQuest(bool completed)
        {
            StopTimer();

            if (m_From.Quest == this)
            {
                m_From.Quest = null;

                TimeSpan restartDelay = RestartDelay;

                if ((completed && restartDelay > TimeSpan.Zero) || (!completed && restartDelay == TimeSpan.MaxValue))
                {
                    List<QuestRestartInfo> doneQuests = m_From.DoneQuests;

                    if (doneQuests == null)
                        m_From.DoneQuests = doneQuests = new List<QuestRestartInfo>();

                    bool found = false;

                    Type ourQuestType = GetType();

                    for (int i = 0; i < doneQuests.Count; ++i)
                    {
                        QuestRestartInfo restartInfo = doneQuests[i];

                        if (restartInfo.QuestType == ourQuestType)
                        {
                            restartInfo.Reset(restartDelay);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        doneQuests.Add(new QuestRestartInfo(ourQuestType, restartDelay));
                }
            }
        }

        public virtual void AddConversation(QuestConversation conv)
        {
            conv.System = this;

            if (conv.Logged)
                m_Conversations.Add(conv);

            m_From.CloseGump(typeof(QuestItemInfoGump));
            m_From.CloseGump(typeof(QuestObjectivesGump));
            m_From.CloseGump(typeof(QuestConversationsGump));

            if (conv.Logged)
                m_From.SendGump(new QuestConversationsGump(m_Conversations));
            else
                m_From.SendGump(new QuestConversationsGump(conv));

            if (conv.Info != null)
                m_From.SendGump(new QuestItemInfoGump(conv.Info));
        }

        public virtual void AddObjective(QuestObjective obj)
        {
            obj.System = this;
            m_Objectives.Add(obj);

            ShowQuestLogUpdated();
        }

        public virtual void Accept()
        {
            if (m_From.Quest != null)
                return;

            m_From.Quest = this;
            m_From.SendLocalizedMessage(1049019); // You have accepted the Quest.

            StartTimer();
        }

        public virtual void Decline()
        {
            m_From.SendLocalizedMessage(1049018); // You have declined the Quest.
        }
    }

    public class QuestCancelGump : BaseQuestGump
    {
        private readonly QuestSystem m_System;
        public QuestCancelGump(QuestSystem system)
            : base(120, 50)
        {
            m_System = system;

            Closable = false;

            AddPage(0);

            AddImageTiled(0, 0, 348, 262, 2702);
            AddAlphaRegion(0, 0, 348, 262);

            AddImage(0, 15, 10152);
            AddImageTiled(0, 30, 17, 200, 10151);
            AddImage(0, 230, 10154);

            AddImage(15, 0, 10252);
            AddImageTiled(30, 0, 300, 17, 10250);
            AddImage(315, 0, 10254);

            AddImage(15, 244, 10252);
            AddImageTiled(30, 244, 300, 17, 10250);
            AddImage(315, 244, 10254);

            AddImage(330, 15, 10152);
            AddImageTiled(330, 30, 17, 200, 10151);
            AddImage(330, 230, 10154);

            AddImage(333, 2, 10006);
            AddImage(333, 248, 10006);
            AddImage(2, 248, 10006);
            AddImage(2, 2, 10006);

            AddHtmlLocalized(25, 22, 200, 20, 1049000, 32000, false, false); // Confirm Quest Cancellation
            AddImage(25, 40, 3007);

            if (system.IsTutorial)
            {
                AddHtmlLocalized(25, 55, 300, 120, 1060836, White, false, false); // This quest will give you valuable information, skills and equipment that will help you advance in the game at a quicker pace.<BR><BR>Are you certain you wish to cancel at this time?
            }
            else
            {
                AddHtmlLocalized(25, 60, 300, 20, 1049001, White, false, false); // You have chosen to abort your quest:
                AddImage(25, 81, 0x25E7);
                AddHtmlObject(48, 80, 280, 20, system.Name, DarkGreen, false, false);

                AddHtmlLocalized(25, 120, 280, 20, 1049002, White, false, false); // Can this quest be restarted after quitting?
                AddImage(25, 141, 0x25E7);
                AddHtmlLocalized(48, 140, 280, 20, (system.RestartDelay < TimeSpan.MaxValue) ? 1049016 : 1049017, DarkGreen, false, false); // Yes/No
            }

            AddRadio(25, 175, 9720, 9723, true, 1);
            AddHtmlLocalized(60, 180, 280, 20, 1049005, White, false, false); // Yes, I really want to quit!

            AddRadio(25, 210, 9720, 9723, false, 0);
            AddHtmlLocalized(60, 215, 280, 20, 1049006, White, false, false); // No, I don't want to quit.

            AddButton(265, 220, 247, 248, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
                m_System.EndCancelQuest(info.IsSwitched(1));
        }
    }

    public class QuestOfferGump : BaseQuestGump
    {
        private readonly QuestSystem m_System;
        public QuestOfferGump(QuestSystem system)
            : base(75, 25)
        {
            m_System = system;

            Closable = false;

            AddPage(0);

            AddImageTiled(50, 20, 400, 400, 2624);
            AddAlphaRegion(50, 20, 400, 400);

            AddImage(90, 33, 9005);
            AddHtmlLocalized(130, 45, 270, 20, 1049010, White, false, false); // Quest Offer
            AddImageTiled(130, 65, 175, 1, 9101);

            AddImage(140, 110, 1209);
            AddHtmlObject(160, 108, 250, 20, system.Name, DarkGreen, false, false);

            AddHtmlObject(98, 140, 312, 200, system.OfferMessage, LightGreen, false, true);

            AddRadio(85, 350, 9720, 9723, true, 1);
            AddHtmlLocalized(120, 356, 280, 20, 1049011, White, false, false); // I accept!

            AddRadio(85, 385, 9720, 9723, false, 0);
            AddHtmlLocalized(120, 391, 280, 20, 1049012, White, false, false); // No thanks, I decline.

            AddButton(340, 390, 247, 248, 1, GumpButtonType.Reply, 0);

            AddImageTiled(50, 29, 30, 390, 10460);
            AddImageTiled(34, 140, 17, 279, 9263);

            AddImage(48, 135, 10411);
            AddImage(-16, 285, 10402);
            AddImage(0, 10, 10421);
            AddImage(25, 0, 10420);

            AddImageTiled(83, 15, 350, 15, 10250);

            AddImage(34, 419, 10306);
            AddImage(442, 419, 10304);
            AddImageTiled(51, 419, 392, 17, 10101);

            AddImageTiled(415, 29, 44, 390, 2605);
            AddImageTiled(415, 29, 30, 390, 10460);
            AddImage(425, 0, 10441);

            AddImage(370, 50, 1417);
            AddImage(379, 60, system.Picture);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (info.IsSwitched(1))
                    m_System.Accept();
                else
                    m_System.Decline();
            }
        }
    }

    public abstract class BaseQuestGump : Gump
    {
        public const int Black = 0x0000;
        public const int White = 0x7FFF;
        public const int DarkGreen = 10000;
        public const int LightGreen = 90000;
        public const int Blue = 19777215;
        public BaseQuestGump(int x, int y)
            : base(x, y)
        {
        }

        public static int C16232(int c16)
        {
            c16 &= 0x7FFF;

            int r = (((c16 >> 10) & 0x1F) << 3);
            int g = (((c16 >> 05) & 0x1F) << 3);
            int b = (((c16 >> 00) & 0x1F) << 3);

            return (r << 16) | (g << 8) | (b << 0);
        }

        public static int C16216(int c16)
        {
            return c16 & 0x7FFF;
        }

        public static int C32216(int c32)
        {
            c32 &= 0xFFFFFF;

            int r = (((c32 >> 16) & 0xFF) >> 3);
            int g = (((c32 >> 08) & 0xFF) >> 3);
            int b = (((c32 >> 00) & 0xFF) >> 3);

            return (r << 10) | (g << 5) | (b << 0);
        }

        public static string Color(string text, int color)
        {
            return string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public static ArrayList BuildList(object obj)
        {
            ArrayList list = new ArrayList();

            list.Add(obj);

            return list;
        }

        public void AddHtmlObject(int x, int y, int width, int height, object message, int color, bool back, bool scroll)
        {
            if (message is string)
            {
                string html = (string)message;

                AddHtml(x, y, width, height, Color(html, C16232(color)), back, scroll);
            }
            else if (message is int)
            {
                int html = (int)message;

                AddHtmlLocalized(x, y, width, height, html, C16216(color), back, scroll);
            }
        }
    }
}
