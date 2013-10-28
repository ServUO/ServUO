using System;
using System.Collections;
using Server.Commands;
using Server.Gumps;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public class XmlQuestPoints : XmlAttachment
    {
        public string guildFilter;
        public string nameFilter;
        private int m_Points;
        private int m_Completed;
        private int m_Credits;
        private ArrayList m_QuestList = new ArrayList();
        private DateTime m_WhenRanked;
        private int m_Rank;
        private int m_DeltaRank;
        public XmlQuestPoints(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlQuestPoints()
        {
        }

        public ArrayList QuestList
        {
            get
            {
                return this.m_QuestList;
            }
            set
            {
                this.m_QuestList = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Rank
        {
            get
            {
                return this.m_Rank;
            }
            set
            {
                this.m_Rank = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int DeltaRank
        {
            get
            {
                return this.m_DeltaRank;
            }
            set
            {
                this.m_DeltaRank = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime WhenRanked
        {
            get
            {
                return this.m_WhenRanked;
            }
            set
            {
                this.m_WhenRanked = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Points
        {
            get
            {
                return this.m_Points;
            }
            set
            {
                this.m_Points = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Credits
        {
            get
            {
                return this.m_Credits;
            }
            set
            {
                this.m_Credits = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int QuestsCompleted
        {
            get
            {
                return this.m_Completed;
            }
            set
            {
                this.m_Completed = value;
            }
        }
        public static new void Initialize()
        {
            CommandSystem.Register("QuestPoints", AccessLevel.Player, new CommandEventHandler(CheckQuestPoints_OnCommand));

            CommandSystem.Register("QuestLog", AccessLevel.Player, new CommandEventHandler(QuestLog_OnCommand));
        }

        [Usage("QuestPoints")]
        [Description("Displays the players quest points and ranking")]
        public static void CheckQuestPoints_OnCommand(CommandEventArgs e)
        {
            if (e == null || e.Mobile == null)
                return;

            string msg = null;

            XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(e.Mobile, typeof(XmlQuestPoints));
            if (p != null)
            {
                msg = p.OnIdentify(e.Mobile);
            }

            if (msg != null)
                e.Mobile.SendMessage(msg);
        }

        [Usage("QuestLog")]
        [Description("Displays players quest history")]
        public static void QuestLog_OnCommand(CommandEventArgs e)
        {
            if (e == null || e.Mobile == null)
                return;

            e.Mobile.CloseGump(typeof(QuestLogGump));
            e.Mobile.SendGump(new QuestLogGump(e.Mobile));
        }

        public static void GiveQuestPoints(Mobile from, IXmlQuest quest)
        {
            if (from == null || quest == null)
                return;

            // find the XmlQuestPoints attachment

            XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(from, typeof(XmlQuestPoints));

            // if doesnt have one yet, then add it
            if (p == null)
            {
                p = new XmlQuestPoints();
                XmlAttach.AttachTo(from, p);
            }

            // if you wanted to scale the points given based on party size, karma, fame, etc.
            // this would be the place to do it
            int points = quest.Difficulty;

            // update the questpoints attachment information
            p.Points += points;
            p.Credits += points;
            p.QuestsCompleted++;

            if (from != null)
            {
                from.SendMessage("You have received {0} quest points!", points);
            }

            // add the completed quest to the quest list
            QuestEntry.AddQuestEntry(from, quest);

            // update the overall ranking list
            XmlQuestLeaders.UpdateQuestRanking(from, p);
        }

        public static int GetCredits(Mobile m)
        {
            int val = 0;

            XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(m, typeof(XmlQuestPoints));
            if (p != null)
            {
                val = p.Credits;
            }
            
            return val;
        }

        public static int GetPoints(Mobile m)
        {
            int val = 0;

            XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(m, typeof(XmlQuestPoints));
            if (p != null)
            {
                val = p.Points;
            }
            
            return val;
        }

        public static bool HasCredits(Mobile m, int credits)
        {
            if (m == null || m.Deleted)
                return false;

            XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(m, typeof(XmlQuestPoints));

            if (p != null)
            {
                if (p.Credits >= credits)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TakeCredits(Mobile m, int credits)
        {
            if (m == null || m.Deleted)
                return false;

            XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(m, typeof(XmlQuestPoints));

            if (p != null)
            {
                if (p.Credits >= credits)
                {
                    p.Credits -= credits;
                    return true;
                }
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_Points);
            writer.Write(this.m_Credits);
            writer.Write(this.m_Completed);
            writer.Write(this.m_Rank);
            writer.Write(this.m_DeltaRank);
            writer.Write(this.m_WhenRanked);

            // save the quest history
            if (this.QuestList != null)
            {
                writer.Write((int)this.QuestList.Count);

                foreach (QuestEntry e in this.QuestList)
                {
                    e.Serialize(writer);
                }
            }
            else
            {
                writer.Write((int)0);
            }

            // need this in order to rebuild the rankings on deser
            if (this.AttachedTo is Mobile)
                writer.Write(this.AttachedTo as Mobile);
            else
                writer.Write((Mobile)null);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch(version)
            {
                case 0:

                    this.m_Points = reader.ReadInt();
                    this.m_Credits = reader.ReadInt();
                    this.m_Completed = reader.ReadInt();
                    this.m_Rank = reader.ReadInt();
                    this.m_DeltaRank = reader.ReadInt();
                    this.m_WhenRanked = reader.ReadDateTime();

                    int nquests = reader.ReadInt();

                    if (nquests > 0)
                    {
                        this.QuestList = new ArrayList(nquests);
                        for (int i = 0; i < nquests; i++)
                        {
                            QuestEntry e = new QuestEntry();
                            e.Deserialize(reader);

                            this.QuestList.Add(e);
                        }
                    }

                    // get the owner of this in order to rebuild the rankings
                    Mobile quester = reader.ReadMobile();

                    // rebuild the ranking list
                    // if they have never made a kill, then dont rank
                    if (quester != null && this.QuestsCompleted > 0)
                    {
                        XmlQuestLeaders.UpdateQuestRanking(quester, this);
                    }
                    break;
            }
        }

        public override string OnIdentify(Mobile from)
        { 
            return String.Format("Quest Points Status:\nTotal Quest Points = {0}\nTotal Quests Completed = {1}\nQuest Credits Available = {2}", this.Points, this.QuestsCompleted, this.Credits);
        }

        public class QuestEntry
        {
            public Mobile Quester;
            public string Name;
            public DateTime WhenCompleted;
            public DateTime WhenStarted;
            public int Difficulty;
            public bool PartyEnabled;
            public int TimesCompleted = 1;
            public QuestEntry()
            {
            }

            public QuestEntry(Mobile m, IXmlQuest quest)
            {
                this.Quester = m;
                if (quest != null)
                {
                    this.WhenStarted = quest.TimeCreated;
                    this.WhenCompleted = DateTime.UtcNow;
                    this.Difficulty = quest.Difficulty;
                    this.Name = quest.Name;
                }
            }

            public static void AddQuestEntry(Mobile m, IXmlQuest quest)
            {
                if (m == null || quest == null)
                    return;

                // get the XmlQuestPoints attachment from the mobile
                XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(m, typeof(XmlQuestPoints));

                if (p == null)
                    return;

                // look through the list of quests and see if it is one that has already been done
                if (p.QuestList == null)
                    p.QuestList = new ArrayList();

                bool found = false;
                foreach (QuestEntry e in p.QuestList)
                {
                    if (e.Name == quest.Name)
                    {
                        // found a match, so just change the number and dates
                        e.TimesCompleted++;
                        e.WhenStarted = quest.TimeCreated;
                        e.WhenCompleted = DateTime.UtcNow;
                        // and update the difficulty and party status
                        e.Difficulty = quest.Difficulty;
                        e.PartyEnabled = quest.PartyEnabled;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // add a new entry
                    p.QuestList.Add(new QuestEntry(m, quest));
                }
            }

            public virtual void Serialize(GenericWriter writer)
            {
                writer.Write((int)0); // version

                writer.Write(this.Quester);
                writer.Write(this.Name);
                writer.Write(this.WhenCompleted);
                writer.Write(this.WhenStarted);
                writer.Write(this.Difficulty);
                writer.Write(this.TimesCompleted);
                writer.Write(this.PartyEnabled);
            }

            public virtual void Deserialize(GenericReader reader)
            {
                int version = reader.ReadInt();

                switch(version)
                {
                    case 0:
                        this.Quester = reader.ReadMobile();
                        this.Name = reader.ReadString();
                        this.WhenCompleted = reader.ReadDateTime();
                        this.WhenStarted = reader.ReadDateTime();
                        this.Difficulty = reader.ReadInt();
                        this.TimesCompleted = reader.ReadInt();
                        this.PartyEnabled = reader.ReadBool();
                        break;
                }
            }
        }
    }
}