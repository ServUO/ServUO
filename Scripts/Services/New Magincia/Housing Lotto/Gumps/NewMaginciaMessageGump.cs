using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Network;

namespace Server.Engines.NewMagincia
{
    public class NewMaginciaMessageGump : BaseGump
    {
        private List<NewMaginciaMessage> m_Messages;
        private NewMaginciaMessage m_Message;

        private readonly int BlueColor = 0x1E90FF;
        private readonly int LightBlueColor = 0x87CEFA;
        private readonly int GreenColor = 0x7FFFD4;
        private readonly int EntryColor = 0xFF7F50;

        public NewMaginciaMessageGump(PlayerMobile from, NewMaginciaMessage message = null)
            : base(from, 75, 75)
        {
            m_Message = message;
            m_Messages = MaginciaLottoSystem.GetMessages(from);
        }

        public override void AddGumpLayout()
        {
            if (m_Messages == null)
            {
                return;
            }

            if (m_Message == null)
            {
                AddImage(5, 10, 5411);

                AddBackground(0, 0, 300, 351, 9500);
                AddButton(278, 7, 5402, 5402, 2, GumpButtonType.Reply, 0);

                for (int i = 0; i < m_Messages.Count; i++)
                {
                    var message = m_Messages[i];

                    if (message == null)
                        continue;

                    if (message.Title != null)
                    {
                        if (message.Title.Number > 0)
                        {
                            AddHtmlLocalized(40, 35 + (i * 22), 260, 20, message.Title.Number, C32216(LightBlueColor), false, false);
                        }
                        else
                        {
                            AddHtml(40, 35 + (i * 22), 260, 20, Color("#87CEFA", message.Title.String), false, false);
                        }
                    }
                    else
                    {
                        if (message.Body.Number > 0)
                        {
                            AddHtmlLocalized(40, 35 + (i * 22), 260, 20, message.Body, C32216(LightBlueColor), false, false);
                        }
                        else
                        {
                            AddHtml(40, 35 + (i * 22), 260, 20, Color("#87CEFA", message.Body.String), false, false);
                        }
                    }

                    AddButton(5, 35 + (i * 22), 4029, 4031, i + 1000, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                int messages = m_Messages.Count;

                AddBackground(0, 0, 424, 351, 9500);
                AddImage(5, 10, 5411);

                AddButton(403, 7, 5401, 5401, 2, GumpButtonType.Reply, 0);

                AddHtmlLocalized(40, 10, 100, 16, 1150425, messages.ToString(), GreenColor, false, false);

                if (m_Message.Title != null)
                {
                    if (m_Message.Title.Number != 0)
                        AddHtmlLocalized(195, 13, 150, 16, m_Message.Title, GreenColor, false, false);
                    else
                        AddHtml(150, 13, 195, 16, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", GreenColor, m_Message.Title), false, false);
                }

                if (m_Message.Body != null)
                {
                    if (m_Message.Body.Number != 0)
                    {
                        if (m_Message.Args == null)
                            AddHtmlLocalized(10, 40, 404, 180, m_Message.Body, BlueColor, true, true);
                        else
                            AddHtmlLocalized(10, 40, 404, 180, m_Message.Body, m_Message.Args, BlueColor, true, true);
                    }
                    else
                        AddHtml(10, 40, 404, 180, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", BlueColor, m_Message.Body), true, true);
                }

                TimeSpan ts = m_Message.Expires - DateTime.UtcNow;

                AddHtmlLocalized(5, 230, 414, 16, 1150432, String.Format("{0}\t{1}\t{2}", ts.Days, ts.Hours, ts.Minutes), GreenColor, false, false); // This message will expire in ~1_DAYS~ days, ~2_HOURS~ hours, and ~3_MIN~ minutes.

                AddButton(5, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 250, 150, 16, 1150433, EntryColor, false, false); // DELETE NOW
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: break;
                case 1:
                    if (m_Message != null)
                    {
                        List<NewMaginciaMessage> messages = MaginciaLottoSystem.MessageQueue[User];

                        if (messages == null)
                        {
                            MaginciaLottoSystem.MessageQueue.Remove(User);
                        }
                        else
                        {
                            MaginciaLottoSystem.RemoveMessageFromQueue(User, m_Message);

                            if (MaginciaLottoSystem.HasMessageInQueue(User))
                            {
                                m_Message = m_Messages[0];
                                Refresh();
                            }
                        }
                    }
                    break;
                case 2:
                    if (m_Message != null)
                    {
                        m_Message = null;
                        Refresh();
                    }
                    else if (MaginciaLottoSystem.HasMessageInQueue(User))
                    {
                        if(m_Messages != null && m_Messages.Count > 0)
                            m_Message = m_Messages[0];

                        Refresh();
                    }
                    break;
                default:
                    int id = info.ButtonID - 1000;
                    if (id >= 0 && id < m_Messages.Count)
                    {
                        m_Message = m_Messages[id];
                    }

                    Refresh();
                    break;
            }
        }
    }
}