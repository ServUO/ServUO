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

            if (m_Item is IMountStatuette)
            {
                m = Activator.CreateInstance(((IMountStatuette)m_Item).MountType) as BaseMount;
            }

            if (m != null)
            {
                if ((from.Followers + m.ControlSlots) >= from.FollowersMax)
                {
                    m.Delete();
                    from.SendLocalizedMessage(1114321); // You have too many followers to control that pet.
                }
                else
                {
                    m.SetControlMaster(from);
                    m.IsBonded = true;
                    m.MoveToWorld(from.Location, from.Map);
                    m_Item.Delete();
                }
            }
        }
    }
}

namespace Server.Mobiles
{
    public interface IMountStatuette
    {
        Type MountType { get; }
    }
}
