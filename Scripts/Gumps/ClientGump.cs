using Server.Accounting;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Targets;

namespace Server.Gumps
{
    public class ClientGump : Gump
    {
        private const int LabelColor32 = 0xFFFFFF;
        private readonly NetState m_State;
        public ClientGump(Mobile from, NetState state)
            : this(from, state, "")
        {
        }

        public ClientGump(Mobile from, NetState state, string initialText)
            : base(30, 20)
        {
            if (state == null)
                return;

            m_State = state;

            AddPage(0);

            AddBackground(0, 0, 400, 274, 5054);

            AddImageTiled(10, 10, 380, 19, 0xA40);
            AddAlphaRegion(10, 10, 380, 19);

            AddImageTiled(10, 32, 380, 232, 0xA40);
            AddAlphaRegion(10, 32, 380, 232);

            AddHtml(10, 10, 380, 20, Color(Center("User Information"), LabelColor32), false, false);

            int line = 0;

            AddHtml(14, 36 + (line * 20), 200, 20, Color("Address:", LabelColor32), false, false);
            AddHtml(70, 36 + (line++ * 20), 200, 20, Color(state.ToString(), LabelColor32), false, false);

            AddHtml(14, 36 + (line * 20), 200, 20, Color("Client:", LabelColor32), false, false);
            AddHtml(70, 36 + (line++ * 20), 200, 20, Color(state.Version == null ? "(null)" : state.Version.ToString(), LabelColor32), false, false);

            AddHtml(14, 36 + (line * 20), 200, 20, Color("Version:", LabelColor32), false, false);

            ExpansionInfo info = state.ExpansionInfo;
            string expansionName = info.Name;

            AddHtml(70, 36 + (line++ * 20), 200, 20, Color(expansionName, LabelColor32), false, false);

            Account a = state.Account as Account;
            Mobile m = state.Mobile;

            if (from.AccessLevel >= AccessLevel.GameMaster && a != null)
            {
                AddHtml(14, 36 + (line * 20), 200, 20, Color("Account:", LabelColor32), false, false);
                AddHtml(70, 36 + (line++ * 20), 200, 20, Color(a.Username, LabelColor32), false, false);
            }

            if (m != null)
            {
                AddHtml(14, 36 + (line * 20), 200, 20, Color("Mobile:", LabelColor32), false, false);
                AddHtml(70, 36 + (line++ * 20), 200, 20, Color(string.Format("{0} (0x{1:X})", m.Name, m.Serial.Value), LabelColor32), false, false);

                AddHtml(14, 36 + (line * 20), 200, 20, Color("Location:", LabelColor32), false, false);
                AddHtml(70, 36 + (line++ * 20), 200, 20, Color(string.Format("{0} [{1}]", m.Location, m.Map), LabelColor32), false, false);

                AddButton(13, 157, 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0);
                AddHtml(48, 158, 200, 20, Color("Send Message", LabelColor32), false, false);

                AddImageTiled(12, 182, 376, 80, 0xA40);
                AddImageTiled(13, 183, 374, 78, 0xBBC);
                AddTextEntry(15, 183, 372, 78, 0x480, 0, "");

                AddImageTiled(245, 35, 142, 144, 5058);

                AddImageTiled(246, 36, 140, 142, 0xA40);
                AddAlphaRegion(246, 36, 140, 142);

                line = 0;

                if (BaseCommand.IsAccessible(from, m))
                {
                    AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
                    AddHtml(280, 38 + (line++ * 20), 100, 20, Color("Properties", LabelColor32), false, false);
                }

                if (from != m)
                {
                    AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0);
                    AddHtml(280, 38 + (line++ * 20), 100, 20, Color("Go to them", LabelColor32), false, false);

                    AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0);
                    AddHtml(280, 38 + (line++ * 20), 100, 20, Color("Bring them here", LabelColor32), false, false);
                }

                AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 7, GumpButtonType.Reply, 0);
                AddHtml(280, 38 + (line++ * 20), 100, 20, Color("Move to target", LabelColor32), false, false);

                if (from.AccessLevel >= AccessLevel.GameMaster && from.AccessLevel > m.AccessLevel)
                {
                    AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 8, GumpButtonType.Reply, 0);
                    AddHtml(280, 38 + (line++ * 20), 100, 20, Color("Disconnect", LabelColor32), false, false);

