using System;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.SkillMasteries
{
    public class CalledShotSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Called Shot", "",
                -1,
                9002
            );

        public override double RequiredSkill { get { return 90; } }
        public override double UpKeep { get { return 0; } }
        public override int RequiredMana { get { return 40; } }

        public override SkillName CastSkill { get { return SkillName.Throwing; } }
        public override SkillName DamageSkill { get { return SkillName.Tactics; } }

        private int _HCIBonus;
        private int _DamageBonus;

        public CalledShotSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (IsInCooldown(Caster, this.GetType()))
                return false;

            if (!CheckWeapon())
            {
                Caster.SendLocalizedMessage(1156016); // You must have a throwing weapon equipped to use this ability.
                return false;
            }

            CalledShotSpell spell = GetSpell(Caster, this.GetType()) as CalledShotSpell;

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

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.CalledShot, 1156025, 1156026, duration, Caster, String.Format("{0}\t{1}", _HCIBonus.ToString(), _DamageBonus.ToString())));
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

            damage = damage + (int)((double)damage * ((double)_DamageBonus / 100.0));

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
