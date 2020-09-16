using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Spells.Spellweaving
{
    public class ImmolatingWeaponSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Immolating Weapon", "Thalshara",
            -1);
        private static readonly Dictionary<Mobile, ImmolatingWeaponEntry> m_WeaponDamageTable = new Dictionary<Mobile, ImmolatingWeaponEntry>();

        public ImmolatingWeaponSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(1.0);
        public override double RequiredSkill => 10.0;
        public override int RequiredMana => 32;
        public static bool IsImmolating(Mobile m, BaseWeapon weapon)
        {
            if (m == null)
                return false;

            return m_WeaponDamageTable.ContainsKey(m) && m_WeaponDamageTable[m].m_Weapon == weapon;
        }

        public static int GetImmolatingDamage(Mobile attacker)
        {
            ImmolatingWeaponEntry entry;

            if (m_WeaponDamageTable.TryGetValue(attacker, out entry))
                return entry.m_Damage;

            return 0;
        }

        public static void DoDelayEffect(Mobile attacker, Mobile target)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(.25), () =>
                {
                    if (m_WeaponDamageTable.ContainsKey(attacker))
                        AOS.Damage(target, attacker, m_WeaponDamageTable[attacker].m_Damage, 0, 100, 0, 0, 0);
                });
        }

        public static void StopImmolating(Mobile mob)
        {
            if (m_WeaponDamageTable.ContainsKey(mob))
            {
                StopImmolating(m_WeaponDamageTable[mob].m_Weapon, mob);
            }
        }

        public static void StopImmolating(BaseWeapon weapon, Mobile mob)
        {
            ImmolatingWeaponEntry entry;

            if (m_WeaponDamageTable.TryGetValue(mob, out entry))
            {
                mob.PlaySound(0x27);

                entry.m_Timer.Stop();

                m_WeaponDamageTable.Remove(mob);

                BuffInfo.RemoveBuff(mob, BuffIcon.ImmolatingWeapon);

                weapon.InvalidateProperties();
            }
        }

        public override bool CheckCast()
        {
            BaseWeapon weapon = Caster.Weapon as BaseWeapon;

            if (Caster.Player && (weapon == null || weapon is Fists || weapon is BaseRanged))
            {
                Caster.SendLocalizedMessage(1060179); // You must be wielding a weapon to use this ability!
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            BaseWeapon weapon = Caster.Weapon as BaseWeapon;

            if (Caster.Player && (weapon == null || weapon is Fists || weapon is BaseRanged))
            {
                Caster.SendLocalizedMessage(1060179); // You must be wielding a weapon to use this ability!
            }
            else if (CheckSequence())
            {
                Caster.PlaySound(0x5CA);
                Caster.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);

                if (!IsImmolating(Caster, weapon)) // On OSI, the effect is not re-applied
                {
                    double skill = Caster.Skills.Spellweaving.Value;

                    int duration = 10 + (int)(skill / 24) + FocusLevel;
                    int damage = 5 + (int)(skill / 24) + FocusLevel;

                    Timer stopTimer = Timer.DelayCall(TimeSpan.FromSeconds(duration), StopImmolating, Caster);

                    m_WeaponDamageTable[Caster] = new ImmolatingWeaponEntry(damage, stopTimer, weapon);

                    BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.ImmolatingWeapon, 1071028, 1153782, damage.ToString()));

                    weapon.InvalidateProperties();
                }
            }

            FinishSequence();
        }

        private class ImmolatingWeaponEntry
        {
            public readonly int m_Damage;
            public readonly Timer m_Timer;
            public readonly BaseWeapon m_Weapon;

            public ImmolatingWeaponEntry(int damage, Timer stopTimer, BaseWeapon weapon)
            {
                m_Damage = damage;
                m_Timer = stopTimer;
                m_Weapon = weapon;
            }
        }
    }
}