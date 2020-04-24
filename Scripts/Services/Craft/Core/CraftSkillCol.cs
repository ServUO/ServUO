namespace Server.Engines.Craft
{
    public class CraftSkillCol : System.Collections.CollectionBase
    {
        public void Add(CraftSkill craftSkill)
        {
            List.Add(craftSkill);
        }

        public void Remove(int index)
        {
            if (index > Count - 1 || index < 0)
            {
            }
            else
            {
                List.RemoveAt(index);
            }
        }

        public CraftSkill GetAt(int index)
        {
            return (CraftSkill)List[index];
        }
    }
}
