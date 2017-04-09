using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Spellweaving
{
    public class GiftOfRenewalSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Gift of Renewal", "Olorisstra",
            -1);

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
                return 0.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 24;
            }
        }

        public GiftOfRenewalSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            if (m_Table.ContainsKey(m))
            {
                this.Caster.SendLocalizedMessage(501775); // This spell is already in effect.
            }
            else if (!this.Caster.CanBeginAction(typeof(GiftOfRenewalSpell)))
            {
                this.Caster.SendLocalizedMessage(501789); // You must wait before trying again.
            }
            else if (this.CheckBSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                this.Caster.FixedEffect(0x374A, 10, 20);
                this.Caster.PlaySound(0x5C9);

                if (m.Poisoned)
                {
                    m.CurePoison(m);
                }
                else
                {
                    double skill = this.Caster.Skills[SkillName.Spellweaving].Value;

                    int hitsPerRound = 5 + (int)(skill / 24) + this.FocusLevel;
                    TimeSpan duration = TimeSpan.FromSeconds(30 + (this.FocusLevel * 10));

                    GiftOfRenewalInfo info = new GiftOfRenewalInfo(this.Caster, m, hitsPerRound);

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

                    this.Caster.BeginAction(typeof(GiftOfRenewalSpell));

                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.GiftOfRenewal, 1031602, 1075797, duration, m, hitsPerRound.ToString()));
                }
            }

            this.FinishSequence();
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
                this.m_Caster = caster;
                this.m_Mobile = mobile;
                this.m_HitsPerRound = hitsPerRound;

                this.m_Timer = new InternalTimer(this);
                this.m_Timer.Start();
            }
        }

        public class InternalTimer : Timer
        {
            public GiftOfRenewalInfo m_Info;

            public InternalTimer(GiftOfRenewalInfo info)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                this.m_Info = info;
            }

            protected override void OnTick()
            {
                Mobile m = this.m_Info.m_Mobile;

                if (!m_Table.ContainsKey(m))
                {
                    this.Stop();
                    return;
                }

                if (!m.Alive)
                {
                    this.Stop();
                    StopEffect(m);
                    return;
                }

                if (m.Hits >= m.HitsMax)
                    return;

                int toHeal = this.m_Info.m_HitsPerRound;

                SpellHelper.Heal(toHeal, m, this.m_Info.m_Caster);
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
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile m, object o)
            {
                if (o is Mobile)
                {
                    this.m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile m)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}