using System;
using Server.Targeting;

namespace Server.Spells.Mystic
{
    public class PurgeMagicSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Purge", "An Ort Sanct ",
            230,
            9022,
            Reagent.Garlic,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh,
            Reagent.FertileDirt);
        public PurgeMagicSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // Attempts to remove a beneficial ward from the Target, chosen randomly.
        public override int RequiredMana
        {
            get
            {
                return 6;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 8;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new MysticSpellTarget(this, TargetFlags.Harmful);
        }

        public override void OnTarget(Object o)
        {
            if (this.CheckSequence())
            {
                // WHAT THE HECK IS THIS SUPPOSED TO DO?!?!?!?!?!?!?!!?!?!?!?!?
                this.Caster.PlaySound(0x656);
            }
        }
    }
}
/*




*/