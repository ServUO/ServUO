using System;
using Server.Multis;

namespace Server.ContextMenus
{
    public class EjectPlayerEntry : ContextMenuEntry
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        private readonly BaseHouse m_TargetHouse;
        public EjectPlayerEntry(Mobile from, Mobile target)
            : base(6206, 12)
        {
            this.m_From = from;
            this.m_Target = target;
            this.m_TargetHouse = BaseHouse.FindHouseAt(this.m_Target);
        }

        public override void OnClick()
        { 
            if (!this.m_From.Alive || this.m_TargetHouse.Deleted || !this.m_TargetHouse.IsFriend(this.m_From))
                return;

            if (this.m_Target is Mobile)
            {
                this.m_TargetHouse.Kick(this.m_From, (Mobile)this.m_Target);
            }
        }
    }
}