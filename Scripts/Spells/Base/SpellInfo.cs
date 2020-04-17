using System;

namespace Server.Spells
{
    public class SpellInfo
    {
        private string m_Name;
        private string m_Mantra;
        private Type[] m_Reagents;
        private int[] m_Amounts;
        private int m_Action;
        private bool m_AllowTown;
        private int m_LeftHandEffect, m_RightHandEffect;
        public SpellInfo(string name, string mantra, params Type[] regs)
            : this(name, mantra, 16, 0, 0, true, regs)
        {
        }

        public SpellInfo(string name, string mantra, bool allowTown, params Type[] regs)
            : this(name, mantra, 16, 0, 0, allowTown, regs)
        {
        }

        public SpellInfo(string name, string mantra, int action, params Type[] regs)
            : this(name, mantra, action, 0, 0, true, regs)
        {
        }

        public SpellInfo(string name, string mantra, int action, bool allowTown, params Type[] regs)
            : this(name, mantra, action, 0, 0, allowTown, regs)
        {
        }

        public SpellInfo(string name, string mantra, int action, int handEffect, params Type[] regs)
            : this(name, mantra, action, handEffect, handEffect, true, regs)
        {
        }

        public SpellInfo(string name, string mantra, int action, int handEffect, bool allowTown, params Type[] regs)
            : this(name, mantra, action, handEffect, handEffect, allowTown, regs)
        {
        }

        public SpellInfo(string name, string mantra, int action, int leftHandEffect, int rightHandEffect, bool allowTown, params Type[] regs)
        {
            m_Name = name;
            m_Mantra = mantra;
            m_Action = action;
            m_Reagents = regs;
            m_AllowTown = allowTown;

            m_LeftHandEffect = leftHandEffect;
            m_RightHandEffect = rightHandEffect;

            m_Amounts = new int[regs.Length];

            for (int i = 0; i < regs.Length; ++i)
                m_Amounts[i] = 1;
        }

        public int Action
        {
            get
            {
                return m_Action;
            }
            set
            {
                m_Action = value;
            }
        }
        public bool AllowTown
        {
            get
            {
                return m_AllowTown;
            }
            set
            {
                m_AllowTown = value;
            }
        }
        public int[] Amounts
        {
            get
            {
                return m_Amounts;
            }
            set
            {
                m_Amounts = value;
            }
        }
        public string Mantra
        {
            get
            {
                return m_Mantra;
            }
            set
            {
                m_Mantra = value;
            }
        }
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
        public Type[] Reagents
        {
            get
            {
                return m_Reagents;
            }
            set
            {
                m_Reagents = value;
            }
        }
        public int LeftHandEffect
        {
            get
            {
                return m_LeftHandEffect;
            }
            set
            {
                m_LeftHandEffect = value;
            }
        }
        public int RightHandEffect
        {
            get
            {
                return m_RightHandEffect;
            }
            set
            {
                m_RightHandEffect = value;
            }
        }
    }
}