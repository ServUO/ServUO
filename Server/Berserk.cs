#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server
{
    [Parsable]
    public abstract class Berserk
    {
        public abstract bool Active { get; set; }
        public abstract bool IsTempBody { get; set; }
        public abstract int TempBodyColor { get; set; }
        public abstract bool FullBestialEquip { get; }
        public abstract List<Item> EquipBestial { get; set; }
        public abstract Timer ConstructTimer(Mobile m);
        public abstract void OnRemoveEffect(Timer t);
        public abstract string Name { get; }
        public abstract int Level { get; }

        private static readonly List<Berserk> m_Berserks = new List<Berserk>();

        public override string ToString()
        {
            return Name;
        }

        public static void Register(Berserk reg)
        {
            m_Berserks.Add(reg);
        }

        public static Berserk SetBerserk { get { return GetBerserk("SetBerserk"); } }

        public static List<Berserk> Berserks { get { return m_Berserks; } }

        public static Berserk Parse(string value)
        {
            Berserk p = null;

            int plevel;

            if (int.TryParse(value, out plevel))
            {
                p = GetBerserk(plevel);
            }

            if (p == null)
            {
                p = GetBerserk(value);
            }

            return p;
        }

        public static Berserk GetBerserk(int level)
        {
            for (int i = 0; i < m_Berserks.Count; ++i)
            {
                Berserk p = m_Berserks[i];

                if (p.Level == level)
                {
                    return p;
                }
            }

            return null;
        }

        public static Berserk GetBerserk(string name)
        {
            for (int i = 0; i < m_Berserks.Count; ++i)
            {
                Berserk p = m_Berserks[i];

                if (Utility.InsensitiveCompare(p.Name, name) == 0)
                {
                    return p;
                }
            }

            return null;
        }
    }
}