using Server.Services.Virtues;
using System;
using System.Collections.Generic;

namespace Server.Spells.Spellweaving
{
    public class AttuneWeaponSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Attune Weapon", "Haeldril",
            -1);

        private static readonly Dictionary<Mobile, ExpireTimer> m_Table = new Dictionary<Mobile, ExpireTimer>();

        public AttuneWeaponSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(1.0);
        public override double RequiredSkill => 0.0;
        public override int RequiredMana => 24;
        public static void TryAbsorb(Mobile defender, ref int damage)
        {
            if (damage == 0 || !IsAbsorbing(defender) || defender.MeleeDamageAbsorb <= 0)
                return;

            int absorbed = Math.Min(damage, defender.MeleeDamageAbsorb);

            damage -= absorbed;
            defender.MeleeDamageAbsorb -= absorbed;

            defender.SendLocalizedMessage(1075127, string.Format("{0}\t{1}", absorbed, defender.MeleeDamageAbsorb)); // ~1_damage~ point(s) of damage have been absorbed. A total of ~2_remaining~ point(s) of shielding remain.

            if (defender.MeleeDamageAbsorb <= 0)
            {
                StopAbsorbing(defender, true);
            }
            else if (m_Table.ContainsKey(defender))
            {
                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.AttuneWeapon, 1075798, m_Table[defender].Expires - DateTime.UtcNow, defender, defender.MeleeDamageAbsorb.ToString()));
            }
        }

        public static bool IsAbsorbing(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static void StopAbsorbing(Mobile m, bool message)
        {
            ExpireTimer t;
            if (m_Table.TryGetValue(m, out t))
            {
                t.DoExpire(message);
            }
        }

        public override bool CheckCast()
        {
            if (m_Table.ContainsKey(Caster))
            {
                Caster.SendLocalizedMessage(501775); // This spell is already in effect.
                return false;
            }
            else if (!Caster.CanBeginAction(typeof(AttuneWeaponSpell)))
            {
                Caster.SendLocalizedMessage(1075124); // You must wait before casting that spell again.
                return false;
            }
            else if (SpiritualityVirtue.IsEmbracee(Caster))
            {
                Caster.SendLocalizedMessage(1156040); // You may not cast Attunement whilst a Spirituality Shield is active!
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.PlaySound(0x5C3);
                Caster.FixedParticles(0x3728, 1, 13, 0x26B8, 0x455, 7, EffectLayer.Waist);
                Caster.FixedParticles(0x3779, 1, 15, 0x251E, 0x3F, 7, EffectLayer.Waist);

                double skill = Caster.Skills[SkillName.Spellweaving].Value;

                int damageAbsorb = (int)(18 + ((skill - 10) / 10) * 3 + (FocusLevel * 6));
                Caster.MeleeDamageAbsorb = damageAbsorb;

                TimeSpan duration = TimeSpan.FromSeconds(60 + (FocusLevel * 12));

                ExpireTimer t = new ExpireTimer(Caster, duration);
                t.Start();

                m_Table[Caster] = t;

                Caster.BeginAction(typeof(AttuneWeaponSpell));

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.AttuneWeapon, 1075798, duration, Caster, damageAbsorb.ToString()));
            }

            FinishSequence();
        }

        public class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public DateTime Expires { get; set; }

            public ExpireTimer(Mobile m, TimeSpan delay)
                : base(delay)
            {
                m_Mobile = m;
                Expires = DateTime.UtcNow + delay;
            }

            public void DoExpire(bool message)
            {
                Stop();

                m_Mobile.MeleeDamageAbsorb = 0;

                if (message)
                {
                    m_Mobile.SendLocalizedMessage(1075126); // Your attunement fades.
                    m_Mobile.PlaySound(0x1F8);
                }

                m_Table.Remove(m_Mobile);

                DelayCall(TimeSpan.FromSeconds(120), delegate { m_Mobile.EndAction(typeof(AttuneWeaponSpell)); });
                BuffInfo.RemoveBuff(m_Mobile, BuffIcon.AttuneWeapon);
            }

            protected override void OnTick()
            {
                DoExpire(true);
            }
        }
    }
}