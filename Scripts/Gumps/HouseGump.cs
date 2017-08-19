using System;
using System.Collections.Generic;
using Server.Multis;
using Server.Network;
using Server.Prompts;

namespace Server.Gumps
{
    public class HouseListGump : Gump
    {
        private readonly BaseHouse m_House;
        public HouseListGump(int number, List<Mobile> list, BaseHouse house, bool accountOf)
            : base(20, 30)
        {
            if (house.Deleted)
                return;

            m_House = house;

            AddPage(0);

            AddBackground(0, 0, 420, 430, 5054);
            AddBackground(10, 10, 400, 410, 3000);

            AddButton(20, 388, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 388, 300, 20, 1011104, false, false); // Return to previous menu

            AddHtmlLocalized(20, 20, 350, 20, number, false, false);

            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 16) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            AddButton(370, 20, 4005, 4007, 0, GumpButtonType.Page, (i / 16) + 1);
                        }

                        AddPage((i / 16) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            AddButton(340, 20, 4014, 4016, 0, GumpButtonType.Page, i / 16);
                        }
                    }

                    Mobile m = list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    AddLabel(55, 55 + ((i % 16) * 20), 0, accountOf && m.Player && m.Account != null ? String.Format("Account of {0}", name) : name);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_House.Deleted)
                return;

            Mobile from = state.Mobile;

            from.SendGump(new HouseGump(from, m_House));
        }
    }

    public class HouseRemoveGump : Gump
    {
        private readonly BaseHouse m_House;
        private readonly List<Mobile> m_List;
        private readonly List<Mobile> m_Copy;
        private readonly int m_Number;
        private readonly bool m_AccountOf;
        public HouseRemoveGump(int number, List<Mobile> list, BaseHouse house, bool accountOf)
            : base(20, 30)
        {
            if (house.Deleted)
                return;

            m_House = house;
            m_List = list;
            m_Number = number;
            m_AccountOf = accountOf;

            AddPage(0);

            AddBackground(0, 0, 420, 430, 5054);
            AddBackground(10, 10, 400, 410, 3000);

            AddButton(20, 388, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 388, 300, 20, 1011104, false, false); // Return to previous menu

            AddButton(20, 365, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 365, 300, 20, 1011270, false, false); // Remove now!

            AddHtmlLocalized(20, 20, 350, 20, number, false, false);

            if (list != null)
            {
                m_Copy = new List<Mobile>(list);

                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 15) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            AddButton(370, 20, 4005, 4007, 0, GumpButtonType.Page, (i / 15) + 1);
                        }

                        AddPage((i / 15) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            AddButton(340, 20, 4014, 4016, 0, GumpButtonType.Page, i / 15);
                        }
                    }

                    Mobile m = list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    AddCheck(34, 52 + ((i % 15) * 20), 0xD2, 0xD3, false, i);
                    AddLabel(55, 52 + ((i % 15) * 20), 0, accountOf && m.Player && m.Account != null ? String.Format("Account of {0}", name) : name);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_House.Deleted)
                return;

            Mobile from = state.Mobile;

            if (m_List != null && info.ButtonID == 1) // Remove now
            {
                int[] switches = info.Switches;

                if (switches.Length > 0)
                {
                    for (int i = 0; i < switches.Length; ++i)
                    {
                        int index = switches[i];

                        if (index >= 0 && index < m_Copy.Count)
                            m_List.Remove(m_Copy[index]);
                    }

                    if (m_List.Count > 0)
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseRemoveGump(m_Number, m_List, m_House, m_AccountOf));
                        return;
                    }
                }
            }

            from.SendGump(new HouseGump(from, m_House));
        }
    }

    public class HouseGump : Gump
    {
        private readonly BaseHouse m_House;
        public HouseGump(Mobile from, BaseHouse house)
            : base(20, 30)
        {
            if (house.Deleted)
                return;

            m_House = house;

            from.CloseGump(typeof(HouseGump));
            from.CloseGump(typeof(HouseListGump));
            from.CloseGump(typeof(HouseRemoveGump));

            bool isCombatRestricted = house.IsCombatRestricted(from);

            bool isOwner = m_House.IsOwner(from);
            bool isCoOwner = isOwner || m_House.IsCoOwner(from);
            bool isFriend = isCoOwner || m_House.IsFriend(from);

            if (isCombatRestricted)
                isFriend = isCoOwner = isOwner = false;

            AddPage(0);

            if (isFriend)
            {
                AddBackground(0, 0, 420, 430, 5054);
                AddBackground(10, 10, 400, 410, 3000);
            }

            AddImage(130, 0, 100);

            if (m_House.Sign != null)
            {
                var lines = Wrap(m_House.Sign.GetName());

                if (lines != null)
                {
                    for (int i = 0, y = (101 - (lines.Count * 14)) / 2; i < lines.Count; ++i, y += 14)
                    {
                        string s = lines[i];

                        AddLabel(130 + ((143 - (s.Length * 8)) / 2), y, 0, s);
                    }
                }
            }

            if (!isFriend)
                return;

            AddHtmlLocalized(55, 103, 75, 20, 1011233, false, false); // INFO
            AddButton(20, 103, 4005, 4007, 0, GumpButtonType.Page, 1);

            AddHtmlLocalized(170, 103, 75, 20, 1011234, false, false); // FRIENDS
            AddButton(135, 103, 4005, 4007, 0, GumpButtonType.Page, 2);

            AddHtmlLocalized(295, 103, 75, 20, 1011235, false, false); // OPTIONS
            AddButton(260, 103, 4005, 4007, 0, GumpButtonType.Page, 3);

            AddHtmlLocalized(295, 390, 75, 20, 1011441, false, false);  // EXIT
            AddButton(260, 390, 4005, 4007, 0, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 390, 200, 20, 1011236, false, false); // Change this house's name!
            AddButton(20, 390, 4005, 4007, 1, GumpButtonType.Reply, 0);

            // Info page
            AddPage(1);

            AddHtmlLocalized(20, 135, 100, 20, 1011242, false, false); // Owned by:
            AddHtml(120, 135, 100, 20, GetOwnerName(), false, false);

            AddHtmlLocalized(20, 170, 275, 20, 1011237, false, false); // Number of locked down items:
            AddHtml(320, 170, 50, 20, m_House.LockDownCount.ToString(), false, false);

            AddHtmlLocalized(20, 190, 275, 20, 1011238, false, false); // Maximum locked down items:
            AddHtml(320, 190, 50, 20, m_House.MaxLockDowns.ToString(), false, false);

            AddHtmlLocalized(20, 210, 275, 20, 1011239, false, false); // Number of secure containers:
            AddHtml(320, 210, 50, 20, m_House.SecureCount.ToString(), false, false);

            AddHtmlLocalized(20, 230, 275, 20, 1011240, false, false); // Maximum number of secure containers:
            AddHtml(320, 230, 50, 20, m_House.MaxSecures.ToString(), false, false);

            AddHtmlLocalized(20, 260, 400, 20, 1018032, false, false); // This house is properly placed.
            AddHtmlLocalized(20, 280, 400, 20, 1018035, false, false); // This house is of modern design.

            if (m_House.Public)
            {
                // TODO: Validate exact placement
                AddHtmlLocalized(20, 305, 275, 20, 1011241, false, false); // Number of visits this building has had
                AddHtml(320, 305, 50, 20, m_House.TotalVisits.ToString(), false, false);
            }

            // Friends page
            AddPage(2);

            AddHtmlLocalized(45, 130, 150, 20, 1011266, false, false); // List of co-owners
            AddButton(20, 130, 2714, 2715, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 150, 150, 20, 1011267, false, false); // Add a co-owner
            AddButton(20, 150, 2714, 2715, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 170, 150, 20, 1018036, false, false); // Remove a co-owner
            AddButton(20, 170, 2714, 2715, 4, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 190, 150, 20, 1011268, false, false); // Clear co-owner list
            AddButton(20, 190, 2714, 2715, 5, GumpButtonType.Reply, 0);

            AddHtmlLocalized(225, 130, 155, 20, 1011243, false, false); // List of Friends
            AddButton(200, 130, 2714, 2715, 6, GumpButtonType.Reply, 0);

            AddHtmlLocalized(225, 150, 155, 20, 1011244, false, false); // Add a Friend
            AddButton(200, 150, 2714, 2715, 7, GumpButtonType.Reply, 0);

            AddHtmlLocalized(225, 170, 155, 20, 1018037, false, false); // Remove a Friend
            AddButton(200, 170, 2714, 2715, 8, GumpButtonType.Reply, 0);

            AddHtmlLocalized(225, 190, 155, 20, 1011245, false, false); // Clear Friends list
            AddButton(200, 190, 2714, 2715, 9, GumpButtonType.Reply, 0);

            AddHtmlLocalized(120, 215, 280, 20, 1011258, false, false); // Ban someone from the house
            AddButton(95, 215, 2714, 2715, 10, GumpButtonType.Reply, 0);

            AddHtmlLocalized(120, 235, 280, 20, 1011259, false, false); // Eject someone from the house
            AddButton(95, 235, 2714, 2715, 11, GumpButtonType.Reply, 0);

            AddHtmlLocalized(120, 255, 280, 20, 1011260, false, false); // View a list of banned people
            AddButton(95, 255, 2714, 2715, 12, GumpButtonType.Reply, 0);

            AddHtmlLocalized(120, 275, 280, 20, 1011261, false, false); // Lift a ban
            AddButton(95, 275, 2714, 2715, 13, GumpButtonType.Reply, 0);

            // Options page
            AddPage(3);

            AddHtmlLocalized(45, 150, 355, 30, 1011248, false, false); // Transfer ownership of the house
            AddButton(20, 150, 2714, 2715, 14, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 180, 355, 30, 1011249, false, false); // Demolish house and get deed back
            AddButton(20, 180, 2714, 2715, 15, GumpButtonType.Reply, 0);

            if (!m_House.Public)
            {
                AddHtmlLocalized(45, 210, 355, 30, 1011247, false, false); // Change the house locks
                AddButton(20, 210, 2714, 2715, 16, GumpButtonType.Reply, 0);

                AddHtmlLocalized(45, 240, 350, 90, 1011253, false, false); // Declare this building to be public. This will make your front door unlockable.
                AddButton(20, 240, 2714, 2715, 17, GumpButtonType.Reply, 0);
            }
            else
            {
                //AddHtmlLocalized( 45, 280, 350, 30, 1011250, false, false ); // Change the sign type
                AddHtmlLocalized(45, 210, 350, 30, 1011250, false, false); // Change the sign type
                AddButton(20, 210, 2714, 2715, 0, GumpButtonType.Page, 4);

                AddHtmlLocalized(45, 240, 350, 30, 1011252, false, false); // Declare this building to be private.
                AddButton(20, 240, 2714, 2715, 17, GumpButtonType.Reply, 0);
			
                // Change the sign type
                AddPage(4);

                for (int i = 0; i < 24; ++i)
                {
                    AddRadio(53 + ((i / 4) * 50), 137 + ((i % 4) * 35), 210, 211, false, i + 1);
                    AddItem(60 + ((i / 4) * 50), 130 + ((i % 4) * 35), 2980 + (i * 2));
                }

                AddHtmlLocalized(200, 305, 129, 20, 1011254, false, false); // Guild sign choices
                AddButton(350, 305, 252, 253, 0, GumpButtonType.Page, 5);

                AddHtmlLocalized(200, 340, 355, 30, 1011277, false, false); // Okay that is fine.
                AddButton(350, 340, 4005, 4007, 18, GumpButtonType.Reply, 0);

                AddPage(5);

                for (int i = 0; i < 29; ++i)
                {
                    AddRadio(53 + ((i / 5) * 50), 137 + ((i % 5) * 35), 210, 211, false, i + 25);
                    AddItem(60 + ((i / 5) * 50), 130 + ((i % 5) * 35), 3028 + (i * 2));
                }

                AddHtmlLocalized(200, 305, 129, 20, 1011255, false, false); // Shop sign choices
                AddButton(350, 305, 250, 251, 0, GumpButtonType.Page, 4);

                AddHtmlLocalized(200, 340, 355, 30, 1011277, false, false); // Okay that is fine.
                AddButton(350, 340, 4005, 4007, 18, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_House.Deleted)
                return;

            Mobile from = sender.Mobile;

            bool isCombatRestricted = m_House.IsCombatRestricted(from);

            bool isOwner = m_House.IsOwner(from);
            bool isCoOwner = isOwner || m_House.IsCoOwner(from);
            bool isFriend = isCoOwner || m_House.IsFriend(from);

            if (isCombatRestricted)
                isFriend = isCoOwner = isOwner = false;

            if (!isFriend || !from.Alive)
                return;

            Item sign = m_House.Sign;

            if (sign == null || from.Map != sign.Map || !from.InRange(sign.GetWorldLocation(), 18))
                return;

            switch ( info.ButtonID )
            {
                case 1: // Rename sign
                    {
                        from.Prompt = new RenamePrompt(m_House);
                        from.SendLocalizedMessage(501302); // What dost thou wish the sign to say?

                        break;
                    }
                case 2: // List of co-owners
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseListGump(1011275, m_House.CoOwners, m_House, false));

                        break;
                    }
                case 3: // Add co-owner
                    {
                        if (isOwner)
                        {
                            from.SendLocalizedMessage(501328); // Target the person you wish to name a co-owner of your household.
                            from.Target = new CoOwnerTarget(true, m_House);
                        }
                        else
                        {
                            from.SendLocalizedMessage(501327); // Only the house owner may add Co-owners.
                        }

                        break;
                    }
                case 4: // Remove co-owner
                    {
                        if (isOwner)
                        {
                            from.CloseGump(typeof(HouseGump));
                            from.CloseGump(typeof(HouseListGump));
                            from.CloseGump(typeof(HouseRemoveGump));
                            from.SendGump(new HouseRemoveGump(1011274, m_House.CoOwners, m_House, false));
                        }
                        else
                        {
                            from.SendLocalizedMessage(501329); // Only the house owner may remove co-owners.
                        }

                        break;
                    }
                case 5: // Clear co-owners
                    {
                        if (isOwner)
                        {
                            if (m_House.CoOwners != null)
                                m_House.CoOwners.Clear();

                            from.SendLocalizedMessage(501333); // All co-owners have been removed from this house.
                        }
                        else
                        {
                            from.SendLocalizedMessage(501330); // Only the house owner may remove co-owners.
                        }

                        break;
                    }
                case 6: // List friends
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseListGump(1011273, m_House.Friends, m_House, false));

                        break;
                    }
                case 7: // Add friend
                    {
                        if (isCoOwner)
                        {
                            from.SendLocalizedMessage(501317); // Target the person you wish to name a friend of your household.
                            from.Target = new HouseFriendTarget(true, m_House);
                        }
                        else
                        {
                            from.SendLocalizedMessage(501316); // Only the house owner may add friends.
                        }

                        break;
                    }
                case 8: // Remove friend
                    {
                        if (isCoOwner)
                        {
                            from.CloseGump(typeof(HouseGump));
                            from.CloseGump(typeof(HouseListGump));
                            from.CloseGump(typeof(HouseRemoveGump));
                            from.SendGump(new HouseRemoveGump(1011272, m_House.Friends, m_House, false));
                        }
                        else
                        {
                            from.SendLocalizedMessage(501318); // Only the house owner may remove friends.
                        }

                        break;
                    }
                case 9: // Clear friends
                    {
                        if (isCoOwner)
                        {
                            if (m_House.Friends != null)
                                m_House.Friends.Clear();

                            from.SendLocalizedMessage(501332); // All friends have been removed from this house.
                        }
                        else
                        {
                            from.SendLocalizedMessage(501319); // Only the house owner may remove friends.
                        }

                        break;
                    }
                case 10: // Ban
                    {
                        from.SendLocalizedMessage(501325); // Target the individual to ban from this house.
                        from.Target = new HouseBanTarget(true, m_House);

                        break;
                    }
                case 11: // Eject
                    {
                        from.SendLocalizedMessage(501326); // Target the individual to eject from this house.
                        from.Target = new HouseKickTarget(m_House);

                        break;
                    }
                case 12: // List bans
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseListGump(1011271, m_House.Bans, m_House, true));

                        break;
                    }
                case 13: // Remove ban
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseRemoveGump(1011269, m_House.Bans, m_House, true));

                        break;
                    }
                case 14: // Transfer ownership
                    {
                        if (isOwner)
                        {
                            from.SendLocalizedMessage(501309); // Target the person to whom you wish to give this house.
                            from.Target = new HouseOwnerTarget(m_House);
                        }
                        else
                        {
                            from.SendLocalizedMessage(501310); // Only the house owner may do 
                        }

                        break;
                    }
                case 15: // Demolish house
                    {
                        if (isOwner)
                        {
                            if (!Guilds.Guild.NewGuildSystem && m_House.FindGuildstone() != null)
                            {
                                from.SendLocalizedMessage(501389); // You cannot redeed a house with a guildstone inside.
                            }
                            else
                            {
                                from.CloseGump(typeof(HouseDemolishGump));
                                from.SendGump(new HouseDemolishGump(from, m_House));
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501320); // Only the house owner may do 
                        }

                        break;
                    }
                case 16: // Change locks
                    {
                        if (m_House.Public)
                        {
                            from.SendLocalizedMessage(501669);// Public houses are always unlocked.
                        }
                        else
                        {
                            if (isOwner)
                            {
                                m_House.RemoveKeys(from);
                                m_House.ChangeLocks(from);

                                from.SendLocalizedMessage(501306); // The locks on your front door have been changed, and new master keys have been placed in your bank and your backpack.
                            }
                            else
                            {
                                from.SendLocalizedMessage(501303); // Only the house owner may change the house locks.
                            }
                        }

                        break;
                    }
                case 17: // Declare public/private
                    {
                        if (isOwner)
                        {
                            if (m_House.Public && m_House.PlayerVendors.Count > 0)
                            {
                                from.SendLocalizedMessage(501887); // You have vendors working out of this building. It cannot be declared private until there are no vendors in place.
                                break;
                            }

                            m_House.Public = !m_House.Public;
                            if (!m_House.Public)
                            {
                                m_House.ChangeLocks(from);

                                from.SendLocalizedMessage(501888); // This house is now private.
                                from.SendLocalizedMessage(501306); // The locks on your front door have been changed, and new master keys have been placed in your bank and your backpack.
                            }
                            else
                            {
                                m_House.RemoveKeys(from);
                                m_House.RemoveLocks();
                                from.SendLocalizedMessage(501886);//This house is now public. Friends of the house my now have vendors working out of this building.
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501307); // Only the house owner may do 
                        }

                        break;
                    }
                case 18: // Change type
                    {
                        if (isOwner)
                        {
                            if (m_House.Public && info.Switches.Length > 0)
                            {
                                int index = info.Switches[0] - 1;

                                if (index >= 0 && index < 53)
                                    m_House.ChangeSignType(2980 + (index * 2));
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501307); // Only the house owner may do 
                        }

                        break;
                    }
            }
        }

        private List<string> Wrap(string value)
        {
            if (value == null || (value = value.Trim()).Length <= 0)
                return null;

            string[] values = value.Split(' ');
            List<string> list = new List<string>();
            string current = "";

            for (int i = 0; i < values.Length; ++i)
            {
                string val = values[i];

                string v = current.Length == 0 ? val : current + ' ' + val;

                if (v.Length < 10)
                {
                    current = v;
                }
                else if (v.Length == 10)
                {
                    list.Add(v);

                    if (list.Count == 6)
                        return list;

                    current = "";
                }
                else if (val.Length <= 10)
                {
                    list.Add(current);

                    if (list.Count == 6)
                        return list;

                    current = val;
                }
                else
                {
                    while (v.Length >= 10)
                    {
                        list.Add(v.Substring(0, 10));

                        if (list.Count == 6)
                            return list;

                        v = v.Substring(10);
                    }

                    current = v;
                }
            }

            if (current.Length > 0)
                list.Add(current);

            return list;
        }

        private string GetOwnerName()
        {
            Mobile m = m_House.Owner;

            if (m == null)
                return "(unowned)";

            string name;

            if ((name = m.Name) == null || (name = name.Trim()).Length <= 0)
                name = "(no name)";

            return name;
        }
    }
}

namespace Server.Prompts
{
    public class RenamePrompt : Prompt
    {
        public override int MessageCliloc { get { return 501302; } }
        private readonly BaseHouse m_House;
        public RenamePrompt(BaseHouse house)
        {
            m_House = house;
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (m_House.IsFriend(from))
            {
                if (m_House.Sign != null)
                    m_House.Sign.Name = text;

                from.SendMessage("Sign changed.");
            }
        }
    }
}