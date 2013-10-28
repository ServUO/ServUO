using System;

namespace Server.Engines.Craft
{
    public class CraftSkill
    {
        private readonly SkillName m_SkillToMake;
        private readonly double m_MinSkill;
        private readonly double m_MaxSkill;
        public CraftSkill(SkillName skillToMake, double minSkill, double maxSkill)
        {
            this.m_SkillToMake = skillToMake;
            this.m_MinSkill = minSkill;
            this.m_MaxSkill = maxSkill;
        }

        public SkillName SkillToMake
        {
            get
            {
                return this.m_SkillToMake;
            }
        }
        public double MinSkill
        {
            get
            {
                return this.m_MinSkill;
            }
        }
        public double MaxSkill
        {
            get
            {
                return this.m_MaxSkill;
            }
        }
    }
}