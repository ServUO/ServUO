using System;
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

            this.m_State = state;

            this.AddPage(0);

            this.AddBackground(0, 0, 400, 274, 5054);

            this.AddImageTiled(10, 10, 380, 19, 0xA40);
            this.AddAlphaRegion(10, 10, 380, 19);

            this.AddImageTiled(10, 32, 380, 232, 0xA40);
            this.AddAlphaRegion(10, 32, 380, 232);

            this.AddHtml(10, 10, 380, 20, this.Color(this.Center("User Information"), LabelColor32), false, false);

            int line = 0;

            this.AddHtml(14, 36 + (line * 20), 200, 20, this.Color("Address:", LabelColor32), false, false);
            this.AddHtml(70, 36 + (line++ * 20), 200, 20, this.Color(state.ToString(), LabelColor32), false, false);

            this.AddHtml(14, 36 + (line * 20), 200, 20, this.Color("Client:", LabelColor32), false, false);
            this.AddHtml(70, 36 + (line++ * 20), 200, 20, this.Color(state.Version == null ? "(null)" : state.Version.ToString(), LabelColor32), false, false);

            this.AddHtml(14, 36 + (line * 20), 200, 20, this.Color("Version:", LabelColor32), false, false);

            ExpansionInfo info = state.ExpansionInfo;
            string expansionName = info.Name;

            this.AddHtml(70, 36 + (line++ * 20), 200, 20, this.Color(expansionName, LabelColor32), false, false);

            Account a = state.Account as Account;
            Mobile m = state.Mobile;

            if (from.AccessLevel >= AccessLevel.GameMaster && a != null)
            {
                this.AddHtml(14, 36 + (line * 20), 200, 20, this.Color("Account:", LabelColor32), false, false);
                this.AddHtml(70, 36 + (line++ * 20), 200, 20, this.Color(a.Username, LabelColor32), false, false);
            }

            if (m != null)
            {
                this.AddHtml(14, 36 + (line * 20), 200, 20, this.Color("Mobile:", LabelColor32), false, false);
                this.AddHtml(70, 36 + (line++ * 20), 200, 20, this.Color(String.Format("{0} (0x{1:X})", m.Name, m.Serial.Value), LabelColor32), false, false);

                this.AddHtml(14, 36 + (line * 20), 200, 20, this.Color("Location:", LabelColor32), false, false);
                this.AddHtml(70, 36 + (line++ * 20), 200, 20, this.Color(String.Format("{0} [{1}]", m.Location, m.Map), LabelColor32), false, false);

                this.AddButton(13, 157, 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0);
                this.AddHtml(48, 158, 200, 20, this.Color("Send Message", LabelColor32), false, false);

                this.AddImageTiled(12, 182, 376, 80, 0xA40);
                this.AddImageTiled(13, 183, 374, 78, 0xBBC);
                this.AddTextEntry(15, 183, 372, 78, 0x480, 0, "");

                this.AddImageTiled(245, 35, 142, 144, 5058);

                this.AddImageTiled(246, 36, 140, 142, 0xA40);
                this.AddAlphaRegion(246, 36, 140, 142);

                line = 0;

                if (BaseCommand.IsAccessible(from, m))
                {
                    this.AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
                    this.AddHtml(280, 38 + (line++ * 20), 100, 20, this.Color("Properties", LabelColor32), false, false);
                }

                if (from != m)
                {
                    this.AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0);
                    this.AddHtml(280, 38 + (line++ * 20), 100, 20, this.Color("Go to them", LabelColor32), false, false);

                    this.AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0);
                    this.AddHtml(280, 38 + (line++ * 20), 100, 20, this.Color("Bring them here", LabelColor32), false, false);
                }

                this.AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 7, GumpButtonType.Reply, 0);
                this.AddHtml(280, 38 + (line++ * 20), 100, 20, this.Color("Move to target", LabelColor32), false, false);

                if (from.AccessLevel >= AccessLevel.GameMaster && from.AccessLevel > m.AccessLevel)
                {
                    this.AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 8, GumpButtonType.Reply, 0);
                    this.AddHtml(280, 38 + (line++ * 20), 100, 20, this.Color("Disconnect", LabelColor32), false, false);

                    if (m.Alive)
                    {
                        this.AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 9, GumpButtonType.Reply, 0);
                        this.AddHtml(280, 38 + (line++ * 20), 100, 20, this.Color("Kill", LabelColor32), false, false);
                    }
                    else
                    {
                        this.AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 10, GumpButtonType.Reply, 0);
                        this.AddHtml(280, 38 + (line++ * 20), 100, 20, this.Color("Resurrect", LabelColor32), false, false);
                    }
                }

                if (from.IsStaff() && from.AccessLevel > m.AccessLevel)
                {
                    this.AddButton(246, 36 + (line * 20), 0xFA5, 0xFA7, 11, GumpButtonType.Reply, 0);
                    this.AddHtml(280, 38 + (line++ * 20), 100, 20, this.Color("Skills browser", LabelColor32), false, false);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (this.m_State == null)
                return;

            Mobile focus = this.m_State.Mobile;
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

            switch ( info.ButtonID )
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

                        from.SendGump(new ClientGump(from, this.m_State));

                        break;
                    }
                case 4: // Props
                    {
                        this.Resend(from, info);

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
                            this.Resend(from, info);

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
                            this.Resend(from, info);

                            CommandLogging.WriteLine(from, "{0} {1} bringing {2} to Location {3}, Map {4}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus), from.Location, from.Map);
                        }

                        break;
                    }
                case 7: // Move
                    {
                        from.Target = new MoveTarget(focus);
                        this.Resend(from, info);

                        break;
                    }
                case 8: // Kick
                    {
                        if (from.AccessLevel >= AccessLevel.GameMaster && from.AccessLevel > focus.AccessLevel)
                        {
                            focus.Say("I've been kicked!");

                            this.m_State.Dispose();

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

                        this.Resend(from, info);

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

                        this.Resend(from, info);

                        break;
                    }
                case 11: // Skills
                    {
                        this.Resend(from, info);

                        if (from.AccessLevel > focus.AccessLevel)
                        {
                            from.SendGump(new SkillsGump(from, (Mobile)focus));
                            CommandLogging.WriteLine(from, "{0} {1} Opening Skills gump of {2} ", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(focus));
                        }

                        break;
                    }
            }
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        private void Resend(Mobile to, RelayInfo info)
        {
            TextRelay te = info.GetTextEntry(0);

            to.SendGump(new ClientGump(to, this.m_State, te == null ? "" : te.Text));
        }
    }
}