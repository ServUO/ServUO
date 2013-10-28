using System;
using System.Collections;
using Server.Items;

namespace Server.Spells.Necromancy
{
    public class CurseWeaponSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Curse Weapon", "An Sanct Gra Char",
            203,
            9031,
            Reagent.PigIron);
        private static readonly Hashtable m_Table = new Hashtable();
        public CurseWeaponSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(0.75);
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
                return 7;
            }
        }
        public override void OnCast()
        {
            BaseWeapon weapon = this.Caster.Weapon as BaseWeapon;

            if (weapon == null || weapon is Fists)
            {
                this.Caster.SendLocalizedMessage(501078); // You must be holding a weapon.
            }
            else if (this.CheckSequence())
            {
                /* Temporarily imbues a weapon with a life draining effect.
                * Half the damage that the weapon inflicts is added to the necromancer's health.
                * The effects lasts for (Spirit Speak skill level / 34) + 1 seconds.
                * 
                * NOTE: Above algorithm is fixed point, should be :
                * (Spirit Speak skill level / 3.4) + 1
                * 
                * TODO: What happens if you curse a weapon then give it to someone else? Should they get the drain effect?
                */
                this.Caster.PlaySound(0x387);
                this.Caster.FixedParticles(0x3779, 1, 15, 9905, 32, 2, EffectLayer.Head);
                this.Caster.FixedParticles(0x37B9, 1, 14, 9502, 32, 5, (EffectLayer)255);
                new SoundEffectTimer(this.Caster).Start();

                TimeSpan duration = TimeSpan.FromSeconds((this.Caster.Skills[SkillName.SpiritSpeak].Value / 3.4) + 1.0);

                Timer t = (Timer)m_Table[weapon];

                if (t != null)
                    t.Stop();

                weapon.Cursed = true;

                m_Table[weapon] = t = new ExpireTimer(weapon, duration);

                t.Start();
            }

            this.FinishSequence();
        }

        private class ExpireTimer : Timer
        {
            private readonly BaseWeapon m_Weapon;
            public ExpireTimer(BaseWeapon weapon, TimeSpan delay)
                : base(delay)
            {
                this.m_Weapon = weapon;
                this.Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                this.m_Weapon.Cursed = false;
                Effects.PlaySound(this.m_Weapon.GetWorldLocation(), this.m_Weapon.Map, 0xFA);
                m_Table.Remove(this);
            }
        }

        private class SoundEffectTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public SoundEffectTimer(Mobile m)
                : base(TimeSpan.FromSeconds(0.75))
            {
                this.m_Mobile = m;
                this.Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                this.m_Mobile.PlaySound(0xFA);
            }
        }
    }
}