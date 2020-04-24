namespace Server.Engines.Craft
{
    public class CraftResCol : System.Collections.CollectionBase
    {
        public void Add(CraftRes craftRes)
        {
            List.Add(craftRes);
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

        public CraftRes GetAt(int index)
        {
            return (CraftRes)List[index];
        }
    }
}
