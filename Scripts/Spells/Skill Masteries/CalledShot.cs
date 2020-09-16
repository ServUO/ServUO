using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Spells.SkillMasteries
{
    public class CalledShotSpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Called Shot", "",
                -1,
                9002
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 0;
        public override int RequiredMana => 40;

        public override SkillName CastSkill => SkillName.Throwing;
        public override SkillName DamageSkill => SkillName.Tactics;
        public override bool CheckManaBeforeCast => !HasSpell(Caster, GetType());

        private int _HCIBonus;
        private int _DamageBonus;

        public CalledShotSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (IsInCooldown(Caster, GetType()))
                return false;

            if (!CheckWeapon())
            {
                Caster.SendLocalizedMessage(1156016); // You must have a throwing weapon equipped to use this ability.
                return false;
            }

            CalledShotSpell spell = GetSpell(Caster, GetType()) as CalledShotSpell;

            if (spell != null)
            {
                spell.Expire();
                return false;
            }

            return base.CheckCast();
        }

        public override void SendCastEffect()
        {
            Caster.PlaySound(0x5AA);
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.PlaySound(0x101);
                Caster.FixedEffect(0x37C4, 10, 40, 2720, 3);

                Caster.PrivateOverheadMessage(MessageType.Regular, 1150, 1156024, Caster.NetState); // *You call your next shot...*

                TimeSpan duration = TimeSpan.FromSeconds(10);

                _HCIBonus = (int)(Caster.Skills[DamageSkill].Value / 2.66);
                _DamageBonus = (int)(Caster.Skills[CastSkill].Value / 1.6);

                Expires = DateTime.UtcNow + duration;
                BeginTimer();

                AddToCooldown(TimeSpan.FromSeconds(60));

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.CalledShot, 1156025, 1156026, duration, Caster, string.Format("{0}\t{1}", _HCIBonus.ToString(), _DamageBonus.ToString())));
                //Hit Chance Increase: ~1_VAL~%<br>Damage Increase: ~2_VAL~%
            }

            FinishSequence();
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.CalledShot);
        }

        public override void OnHit(Mobile defender, ref int damage)
        {
            if (SpecialMove.GetCurrentMove(Caster) != null)
                return;

            damage = damage + (int)(damage * (_DamageBonus / 100.0));

            if (defender is PlayerMobile && damage > 100)
                damage = 100;
        }

        public static int GetHitChanceBonus(Mobile m)
        {
            CalledShotSpell spell = GetSpell(m, typeof(CalledShotSpell)) as CalledShotSpell;

            if (spell != null)
                return spell._HCIBonus;

            return 0;
        }
    }
}
