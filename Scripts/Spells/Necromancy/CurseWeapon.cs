using System;
using System.Collections.Generic;
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

        private static readonly Dictionary<Mobile, ExpireTimer> m_Table = new Dictionary<Mobile, ExpireTimer>();

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
            BaseWeapon weapon = Caster.Weapon as BaseWeapon;

            if (Caster.Player && (weapon == null || weapon is Fists))
            {
                Caster.SendLocalizedMessage(501078); // You must be holding a weapon.
            }
            else if (CheckSequence())
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
                Caster.PlaySound(0x387);
                Caster.FixedParticles(0x3779, 1, 15, 9905, 32, 2, EffectLayer.Head);
                Caster.FixedParticles(0x37B9, 1, 14, 9502, 32, 5, (EffectLayer)255);
                new SoundEffectTimer(Caster).Start();

                TimeSpan duration = TimeSpan.FromSeconds((Caster.Skills[SkillName.SpiritSpeak].Value / 3.4) + 1.0);

                ExpireTimer t = null;

                if (m_Table.ContainsKey(Caster))
                {
                    t = m_Table[Caster];
                }

                if (t != null)
                    t.Stop();

                m_Table[Caster] = t = new ExpireTimer(weapon, Caster, duration);

                t.Start();

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.CurseWeapon, 1060512, 1153780, duration, Caster));
            }

            FinishSequence();
        }

        public static bool IsCursed(Mobile attacker, BaseWeapon wep)
        {
            return m_Table.ContainsKey(attacker) && m_Table[attacker].Weapon == wep;
        }

        public class ExpireTimer : Timer
        {
            public BaseWeapon Weapon { get; private set; }
            public Mobile Owner { get; private set; }

            public ExpireTimer(BaseWeapon weapon, Mobile owner, TimeSpan delay)
                : base(delay)
            {
                Weapon = weapon;
                Owner = owner;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                Effects.PlaySound(Weapon.GetWorldLocation(), Weapon.Map, 0xFA);

                if (m_Table.ContainsKey(Owner))
                {
                    m_Table.Remove(Owner);
                }
            }
        }

        private class SoundEffectTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public SoundEffectTimer(Mobile m)
                : base(TimeSpan.FromSeconds(0.75))
            {
                m_Mobile = m;
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                m_Mobile.PlaySound(0xFA);
            }
        }
    }
}