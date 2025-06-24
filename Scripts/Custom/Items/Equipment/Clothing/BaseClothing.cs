// **********
// ServUO - BaseClothing.cs
// **********

namespace Server.Items
{
    public partial class BaseClothing
    {
        private string m_CrafterName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string CrafterName
        {
            get => m_CrafterName;
            set
            {
                m_CrafterName = value;
                InvalidateProperties();
            }
        }
    }
}
