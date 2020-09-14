using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.Spellweaving
{
    public class EssenceOfWindSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Essence of Wind", "Anathrae", -1);
        private static readonly Dictionary<Mobile, EssenceOfWindInfo> m_Table = new Dictionary<Mobile, EssenceOfWindInfo>();

        public override DamageType SpellDamageType => DamageType.SpellAOE;

        public EssenceOfWindSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(3.0);
        public override double RequiredSkill => 52.0;
        public override int RequiredMana => 40;
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

                int damage = 10 + FocusLevel;

                double skill = Caster.Skills[SkillName.Spellweaving].Value;
                int dmgBonus = Math.Max((int)(skill / 24.0d), 1);
                damage += dmgBonus;

                TimeSpan duration = TimeSpan.FromSeconds((int)(skill / 24) + FocusLevel);

                int fcMalus = FocusLevel + 1;
                int ssiMalus = 2 * (FocusLevel + 1);

                foreach (Mobile m in AcquireIndirectTargets(Caster.Location, 5 + FocusLevel).OfType<Mobile>())
                {
                    Caster.DoHarmful(m);

                    SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);

                    if (!CheckResisted(m))	//No message on resist
                    {
                        m_Table[m] = new EssenceOfWindInfo(m, fcMalus, ssiMalus, duration);

                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.EssenceOfWind, 1075802, duration, m, string.Format("{0}\t{1}", fcMalus.ToString(), ssiMalus.ToString())));

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

            public Mobile Defender => m_Defender;
            public int FCMalus => m_FCMalus;
            public int SSIMalus => m_SSIMalus;
            public ExpireTimer Timer => m_Timer;
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