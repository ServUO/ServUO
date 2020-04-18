using Server.Targeting;

namespace Server.Engines.BulkOrders
{
    public class SmallBODTarget : Target
    {
        private readonly SmallBOD m_Deed;
        public SmallBODTarget(SmallBOD deed)
            : base(18, false, TargetFlags.None)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_Deed.Deleted || !m_Deed.IsChildOf(from.Backpack))
                return;

            m_Deed.EndCombine(from, targeted);
        }
    }
}