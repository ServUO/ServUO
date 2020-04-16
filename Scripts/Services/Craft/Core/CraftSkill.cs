namespace Server.Engines.Craft
{
    public class CraftSkill
    {
        private readonly SkillName m_SkillToMake;
        private readonly double m_MinSkill;
        private readonly double m_MaxSkill;
        public CraftSkill(SkillName skillToMake, double minSkill, double maxSkill)
        {
            m_SkillToMake = skillToMake;
            m_MinSkill = minSkill;
            m_MaxSkill = maxSkill;
        }

        public SkillName SkillToMake => m_SkillToMake;

        public double MinSkill => m_MinSkill;

        public double MaxSkill => m_MaxSkill;
    }
}