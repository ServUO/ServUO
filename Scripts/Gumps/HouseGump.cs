using System;
using System.Collections;
using Server.Multis;
using Server.Network;
using Server.Prompts;

namespace Server.Gumps
{
    public class HouseListGump : Gump
    {
        private readonly BaseHouse m_House;
        public HouseListGump(int number, ArrayList list, BaseHouse house, bool accountOf)
            : base(20, 30)
        {
            if (house.Deleted)
                return;

            this.m_House = house;

            this.AddPage(0);

            this.AddBackground(0, 0, 420, 430, 5054);
            this.AddBackground(10, 10, 400, 410, 3000);

            this.AddButton(20, 388, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 388, 300, 20, 1011104, false, false); // Return to previous menu

            this.AddHtmlLocalized(20, 20, 350, 20, number, false, false);

            if (list != null)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 16) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            this.AddButton(370, 20, 4005, 4007, 0, GumpButtonType.Page, (i / 16) + 1);
                        }

                        this.AddPage((i / 16) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            this.AddButton(340, 20, 4014, 4016, 0, GumpButtonType.Page, i / 16);
                        }
                    }

                    Mobile m = (Mobile)list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    this.AddLabel(55, 55 + ((i % 16) * 20), 0, accountOf && m.Player && m.Account != null ? String.Format("Account of {0}", name) : name);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (this.m_House.Deleted)
                return;

            Mobile from = state.Mobile;

            from.SendGump(new HouseGump(from, this.m_House));
        }
    }

    public class HouseRemoveGump : Gump
    {
        private readonly BaseHouse m_House;
        private readonly ArrayList m_List;
        private readonly ArrayList m_Copy;
        private readonly int m_Number;
        private readonly bool m_AccountOf;
        public HouseRemoveGump(int number, ArrayList list, BaseHouse house, bool accountOf)
            : base(20, 30)
        {
            if (house.Deleted)
                return;

            this.m_House = house;
            this.m_List = list;
            this.m_Number = number;
            this.m_AccountOf = accountOf;

            this.AddPage(0);

            this.AddBackground(0, 0, 420, 430, 5054);
            this.AddBackground(10, 10, 400, 410, 3000);

            this.AddButton(20, 388, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 388, 300, 20, 1011104, false, false); // Return to previous menu

            this.AddButton(20, 365, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 365, 300, 20, 1011270, false, false); // Remove now!

            this.AddHtmlLocalized(20, 20, 350, 20, number, false, false);

            if (list != null)
            {
                this.m_Copy = new ArrayList(list);

                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 15) == 0)
                    {
                        if (i != 0)
                        {
                            // Next button
                            this.AddButton(370, 20, 4005, 4007, 0, GumpButtonType.Page, (i / 15) + 1);
                        }

                        this.AddPage((i / 15) + 1);

                        if (i != 0)
                        {
                            // Previous button
                            this.AddButton(340, 20, 4014, 4016, 0, GumpButtonType.Page, i / 15);
                        }
                    }

                    Mobile m = (Mobile)list[i];

                    string name;

                    if (m == null || (name = m.Name) == null || (name = name.Trim()).Length <= 0)
                        continue;

                    this.AddCheck(34, 52 + ((i % 15) * 20), 0xD2, 0xD3, false, i);
                    this.AddLabel(55, 52 + ((i % 15) * 20), 0, accountOf && m.Player && m.Account != null ? String.Format("Account of {0}", name) : name);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (this.m_House.Deleted)
                return;

            Mobile from = state.Mobile;

            if (this.m_List != null && info.ButtonID == 1) // Remove now
            {
                int[] switches = info.Switches;

                if (switches.Length > 0)
                {
                    for (int i = 0; i < switches.Length; ++i)
                    {
                        int index = switches[i];

                        if (index >= 0 && index < this.m_Copy.Count)
                            this.m_List.Remove(this.m_Copy[index]);
                    }

                    if (this.m_List.Count > 0)
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseRemoveGump(this.m_Number, this.m_List, this.m_House, this.m_AccountOf));
                        return;
                    }
                }
            }

            from.SendGump(new HouseGump(from, this.m_House));
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

            this.m_House = house;

            from.CloseGump(typeof(HouseGump));
            from.CloseGump(typeof(HouseListGump));
            from.CloseGump(typeof(HouseRemoveGump));

            bool isCombatRestricted = house.IsCombatRestricted(from);

            bool isOwner = this.m_House.IsOwner(from);
            bool isCoOwner = isOwner || this.m_House.IsCoOwner(from);
            bool isFriend = isCoOwner || this.m_House.IsFriend(from);

            if (isCombatRestricted)
                isFriend = isCoOwner = isOwner = false;

            this.AddPage(0);

            if (isFriend)
            {
                this.AddBackground(0, 0, 420, 430, 5054);
                this.AddBackground(10, 10, 400, 410, 3000);
            }

            this.AddImage(130, 0, 100);

            if (this.m_House.Sign != null)
            {
                ArrayList lines = this.Wrap(this.m_House.Sign.GetName());

                if (lines != null)
                {
                    for (int i = 0, y = (101 - (lines.Count * 14)) / 2; i < lines.Count; ++i, y += 14)
                    {
                        string s = (string)lines[i];

                        this.AddLabel(130 + ((143 - (s.Length * 8)) / 2), y, 0, s);
                    }
                }
            }

            if (!isFriend)
                return;

            this.AddHtmlLocalized(55, 103, 75, 20, 1011233, false, false); // INFO
            this.AddButton(20, 103, 4005, 4007, 0, GumpButtonType.Page, 1);

            this.AddHtmlLocalized(170, 103, 75, 20, 1011234, false, false); // FRIENDS
            this.AddButton(135, 103, 4005, 4007, 0, GumpButtonType.Page, 2);

            this.AddHtmlLocalized(295, 103, 75, 20, 1011235, false, false); // OPTIONS
            this.AddButton(260, 103, 4005, 4007, 0, GumpButtonType.Page, 3);

            this.AddHtmlLocalized(295, 390, 75, 20, 1011441, false, false);  // EXIT
            this.AddButton(260, 390, 4005, 4007, 0, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(55, 390, 200, 20, 1011236, false, false); // Change this house's name!
            this.AddButton(20, 390, 4005, 4007, 1, GumpButtonType.Reply, 0);

            // Info page
            this.AddPage(1);

            this.AddHtmlLocalized(20, 135, 100, 20, 1011242, false, false); // Owned by:
            this.AddHtml(120, 135, 100, 20, this.GetOwnerName(), false, false);

            this.AddHtmlLocalized(20, 170, 275, 20, 1011237, false, false); // Number of locked down items:
            this.AddHtml(320, 170, 50, 20, this.m_House.LockDownCount.ToString(), false, false);

            this.AddHtmlLocalized(20, 190, 275, 20, 1011238, false, false); // Maximum locked down items:
            this.AddHtml(320, 190, 50, 20, this.m_House.MaxLockDowns.ToString(), false, false);

            this.AddHtmlLocalized(20, 210, 275, 20, 1011239, false, false); // Number of secure containers:
            this.AddHtml(320, 210, 50, 20, this.m_House.SecureCount.ToString(), false, false);

            this.AddHtmlLocalized(20, 230, 275, 20, 1011240, false, false); // Maximum number of secure containers:
            this.AddHtml(320, 230, 50, 20, this.m_House.MaxSecures.ToString(), false, false);

            this.AddHtmlLocalized(20, 260, 400, 20, 1018032, false, false); // This house is properly placed.
            this.AddHtmlLocalized(20, 280, 400, 20, 1018035, false, false); // This house is of modern design.

            if (this.m_House.Public)
            {
                // TODO: Validate exact placement
                this.AddHtmlLocalized(20, 305, 275, 20, 1011241, false, false); // Number of visits this building has had
                this.AddHtml(320, 305, 50, 20, this.m_House.Visits.ToString(), false, false);
            }

            // Friends page
            this.AddPage(2);

            this.AddHtmlLocalized(45, 130, 150, 20, 1011266, false, false); // List of co-owners
            this.AddButton(20, 130, 2714, 2715, 2, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(45, 150, 150, 20, 1011267, false, false); // Add a co-owner
            this.AddButton(20, 150, 2714, 2715, 3, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(45, 170, 150, 20, 1018036, false, false); // Remove a co-owner
            this.AddButton(20, 170, 2714, 2715, 4, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(45, 190, 150, 20, 1011268, false, false); // Clear co-owner list
            this.AddButton(20, 190, 2714, 2715, 5, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(225, 130, 155, 20, 1011243, false, false); // List of Friends
            this.AddButton(200, 130, 2714, 2715, 6, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(225, 150, 155, 20, 1011244, false, false); // Add a Friend
            this.AddButton(200, 150, 2714, 2715, 7, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(225, 170, 155, 20, 1018037, false, false); // Remove a Friend
            this.AddButton(200, 170, 2714, 2715, 8, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(225, 190, 155, 20, 1011245, false, false); // Clear Friends list
            this.AddButton(200, 190, 2714, 2715, 9, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(120, 215, 280, 20, 1011258, false, false); // Ban someone from the house
            this.AddButton(95, 215, 2714, 2715, 10, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(120, 235, 280, 20, 1011259, false, false); // Eject someone from the house
            this.AddButton(95, 235, 2714, 2715, 11, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(120, 255, 280, 20, 1011260, false, false); // View a list of banned people
            this.AddButton(95, 255, 2714, 2715, 12, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(120, 275, 280, 20, 1011261, false, false); // Lift a ban
            this.AddButton(95, 275, 2714, 2715, 13, GumpButtonType.Reply, 0);

            // Options page
            this.AddPage(3);

            this.AddHtmlLocalized(45, 150, 355, 30, 1011248, false, false); // Transfer ownership of the house
            this.AddButton(20, 150, 2714, 2715, 14, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(45, 180, 355, 30, 1011249, false, false); // Demolish house and get deed back
            this.AddButton(20, 180, 2714, 2715, 15, GumpButtonType.Reply, 0);

            if (!this.m_House.Public)
            {
                this.AddHtmlLocalized(45, 210, 355, 30, 1011247, false, false); // Change the house locks
                this.AddButton(20, 210, 2714, 2715, 16, GumpButtonType.Reply, 0);

                this.AddHtmlLocalized(45, 240, 350, 90, 1011253, false, false); // Declare this building to be public. This will make your front door unlockable.
                this.AddButton(20, 240, 2714, 2715, 17, GumpButtonType.Reply, 0);
            }
            else
            {
                //AddHtmlLocalized( 45, 280, 350, 30, 1011250, false, false ); // Change the sign type
                this.AddHtmlLocalized(45, 210, 350, 30, 1011250, false, false); // Change the sign type
                this.AddButton(20, 210, 2714, 2715, 0, GumpButtonType.Page, 4);

                this.AddHtmlLocalized(45, 240, 350, 30, 1011252, false, false); // Declare this building to be private.
                this.AddButton(20, 240, 2714, 2715, 17, GumpButtonType.Reply, 0);
			
                // Change the sign type
                this.AddPage(4);

                for (int i = 0; i < 24; ++i)
                {
                    this.AddRadio(53 + ((i / 4) * 50), 137 + ((i % 4) * 35), 210, 211, false, i + 1);
                    this.AddItem(60 + ((i / 4) * 50), 130 + ((i % 4) * 35), 2980 + (i * 2));
                }

                this.AddHtmlLocalized(200, 305, 129, 20, 1011254, false, false); // Guild sign choices
                this.AddButton(350, 305, 252, 253, 0, GumpButtonType.Page, 5);

                this.AddHtmlLocalized(200, 340, 355, 30, 1011277, false, false); // Okay that is fine.
                this.AddButton(350, 340, 4005, 4007, 18, GumpButtonType.Reply, 0);

                this.AddPage(5);

                for (int i = 0; i < 29; ++i)
                {
                    this.AddRadio(53 + ((i / 5) * 50), 137 + ((i % 5) * 35), 210, 211, false, i + 25);
                    this.AddItem(60 + ((i / 5) * 50), 130 + ((i % 5) * 35), 3028 + (i * 2));
                }

                this.AddHtmlLocalized(200, 305, 129, 20, 1011255, false, false); // Shop sign choices
                this.AddButton(350, 305, 250, 251, 0, GumpButtonType.Page, 4);

                this.AddHtmlLocalized(200, 340, 355, 30, 1011277, false, false); // Okay that is fine.
                this.AddButton(350, 340, 4005, 4007, 18, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_House.Deleted)
                return;

            Mobile from = sender.Mobile;

            bool isCombatRestricted = this.m_House.IsCombatRestricted(from);

            bool isOwner = this.m_House.IsOwner(from);
            bool isCoOwner = isOwner || this.m_House.IsCoOwner(from);
            bool isFriend = isCoOwner || this.m_House.IsFriend(from);

            if (isCombatRestricted)
                isFriend = isCoOwner = isOwner = false;

            if (!isFriend || !from.Alive)
                return;

            Item sign = this.m_House.Sign;

            if (sign == null || from.Map != sign.Map || !from.InRange(sign.GetWorldLocation(), 18))
                return;

            switch ( info.ButtonID )
            {
                case 1: // Rename sign
                    {
                        from.Prompt = new RenamePrompt(this.m_House);
                        from.SendLocalizedMessage(501302); // What dost thou wish the sign to say?

                        break;
                    }
                case 2: // List of co-owners
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseListGump(1011275, this.m_House.CoOwners, this.m_House, false));

                        break;
                    }
                case 3: // Add co-owner
                    {
                        if (isOwner)
                        {
                            from.SendLocalizedMessage(501328); // Target the person you wish to name a co-owner of your household.
                            from.Target = new CoOwnerTarget(true, this.m_House);
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
                            from.SendGump(new HouseRemoveGump(1011274, this.m_House.CoOwners, this.m_House, false));
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
                            if (this.m_House.CoOwners != null)
                                this.m_House.CoOwners.Clear();

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
                        from.SendGump(new HouseListGump(1011273, this.m_House.Friends, this.m_House, false));

                        break;
                    }
                case 7: // Add friend
                    {
                        if (isCoOwner)
                        {
                            from.SendLocalizedMessage(501317); // Target the person you wish to name a friend of your household.
                            from.Target = new HouseFriendTarget(true, this.m_House);
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
                            from.SendGump(new HouseRemoveGump(1011272, this.m_House.Friends, this.m_House, false));
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
                            if (this.m_House.Friends != null)
                                this.m_House.Friends.Clear();

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
                        from.Target = new HouseBanTarget(true, this.m_House);

                        break;
                    }
                case 11: // Eject
                    {
                        from.SendLocalizedMessage(501326); // Target the individual to eject from this house.
                        from.Target = new HouseKickTarget(this.m_House);

                        break;
                    }
                case 12: // List bans
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseListGump(1011271, this.m_House.Bans, this.m_House, true));

                        break;
                    }
                case 13: // Remove ban
                    {
                        from.CloseGump(typeof(HouseGump));
                        from.CloseGump(typeof(HouseListGump));
                        from.CloseGump(typeof(HouseRemoveGump));
                        from.SendGump(new HouseRemoveGump(1011269, this.m_House.Bans, this.m_House, true));

                        break;
                    }
                case 14: // Transfer ownership
                    {
                        if (isOwner)
                        {
                            from.SendLocalizedMessage(501309); // Target the person to whom you wish to give this house.
                            from.Target = new HouseOwnerTarget(this.m_House);
                        }
                        else
                        {
                            from.SendLocalizedMessage(501310); // Only the house owner may do this.
                        }

                        break;
                    }
                case 15: // Demolish house
                    {
                        if (isOwner)
                        {
                            if (!Guilds.Guild.NewGuildSystem && this.m_House.FindGuildstone() != null)
                            {
                                from.SendLocalizedMessage(501389); // You cannot redeed a house with a guildstone inside.
                            }
                            else
                            {
                                from.CloseGump(typeof(HouseDemolishGump));
                                from.SendGump(new HouseDemolishGump(from, this.m_House));
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501320); // Only the house owner may do this.
                        }

                        break;
                    }
                case 16: // Change locks
                    {
                        if (this.m_House.Public)
                        {
                            from.SendLocalizedMessage(501669);// Public houses are always unlocked.
                        }
                        else
                        {
                            if (isOwner)
                            {
                                this.m_House.RemoveKeys(from);
                                this.m_House.ChangeLocks(from);

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
                            if (this.m_House.Public && this.m_House.PlayerVendors.Count > 0)
                            {
                                from.SendLocalizedMessage(501887); // You have vendors working out of this building. It cannot be declared private until there are no vendors in place.
                                break;
                            }

                            this.m_House.Public = !this.m_House.Public;
                            if (!this.m_House.Public)
                            {
                                this.m_House.ChangeLocks(from);

                                from.SendLocalizedMessage(501888); // This house is now private.
                                from.SendLocalizedMessage(501306); // The locks on your front door have been changed, and new master keys have been placed in your bank and your backpack.
                            }
                            else
                            {
                                this.m_House.RemoveKeys(from);
                                this.m_House.RemoveLocks();
                                from.SendLocalizedMessage(501886);//This house is now public. Friends of the house my now have vendors working out of this building.
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501307); // Only the house owner may do this.
                        }

                        break;
                    }
                case 18: // Change type
                    {
                        if (isOwner)
                        {
                            if (this.m_House.Public && info.Switches.Length > 0)
                            {
                                int index = info.Switches[0] - 1;

                                if (index >= 0 && index < 53)
                                    this.m_House.ChangeSignType(2980 + (index * 2));
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(501307); // Only the house owner may do this.
                        }

                        break;
                    }
            }
        }

        private ArrayList Wrap(string value)
        {
            if (value == null || (value = value.Trim()).Length <= 0)
                return null;

            string[] values = value.Split(' ');
            ArrayList list = new ArrayList();
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
            Mobile m = this.m_House.Owner;

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
        private readonly BaseHouse m_House;
        public RenamePrompt(BaseHouse house)
        {
            this.m_House = house;
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (this.m_House.IsFriend(from))
            {
                if (this.m_House.Sign != null)
                    this.m_House.Sign.Name = text;

                from.SendMessage("Sign changed.");
            }
        }
    }
}