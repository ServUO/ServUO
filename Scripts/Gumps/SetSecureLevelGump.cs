using System;
using Server.Guilds;
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
        public SetSecureLevelGump(Mobile owner, ISecurable info, BaseHouse house)
            : base(50, 50)
        {
            this.m_Info = info;

            this.AddPage(0);

            int offset = (Guild.NewGuildSystem) ? 20 : 0;

            this.AddBackground(0, 0, 220, 160 + offset, 5054);

            this.AddImageTiled(10, 10, 200, 20, 5124);
            this.AddImageTiled(10, 40, 200, 20, 5124);
            this.AddImageTiled(10, 70, 200, 80 + offset, 5124);

            this.AddAlphaRegion(10, 10, 200, 140 + offset);

            this.AddHtmlLocalized(10, 10, 200, 20, 1061276, 32767, false, false); // <CENTER>SET ACCESS</CENTER>
            this.AddHtmlLocalized(10, 40, 100, 20, 1041474, 32767, false, false); // Owner:

            this.AddLabel(110, 40, 1152, owner == null ? "" : owner.Name);

            this.AddButton(10, 70, this.GetFirstID(SecureLevel.Owner), 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 70, 150, 20, 1061277, this.GetColor(SecureLevel.Owner), false, false); // Owner Only

            this.AddButton(10, 90, this.GetFirstID(SecureLevel.CoOwners), 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 90, 150, 20, 1061278, this.GetColor(SecureLevel.CoOwners), false, false); // Co-Owners

            this.AddButton(10, 110, this.GetFirstID(SecureLevel.Friends), 4007, 3, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 110, 150, 20, 1061279, this.GetColor(SecureLevel.Friends), false, false); // Friends

            Mobile houseOwner = house.Owner;
            if (Guild.NewGuildSystem && house != null && houseOwner != null && houseOwner.Guild != null && ((Guild)houseOwner.Guild).Leader == houseOwner)	//Only the actual House owner AND guild master can set guild secures
            {
                this.AddButton(10, 130, this.GetFirstID(SecureLevel.Guild), 4007, 5, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 130, 150, 20, 1063455, this.GetColor(SecureLevel.Guild), false, false); // Guild Members
            }

            this.AddButton(10, 130 + offset, this.GetFirstID(SecureLevel.Anyone), 4007, 4, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(45, 130 + offset, 150, 20, 1061626, this.GetColor(SecureLevel.Anyone), false, false); // Anyone
        }

        public int GetColor(SecureLevel level)
        {
            return (this.m_Info.Level == level) ? 0x7F18 : 0x7FFF;
        }

        public int GetFirstID(SecureLevel level)
        {
            return (this.m_Info.Level == level) ? 4006 : 4005;
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            SecureLevel level = this.m_Info.Level;

            switch ( info.ButtonID )
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

            if (this.m_Info.Level == level)
            {
                state.Mobile.SendLocalizedMessage(1061281); // Access level unchanged.
            }
            else
            {
                this.m_Info.Level = level;
                state.Mobile.SendLocalizedMessage(1061280); // New access level set.
            }
        }
    }
}