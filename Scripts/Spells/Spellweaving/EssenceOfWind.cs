using System;
using System.Collections.Generic;

namespace Server.Spells.Spellweaving
{
    public class EssenceOfWindSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Essence of Wind", "Anathrae", -1);
        private static readonly Dictionary<Mobile, EssenceOfWindInfo> m_Table = new Dictionary<Mobile, EssenceOfWindInfo>();
        public EssenceOfWindSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(3.0);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 52.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 40;
            }
        }
        public static int GetFCMalus(Mobile m)
        {
            EssenceOfWindInfo info;

            if (m_Table.TryGetValue(m, out info))
                return info.FCMalus;

            return 0;
        }

        public static int GetSSIMalus(Mobile m)
        {
            EssenceOfWindInfo info;

            if (m_Table.TryGetValue(m, out info))
                return info.SSIMalus;

            return 0;
        }

        public static bool IsDebuffed(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static void StopDebuffing(Mobile m, bool message)
        {
            EssenceOfWindInfo info;

            if (m_Table.TryGetValue(m, out info))
                info.Timer.DoExpire(message);
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                this.Caster.PlaySound(0x5C6);

                int range = 5 + this.FocusLevel;
                int damage = 10 + this.FocusLevel;

                double skill = this.Caster.Skills[SkillName.Spellweaving].Value;
                int dmgBonus = Math.Max((int)(skill / 24.0d), 1);
                damage += dmgBonus;

                TimeSpan duration = TimeSpan.FromSeconds((int)(skill / 24) + this.FocusLevel);

                int fcMalus = this.FocusLevel + 1;
                int ssiMalus = 2 * (this.FocusLevel + 1);

                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in this.Caster.GetMobilesInRange(range))
                {
                    if (this.Caster != m && this.Caster.InLOS(m) && SpellHelper.ValidIndirectTarget(this.Caster, m) && this.Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }

                for (int i = 0; i < targets.Count; i++)
                {
                    Mobile m = targets[i];

                    this.Caster.DoHarmful(m);

                    SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);

                    if (!this.CheckResisted(m))	//No message on resist
                    {
                        m_Table[m] = new EssenceOfWindInfo(m, fcMalus, ssiMalus, duration);

                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.EssenceOfWind, 1075802, duration, m, String.Format("{0}\t{1}", fcMalus.ToString(), ssiMalus.ToString())));
                    }
                }
            }

            this.FinishSequence();
        }

        private class EssenceOfWindInfo
        {
            private readonly Mobile m_Defender;
            private readonly int m_FCMalus;
            private readonly int m_SSIMalus;
            private readonly ExpireTimer m_Timer;
            public EssenceOfWindInfo(Mobile defender, int fcMalus, int ssiMalus, TimeSpan duration)
            {
                this.m_Defender = defender;
                this.m_FCMalus = fcMalus;
                this.m_SSIMalus = ssiMalus;

                this.m_Timer = new ExpireTimer(this.m_Defender, duration);
                this.m_Timer.Start();
            }

            public Mobile Defender
            {
                get
                {
                    return this.m_Defender;
                }
            }
            public int FCMalus
            {
                get
                {
                    return this.m_FCMalus;
                }
            }
            public int SSIMalus
            {
                get
                {
                    return this.m_SSIMalus;
                }
            }
            public ExpireTimer Timer
            {
                get
                {
                    return this.m_Timer;
                }
            }
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public ExpireTimer(Mobile m, TimeSpan delay)
                : base(delay)
            {
                this.m_Mobile = m;
            }

            public void DoExpire(bool message)
            {
                this.Stop();
                /*
                if( message )
                {
                }
                */
                m_Table.Remove(this.m_Mobile);

                BuffInfo.RemoveBuff(this.m_Mobile, BuffIcon.EssenceOfWind);
            }

            protected override void OnTick()
            {
                this.DoExpire(true);
            }
        }
    }
}