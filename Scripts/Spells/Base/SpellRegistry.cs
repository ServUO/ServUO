using System;
using System.Collections.Generic;

namespace Server.Spells
{
    public class SpellRegistry
    {
        private static readonly Type[] m_Types = new Type[745];
        private static int m_Count;

        public static Type[] Types
        {
            get
            {
                m_Count = -1;
                return m_Types;
            }
        }

        //What IS this used for anyways.
        public static int Count
        {
            get
            {
                if (m_Count == -1)
                {
                    m_Count = 0;

                    for (int i = 0; i < m_Types.Length; ++i)
                        if (m_Types[i] != null)
                            ++m_Count;
                }

                return m_Count;
            }
        }

        private static readonly Dictionary<Type, int> m_IDsFromTypes = new Dictionary<Type, int>(m_Types.Length);

        private static readonly Dictionary<int, SpecialMove> m_SpecialMoves = new Dictionary<int, SpecialMove>();
        public static Dictionary<int, SpecialMove> SpecialMoves => m_SpecialMoves;

        public static int GetRegistryNumber(ISpell s)
        {
            return GetRegistryNumber(s.GetType());
        }

        public static int GetRegistryNumber(SpecialMove s)
        {
            return GetRegistryNumber(s.GetType());
        }

        public static int GetRegistryNumber(Type type)
        {
            if (m_IDsFromTypes.ContainsKey(type))
                return m_IDsFromTypes[type];

            return -1;
        }

        public static void Register(int spellID, Type type)
        {
            if (spellID < 0 || spellID >= m_Types.Length)
                return;

            if (m_Types[spellID] == null)
                ++m_Count;

            m_Types[spellID] = type;

            if (!m_IDsFromTypes.ContainsKey(type))
                m_IDsFromTypes.Add(type, spellID);

            if (type.IsSubclassOf(typeof(SpecialMove)))
            {
                SpecialMove spm = null;

                try
                {
                    spm = Activator.CreateInstance(type) as SpecialMove;
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }

                if (spm != null)
                    m_SpecialMoves.Add(spellID, spm);
            }
        }

        public static SpecialMove GetSpecialMove(int spellID)
        {
            if (spellID < 0 || spellID >= m_Types.Length)
                return null;

            Type t = m_Types[spellID];

            if (t == null || !t.IsSubclassOf(typeof(SpecialMove)) || !m_SpecialMoves.ContainsKey(spellID))
                return null;

            return m_SpecialMoves[spellID];
        }

        private static readonly object[] m_Params = new object[2];

        public static Spell NewSpell(int spellID, Mobile caster, Item scroll)
        {
            if (spellID < 0 || spellID >= m_Types.Length)
                return null;

            Type t = m_Types[spellID];

            if (t != null && !t.IsSubclassOf(typeof(SpecialMove)))
            {
                m_Params[0] = caster;
                m_Params[1] = scroll;

                try
                {
                    return (Spell)Activator.CreateInstance(t, m_Params);
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }
            }

            return null;
        }

        private static readonly string[] m_CircleNames = new string[]
        {
            "First",
            "Second",
            "Third",
            "Fourth",
            "Fifth",
            "Sixth",
            "Seventh",
            "Eighth",
            "Necromancy",
            "Chivalry",
            "Bushido",
            "Ninjitsu",
            "Spellweaving",
            #region Stygian Abyss
            "Mystic",
            #endregion
            #region TOL
            "SkillMasteries"
            #endregion
        };

        public static Spell NewSpell(string name, Mobile caster, Item scroll)
        {
            for (int i = 0; i < m_CircleNames.Length; ++i)
            {
                Type t = ScriptCompiler.FindTypeByFullName(string.Format("Server.Spells.{0}.{1}", m_CircleNames[i], name));

                if (t != null && !t.IsSubclassOf(typeof(SpecialMove)))
                {
                    m_Params[0] = caster;
                    m_Params[1] = scroll;

                    try
                    {
                        return (Spell)Activator.CreateInstance(t, m_Params);
                    }
                    catch (Exception e)
                    {
                        Diagnostics.ExceptionLogging.LogException(e);
                    }
                }
            }

            return null;
        }
    }
}
