using System;
using Server.Targeting;

namespace Server.Spells.Mystic
{
    public class SleepSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Sleep", "In Zu",
            230,
            9022,
            Reagent.Bloodmoss,
            Reagent.Garlic,
            Reagent.SulfurousAsh,
            Reagent.DragonBlood);
        public SleepSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // Hurls a magical boulder at the Target, dealing physical damage. 
        // This spell also has a chance to knockback and stun a player Target. 
        public override int RequiredMana
        {
            get
            {
                return 9;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 20;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new MysticSpellTarget(this, TargetFlags.Harmful);
        }

        public override void OnTarget(Object o)
        {
            Mobile target = o as Mobile;

            if (target == null)
            {
                return;
            }
            else if (this.CheckHSequence(target))
            {
                //duration = 7.0 + (Caster.Skills[SkillName.Mysticism].Value * 0.2);
                target.Paralyze(TimeSpan.FromSeconds(12));
                target.Sleep(TimeSpan.FromSeconds(12));
                target.Say("ZZZzzzzz");///////
            }

            this.FinishSequence();
        }
    }
}