using System;
using System.Text;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.Misc
{
    public class TestCenter
    {
        private const bool m_Enabled = false;
        public static bool Enabled
        {
            get
            {
                return m_Enabled;
            }
        }
        public static void Initialize()
        {
            // Register our speech handler
            if (Enabled)
                EventSink.Speech += new SpeechEventHandler(EventSink_Speech);
        }

        private static void EventSink_Speech(SpeechEventArgs args)
        {
            if (!args.Handled)
            {
                if (Insensitive.StartsWith(args.Speech, "set"))
                {
                    Mobile from = args.Mobile;

                    string[] split = args.Speech.Split(' ');

                    if (split.Length == 3)
                    {
                        try
                        {
                            string name = split[1];
                            double value = Convert.ToDouble(split[2]);

                            if (Insensitive.Equals(name, "str"))
                                ChangeStrength(from, (int)value);
                            else if (Insensitive.Equals(name, "dex"))
                                ChangeDexterity(from, (int)value);
                            else if (Insensitive.Equals(name, "int"))
                                ChangeIntelligence(from, (int)value);
                            else
                                ChangeSkill(from, name, value);
                        }
                        catch
                        {
                        }
                    }
                }
                else if (Insensitive.Equals(args.Speech, "help"))
                {
                    args.Mobile.SendGump(new TCHelpGump());

                    args.Handled = true;
                }
            }
        }

        private static void ChangeStrength(Mobile from, int value)
        {
            if (value < 10 || value > 125)
            {
                from.SendLocalizedMessage(1005628); // Stats range between 10 and 125.
            }
            else
            {
                if ((value + from.RawDex + from.RawInt) > from.StatCap)
                {
                    from.SendLocalizedMessage(1005629); // You can not exceed the stat cap.  Try setting another stat lower first.
                }
                else
                {
                    from.RawStr = value;
                    from.SendLocalizedMessage(1005630); // Your stats have been adjusted.
                }
            }
        }

        private static void ChangeDexterity(Mobile from, int value)
        {
            if (value < 10 || value > 125)
            {
                from.SendLocalizedMessage(1005628); // Stats range between 10 and 125.
            }
            else
            {
                if ((from.RawStr + value + from.RawInt) > from.StatCap)
                {
                    from.SendLocalizedMessage(1005629); // You can not exceed the stat cap.  Try setting another stat lower first.
                }
                else
                {
                    from.RawDex = value;
                    from.SendLocalizedMessage(1005630); // Your stats have been adjusted.
                }
            }
        }

        private static void ChangeIntelligence(Mobile from, int value)
        {
            if (value < 10 || value > 125)
            {
                from.SendLocalizedMessage(1005628); // Stats range between 10 and 125.
            }
            else
            {
                if ((from.RawStr + from.RawDex + value) > from.StatCap)
                {
                    from.SendLocalizedMessage(1005629); // You can not exceed the stat cap.  Try setting another stat lower first.
                }
                else
                {
                    from.RawInt = value;
                    from.SendLocalizedMessage(1005630); // Your stats have been adjusted.
                }
            }
        }

        private static void ChangeSkill(Mobile from, string name, double value)
        {
            SkillName index;

            if (!Enum.TryParse(name, true, out index) || (!Core.SE && (int)index > 51) || (!Core.AOS && (int)index > 48))
            {
                from.SendLocalizedMessage(1005631); // You have specified an invalid skill to set.
                return;
            }

            Skill skill = from.Skills[index];

            if (skill != null)
            {
                if (value < 0 || value > skill.Cap)
                {
                    from.SendMessage(String.Format("Your skill in {0} is capped at {1:F1}.", skill.Info.Name, skill.Cap));
                }
                else
                {
                    int newFixedPoint = (int)(value * 10.0);
                    int oldFixedPoint = skill.BaseFixedPoint;

                    if (((skill.Owner.Total - oldFixedPoint) + newFixedPoint) > skill.Owner.Cap)
                    {
                        from.SendMessage("You can not exceed the skill cap.  Try setting another skill lower first.");
                    }
                    else
                    {
                        skill.BaseFixedPoint = newFixedPoint;
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(1005631); // You have specified an invalid skill to set.
            }
        }

        public class TCHelpGump : Gump
        {
            public TCHelpGump()
                : base(40, 40)
            {
                this.AddPage(0);
                this.AddBackground(0, 0, 160, 120, 5054);

                this.AddButton(10, 10, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
                this.AddLabel(45, 10, 0x34, "ServUO");

                this.AddButton(10, 35, 0xFB7, 0xFB9, 2, GumpButtonType.Reply, 0);
                this.AddLabel(45, 35, 0x34, "List of skills");

                this.AddButton(10, 60, 0xFB7, 0xFB9, 3, GumpButtonType.Reply, 0);
                this.AddLabel(45, 60, 0x34, "Command list");

                this.AddButton(10, 85, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
                this.AddLabel(45, 85, 0x34, "Close");
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                switch ( info.ButtonID )
                {
                    case 1:
                        {
                            sender.LaunchBrowser("http://ServUO.craftuo.com/");
                            break;
                        }
                    case 2: // List of skills
                        {
                            string[] strings = Enum.GetNames(typeof(SkillName));

                            Array.Sort(strings);

                            StringBuilder sb = new StringBuilder();

                            if (strings.Length > 0)
                                sb.Append(strings[0]);

                            for (int i = 1; i < strings.Length; ++i)
                            {
                                string v = strings[i];

                                if ((sb.Length + 1 + v.Length) >= 256)
                                {
                                    sender.Send(new AsciiMessage(Server.Serial.MinusOne, -1, MessageType.Label, 0x35, 3, "System", sb.ToString()));
                                    sb = new StringBuilder();
                                    sb.Append(v);
                                }
                                else
                                {
                                    sb.Append(' ');
                                    sb.Append(v);
                                }
                            }

                            if (sb.Length > 0)
                            {
                                sender.Send(new AsciiMessage(Server.Serial.MinusOne, -1, MessageType.Label, 0x35, 3, "System", sb.ToString()));
                            }

                            break;
                        }
                    case 3: // Command list
                        {
                            sender.Mobile.SendAsciiMessage(0x482, "The command prefix is \"{0}\"", CommandSystem.Prefix);
                            CommandHandlers.Help_OnCommand(new CommandEventArgs(sender.Mobile, "help", "", new string[0]));

                            break;
                        }
                }
            }
        }
    }
}