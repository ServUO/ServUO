using System;

namespace CustomsFramework.Systems.FoodEffects
{
    public class FoodEffect
    {
        private Int32 m_RegenHits = 0;
        public Int32 RegenHits { get { return m_RegenHits; } }

        private Int32 m_RegenStam = 0;
        public Int32 RegenStam { get { return m_RegenStam; } }

        private Int32 m_RegenMana = 0;
        public Int32 RegenMana { get { return m_RegenMana; } }

        private Int32 m_StrBonus = 0;
        public Int32 StrBonus { get { return m_StrBonus; } }

        private Int32 m_DexBonus = 0;
        public Int32 DexBonus { get { return m_DexBonus; } }

        private Int32 m_IntBonus = 0;
        public Int32 IntBonus { get { return m_IntBonus; } }

        private Int32 m_Duration = 0;
        public Int32 Duration { get { return m_Duration; } }

        private DateTime m_Added;
        public DateTime Added { get { return m_Added; } }

        public TimeSpan EffectTimeSpan { get { return TimeSpan.FromMinutes(m_Duration); } }

        public FoodEffect(Int32 regenHits, Int32 regenStam, Int32 regenMana, Int32 strBonus, Int32 dexBonus, Int32 intBonus, Int32 duration) :
            this(regenHits, regenStam, regenMana, strBonus, dexBonus, intBonus, duration, DateTime.UtcNow)
        {
        }

        public FoodEffect(Int32 regenHits, Int32 regenStam, Int32 regenMana, Int32 strBonus, Int32 dexBonus, Int32 intBonus, Int32 duration, DateTime added)
        {
            m_RegenHits = regenHits;
            m_RegenStam = regenStam;
            m_RegenMana = regenMana;
            m_StrBonus = strBonus;
            m_DexBonus = dexBonus;
            m_IntBonus = intBonus;
            m_Duration = duration;
            m_Added = added;
        }

        public Boolean IsExpired
        {
            get
            {
                if (m_Added.AddMinutes((double)m_Duration) < DateTime.UtcNow)
                    return false;
                else
                    return true;
            }
        }

        public String GetBuffInfoText()
        {
            String buffText = "";

            if (m_StrBonus != 0)
                buffText = String.Format("{0}{1}{2}{3} Str", buffText, (buffText != "" ? "<BR>" : ""), (m_StrBonus < 0 ? "-" : "+"), m_StrBonus);

            if (m_DexBonus != 0)
                buffText = String.Format("{0}{1}{2}{3} Dex", buffText, (buffText != "" ? "<BR>" : ""), (m_DexBonus < 0 ? "-" : "+"), m_DexBonus);

            if (m_IntBonus != 0)
                buffText = String.Format("{0}{1}{2}{3} Int", buffText, (buffText != "" ? "<BR>" : ""), (m_IntBonus < 0 ? "-" : "+"), m_IntBonus);

            if (m_RegenHits != 0)
                buffText = String.Format("{0}{1}{2}{3} HP Regen", buffText, (buffText != "" ? "<BR>" : ""), (m_RegenHits < 0 ? "-" : "+"), m_RegenHits);

            if (m_RegenStam != 0)
                buffText = String.Format("{0}{1}{2}{3} Stam Regen", buffText, (buffText != "" ? "<BR>" : ""), (m_RegenStam < 0 ? "-" : "+"), m_RegenStam);

            if (m_RegenMana != 0)
                buffText = String.Format("{0}{1}{2}{3} Mana Regen", buffText, (buffText != "" ? "<BR>" : ""), (m_RegenMana < 0 ? "-" : "+"), m_RegenMana);

            return buffText;
        }
    }
}
