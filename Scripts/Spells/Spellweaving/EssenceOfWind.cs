using System;
using System.Collections.Generic;

namespace Server.Spells.Spellweaving
{
    public class EssenceOfWindSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Essence of Wind", "Anathrae", -1);
        private static readonly Dictionary<Mobile, EssenceOfWindInfo> m_Table = new Dictionary<Mobile, EssenceOfWindInfo>();

        public override DamageType SpellDamageType { get { return DamageType.SpellAOE; } }

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
            if (CheckSequence())
            {
                Caster.PlaySound(0x5C6);

                int range = 5 + FocusLevel;
                int damage = 10 + FocusLevel;

                double skill = Caster.Skills[SkillName.Spellweaving].Value;
                int dmgBonus = Math.Max((int)(skill / 24.0d), 1);
                damage += dmgBonus;

                TimeSpan duration = TimeSpan.FromSeconds((int)(skill / 24) + FocusLevel);

                int fcMalus = FocusLevel + 1;
                int ssiMalus = 2 * (FocusLevel + 1);

                List<Mobile> targets = new List<Mobile>();

                IPooledEnumerable eable = Caster.GetMobilesInRange(range);

                foreach (Mobile m in eable)
                {
                    if (Caster != m && Caster.InLOS(m) && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false))
                        targets.Add(m);
                }

                eable.Free();

                for (int i = 0; i < targets.Count; i++)
                {
                    Mobile m = targets[i];

                    Caster.DoHarmful(m);

                    SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);

                    if (!CheckResisted(m))	//No message on resist
                    {
                        m_Table[m] = new EssenceOfWindInfo(m, fcMalus, ssiMalus, duration);

                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.EssenceOfWind, 1075802, duration, m, String.Format("{0}\t{1}", fcMalus.ToString(), ssiMalus.ToString())));

                        m.Delta(MobileDelta.WeaponDamage);
                    }
                }
            }

            FinishSequence();
        }

        private class EssenceOfWindInfo
        {
            private readonly Mobile m_Defender;
            private readonly int m_FCMalus;
            private readonly int m_SSIMalus;
            private readonly ExpireTimer m_Timer;
            public EssenceOfWindInfo(Mobile defender, int fcMalus, int ssiMalus, TimeSpan duration)
            {
                m_Defender = defender;
                m_FCMalus = fcMalus;
                m_SSIMalus = ssiMalus;

                m_Timer = new ExpireTimer(m_Defender, duration);
                m_Timer.Start();
            }

            public Mobile Defender
            {
                get
                {
                    return m_Defender;
                }
            }
            public int FCMalus
            {
                get
                {
                    return m_FCMalus;
                }
            }
            public int SSIMalus
            {
                get
                {
                    return m_SSIMalus;
                }
            }
            public ExpireTimer Timer
            {
                get
                {
                    return m_Timer;
                }
            }
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public ExpireTimer(Mobile m, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
            }

            public void DoExpire(bool message)
            {
                Stop();

                m_Table.Remove(m_Mobile);

                BuffInfo.RemoveBuff(m_Mobile, BuffIcon.EssenceOfWind);
                m_Mobile.Delta(MobileDelta.WeaponDamage);
            }

            protected override void OnTick()
            {
                DoExpire(true);
            }
        }
    }
}