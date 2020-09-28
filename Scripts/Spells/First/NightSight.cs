using Server.Targeting;

namespace Server.Spells.First
{
    public class NightSightSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Night Sight", "In Lor",
            236,
            9031,
            Reagent.SulfurousAsh,
            Reagent.SpidersSilk);
        public NightSightSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.First;

        public override void OnCast()
        {
            Caster.Target = new NightSightTarget(this);
        }

        public void Target(Mobile targ)
        {
            SpellHelper.Turn(Caster, targ);

            targ.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
            targ.PlaySound(0x1E3);

            if (targ.BeginAction(typeof(LightCycle)))
            {
                new LightCycle.NightSightTimer(targ).Start();
                int level = (int)(LightCycle.DungeonLevel * targ.Skills[SkillName.Magery].Value);

                if (level < 0)
                    level = 0;

                targ.LightLevel = level;

                BuffInfo.AddBuff(targ, new BuffInfo(BuffIcon.NightSight, 1075643));	//Night Sight/You ignore lighting effects
            }
        }

        private class NightSightTarget : Target
        {
            private readonly NightSightSpell m_Spell;

            public NightSightTarget(NightSightSpell spell)
                : base(12, false, TargetFlags.Beneficial)
            {
                m_Spell = spell;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile && m_Spell.CheckBSequence((Mobile)targeted))
                {
                    m_Spell.Target((Mobile)targeted);
                }

                m_Spell.FinishSequence();
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Spell.FinishSequence();
            }
        }
    }
}
