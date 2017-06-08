using System;
using Server.Mobiles;

namespace Server.Gumps
{
    public class ConfirmMountStatuetteGump : BaseConfirmGump
    {
        private readonly Item m_Item;

        public ConfirmMountStatuetteGump(Item item)
            : base()
        {
            m_Item = item;
        }

        public override int LabelNumber { get { return 1075084; } } // This statuette will be destroyed when its trapped creature is summoned. The creature will be bonded to you but will disappear if released. <br><br>Do you wish to proceed?

        public override void Confirm(Mobile from)
        {
            if (m_Item == null || m_Item.Deleted)
                return;

            BaseMount m = null;

            if (m_Item is WindrunnerStatue)
            {
                m = new Windrunner();
            }

            if (m_Item is LasherStatue)
            {
                m = new Lasher();
            }

            m.SetControlMaster(from);
            m.IsBonded = true;
            m.MoveToWorld(from.Location, from.Map);
            m_Item.Delete();
        }
    }
}