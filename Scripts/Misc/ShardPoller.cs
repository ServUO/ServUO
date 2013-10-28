using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Server.Gumps;
using Server.Network;
using Server.Prompts;

namespace Server.Misc
{
    public class ShardPoller : Item
    {
        private static readonly List<ShardPoller> m_ActivePollers = new List<ShardPoller>();
        private string m_Title;
        private ShardPollOption[] m_Options;
        private IPAddress[] m_Addresses;
        private TimeSpan m_Duration;
        private DateTime m_StartTime;
        private bool m_Active;
        [Constructable(AccessLevel.Administrator)]
        public ShardPoller()
            : base(0x1047)
        {
            this.m_Duration = TimeSpan.FromHours(24.0);
            this.m_Options = new ShardPollOption[0];
            this.m_Addresses = new IPAddress[0];

            this.Movable = false;
        }

        public ShardPoller(Serial serial)
            : base(serial)
        {
        }

        public ShardPollOption[] Options
        {
            get
            {
                return this.m_Options;
            }
            set
            {
                this.m_Options = value;
            }
        }
        public IPAddress[] Addresses
        {
            get
            {
                return this.m_Addresses;
            }
            set
            {
                this.m_Addresses = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public string Title
        {
            get
            {
                return this.m_Title;
            }
            set
            {
                this.m_Title = ShardPollPrompt.UrlToHref(value);
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public TimeSpan Duration
        {
            get
            {
                return this.m_Duration;
            }
            set
            {
                this.m_Duration = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public DateTime StartTime
        {
            get
            {
                return this.m_StartTime;
            }
            set
            {
                this.m_StartTime = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public TimeSpan TimeRemaining
        {
            get
            {
                if (this.m_StartTime == DateTime.MinValue || !this.m_Active)
                    return TimeSpan.Zero;

                try
                {
                    TimeSpan ts = (this.m_StartTime + this.m_Duration) - DateTime.UtcNow;

                    if (ts < TimeSpan.Zero)
                        return TimeSpan.Zero;

                    return ts;
                }
                catch
                {
                    return TimeSpan.Zero;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public bool Active
        {
            get
            {
                return this.m_Active;
            }
            set
            {
                if (this.m_Active == value)
                    return;

                this.m_Active = value;

                if (this.m_Active)
                {
                    this.m_StartTime = DateTime.UtcNow;
                    m_ActivePollers.Add(this);
                }
                else
                {
                    m_ActivePollers.Remove(this);
                }
            }
        }
        public override string DefaultName
        {
            get
            {
                return "shard poller";
            }
        }
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        public bool HasAlreadyVoted(NetState ns)
        {
            for (int i = 0; i < this.m_Options.Length; ++i)
            {
                if (this.m_Options[i].HasAlreadyVoted(ns))
                    return true;
            }

            return false;
        }

        public void AddVote(NetState ns, ShardPollOption option)
        {
            option.AddVote(ns);
        }

        public void RemoveOption(ShardPollOption option)
        {
            int index = Array.IndexOf(this.m_Options, option);

            if (index < 0)
                return;

            ShardPollOption[] old = this.m_Options;
            this.m_Options = new ShardPollOption[old.Length - 1];

            for (int i = 0; i < index; ++i)
                this.m_Options[i] = old[i];

            for (int i = index; i < this.m_Options.Length; ++i)
                this.m_Options[i] = old[i + 1];
        }

        public void AddOption(ShardPollOption option)
        {
            ShardPollOption[] old = this.m_Options;
            this.m_Options = new ShardPollOption[old.Length + 1];

            for (int i = 0; i < old.Length; ++i)
                this.m_Options[i] = old[i];

            this.m_Options[old.Length] = option;
        }

        public void SendQueuedPoll_Callback(object state)
        {
            object[] states = (object[])state;
            Mobile from = (Mobile)states[0];
            Queue<ShardPoller> queue = (Queue<ShardPoller>)states[1];

            from.SendGump(new ShardPollGump(from, this, false, queue));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.Administrator)
                from.SendGump(new ShardPollGump(from, this, true, null));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Title);
            writer.Write(this.m_Duration);
            writer.Write(this.m_StartTime);
            writer.Write(this.m_Active);

            writer.Write(this.m_Options.Length);

            for (int i = 0; i < this.m_Options.Length; ++i)
                this.m_Options[i].Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Title = reader.ReadString();
                        this.m_Duration = reader.ReadTimeSpan();
                        this.m_StartTime = reader.ReadDateTime();
                        this.m_Active = reader.ReadBool();

                        this.m_Options = new ShardPollOption[reader.ReadInt()];

                        for (int i = 0; i < this.m_Options.Length; ++i)
                            this.m_Options[i] = new ShardPollOption(reader);

                        if (this.m_Active)
                            m_ActivePollers.Add(this);

                        break;
                    }
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            this.Active = false;
        }

        private static void EventSink_Login(LoginEventArgs e)
        {
            if (m_ActivePollers.Count == 0)
                return;

            Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(EventSink_Login_Callback), e.Mobile);
        }

        private static void EventSink_Login_Callback(object state)
        {
            Mobile from = (Mobile)state;
            NetState ns = from.NetState;

            if (ns == null)
                return;

            ShardPollGump spg = null;

            for (int i = 0; i < m_ActivePollers.Count; ++i)
            {
                ShardPoller poller = m_ActivePollers[i];

                if (poller.Deleted || !poller.Active)
                    continue;

                if (poller.TimeRemaining > TimeSpan.Zero)
                {
                    if (poller.HasAlreadyVoted(ns))
                        continue;

                    if (spg == null)
                    {
                        spg = new ShardPollGump(from, poller, false, null);
                        from.SendGump(spg);
                    }
                    else
                    {
                        spg.QueuePoll(poller);
                    }
                }
                else
                {
                    poller.Active = false;
                }
            }
        }
    }

    public class ShardPollOption
    {
        private string m_Title;
        private int m_LineBreaks;
        private IPAddress[] m_Voters;
        public ShardPollOption(string title)
        {
            this.m_Title = title;
            this.m_LineBreaks = this.GetBreaks(this.m_Title);
            this.m_Voters = new IPAddress[0];
        }

        public ShardPollOption(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Title = reader.ReadString();
                        this.m_LineBreaks = this.GetBreaks(this.m_Title);

                        this.m_Voters = new IPAddress[reader.ReadInt()];

                        for (int i = 0; i < this.m_Voters.Length; ++i)
                            this.m_Voters[i] = Utility.Intern(reader.ReadIPAddress());

                        break;
                    }
            }
        }

        public string Title
        {
            get
            {
                return this.m_Title;
            }
            set
            {
                this.m_Title = value;
                this.m_LineBreaks = this.GetBreaks(this.m_Title);
            }
        }
        public int LineBreaks
        {
            get
            {
                return this.m_LineBreaks;
            }
        }
        public int Votes
        {
            get
            {
                return this.m_Voters.Length;
            }
        }
        public IPAddress[] Voters
        {
            get
            {
                return this.m_Voters;
            }
            set
            {
                this.m_Voters = value;
            }
        }
        public bool HasAlreadyVoted(NetState ns)
        {
            if (ns == null)
                return false;

            IPAddress ipAddress = ns.Address;

            for (int i = 0; i < this.m_Voters.Length; ++i)
            {
                if (Utility.IPMatchClassC(this.m_Voters[i], ipAddress))
                    return true;
            }

            return false;
        }

        public void AddVote(NetState ns)
        {
            if (ns == null)
                return;

            IPAddress[] old = this.m_Voters;
            this.m_Voters = new IPAddress[old.Length + 1];

            for (int i = 0; i < old.Length; ++i)
                this.m_Voters[i] = old[i];

            this.m_Voters[old.Length] = ns.Address;
        }

        public int ComputeHeight()
        {
            int height = this.m_LineBreaks * 18;

            if (height > 30)
                return height;

            return 30;
        }

        public int GetBreaks(string title)
        {
            if (title == null)
                return 1;

            int count = 0;
            int index = -1;

            do
            {
                ++count;
                index = title.IndexOf("<br>", index + 1);
            }
            while (index >= 0);

            return count;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); // version

            writer.Write(this.m_Title);

            writer.Write(this.m_Voters.Length);

            for (int i = 0; i < this.m_Voters.Length; ++i)
                writer.Write(this.m_Voters[i]);
        }
    }

    public class ShardPollGump : Gump
    {
        private const int LabelColor32 = 0xFFFFFF;
        private readonly Mobile m_From;
        private readonly ShardPoller m_Poller;
        private readonly bool m_Editing;
        private Queue<ShardPoller> m_Polls;
        public ShardPollGump(Mobile from, ShardPoller poller, bool editing, Queue<ShardPoller> polls)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Poller = poller;
            this.m_Editing = editing;
            this.m_Polls = polls;

            this.Closable = false;

            this.AddPage(0);

            int totalVotes = 0;
            int totalOptionHeight = 0;

            for (int i = 0; i < poller.Options.Length; ++i)
            {
                totalVotes += poller.Options[i].Votes;
                totalOptionHeight += poller.Options[i].ComputeHeight() + 5;
            }

            bool isViewingResults = editing && poller.Active;
            bool isCompleted = totalVotes > 0 && !poller.Active;

            if (editing && !isViewingResults)
                totalOptionHeight += 35;

            int height = 115 + totalOptionHeight;

            this.AddBackground(1, 1, 398, height - 2, 3600);
            this.AddAlphaRegion(16, 15, 369, height - 31);

            this.AddItem(308, 30, 0x1E5E);

            string title;

            if (editing)
                title = (isCompleted ? "Poll Completed" : "Poll Editor");
            else
                title = "Shard Poll";

            this.AddHtml(22, 22, 294, 20, this.Color(this.Center(title), LabelColor32), false, false);

            if (editing)
            {
                this.AddHtml(22, 22, 294, 20, this.Color(String.Format("{0} total", totalVotes), LabelColor32), false, false);
                this.AddButton(287, 23, 0x2622, 0x2623, 2, GumpButtonType.Reply, 0);
            }

            this.AddHtml(22, 50, 294, 40, this.Color(poller.Title, 0x99CC66), false, false);

            this.AddImageTiled(32, 88, 264, 1, 9107);
            this.AddImageTiled(42, 90, 264, 1, 9157);

            int y = 100;

            for (int i = 0; i < poller.Options.Length; ++i)
            {
                ShardPollOption option = poller.Options[i];
                string text = option.Title;

                if (editing && totalVotes > 0)
                {
                    double perc = option.Votes / (double)totalVotes;

                    text = String.Format("[{1}: {2}%] {0}", text, option.Votes, (int)(perc * 100));
                }

                int optHeight = option.ComputeHeight();

                y += optHeight / 2;

                if (isViewingResults)
                    this.AddImage(24, y - 15, 0x25FE);
                else
                    this.AddRadio(24, y - 15, 0x25F9, 0x25FC, false, 1 + i);

                this.AddHtml(60, y - (9 * option.LineBreaks), 250, 18 * option.LineBreaks, this.Color(text, LabelColor32), false, false);

                y += optHeight / 2;
                y += 5;
            }

            if (editing && !isViewingResults)
            {
                this.AddRadio(24, y + 15 - 15, 0x25F9, 0x25FC, false, 1 + poller.Options.Length);
                this.AddHtml(60, y + 15 - 9, 250, 18, this.Color("Create new option.", 0x99CC66), false, false);
            }

            this.AddButton(314, height - 73, 247, 248, 1, GumpButtonType.Reply, 0);
            this.AddButton(314, height - 47, 242, 241, 0, GumpButtonType.Reply, 0);
        }

        public bool Editing
        {
            get
            {
                return this.m_Editing;
            }
        }
        public void QueuePoll(ShardPoller poller)
        {
            if (this.m_Polls == null)
                this.m_Polls = new Queue<ShardPoller>(4);

            this.m_Polls.Enqueue(poller);
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Polls != null && this.m_Polls.Count > 0)
            {
                ShardPoller poller = this.m_Polls.Dequeue();

                if (poller != null)
                    Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(poller.SendQueuedPoll_Callback), new object[] { this.m_From, this.m_Polls });
            }

            if (info.ButtonID == 1)
            {
                int[] switches = info.Switches;

                if (switches.Length == 0)
                    return;

                int switched = switches[0] - 1;
                ShardPollOption opt = null;

                if (switched >= 0 && switched < this.m_Poller.Options.Length)
                    opt = this.m_Poller.Options[switched];

                if (opt == null && !this.m_Editing)
                    return;

                if (this.m_Editing)
                {
                    if (!this.m_Poller.Active)
                    {
                        this.m_From.SendMessage("Enter a title for the option. Escape to cancel.{0}", opt == null ? "" : " Use \"DEL\" to delete.");
                        this.m_From.Prompt = new ShardPollPrompt(this.m_Poller, opt);
                    }
                    else
                    {
                        this.m_From.SendMessage("You may not edit an active poll. Deactivate it first.");
                        this.m_From.SendGump(new ShardPollGump(this.m_From, this.m_Poller, this.m_Editing, this.m_Polls));
                    }
                }
                else
                {
                    if (!this.m_Poller.Active)
                        this.m_From.SendMessage("The poll has been deactivated.");
                    else if (this.m_Poller.HasAlreadyVoted(sender))
                        this.m_From.SendMessage("You have already voted on this poll.");
                    else
                        this.m_Poller.AddVote(sender, opt);
                }
            }
            else if (info.ButtonID == 2 && this.m_Editing)
            {
                this.m_From.SendGump(new ShardPollGump(this.m_From, this.m_Poller, this.m_Editing, this.m_Polls));
                this.m_From.SendGump(new PropertiesGump(this.m_From, this.m_Poller));
            }
        }
    }

