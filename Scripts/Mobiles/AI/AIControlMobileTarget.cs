#region References
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;
#endregion

namespace Server.Targets
{
    public class AIControlMobileTarget : Target
    {
        private readonly List<BaseAI> m_List;
        private readonly OrderType m_Order;
        private readonly BaseCreature m_Mobile;

        public AIControlMobileTarget(BaseAI ai, OrderType order)
            : base(-1, false, (order == OrderType.Attack ? TargetFlags.Harmful : TargetFlags.None))
        {
            m_List = new List<BaseAI>();
            m_Order = order;

            AddAI(ai);
            m_Mobile = ai.m_Mobile;
        }

        public OrderType Order => m_Order;

        public void AddAI(BaseAI ai)
        {
            if (!m_List.Contains(ai))
                m_List.Add(ai);
        }

        protected override void OnTarget(Mobile from, object o)
        {
            if (o is IDamageable)
            {
                IDamageable dam = o as IDamageable;

                for (int i = 0; i < m_List.Count; ++i)
                    m_List[i].EndPickTarget(from, dam, m_Order);
            }
            else if (o is MoonglowDonationBox && m_Order == OrderType.Transfer && from is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)from;
                MoonglowDonationBox box = (MoonglowDonationBox)o;

                pm.SendGump(new ConfirmTransferPetGump(box, from.Location, m_Mobile));
            }
        }
    }
}