                    if (m.Alive)
                    {
                        AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 9, GumpButtonType.Reply, 0);
                        AddHtml(280, 38 + (line++ * 20), 100, 20, Color("Kill", LabelColor32), false, false);
                    }
                    else
                    {
                        AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 10, GumpButtonType.Reply, 0);
                        AddHtml(280, 38 + (line++ * 20), 100, 20, Color("Resurrect", LabelColor32), false, false);
                    }
                }

                if (from.IsStaff() && from.AccessLevel > m.AccessLevel)
                {
                    AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 11, GumpButtonType.Reply, 0);
                    AddHtml(280, 38 + (line++ * 20), 100, 20, Color("Skills browser", LabelColor32), false, false);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_State == null)
                return;

            Mobile focus = m_State.Mobile;
            Mobile from = state.Mobile;

            if (focus == null)
            {
                from.SendMessage("That character is no longer online.");
                return;
            }
            else if (focus.Deleted)
            {
                from.SendMessage("That character no longer exists.");
                return;
            }
            else if (from != focus && focus.Hidden && from.AccessLevel < focus.AccessLevel && (!(focus is PlayerMobile) || !((PlayerMobile)focus).VisibilityList.Contains(from)))
            {
                from.SendMessage("That character is no longer visible.");
                return;
            }

            switch (info.ButtonID)
            {
                case 1: // Tell
                    {
                        TextRelay text = info.GetTextEntry(0);

                        if (text != null)
                        {
                            focus.SendMessage(0x482, "{0} tells you:", from.Name);
                            focus.SendMessage(0x482, text.Text);

                            CommandLogging.WriteLine(from, "{0} {1} telling {2} \"{3}\" ", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus), text.Text);
                        }

                        from.SendGump(new ClientGump(from, m_State));

                        break;
                    }
                case 4: // Props
                    {
                        Resend(from, info);

                        if (!BaseCommand.IsAccessible(from, focus))
                            from.SendMessage("That is not accessible.");
                        else
                        {
                            from.SendGump(new PropertiesGump(from, focus));
                            CommandLogging.WriteLine(from, "{0} {1} opening properties gump of {2} ", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus));
                        }

                        break;
                    }
                case 5: // Go to
                    {
                        if (focus.Map == null || focus.Map == Map.Internal)
                        {
                            from.SendMessage("That character is not in the world.");
                        }
                        else
                        {
                            from.MoveToWorld(focus.Location, focus.Map);
                            Resend(from, info);

                            CommandLogging.WriteLine(from, "{0} {1} going to {2}, Location {3}, Map {4}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus), focus.Location, focus.Map);
                        }

                        break;
                    }
                case 6: // Get
                    {
                        if (from.Map == null || from.Map == Map.Internal)
                        {
                            from.SendMessage("You cannot bring that person here.");
                        }
                        else
                        {
                            focus.MoveToWorld(from.Location, from.Map);
                            Resend(from, info);

                            CommandLogging.WriteLine(from, "{0} {1} bringing {2} to Location {3}, Map {4}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus), from.Location, from.Map);
                        }

                        break;
                    }
                case 7: // Move
                    {
                        from.Target = new MoveTarget(focus);
                        Resend(from, info);

                        break;
                    }
                case 8: // Kick
                    {
                        if (from.AccessLevel >= AccessLevel.GameMaster && from.AccessLevel > focus.AccessLevel)
                        {
                            focus.Say("I've been kicked!");

                            m_State.Dispose();

                            CommandLogging.WriteLine(from, "{0} {1} kicking {2} ", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus));
                        }

                        break;
                    }
                case 9: // Kill
                    {
                        if (from.AccessLevel >= AccessLevel.GameMaster && from.AccessLevel > focus.AccessLevel)
                        {
                            focus.Kill();
                            CommandLogging.WriteLine(from, "{0} {1} killing {2} ", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus));
                        }

                        Resend(from, info);

                        break;
                    }
                case 10: //Res
                    {
                        if (from.AccessLevel >= AccessLevel.GameMaster && from.AccessLevel > focus.AccessLevel)
                        {
                            focus.PlaySound(0x214);
                            focus.FixedEffect(0x376A, 10, 16);

                            focus.Resurrect();

                            CommandLogging.WriteLine(from, "{0} {1} resurrecting {2} ", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus));
                        }

                        Resend(from, info);

                        break;
                    }
                case 11: // Skills
                    {
                        Resend(from, info);

                        if (from.AccessLevel > focus.AccessLevel)
                        {
                            from.SendGump(new SkillsGump(from, focus));
                            CommandLogging.WriteLine(from, "{0} {1} Opening Skills gump of {2} ", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus));
                        }

                        break;
                    }
            }
        }

        public string Center(string text)
        {
            return string.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        private void Resend(Mobile to, RelayInfo info)
        {
            TextRelay te = info.GetTextEntry(0);

            to.SendGump(new ClientGump(to, m_State, te == null ? "" : te.Text));
        }
    }
}