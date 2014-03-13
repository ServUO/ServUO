//----------------------------------------------------------------------------------//
// Created by Vano. Email: vano2006uo@mail.ru      //
//---------------------------------------------------------------------------------//
using System;
using System.Collections;

namespace Server.Items
{
    public class ForceOfNature : WeaponAbility
    {
        private static readonly Hashtable m_Table = new Hashtable();
        public ForceOfNature()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 35;
            }
        }
        public static bool RemoveCurse(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t == null)
                return false;

            t.Stop();
            m.SendLocalizedMessage(1061687); // You can breath normally again.

            m_Table.Remove(m);
            return true;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendMessage("You attack with Nature's Fury"); 
            defender.SendMessage("You are attacked by Nature's Fury");

            defender.PlaySound(0x22F);
            defender.FixedParticles(0x36CB, 1, 9, 9911, 67, 5, EffectLayer.Head);
            defender.FixedParticles(0x374A, 1, 17, 9502, 1108, 4, (EffectLayer)255);
            if (!m_Table.Contains(defender))
            {
                Timer t = new InternalTimer(defender, attacker);
                t.Start();

                m_Table[defender] = t;
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Target;
            private readonly Mobile m_From;
            private readonly double m_MinBaseDamage;
            private readonly double m_MaxBaseDamage;
            private readonly int m_MaxCount;
            private DateTime m_NextHit;
            private int m_HitDelay;
            private int m_Count;
            public InternalTimer(Mobile target, Mobile from)
                : base(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1))
            {
                this.Priority = TimerPriority.FiftyMS;

                this.m_Target = target;
                this.m_From = from;

                double spiritLevel = from.Skills[SkillName.SpiritSpeak].Value / 15;

                this.m_MinBaseDamage = spiritLevel - 2;
                this.m_MaxBaseDamage = spiritLevel + 1;

                this.m_HitDelay = 5;
                this.m_NextHit = DateTime.UtcNow + TimeSpan.FromSeconds(this.m_HitDelay);

                this.m_Count = (int)spiritLevel;

                if (this.m_Count < 4)
                    this.m_Count = 4;

                this.m_MaxCount = this.m_Count;
            }

            protected override void OnTick()
            {
                if (!this.m_Target.Alive)
                {
                    m_Table.Remove(this.m_Target);
                    this.Stop();
                }

                if (!this.m_Target.Alive || DateTime.UtcNow < this.m_NextHit)
                    return;

                --this.m_Count;

                if (this.m_HitDelay > 1)
                {
                    if (this.m_MaxCount < 5)
                    {
                        --this.m_HitDelay;
                    }
                    else
                    {
                        int delay = (int)(Math.Ceiling((1.0 + (5 * this.m_Count)) / this.m_MaxCount));

                        if (delay <= 5)
                            this.m_HitDelay = delay;
                        else
                            this.m_HitDelay = 5;
                    }
                }

                if (this.m_Count == 0)
                {
                    this.m_Target.SendLocalizedMessage(1061687); // You can breath normally again.
                    m_Table.Remove(this.m_Target);
                    this.Stop();
                }
                else
                {
                    this.m_NextHit = DateTime.UtcNow + TimeSpan.FromSeconds(this.m_HitDelay);

                    double damage = this.m_MinBaseDamage + (Utility.RandomDouble() * (this.m_MaxBaseDamage - this.m_MinBaseDamage));

                    damage *= (3 - (((double)this.m_Target.Stam / this.m_Target.StamMax) * 2));

                    if (damage < 1)
                        damage = 1;

                    if (!this.m_Target.Player)
                        damage *= 1.75;

                    AOS.Damage(this.m_Target, this.m_From, (int)damage, 0, 0, 0, 100, 0);
                }
            }
        }
    }
}