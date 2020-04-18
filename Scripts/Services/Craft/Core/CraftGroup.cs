namespace Server.Engines.Craft
{
    public class CraftGroup
    {
        private readonly CraftItemCol m_arCraftItem;
        private readonly string m_NameString;
        private readonly int m_NameNumber;
        public CraftGroup(TextDefinition groupName)
        {
            m_NameNumber = groupName;
            m_NameString = groupName;
            m_arCraftItem = new CraftItemCol();
        }

        public CraftItemCol CraftItems => m_arCraftItem;

        public string NameString => m_NameString;

        public int NameNumber => m_NameNumber;

        public void AddCraftItem(CraftItem craftItem)
        {
            m_arCraftItem.Add(craftItem);
        }
    }
}