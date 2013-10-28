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
            this.m_Name = name;
            this.m_Mantra = mantra;
            this.m_Action = action;
            this.m_Reagents = regs;
            this.m_AllowTown = allowTown;

            this.m_LeftHandEffect = leftHandEffect;
            this.m_RightHandEffect = rightHandEffect;

            this.m_Amounts = new int[regs.Length];

            for (int i = 0; i < regs.Length; ++i)
                this.m_Amounts[i] = 1;
        }

        public int Action
        {
            get
            {
                return this.m_Action;
            }
            set
            {
                this.m_Action = value;
            }
        }
        public bool AllowTown
        {
            get
            {
                return this.m_AllowTown;
            }
            set
            {
                this.m_AllowTown = value;
            }
        }
        public int[] Amounts
        {
            get
            {
                return this.m_Amounts;
            }
            set
            {
                this.m_Amounts = value;
            }
        }
        public string Mantra
        {
            get
            {
                return this.m_Mantra;
            }
            set
            {
                this.m_Mantra = value;
            }
        }
        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        public Type[] Reagents
        {
            get
            {
                return this.m_Reagents;
            }
            set
            {
                this.m_Reagents = value;
            }
        }
        public int LeftHandEffect
        {
            get
            {
                return this.m_LeftHandEffect;
            }
            set
            {
                this.m_LeftHandEffect = value;
            }
        }
        public int RightHandEffect
        {
            get
            {
                return this.m_RightHandEffect;
            }
            set
            {
                this.m_RightHandEffect = value;
            }
        }
    }
}