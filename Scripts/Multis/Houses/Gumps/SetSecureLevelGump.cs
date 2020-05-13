
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    public interface ISecurable
    {
        SecureLevel Level { get; set; }
    }

    public class SetSecureLevelGump : Gump
    {
        private readonly ISecurable m_Info;

        public SetSecureLevelGump(Mobile from, ISecurable info, BaseHouse house)
            : base(50, 50)
        {
            m_Info = info;
            Mobile owner = house.Owner;

            AddPage(0);

            int offset = 20;

            AddBackground(0, 0, 220, 160 + offset, 5054);

            AddImageTiled(10, 10, 200, 20, 5124);
            AddImageTiled(10, 40, 200, 20, 5124);
            AddImageTiled(10, 70, 200, 80 + offset, 5124);

            AddAlphaRegion(10, 10, 200, 140 + offset);

            AddHtmlLocalized(10, 10, 200, 20, 1061276, 32767, false, false); // <CENTER>SET ACCESS</CENTER>
            AddHtmlLocalized(10, 40, 100, 20, 1041474, 32767, false, false); // Owner:

            AddLabel(110, 40, 1152, owner == null ? "" : owner.Name);

            AddButton(10, 70, GetFirstID(SecureLevel.Owner), 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 70, 150, 20, 1061277, GetColor(SecureLevel.Owner), false, false); // Owner Only

            AddButton(10, 90, GetFirstID(SecureLevel.CoOwners), 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 90, 150, 20, 1061278, GetColor(SecureLevel.CoOwners), false, false); // Co-Owners

            AddButton(10, 110, GetFirstID(SecureLevel.Friends), 4007, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 110, 150, 20, 1061279, GetColor(SecureLevel.Friends), false, false); // Friends

            if (from != null && owner == from && from.Guild != null && ((PlayerMobile)from).GuildRank.Rank >= 1)
            {
                AddButton(10, 130, GetFirstID(SecureLevel.Guild), 4007, 5, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 130, 150, 20, 1063455, GetColor(SecureLevel.Guild), false, false); // Guild Members
            }

            AddButton(10, 130 + offset, GetFirstID(SecureLevel.Anyone), 4007, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 130 + offset, 150, 20, 1061626, GetColor(SecureLevel.Anyone), false, false); // Anyone
        }

        public int GetColor(SecureLevel level)
        {
            return (m_Info.Level == level) ? 0x7F18 : 0x7FFF;
        }

        public int GetFirstID(SecureLevel level)
        {
            return (m_Info.Level == level) ? 4006 : 4005;
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            SecureLevel level = m_Info.Level;

            switch (info.ButtonID)
            {
                case 1:
                    level = SecureLevel.Owner;
                    break;
                case 2:
                    level = SecureLevel.CoOwners;
                    break;
                case 3:
                    level = SecureLevel.Friends;
                    break;
                case 4:
                    level = SecureLevel.Anyone;
                    break;
                case 5:
                    level = SecureLevel.Guild;
                    break;
            }

            if (m_Info.Level == level)
            {
                state.Mobile.SendLocalizedMessage(1061281); // Access level unchanged.
            }
            else
            {
                m_Info.Level = level;
                state.Mobile.SendLocalizedMessage(1061280); // New access level set.
            }
        }
    }
}
