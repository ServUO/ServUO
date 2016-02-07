using System;
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
            this.m_From = from;
            this.m_Food = food;
        }

        public override void OnClick()
        {
            if (this.m_Food.Deleted || !this.m_Food.Movable || !this.m_From.CheckAlive() || !this.m_Food.CheckItemUse(this.m_From))
                return;

            this.m_Food.Eat(this.m_From);
        }
    }
}