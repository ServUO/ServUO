using System;
using System.Collections.Generic;
using System.Text;
using Server.ContextMenus;
using Server.Items;

namespace Server.ACC.YS
{
    public class StairRefundEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private YardStair m_Stair;
        private int value = 0;

        public StairRefundEntry(Mobile from, YardStair stair, int price)
            : base(6104, 9)
        {
            m_From = from;
            m_Stair = stair;
            value = price;
        }

        public override void OnClick()
        {
            m_Stair.Refund();
        }
    }

    public class YardSecurityEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private BaseDoor m_Gate;

        public YardSecurityEntry(Mobile from, YardGate gate)
            : base(6203, 9)
        {
            m_From = from;
            m_Gate = gate;
        }

        public override void OnClick()
        {
            m_From.SendGump(new YardSecurityGump(m_From, m_Gate));
        }
    }

    public class RefundEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private YardGate m_Gate;
        private int value = 0;

        public RefundEntry(Mobile from, YardGate gate, int price)
            : base(6104, 9)
        {
            m_From = from;
            m_Gate = gate;
            value = price;
        }

        public override void OnClick()
        {
            m_Gate.Refund();
        }
    }
}
