using System;
using Server.Mobiles;

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

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 38.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
            }
        }
        public override int Sound
        {
            get
            {
                return 0x217;
            }
        }
    }
}