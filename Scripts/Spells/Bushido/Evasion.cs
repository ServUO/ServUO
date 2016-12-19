using System;
using System.Collections;
using Server.Items;

namespace Server.Spells.Bushido
{
    public class Evasion : SamuraiSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Evasion", null,
            -1,
            9002);
        private static readonly Hashtable m_Table = new Hashtable();
        public Evasion(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(0.25);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 60.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
            }
        }
        public static bool VerifyCast(Mobile Caster, bool messages)
        {
            if (Caster == null) // Sanity
                return false;

            BaseWeapon weap = Caster.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;

            if (weap == null)
                weap = Caster.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

            if (weap != null)
            {
                if (Core.ML && Caster.Skills[weap.Skill].Base < 50)
                {
                    if (messages)
                    {
                        Caster.SendLocalizedMessage(1076206); // Your skill with your equipped weapon must be 50 or higher to use Evasion.
                    }
                    return false;
                }
            }
            else if (!(Caster.FindItemOnLayer(Layer.TwoHanded) is BaseShield))
            {
                if (messages)
                {
                    Caster.SendLocalizedMessage(1062944); // You must have a weapon or a shield equipped to use this ability!
                }
                return false;
            }

            if (!Caster.CanBeginAction(typeof(Evasion)))
            {
                if (messages)
                {
                    Caster.SendLocalizedMessage(501789); // You must wait before trying again.
                }
                return false;
            }

            return true;
        }

        public static bool CheckSpellEvasion(Mobile defender)
        {
            BaseWeapon weap = defender.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;

            if (weap == null)
                weap = defender.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

            if (Core.ML)
            {
                if (defender.Spell != null && defender.Spell.IsCasting)
                {
                    return false;
                }
				
                if (weap != null)
                {
                    if (defender.Skills[weap.Skill].Base < 50)
                    {
                        return false;
                    }
                }
                else if (!(defender.FindItemOnLayer(Layer.TwoHanded) is BaseShield))
                {
                    return false;
                }
            }
			
            if (IsEvading(defender) && BaseWeapon.CheckParry(defender))
            {
                defender.Emote("*evades*"); // Yes.  Eew.  Blame OSI.
                defender.FixedEffect(0x37B9, 10, 16);
                return true;
            }

            return false;
        }

        public static bool IsEvading(Mobile m)
        {
            return m_Table.Contains(m);
        }

        public static TimeSpan GetEvadeDuration(Mobile m)
        {
            /* Evasion duration now scales with Bushido skill
            * 
            * If the player has higher than GM Bushido, and GM Tactics and Anatomy, they get a 1 second bonus
            * Evasion duration range:
            * o 3-6 seconds w/o tactics/anatomy
            * o 6-7 seconds w/ GM+ Bushido and GM tactics/anatomy 
            */
            if (!Core.ML)
                return TimeSpan.FromSeconds(8.0);

            double seconds = 3;

            if (m.Skills.Bushido.Value > 60)
                seconds += (m.Skills.Bushido.Value - 60) / 20;

            if (m.Skills.Anatomy.Value >= 100.0 && m.Skills.Tactics.Value >= 100.0 && m.Skills.Bushido.Value > 100.0)	//Bushido being HIGHER than 100 for bonus is intended
                seconds++;

            return TimeSpan.FromSeconds((int)seconds);
        }

        public static double GetParryScalar(Mobile m)
        {
            /* Evasion modifier to parry now scales with Bushido skill
            * 
            * If the player has higher than GM Bushido, and at least GM Tactics and Anatomy, they get a bonus to their evasion modifier (10% bonus to the evasion modifier to parry NOT 10% to the final parry chance)
            * 
            * Bonus modifier to parry range: (these are the ranges for the evasion modifier)
            * o 16-40% bonus w/o tactics/anatomy
            * o 42-50% bonus w/ GM+ bushido and GM tactics/anatomy
            */
            if (!Core.ML)
                return 1.5;

            double bonus = 0;

            if (m.Skills.Bushido.Value >= 60)
                bonus += (((m.Skills.Bushido.Value - 60) * .004) + 0.16);

            if (m.Skills.Anatomy.Value >= 100 && m.Skills.Tactics.Value >= 100 && m.Skills.Bushido.Value > 100) //Bushido being HIGHER than 100 for bonus is intended
                bonus += 0.10;

            return 1.0 + bonus;
        }

        public static void BeginEvasion(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            TimeSpan duration = GetEvadeDuration(m);
            t = new InternalTimer(m, duration);

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Evasion, 1060597, 1153810, duration, m));

            m_Table[m] = t;

            t.Start();
        }

        public static void EndEvasion(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            m_Table.Remove(m);

            BuffInfo.RemoveBuff(m, BuffIcon.Evasion);

            OnEffectEnd(m, typeof(Evasion));
        }

        public override bool CheckCast()
        {
            if (VerifyCast(this.Caster, true))
                return base.CheckCast();

            return false;
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            this.Caster.FixedEffect(0x37C4, 10, 7, 4, 3);
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                this.Caster.SendLocalizedMessage(1063120); // You feel that you might be able to deflect any attack!
                this.Caster.FixedParticles(0x376A, 1, 20, 0x7F5, 0x960, 3, EffectLayer.Waist);
                this.Caster.PlaySound(0x51B);

                this.OnCastSuccessful(this.Caster);

                BeginEvasion(this.Caster);

                this.Caster.BeginAction(typeof(Evasion));
                Timer.DelayCall(TimeSpan.FromSeconds(20.0), delegate { this.Caster.EndAction(typeof(Evasion)); });
            }

            this.FinishSequence();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public InternalTimer(Mobile m, TimeSpan delay)
                : base(delay)
            {
                this.m_Mobile = m;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                EndEvasion(this.m_Mobile);
                this.m_Mobile.SendLocalizedMessage(1063121); // You no longer feel that you could deflect any attack.
            }
        }
    }
}