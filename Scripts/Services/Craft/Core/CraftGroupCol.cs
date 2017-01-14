using System;

namespace Server.Engines.Craft
{
    public class CraftGroupCol : System.Collections.CollectionBase
    {
        public CraftGroupCol()
        {
        }

        public int Add(CraftGroup craftGroup)
        {
            return this.List.Add(craftGroup);
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

        public CraftGroup GetAt(int index)
        {
            return index >= 0 && index < List.Count ? (CraftGroup)List[index] : null;
        }

        public int SearchFor(TextDefinition groupName)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                CraftGroup craftGroup = (CraftGroup)this.List[i];

                int nameNumber = craftGroup.NameNumber;
                string nameString = craftGroup.NameString;

                if ((nameNumber != 0 && nameNumber == groupName.Number) || (nameString != null && nameString == groupName.String))
                    return i;
            }

            return -1;
        }
    }
}