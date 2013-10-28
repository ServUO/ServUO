using System;

namespace Server.Engines.Craft
{
    public class CraftSkillCol : System.Collections.CollectionBase
    {
        public CraftSkillCol()
        {
        }

        public void Add(CraftSkill craftSkill)
        {
            this.List.Add(craftSkill);
        }

        public void Remove(int index)
        {
            if (index > this.Count - 1 || index < 0)
            {
            }
            else
            {
                this.List.RemoveAt(index);
            }
        }

        public CraftSkill GetAt(int index)
        {
            return (CraftSkill)this.List[index];
        }
    }
}