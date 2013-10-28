using System;
using System.Collections.Generic;
using System.Text;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Prompts;

namespace Server.Mobiles
{
    public interface ITownCrierEntryList
    {
        List<TownCrierEntry> Entries { get; }
        TownCrierEntry GetRandomEntry();

        TownCrierEntry AddEntry(string[] lines, TimeSpan duration);

        void RemoveEntry(TownCrierEntry entry);
    }

    public class GlobalTownCrierEntryList : ITownCrierEntryList
    {
        private static GlobalTownCrierEntryList m_Instance;
        private List<TownCrierEntry> m_Entries;
        public GlobalTownCrierEntryList()
        {
        }

        public static GlobalTownCrierEntryList Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new GlobalTownCrierEntryList();

                return m_Instance;
            }
        }
        public bool IsEmpty
        {
            get
            {
                return (this.m_Entries == null || this.m_Entries.Count == 0);
            }
        }
        public List<TownCrierEntry> Entries
        {
            get
            {
                return this.m_Entries;
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("TownCriers", AccessLevel.GameMaster, new CommandEventHandler(TownCriers_OnCommand));
        }

        [Usage("TownCriers")]
        [Description("Manages the global town crier list.")]
        public static void TownCriers_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new TownCrierGump(e.Mobile, Instance));
        }

        public TownCrierEntry GetRandomEntry()
        {
            if (this.m_Entries == null || this.m_Entries.Count == 0)
                return null;

            for (int i = this.m_Entries.Count - 1; this.m_Entries != null && i >= 0; --i)
            {
                if (i >= this.m_Entries.Count)
                    continue;

                TownCrierEntry tce = this.m_Entries[i];

                if (tce.Expired)
                    this.RemoveEntry(tce);
            }

            if (this.m_Entries == null || this.m_Entries.Count == 0)
                return null;

            return this.m_Entries[Utility.Random(this.m_Entries.Count)];
        }

        public TownCrierEntry AddEntry(string[] lines, TimeSpan duration)
        {
            if (this.m_Entries == null)
                this.m_Entries = new List<TownCrierEntry>();

            TownCrierEntry tce = new TownCrierEntry(lines, duration);

            this.m_Entries.Add(tce);

            List<TownCrier> instances = TownCrier.Instances;

            for (int i = 0; i < instances.Count; ++i)
                instances[i].ForceBeginAutoShout();

            return tce;
        }

        public void RemoveEntry(TownCrierEntry tce)
        {
            if (this.m_Entries == null)
                return;

            this.m_Entries.Remove(tce);

            if (this.m_Entries.Count == 0)
                this.m_Entries = null;
        }
    }

    public class TownCrierEntry
    {
        private readonly string[] m_Lines;
        private readonly DateTime m_ExpireTime;
        public TownCrierEntry(string[] lines, TimeSpan duration)
        {
            this.m_Lines = lines;

            if (duration < TimeSpan.Zero)
                duration = TimeSpan.Zero;
            else if (duration > TimeSpan.FromDays(365.0))
                duration = TimeSpan.FromDays(365.0);

            this.m_ExpireTime = DateTime.UtcNow + duration;
        }

        public string[] Lines
        {
            get
            {
                return this.m_Lines;
            }
        }
        public DateTime ExpireTime
        {
            get
            {
                return this.m_ExpireTime;
            }
        }
        public bool Expired
        {
            get
            {
                return (DateTime.UtcNow >= this.m_ExpireTime);
            }
        }
    }

    public class TownCrierDurationPrompt : Prompt
    {
        private readonly ITownCrierEntryList m_Owner;
        public TownCrierDurationPrompt(ITownCrierEntryList owner)
        {
            this.m_Owner = owner;
        }

        public override void OnResponse(Mobile from, string text)
        {
            TimeSpan ts;

            if (!TimeSpan.TryParse(text, out ts))
            {
                from.SendMessage("Value was not properly formatted. Use: <hours:minutes:seconds>");
                from.SendGump(new TownCrierGump(from, this.m_Owner));
                return;
            }

            if (ts < TimeSpan.Zero)
                ts = TimeSpan.Zero;

            from.SendMessage("Duration set to: {0}", ts);
            from.SendMessage("Enter the first line to shout:");

            from.Prompt = new TownCrierLinesPrompt(this.m_Owner, null, new List<String>(), ts);
        }

        public override void OnCancel(Mobile from)
        {
            from.SendLocalizedMessage(502980); // Message entry cancelled.
            from.SendGump(new TownCrierGump(from, this.m_Owner));
        }
    }

    public class TownCrierLinesPrompt : Prompt
    {
        private readonly ITownCrierEntryList m_Owner;
        private readonly TownCrierEntry m_Entry;
        private readonly List<String> m_Lines;
        private readonly TimeSpan m_Duration;
        public TownCrierLinesPrompt(ITownCrierEntryList owner, TownCrierEntry entry, List<String> lines, TimeSpan duration)
        {
            this.m_Owner = owner;
            this.m_Entry = entry;
            this.m_Lines = lines;
            this.m_Duration = duration;
        }

        public override void OnResponse(Mobile from, string text)
        {
            this.m_Lines.Add(text);

            from.SendMessage("Enter the next line to shout, or press <ESC> if the message is finished.");
            from.Prompt = new TownCrierLinesPrompt(this.m_Owner, this.m_Entry, this.m_Lines, this.m_Duration);
        }

        public override void OnCancel(Mobile from)
        {
            if (this.m_Entry != null)
                this.m_Owner.RemoveEntry(this.m_Entry);

            if (this.m_Lines.Count > 0)
            {
                this.m_Owner.AddEntry(this.m_Lines.ToArray(), this.m_Duration);
                from.SendMessage("Message has been set.");
            }
            else
            {
                if (this.m_Entry != null)
                    from.SendMessage("Message deleted.");
                else
                    from.SendLocalizedMessage(502980); // Message entry cancelled.
            }

            from.SendGump(new TownCrierGump(from, this.m_Owner));
        }
    }

    public class TownCrierGump : Gump
    {
        private readonly Mobile m_From;
        private readonly ITownCrierEntryList m_Owner;
        public TownCrierGump(Mobile from, ITownCrierEntryList owner)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Owner = owner;

            from.CloseGump(typeof(TownCrierGump));

            this.AddPage(0);

            List<TownCrierEntry> entries = owner.Entries;

            owner.GetRandomEntry(); // force expiration checks

            int count = 0;

            if (entries != null)
                count = entries.Count;

            this.AddImageTiled(0, 0, 300, 38 + (count == 0 ? 20 : (count * 85)), 0xA40);
            this.AddAlphaRegion(1, 1, 298, 36 + (count == 0 ? 20 : (count * 85)));

            this.AddHtml(8, 8, 300 - 8 - 30, 20, "<basefont color=#FFFFFF><center>TOWN CRIER MESSAGES</center></basefont>", false, false);

            this.AddButton(300 - 8 - 30, 8, 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0);

            if (count == 0)
            {
                this.AddHtml(8, 30, 284, 20, "<basefont color=#FFFFFF>The crier has no news.</basefont>", false, false);
            }
            else
            {
                for (int i = 0; i < entries.Count; ++i)
                {
                    TownCrierEntry tce = (TownCrierEntry)entries[i];

                    TimeSpan toExpire = tce.ExpireTime - DateTime.UtcNow;

                    if (toExpire < TimeSpan.Zero)
                        toExpire = TimeSpan.Zero;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("[Expires: ");

                    if (toExpire.TotalHours >= 1)
                    {
                        sb.Append((int)toExpire.TotalHours);
                        sb.Append(':');
                        sb.Append(toExpire.Minutes.ToString("D2"));
                    }
                    else
                    {
                        sb.Append(toExpire.Minutes);
                    }

                    sb.Append(':');
                    sb.Append(toExpire.Seconds.ToString("D2"));

                    sb.Append("] ");

                    for (int j = 0; j < tce.Lines.Length; ++j)
                    {
                        if (j > 0)
                            sb.Append("<br>");

                        sb.Append(tce.Lines[j]);
                    }

                    this.AddHtml(8, 35 + (i * 85), 254, 80, sb.ToString(), true, true);

                    this.AddButton(300 - 8 - 26, 35 + (i * 85), 0x15E1, 0x15E5, 2 + i, GumpButtonType.Reply, 0);
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                this.m_From.SendMessage("Enter the duration for the new message. Format: <hours:minutes:seconds>");
                this.m_From.Prompt = new TownCrierDurationPrompt(this.m_Owner);
            }
            else if (info.ButtonID > 1)
            {
                List<TownCrierEntry> entries = this.m_Owner.Entries;
                int index = info.ButtonID - 2;

                if (entries != null && index < entries.Count)
                {
                    TownCrierEntry tce = entries[index];
                    TimeSpan ts = tce.ExpireTime - DateTime.UtcNow;

                    if (ts < TimeSpan.Zero)
                        ts = TimeSpan.Zero;

                    this.m_From.SendMessage("Editing entry #{0}.", index + 1);
                    this.m_From.SendMessage("Enter the first line to shout:");
                    this.m_From.Prompt = new TownCrierLinesPrompt(this.m_Owner, tce, new List<String>(), ts);
                }
            }
        }
    }

    public class TownCrier : Mobile, ITownCrierEntryList
    {
        private static readonly List<TownCrier> m_Instances = new List<TownCrier>();
        private List<TownCrierEntry> m_Entries;
        private Timer m_NewsTimer;
        private Timer m_AutoShoutTimer;
        [Constructable]
        public TownCrier()
        {
            m_Instances.Add(this);

            this.InitStats(100, 100, 25);

            this.Title = "the town crier";
            this.Hue = Utility.RandomSkinHue();

            if (!Core.AOS)
                this.NameHue = 0x35;

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
            }

            this.AddItem(new FancyShirt(Utility.RandomBlueHue()));

            Item skirt;

            switch ( Utility.Random(2) )
            {
                case 0:
                    skirt = new Skirt();
                    break;
                default:
                case 1:
                    skirt = new Kilt();
                    break;
            }

            skirt.Hue = Utility.RandomGreenHue();

            this.AddItem(skirt);

            this.AddItem(new FeatheredHat(Utility.RandomGreenHue()));

            Item boots;

            switch ( Utility.Random(2) )
            {
                case 0:
                    boots = new Boots();
                    break;
                default:
                case 1:
                    boots = new ThighBoots();
                    break;
            }

            this.AddItem(boots);

            Utility.AssignRandomHair(this);
        }

        public TownCrier(Serial serial)
            : base(serial)
        {
            m_Instances.Add(this);
        }

        public static List<TownCrier> Instances
        {
            get
            {
                return m_Instances;
            }
        }
        public List<TownCrierEntry> Entries
        {
            get
            {
                return this.m_Entries;
            }
        }
        public TownCrierEntry GetRandomEntry()
        {
            if (this.m_Entries == null || this.m_Entries.Count == 0)
                return GlobalTownCrierEntryList.Instance.GetRandomEntry();

            for (int i = this.m_Entries.Count - 1; this.m_Entries != null && i >= 0; --i)
            {
                if (i >= this.m_Entries.Count)
                    continue;

                TownCrierEntry tce = this.m_Entries[i];

                if (tce.Expired)
                    this.RemoveEntry(tce);
            }

            if (this.m_Entries == null || this.m_Entries.Count == 0)
                return GlobalTownCrierEntryList.Instance.GetRandomEntry();

            TownCrierEntry entry = GlobalTownCrierEntryList.Instance.GetRandomEntry();

            if (entry == null || Utility.RandomBool())
                entry = this.m_Entries[Utility.Random(this.m_Entries.Count)];

            return entry;
        }

        public void ForceBeginAutoShout()
        {
            if (this.m_AutoShoutTimer == null)
                this.m_AutoShoutTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), TimeSpan.FromMinutes(1.0), new TimerCallback(AutoShout_Callback));
        }

        public TownCrierEntry AddEntry(string[] lines, TimeSpan duration)
        {
            if (this.m_Entries == null)
                this.m_Entries = new List<TownCrierEntry>();

            TownCrierEntry tce = new TownCrierEntry(lines, duration);

            this.m_Entries.Add(tce);

            if (this.m_AutoShoutTimer == null)
                this.m_AutoShoutTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), TimeSpan.FromMinutes(1.0), new TimerCallback(AutoShout_Callback));

            return tce;
        }

        public void RemoveEntry(TownCrierEntry tce)
        {
            if (this.m_Entries == null)
                return;

            this.m_Entries.Remove(tce);

            if (this.m_Entries.Count == 0)
                this.m_Entries = null;

            if (this.m_Entries == null && GlobalTownCrierEntryList.Instance.IsEmpty)
            {
                if (this.m_AutoShoutTimer != null)
                    this.m_AutoShoutTimer.Stop();

                this.m_AutoShoutTimer = null;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                from.SendGump(new TownCrierGump(from, this));
            else
                base.OnDoubleClick(from);
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return (this.m_NewsTimer == null && from.Alive && this.InRange(from, 12));
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (this.m_NewsTimer == null && e.HasKeyword(0x30) && e.Mobile.Alive && this.InRange(e.Mobile, 12)) // *news*
            {
                this.Direction = this.GetDirectionTo(e.Mobile);

                TownCrierEntry tce = this.GetRandomEntry();

                if (tce == null)
                {
                    this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1005643); // I have no news at this time.
                }
                else
                {
                    this.m_NewsTimer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(3.0), new TimerStateCallback(ShoutNews_Callback), new object[] { tce, 0 });

                    this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 502978); // Some of the latest news!
                }
            }
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public override void OnDelete()
        {
            m_Instances.Remove(this);
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Core.AOS && this.NameHue == 0x35)
                this.NameHue = -1;
        }

        private void AutoShout_Callback()
        {
            TownCrierEntry tce = this.GetRandomEntry();

            if (tce == null)
            {
                if (this.m_AutoShoutTimer != null)
                    this.m_AutoShoutTimer.Stop();

                this.m_AutoShoutTimer = null;
            }
            else if (this.m_NewsTimer == null)
            {
                this.m_NewsTimer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(3.0), new TimerStateCallback(ShoutNews_Callback), new object[] { tce, 0 });

                this.PublicOverheadMessage(MessageType.Regular, 0x3B2, 502976); // Hear ye! Hear ye!
            }
        }

        private void ShoutNews_Callback(object state)
        {
            object[] states = (object[])state;
            TownCrierEntry tce = (TownCrierEntry)states[0];
            int index = (int)states[1];

            if (index < 0 || index >= tce.Lines.Length)
            {
                if (this.m_NewsTimer != null)
                    this.m_NewsTimer.Stop();

                this.m_NewsTimer = null;
            }
            else
            {
                this.PublicOverheadMessage(MessageType.Regular, 0x3B2, false, tce.Lines[index]);
                states[1] = index + 1;
            }
        }
    }
}