using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.SkillMasteries
{
    public class FocusedEyeSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Focused Eye", "",
                -1,
                9002
            );

        public override double RequiredSkill { get { return 90; } }
        public override double UpKeep { get { return 20; } }
        public override int RequiredMana { get { return 20; } }

        public override SkillName CastSkill { get { return SkillName.Swords; } }
        public override SkillName DamageSkill { get { return SkillName.Tactics; } }

        private int _PropertyBonus;

        public FocusedEyeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            SkillMasterySpell spell = GetSpell(Caster, this.GetType());

            if (spell != null)
            {
                spell.Expire();
                return false;
            }

            BaseWeapon weapon = GetWeapon();

            if (weapon != null && weapon.DefSkill != CastSkill)
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
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.FocusedEye, 1156003, 1156004, String.Format("{0}\t{1}", _PropertyBonus.ToString(), ScaleUpkeep().ToString()))); // +~1_VAL~% Hit Chance Increase.<br>Mana Upkeep Cost: ~2_VAL~.

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