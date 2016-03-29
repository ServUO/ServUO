using System;
using Server.Items;

namespace Server.Targeting
{
    public class WandTarget : Target
    {
        private readonly BaseWand m_Item;
        public WandTarget(BaseWand item)
            : base(6, false, TargetFlags.None)
        {
            this.m_Item = item;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            this.m_Item.DoWandTarget(from, targeted);
        }

        private static int GetOffset(Mobile caster)
        {
            return 5 + (int)(caster.Skills[SkillName.Magery].Value * 0.02);
        }
    }
}