using System;
using Server.Targeting;

namespace Server.Engines.BulkOrders
{
    public class SmallBODTarget : Target
    {
        private readonly SmallBOD m_Deed;
        public SmallBODTarget(SmallBOD deed)
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