    public class ShardPollPrompt : Prompt
    {
        private static readonly Regex m_UrlRegex = new Regex(@"\[url(?:=(.*?))?\](.*?)\[/url\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly ShardPoller m_Poller;
        private readonly ShardPollOption m_Option;
        public ShardPollPrompt(ShardPoller poller, ShardPollOption opt)
        {
            this.m_Poller = poller;
            this.m_Option = opt;
        }

        public static string UrlToHref(string text)
        {
            if (text == null)
                return null;

            return m_UrlRegex.Replace(text, new MatchEvaluator(UrlRegex_Match));
        }

        public override void OnCancel(Mobile from)
        {
            from.SendGump(new ShardPollGump(from, this.m_Poller, true, null));
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (this.m_Poller.Active)
            {
                from.SendMessage("You may not edit an active poll. Deactivate it first.");
            }
            else if (text == "DEL")
            {
                if (this.m_Option != null)
                    this.m_Poller.RemoveOption(this.m_Option);
            }
            else
            {
                text = UrlToHref(text);

                if (this.m_Option == null)
                    this.m_Poller.AddOption(new ShardPollOption(text));
                else
                    this.m_Option.Title = text;
            }

            from.SendGump(new ShardPollGump(from, this.m_Poller, true, null));
        }

        private static string UrlRegex_Match(Match m)
        {
            if (m.Groups[1].Success)
            {
                if (m.Groups[2].Success)
                    return String.Format("<a href=\"{0}\">{1}</a>", m.Groups[1].Value, m.Groups[2].Value);
            }
            else if (m.Groups[2].Success)
            {
                return String.Format("<a href=\"{0}\">{0}</a>", m.Groups[2].Value);
            }

            return m.Value;
        }
    }
}