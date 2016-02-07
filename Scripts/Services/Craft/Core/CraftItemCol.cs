using System;

namespace Server.Engines.Craft
{
    public class CraftItemCol : System.Collections.CollectionBase
    {
        public CraftItemCol()
        {
        }

        public int Add(CraftItem craftItem)
        {
            return this.List.Add(craftItem);
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

        public CraftItem GetAt(int index)
        {
            return (CraftItem)this.List[index];
        }

        public CraftItem SearchForSubclass(Type type)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                CraftItem craftItem = (CraftItem)this.List[i];

                if (craftItem.ItemType == type || type.IsSubclassOf(craftItem.ItemType))
                    return craftItem;
            }

            return null;
        }

        public CraftItem SearchFor(Type type)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                CraftItem craftItem = (CraftItem)this.List[i];
                if (craftItem.ItemType == type)
                {
                    return craftItem;
                }
            }
            return null;
        }
    }
}