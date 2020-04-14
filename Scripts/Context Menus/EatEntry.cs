using Server.Items;

namespace Server.ContextMenus
{
    public class EatEntry : ContextMenuEntry
    {
        private readonly Mobile m_From;
        private readonly Food m_Food;

        public EatEntry(Mobile from, Food food)
            : base(6135, 1)
        {
            m_From = from;
            m_Food = food;
        }

        public override void OnClick()
        {
            m_Food.TryEat(m_From);
        }
    }
}