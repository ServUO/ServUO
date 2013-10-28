using System;
using Server.Gumps;

namespace Server.Spells.Mystic
{
    public class SpellTriggerSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Spell Trigger", "In Vas Ort Ex ",
            230,
            9022,
            Reagent.Garlic,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk,
            Reagent.DragonBlood);
        public SpellTriggerSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override int RequiredMana
        {
            get
            {
                return 14;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 45;
            }
        }
        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                if (this.Caster.HasGump(typeof(SpellTriggerGump)))
                    this.Caster.CloseGump(typeof(SpellTriggerGump));

                this.Caster.SendGump(new SpellTriggerGump(this.Caster));
            }
        }
    }
}
/*




*/