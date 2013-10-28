using System;

namespace Server.Engines.Craft
{
    public class CraftResCol : System.Collections.CollectionBase
    {
        public CraftResCol()
        {
        }

        public void Add(CraftRes craftRes)
        {
            this.List.Add(craftRes);
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

        public CraftRes GetAt(int index)
        {
            return (CraftRes)this.List[index];
        }
    }
}