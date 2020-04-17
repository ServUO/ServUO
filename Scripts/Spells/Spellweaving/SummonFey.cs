using Server.Mobiles;
using System;

namespace Server.Spells.Spellweaving
{
    public class SummonFeySpell : ArcaneSummon<ArcaneFey>
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Summon Fey", "Alalithra",
            -1);
        public SummonFeySpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(1.5);
        public override double RequiredSkill => 38.0;
        public override int RequiredMana => 10;
        public override int Sound => 0x217;
    }
}