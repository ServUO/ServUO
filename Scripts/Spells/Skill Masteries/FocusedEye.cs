using Server.Network;

namespace Server.Spells.SkillMasteries
{
    public class FocusedEyeSpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Focused Eye", "",
                -1,
                9002
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 20;
        public override int RequiredMana => 20;

        public override SkillName CastSkill => SkillName.Swords;
        public override SkillName DamageSkill => SkillName.Tactics;
        public override bool CheckManaBeforeCast => !HasSpell(Caster, GetType());

        private int _PropertyBonus;

        public FocusedEyeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            SkillMasterySpell spell = GetSpell(Caster, GetType());

            if (spell != null)
            {
                spell.Expire();
                return false;
            }

            if (!CheckWeapon())
            {
                Caster.SendLocalizedMessage(1156006); // You must have a swordsmanship weapon equipped to use this ability.
                return false;
            }

            return base.CheckCast();
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            Caster.PlaySound(0x1FD);
        }

        public override void SendCastEffect()
        {
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                _PropertyBonus = (int)((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value + (GetMasteryLevel() * 40)) / 12);

                Caster.PrivateOverheadMessage(MessageType.Regular, 1150, 1156002, Caster.NetState); // *You focus your eye on your opponents!*
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.FocusedEye, 1156003, 1156004, string.Format("{0}\t{1}", _PropertyBonus.ToString(), ScaleUpkeep().ToString()))); // +~1_VAL~% Hit Chance Increase.<br>Mana Upkeep Cost: ~2_VAL~.

                Caster.PlaySound(0x101);
                Effects.SendTargetParticles(Caster, 0x3789, 1, 40, 2726, 5, 9907, EffectLayer.RightFoot, 0);

                BeginTimer();
            }

            FinishSequence();
        }

        public override void OnExpire()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.FocusedEye);
        }

        public override int PropertyBonus()
        {
            return _PropertyBonus;
        }

        public static int HitChanceBonus(Mobile attacker)
        {
            SkillMasterySpell spell = GetSpell(attacker, typeof(FocusedEyeSpell));

            if (spell != null)
                return spell.PropertyBonus();

            return 0;
        }
    }
}
