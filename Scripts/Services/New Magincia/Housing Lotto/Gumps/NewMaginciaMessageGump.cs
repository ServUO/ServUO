using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public class NewMaginciaMessageGump : BaseGump
    {
        public List<NewMaginciaMessage> Messages;

        public readonly int LightBlueColor = 0x4AFD;
        public readonly int GreenColor = 0x4BB7;        

        public NewMaginciaMessageGump(PlayerMobile from)
            : base(from, 490, 30)
        {
            Messages = MaginciaLottoSystem.GetMessages(from);            
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 164, 32, 0x24B8);
            AddButton(7, 7, 0x1523, 0x1523, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(37, 7, 120, 18, 1150425, String.Format("{0}", Messages.Count), GreenColor, false, false); // ~1_COUNT~ Messages
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                    {
                        if (Messages.Count != 0)
                        {
                            SendGump(new NewMaginciaMessageGump(User));
                        }

                        break;
                    }
                case 1:
                    {
                        SendGump(new NewMaginciaMessageListGump(User));
                        break;
                    }
            }
        }
    }

    public class NewMaginciaMessageListGump : BaseGump
    {
        public readonly int GreenColor = 0x4BB7;
        public readonly int LightBlueColor = 0x4AFD;

        public bool Widescreen;
        public List<NewMaginciaMessage> Messages;

        public NewMaginciaMessageListGump(PlayerMobile from, bool widescreen = false)
            : base(from, 490, 30)
        {
            Widescreen = widescreen;
            Messages = MaginciaLottoSystem.GetMessages(from);            
        }

        public override void AddGumpLayout()
        {
            if (Messages == null)
            {
                return;
            }

            AddPage(0);

            int width = (Widescreen ? 200 : 0);
            int buttonid = (Widescreen ? 0x1519 : 0x151A);

            AddBackground(0, 0, 314 + width, 241 + width, 0x24B8);
            AddButton(7, 7, 0x1523, 0x1523, 0, GumpButtonType.Reply, 0);
            AddButton(290 + width, 7, buttonid, buttonid, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(47, 7, Widescreen ? 460 : 194, 18, 1150425, String.Format("{0}", Messages.Count), GreenColor, false, false); // ~1_COUNT~ Messages   

            int page = 1;
            int y = 0;

            AddPage(page);

            for (int i = 0; i < Messages.Count; i++)
            {
                if (page > 1)
                    AddButton(Widescreen ? 446 : 246, 7, 0x1458, 0x1458, 0, GumpButtonType.Page, page - 1);

                var message = Messages[i];

                if (message == null)
                    continue;

                if (message.Body.Number > 0)
                {
                    if (message.Args == null)
                    {
                        AddHtmlLocalized(47, 34 + (y * 32), 260 + width, 16, message.Body, LightBlueColor, false, false);
                    }
                    else
                    {
                        AddHtmlLocalized(47, 34 + (y * 32), 260 + width, 16, message.Body, message.Args, LightBlueColor, false, false);
                    }
                }
                else
                {
                    AddHtml(40, 34 + (y * 32), 260 + width, 16, Color("#94BDEF", message.Body.String), false, false);
                }

                AddButton(7, 34 + (y * 32), 4029, 4031, i + 1000, GumpButtonType.Reply, 0);

                y++;

                bool pages = Widescreen && (i + 1) % 12 == 0 || !Widescreen && (i + 1) % 6 == 0;

                if (pages && Messages.Count - 1 != i)
                {
                    AddButton(Widescreen ? 468 : 268, 7, 0x1459, 0x1459, 0, GumpButtonType.Page, page + 1);
                    page++;
                    y = 0;

                    AddPage(page);
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                    {
                        SendGump(new NewMaginciaMessageGump(User));
                        break;
                    }
                case 1:
                    {
                        SendGump(new NewMaginciaMessageListGump(User, Widescreen ? false : true));
                        break;
                    }
                default:
                    {
                        int id = info.ButtonID - 1000;

                        if (id >= 0 && id < Messages.Count)
                        {
                            SendGump(new NewMaginciaMessageDetailGump(User, Messages, id));
                        }

                        break;
                    }
            }
        }
    }

    public class NewMaginciaMessageDetailGump : BaseGump
    {
        public NewMaginciaMessage Message;
        public List<NewMaginciaMessage> Messages;

        public readonly int GreenColor = 0x4BB7;
        public readonly int BlueColor = 0x110;
        public readonly int EntryColor = 0x76F2;

        public NewMaginciaMessageDetailGump(PlayerMobile from, List<NewMaginciaMessage> messages, int messageid)
            : base(from, 490, 30)
        {
            Messages = messages;
            Message = messages[messageid];

            
        }

        public override void AddGumpLayout()
        {
            if (Message != null)
            {
                AddPage(0);

                AddBackground(0, 0, 414, 341, 0x24B8);
                AddButton(7, 7, 0x1523, 0x1523, 0, GumpButtonType.Reply, 0);
                AddButton(390, 7, 0x1519, 0x151A, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(47, 7, 360, 18, 1150425, String.Format("{0}", Messages.Count), GreenColor, false, false); // ~1_COUNT~ Messages

                if (Message.Body != null)
                {
                    if (Message.Body.Number != 0)
                    {
                        if (Message.Args == null)
                        {
                            AddHtmlLocalized(7, 34, 404, 150, Message.Body.Number, BlueColor, true, true);
                        }
                        else
                        {
                            AddHtmlLocalized(7, 34, 404, 150, Message.Body.Number, Message.Args, BlueColor, true, true);
                        }
                    }
                    else
                    {
                        AddHtml(7, 34, 404, 150, Color("#004284", Message.Body.String), true, true);
                    }
                }

                TimeSpan ts = Message.Expires - DateTime.UtcNow;

                AddHtmlLocalized(7, 194, 400, 18, 1150432, String.Format("@{0}@{1}@{2}", ts.Days, ts.Hours, ts.Minutes), GreenColor, false, false); // This message will expire in ~1_DAYS~ days, ~2_HOURS~ hours, and ~3_MIN~ minutes.

                AddHtmlLocalized(47, 212, 360, 22, 1150433, EntryColor, false, false); // DELETE NOW
                AddButton(7, 212, 4005, 4007, 2, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                    {
                        SendGump(new NewMaginciaMessageGump(User));

                        break;
                    }
                case 1:
                    {
                        SendGump(new NewMaginciaMessageListGump(User));
                    }
                    break;
                case 2:
                    {
                        if (Message != null)
                        {
                            List<NewMaginciaMessage> messages = MaginciaLottoSystem.MessageQueue[User];

                            if (messages == null)
                            {
                                MaginciaLottoSystem.MessageQueue.Remove(User);
                            }
                            else
                            {
                                MaginciaLottoSystem.RemoveMessageFromQueue(User, Message);

                                if (MaginciaLottoSystem.HasMessageInQueue(User))
                                {
                                    SendGump(new NewMaginciaMessageListGump(User));
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
