// Created by Peoharen
using System;
using System.Collections.Generic;

namespace Server
{
    public class TimedResistanceMod
    {
        private static readonly Dictionary<string, ResistanceModTimer> m_Table = new Dictionary<string, ResistanceModTimer>();
        public static void AddMod(Mobile m, string name, ResistanceMod[] mods, TimeSpan duration)
        {
            string fullname = name + ":" + m.Serial.ToString();

            if (m_Table.ContainsKey(fullname))
            {
                ResistanceModTimer timer = m_Table[fullname];
                timer.End();
                m_Table.Remove(fullname);
            }

            ResistanceModTimer timertostart = new ResistanceModTimer(m, name, mods, duration);
            timertostart.Start();
            m_Table.Add(fullname, timertostart);
        }

        public static void RemoveMod(Mobile m, string name)
        {
            string fullname = name + ":" + m.Serial.ToString();

            if (m_Table.ContainsKey(fullname))
            {
                ResistanceModTimer t = m_Table[fullname];

                if (t != null)
                    t.End();

                m_Table.Remove(fullname);
            }
        }

        public class ResistanceModTimer : Timer
        {
            public Mobile m_Mobile;
            public ResistanceMod[] m_Mods;
            public String m_Name;
            public ResistanceModTimer(Mobile m, string name, ResistanceMod[] mods, TimeSpan duration)
                : base(duration)
            {
                this.m_Mobile = m;
                this.m_Name = name;
                this.m_Mods = mods;
            }

            public void End()
            {
                for (int i = 0; i < this.m_Mods.Length; ++i)
                    this.m_Mobile.RemoveResistanceMod(this.m_Mods[i]);

                this.Stop();
            }

            protected override void OnTick()
            {
                RemoveMod(this.m_Mobile, this.m_Name);
            }
        }
    }
}