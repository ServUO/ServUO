#region

using Server.Multis;

#endregion

namespace Server.ContextMenus
{
    public enum HouseAccessType
    {
        None,
        Friend,
        CoOwner,
        Owner
    }

    public class EjectPlayerEntry : ContextMenuEntry
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        private readonly BaseHouse m_TargetHouse;

        public EjectPlayerEntry(Mobile from, Mobile target)
            : base(6206, 12)
        {
            m_From = from;
            m_Target = target;
            m_TargetHouse = BaseHouse.FindHouseAt(m_Target);
        }

        public static bool CheckAccessible(Mobile from, Mobile target)
        {
            var house = BaseHouse.FindHouseAt(target);

            if (house != null)
            {
                var fromaccess = GetAccess(from, house);
                var targetaccess = GetAccess(target, house);

                if (house.IsFriend(from) && fromaccess > targetaccess)
                {
                    return true;
                }
            }

            return false;
        }

        public static HouseAccessType GetAccess(Mobile from, BaseHouse house)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                return HouseAccessType.Owner; // Staff can access anything
            }

            var type = HouseAccessType.None;

            if (house != null)
            {
                if (house.IsOwner(from))
                {
                    type = HouseAccessType.Owner;
                }
                else if (house.IsCoOwner(from))
                {
                    type = HouseAccessType.CoOwner;
                }
                else if (house.IsFriend(from))
                {
                    type = HouseAccessType.Friend;
                }
            }

            return type;
        }

        public override void OnClick()
        {
            if (!m_From.Alive || m_TargetHouse.Deleted || !m_TargetHouse.IsFriend(m_From))
            {
                return;
            }

            if (m_Target is Mobile)
            {
                m_TargetHouse.Kick(m_From, m_Target);
            }
        }
    }
}
