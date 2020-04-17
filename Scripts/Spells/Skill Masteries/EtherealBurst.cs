using System;

namespace Server.Spells.SkillMasteries
{
    public class EtherealBurstSpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Ethereal Blast", "Uus Ort Grav",
                -1,
                9002,
                Reagent.Bloodmoss,
                Reagent.Ginseng,
                Reagent.MandrakeRoot
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 0;
        public override int RequiredMana => 0;
        public override bool PartyEffects => false;

        public override SkillName CastSkill => SkillName.Magery;
        public override SkillName DamageSkill => SkillName.EvalInt;

        public EtherealBurstSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.Mana = Caster.ManaMax;

                int duration = 120;
                double skill = ((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 2.1) + GetMasteryLevel() * 2;

                if (skill >= 120)
                    duration = 30;

                if (skill >= 100)
                    duration = 60;

                if (duration >= 60)
                    duration = 90;

                AddToCooldown(TimeSpan.FromMinutes(duration));

                Caster.PlaySound(0x102);
                Effects.SendTargetParticles(Caster, 0x376A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);
                Caster.SendLocalizedMessage(1155789); // You feel completely rejuvinated!
            }

            FinishSequence();
        }
    }
}