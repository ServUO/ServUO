using System;
using System.Collections;

namespace Server.Items
{
    /// <summary>
    /// A successful Paralyzing Blow will leave the target stunned, unable to move, attack, or cast spells, for a few seconds.
    /// </summary>
    public class ParalyzingBlow : WeaponAbility
    {
        public static readonly TimeSpan PlayerFreezeDuration = TimeSpan.FromSeconds(3.0);
        public static readonly TimeSpan NPCFreezeDuration = TimeSpan.FromSeconds(6.0);
        public static readonly TimeSpan FreezeDelayDuration = TimeSpan.FromSeconds(8.0);
        // No longer active in pub21:
        private static readonly Hashtable m_Table = new Hashtable();
        public ParalyzingBlow()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 30;
            }
        }
        public static bool IsImmune(Mobile m)
        {
            return m_Table.Contains(m);
        }

        public static void BeginImmunity(Mobile m, TimeSpan duration)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            t = new InternalTimer(m, duration);
            m_Table[m] = t;

            t.Start();
        }

        public static void EndImmunity(Mobile m)
        {
            Timer t = (Timer)m_Table[m];

            if (t != null)
                t.Stop();

            m_Table.Remove(m);
        }

        /*public override bool CheckSkills( Mobile from )
        {
        if ( !base.CheckSkills( from ) )
        return false;

        if ( !(from.Weapon is Fists) )
        return true;

        Skill skill = from.Skills[SkillName.Anatomy];

        if ( skill != null && skill.Base >= 80.0 )
        return true;

        from.SendLocalizedMessage( 1061811 ); // You lack the required anatomy skill to perform that attack!

        return false;
        }*/
        public override bool RequiresTactics(Mobile from)
        {
            BaseWeapon weapon = from.Weapon as BaseWeapon;

            if (weapon == null)
                return true;

            return weapon.Skill != SkillName.Wrestling;
        }

        public override bool OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            if (defender.Paralyzed)
            {
                attacker.SendLocalizedMessage(1061923); // The target is already frozen.
                return false;
            }

            return true;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            if (IsImmune(defender))	//Intentionally going after Mana consumption
            {
                attacker.SendLocalizedMessage(1070804); // Your target resists paralysis.
                defender.SendLocalizedMessage(1070813); // You resist paralysis.
                return;
            }

            defender.FixedEffect(0x376A, 9, 32);
            defender.PlaySound(0x204);

            attacker.SendLocalizedMessage(1060163); // You deliver a paralyzing blow!
            defender.SendLocalizedMessage(1060164); // The attack has temporarily paralyzed you!

            TimeSpan duration = defender.Player ? PlayerFreezeDuration : NPCFreezeDuration;

            // Treat it as paralyze not as freeze, effect must be removed when damaged.
            defender.Paralyze(duration);

            BeginImmunity(defender, duration + FreezeDelayDuration);
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public InternalTimer(Mobile m, TimeSpan duration)
                : base(duration)
            {
                this.m_Mobile = m;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                EndImmunity(this.m_Mobile);
            }
        }
    }
}