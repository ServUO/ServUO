using System;

/*Toggle ability that grants the poisoner a reduction to 
  poison level when poisoned at a stamina cost based on mastery level.*/

namespace Server.Spells.SkillMasteries
{
    public class ToleranceSpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Tolerence", "",
                -1,
                9002
            );

        public override int RequiredMana => 20;
        public override SkillName CastSkill => SkillName.Poisoning;
        public override bool CheckManaBeforeCast => !HasSpell(Caster, GetType());

        public ToleranceSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            ToleranceSpell spell = GetSpell(Caster, typeof(ToleranceSpell)) as ToleranceSpell;

            if (spell != null)
            {
                spell.Expire();
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.SendSound(0xF6);
                Effects.SendTargetParticles(Caster, 0x3709, 10, 30, 1166, 0, 9907, EffectLayer.LeftFoot, 0);

                BeginTimer();

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Tolerance, 1155926, 1156063)); // Reduces poison strength when poisoned at the cost of stamina.
            }

            FinishSequence();
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.Tolerance);
        }

        public static bool OnPoisonApplied(Mobile m)
        {
            ToleranceSpell spell = GetSpell(m, typeof(ToleranceSpell)) as ToleranceSpell;

            if (spell != null)
            {
                double stamCost = (m.Skills[spell.CastSkill].Base + ((MasteryInfo.GetMasteryLevel(m, SkillName.Poisoning) * 30) + 10)) / 2;

                stamCost /= 4;
                stamCost = Math.Max(18, (25 - stamCost) + 18);

                if (m.Stam < (int)stamCost)
                {
                    spell.Caster.SendLocalizedMessage(1156036, ((int)stamCost).ToString());  // You must have at least ~1_STAM_REQUIREMENT~ Stamina to use this ability.
                    return false;
                }

                spell.Caster.Stam -= (int)stamCost;
                return true;
            }

            return false;
        }
    }
}
