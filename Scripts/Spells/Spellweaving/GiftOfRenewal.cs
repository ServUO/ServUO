using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Spells.Spellweaving
{
    public class GiftOfRenewalSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Gift of Renewal", "Olorisstra",
            -1);

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(3.0);

        public override double RequiredSkill => 0.0;
        public override int RequiredMana => 24;

        public GiftOfRenewalSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            if (m_Table.ContainsKey(m))
            {
                Caster.SendLocalizedMessage(501775); // This spell is already in effect.
            }
            else if (!Caster.CanBeginAction(typeof(GiftOfRenewalSpell)))
            {
                Caster.SendLocalizedMessage(501789); // You must wait before trying again.
            }
            else if (CheckBSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                Caster.FixedEffect(0x374A, 10, 20);
                Caster.PlaySound(0x5C9);

                if (m.Poisoned)
                {
                    m.CurePoison(m);
                }
                else
                {
                    double skill = Caster.Skills[SkillName.Spellweaving].Value;

                    int hitsPerRound = 5 + (int)(skill / 24) + FocusLevel;
                    TimeSpan duration = TimeSpan.FromSeconds(30 + (FocusLevel * 10));

                    GiftOfRenewalInfo info = new GiftOfRenewalInfo(Caster, m, hitsPerRound);

                    Timer.DelayCall(duration,
                        delegate
                        {
                            if (StopEffect(m))
                            {
                                m.PlaySound(0x455);
                                m.SendLocalizedMessage(1075071); // The Gift of Renewal has faded.
                            }
                        });

                    m_Table[m] = info;

                    Caster.BeginAction(typeof(GiftOfRenewalSpell));

                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.GiftOfRenewal, 1031602, 1075797, duration, m, hitsPerRound.ToString()));
                }
            }

            FinishSequence();
        }

        public static Dictionary<Mobile, GiftOfRenewalInfo> m_Table = new Dictionary<Mobile, GiftOfRenewalInfo>();

        public class GiftOfRenewalInfo
        {
            public Mobile m_Caster;
            public Mobile m_Mobile;
            public int m_HitsPerRound;
            public InternalTimer m_Timer;

            public GiftOfRenewalInfo(Mobile caster, Mobile mobile, int hitsPerRound)
            {
                m_Caster = caster;
                m_Mobile = mobile;
                m_HitsPerRound = hitsPerRound;

                m_Timer = new InternalTimer(this);
                m_Timer.Start();
            }
        }

        public class InternalTimer : Timer
        {
            public GiftOfRenewalInfo m_Info;

            public InternalTimer(GiftOfRenewalInfo info)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Info = info;
            }

            protected override void OnTick()
            {
                Mobile m = m_Info.m_Mobile;

                if (!m_Table.ContainsKey(m))
                {
                    Stop();
                    return;
                }

                if (!m.Alive)
                {
                    Stop();
                    StopEffect(m);
                    return;
                }

                if (m.Hits >= m.HitsMax)
                    return;

                int toHeal = m_Info.m_HitsPerRound;

                SpellHelper.Heal(toHeal, m, m_Info.m_Caster);
                m.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            }
        }

        public static bool IsUnderEffects(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static bool StopEffect(Mobile m)
        {
            GiftOfRenewalInfo info;

            if (m_Table.TryGetValue(m, out info))
            {
                m_Table.Remove(m);

                info.m_Timer.Stop();
                BuffInfo.RemoveBuff(m, BuffIcon.GiftOfRenewal);

                Timer.DelayCall(TimeSpan.FromSeconds(60), delegate { info.m_Caster.EndAction(typeof(GiftOfRenewalSpell)); });

                return true;
            }

            return false;
        }

        public class InternalTarget : Target
        {
            private readonly GiftOfRenewalSpell m_Owner;

            public InternalTarget(GiftOfRenewalSpell owner)
                : base(10, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile m, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile m)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}