using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.SkillMasteries
{
    public class ShadowSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Shadow", "",
                -1,
                9002
            );

        public override double UpKeep { get { return 4; } }
        public override int RequiredMana { get { return 10; } }

        public override SkillName CastSkill { get { return SkillName.Ninjitsu; } }
        public override SkillName DamageSkill { get { return SkillName.Stealth; } }

        public ShadowSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            SkillMasterySpell spell = GetSpell(Caster, this.GetType());

            if (spell != null)
            {
                spell.Expire();
                BuffInfo.RemoveBuff(Caster, BuffIcon.Shadow);
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.FixedParticles(0x3709, 10, 30, 5052, 2050, 7, EffectLayer.LeftFoot, 0);
                Caster.PlaySound(0x22F);

                double skill = (Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value + (GetMasteryLevel() * 40)) / 3;
                int duration = (int)(skill / 3.4);

                Expires = DateTime.UtcNow + TimeSpan.FromSeconds(duration);
                BeginTimer();

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Shadow, 1155910, 1156059, TimeSpan.FromSeconds(duration), Caster)); // Increases difficulty to be detected while hidden. <br>Increases difficulty to unhide from taking damage.
            }

            FinishSequence();
        }

        public static double GetDifficultyFactor(Mobile m)
        {
            ShadowSpell spell = GetSpell(m, typeof(ShadowSpell)) as ShadowSpell;

            if(spell != null)
                return ((spell.Caster.Skills[spell.CastSkill].Value + spell.Caster.Skills[spell.DamageSkill].Value + (spell.GetMasteryLevel() * 40)) / 3) / 100;

            return 0.0;
        }

        protected override void DoEffects()
        {
            Caster.FixedParticles(0x376A, 9, 32, 5005, 2123, 0, EffectLayer.Waist, 0);
        }
    }
}