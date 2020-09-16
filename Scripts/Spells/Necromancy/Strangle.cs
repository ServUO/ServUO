using Server.Spells.SkillMasteries;
using Server.Targeting;
using System;
using System.Collections;

namespace Server.Spells.Necromancy
{
    public class StrangleSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Strangle", "In Bal Nox",
            209,
            9031,
            Reagent.DaemonBlood,
            Reagent.NoxCrystal);
        private static readonly Hashtable m_Table = new Hashtable();
        public StrangleSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.25);
        public override double RequiredSkill => 65.0;
        public override int RequiredMana => 29;

        public static bool UnderEffects(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static bool RemoveCurse(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t == null)
                return false;

            t.Stop();
            m.SendLocalizedMessage(1061687); // You can breath normally again.

            BuffInfo.RemoveBuff(m, BuffIcon.Strangle);

            m_Table.Remove(m);
            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (CheckHSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                ApplyEffects(m);
                ConduitSpell.CheckAffected(Caster, m, ApplyEffects);
            }

            FinishSequence();
        }

        public void ApplyEffects(Mobile m, double strength = 1.0)
        {
            SpellHelper.CheckReflect(this, Caster, ref m);

            /* Temporarily chokes off the air suply of the target with poisonous fumes.
             * The target is inflicted with poison damage over time.
             * The amount of damage dealt each "hit" is based off of the caster's Spirit Speak skill and the Target's current Stamina.
             * The less Stamina the target has, the more damage is done by Strangle.
             * Duration of the effect is Spirit Speak skill level / 10 rounds, with a minimum number of 4 rounds.
             * The first round of damage is dealt after 5 seconds, and every next round after that comes 1 second sooner than the one before, until there is only 1 second between rounds.
             * The base damage of the effect lies between (Spirit Speak skill level / 10) - 2 and (Spirit Speak skill level / 10) + 1.
             * Base damage is multiplied by the following formula: (3 - (target's current Stamina / target's maximum Stamina) * 2).
             * Example:
             * For a target at full Stamina the damage multiplier is 1,
             * for a target at 50% Stamina the damage multiplier is 2 and
             * for a target at 20% Stamina the damage multiplier is 2.6
             */

            if (m.Spell != null)
                m.Spell.OnCasterHurt();

            m.PlaySound(0x22F);
            m.FixedParticles(0x36CB, 1, 9, 9911, 67, 5, EffectLayer.Head);
            m.FixedParticles(0x374A, 1, 17, 9502, 1108, 4, (EffectLayer)255);

            if (Mysticism.StoneFormSpell.CheckImmunity(m))
            {
                Caster.SendLocalizedMessage(1095250); // Your target resists strangle.
            }
            else if (!m_Table.ContainsKey(m))
            {
                Timer t = new InternalTimer(m, Caster, strength);
                t.Start();

                m_Table[m] = t;

                //Calculations for the buff bar
                double spiritlevel = Caster.Skills[SkillName.SpiritSpeak].Value / 10;
                if (spiritlevel < 4)
                    spiritlevel = 4;
                int d_MinDamage = (int)(4.0 * strength);
                int d_MaxDamage = (int)(((spiritlevel + 1) * 3) * strength);
                string args = string.Format("{0}\t{1}", d_MinDamage, d_MaxDamage);

                int i_Count = (int)spiritlevel;
                int i_MaxCount = i_Count;
                int i_HitDelay = 5;
                int i_Length = i_HitDelay;

                while (i_Count > 1)
                {
                    --i_Count;
                    if (i_HitDelay > 1)
                    {
                        if (i_MaxCount < 5)
                        {
                            --i_HitDelay;
                        }
                        else
                        {
                            int delay = (int)(Math.Ceiling((1.0 + (5 * i_Count)) / i_MaxCount));

                            if (delay <= 5)
                                i_HitDelay = delay;
                            else
                                i_HitDelay = 5;
                        }
                    }
                    i_Length += i_HitDelay;
                }

                TimeSpan t_Duration = TimeSpan.FromSeconds(i_Length * strength);
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Strangle, 1075794, 1075795, t_Duration, m, args.ToString()));
            }

            HarmfulSpell(m);
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Target, m_From;
            private readonly double m_MinBaseDamage, m_MaxBaseDamage;

            private DateTime m_NextHit;
            private int m_HitDelay;
            private int m_Count;
            private readonly int m_MaxCount;

            public InternalTimer(Mobile target, Mobile from, double strength)
                : base(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1))
            {
                Priority = TimerPriority.FiftyMS;

                m_Target = target;
                m_From = from;

                double spiritLevel = from.Skills[SkillName.SpiritSpeak].Value / 10;

                m_MinBaseDamage = (spiritLevel - 2) * strength;
                m_MaxBaseDamage = (spiritLevel + 1) * strength;

                m_HitDelay = 5;
                m_NextHit = DateTime.UtcNow + TimeSpan.FromSeconds(m_HitDelay);

                m_Count = (int)spiritLevel;

                if (m_Count < 4)
                    m_Count = 4;

                m_MaxCount = m_Count;
            }

            protected override void OnTick()
            {
                if (!m_Target.Alive)
                {
                    m_Table.Remove(m_Target);
                    Stop();
                }

                if (!m_Target.Alive || DateTime.UtcNow < m_NextHit)
                    return;

                --m_Count;

                if (m_HitDelay > 1)
                {
                    if (m_MaxCount < 5)
                    {
                        --m_HitDelay;
                    }
                    else
                    {
                        int delay = (int)(Math.Ceiling((1.0 + (5 * m_Count)) / m_MaxCount));

                        if (delay <= 5)
                            m_HitDelay = delay;
                        else
                            m_HitDelay = 5;
                    }
                }

                if (m_Count == 0)
                {
                    m_Target.SendLocalizedMessage(1061687); // You can breath normally again.
                    m_Table.Remove(m_Target);
                    Stop();
                }
                else
                {
                    m_NextHit = DateTime.UtcNow + TimeSpan.FromSeconds(m_HitDelay);

                    double damage = m_MinBaseDamage + (Utility.RandomDouble() * (m_MaxBaseDamage - m_MinBaseDamage));

                    damage *= (3 - (((double)m_Target.Stam / m_Target.StamMax) * 2));

                    if (damage < 1)
                        damage = 1;

                    if (!m_Target.Player)
                        damage *= 1.75;

                    AOS.Damage(m_Target, m_From, (int)damage, 0, 0, 0, 100, 0);

                    if (0.60 <= Utility.RandomDouble()) // OSI: randomly revealed between first and third damage tick, guessing 60% chance
                        m_Target.RevealingAction();
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly StrangleSpell m_Owner;
            public InternalTarget(StrangleSpell owner)
                : base(10, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
