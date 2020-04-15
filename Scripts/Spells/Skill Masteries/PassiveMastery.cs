namespace Server.Spells.SkillMasteries
{
    public class PassiveMasterySpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Passive", "",
                -1,
                9002
            );

        public PassiveMasterySpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            return false;
        }

        public override void OnCast()
        {
        }
    }
}