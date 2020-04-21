using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.Help
{
    public class SpeechLog : IEnumerable<SpeechLogEntry>
    {
        // Are speech logs enabled?
        public static readonly bool Enabled = true;

        // How long should we maintain each speech entry?
        public static readonly TimeSpan EntryDuration = TimeSpan.FromMinutes(20.0);

        // What is the maximum number of entries a log can contain? (0 -> no limit)
        public static readonly int MaxLength = 0;

        public static void Initialize()
        {
            CommandSystem.Register("SpeechLog", AccessLevel.GameMaster, SpeechLog_OnCommand);
        }

        [Usage("SpeechLog")]
        [Description("Opens the speech log of a given target.")]
        private static void SpeechLog_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("Target a player to view his speech log.");
            e.Mobile.Target = new SpeechLogTarget();
        }

        private class SpeechLogTarget : Target
        {
            public SpeechLogTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = targeted as PlayerMobile;

                if (pm == null)
                {
                    from.SendMessage("Speech logs aren't supported on that target.");
                }
                else if (from != targeted && from.AccessLevel <= pm.AccessLevel && from.AccessLevel != AccessLevel.Owner)
                {
                    from.SendMessage("You don't have the required access level to view {0} speech log.", pm.Female ? "her" : "his");
                }
                else if (pm.SpeechLog == null)
                {
                    from.SendMessage("{0} has no speech log.", pm.Female ? "She" : "He");
                }
                else
                {
                    CommandLogging.WriteLine(from, "{0} {1} viewing speech log of {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(targeted));

                    Gump gump = new SpeechLogGump(pm, pm.SpeechLog);
                    from.SendGump(gump);
                }
            }
        }

        private readonly Queue<SpeechLogEntry> m_Queue;

        public int Count => m_Queue.Count;

        public SpeechLog()
        {
            m_Queue = new Queue<SpeechLogEntry>();
        }

        public void Add(Mobile from, string speech)
        {
            Add(new SpeechLogEntry(from, speech));
        }

        public void Add(SpeechLogEntry entry)
        {
            if (MaxLength > 0 && m_Queue.Count >= MaxLength)
                m_Queue.Dequeue();

            Clean();

            m_Queue.Enqueue(entry);
        }

        public void Clean()
        {
            while (m_Queue.Count > 0)
            {
                SpeechLogEntry entry = m_Queue.Peek();

                if (DateTime.UtcNow - entry.Created > EntryDuration)
                    m_Queue.Dequeue();
                else
                    break;
            }
        }

        public void CopyTo(SpeechLogEntry[] array, int index)
        {
            m_Queue.CopyTo(array, index);
        }

        #region IEnumerable<SpeechLogEntry> Members

        IEnumerator<SpeechLogEntry> IEnumerable<SpeechLogEntry>.GetEnumerator()
        {
            return m_Queue.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Queue.GetEnumerator();
        }
        #endregion
    }

    public class SpeechLogEntry
    {
        private readonly Mobile m_From;
        private readonly string m_Speech;
        private DateTime m_Created;

        public Mobile From => m_From;
        public string Speech => m_Speech;
        public DateTime Created
        {
            get
            {
                return m_Created;
            }
            set { m_Created = value; }
        }

        public SpeechLogEntry(Mobile from, string speech)
        {
            m_From = from;
            m_Speech = speech;
            m_Created = DateTime.UtcNow;
        }
    }
}
