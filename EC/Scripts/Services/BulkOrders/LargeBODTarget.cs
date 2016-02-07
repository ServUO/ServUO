using System;
using Server.Targeting;

namespace Server.Engines.BulkOrders
{
    public class LargeBODTarget : Target
    {
        private readonly LargeBOD m_Deed;
        public LargeBODTarget(LargeBOD deed)
            : base(18, false, TargetFlags.None)
        {
            this.m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (this.m_Deed.Deleted || !this.m_Deed.IsChildOf(from.Backpack))
                return;

            this.m_Deed.EndCombine(from, targeted);
        }
    }
}