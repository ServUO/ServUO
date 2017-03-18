using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Network;

namespace Server.Engines.NewMagincia
{
    public class NewMaginciaMessageGump : Gump
    {
        private Mobile m_From;
        private NewMaginciaMessage m_Message;

        //private readonly int BlueColor = 0x00BFFF;
        private readonly int BlueColor = 0x1E90FF;
        private readonly int GreenColor = 0x7FFFD4;
        private readonly int EntryColor = 0xFF7F50;

        public NewMaginciaMessageGump(Mobile from, NewMaginciaMessage message) : base(75, 75)
        {
            m_From = from;
            m_Message = message;

            int messages = 1;

            if (MaginciaLottoSystem.MessageQueue.ContainsKey(from) && MaginciaLottoSystem.MessageQueue[from] != null)
                messages = MaginciaLottoSystem.MessageQueue[from].Count;

            AddBackground(0, 0, 424, 351, 9500);
            AddImage(5, 10, 5411);

            AddHtmlLocalized(40, 10, 100, 16, 1150425, messages.ToString(), GreenColor, false, false);

            if (message.Title != null)
            {
                if (message.Title.Number != 0)
                    AddHtmlLocalized(195, 13, 150, 16, message.Title, GreenColor, false, false);
                else
                    AddHtml(150, 13, 195, 16, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", GreenColor, message.Title), false, false);
            }

            if (message.Body != null)
            {
                if (message.Body.Number != 0)
                {
                    if(message.Args == null)
                        AddHtmlLocalized(10, 40, 404, 180, message.Body, BlueColor, true, true);
                    else
                        AddHtmlLocalized(10, 40, 404, 180, message.Body, message.Args, BlueColor, true, true);
                }
                else
                    AddHtml(10, 40, 404, 180, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", BlueColor, message.Body), true, true);
            }

            DateTime expires = message.Created + NewMaginciaMessage.ExpirePeriod;
            TimeSpan ts = expires - DateTime.UtcNow;

            AddHtmlLocalized(5, 230, 414, 16, 1150432, String.Format("{0}\t{1}\t{2}", ts.Days, ts.Hours, ts.Minutes), GreenColor, false, false); // This message will expire in ~1_DAYS~ days, ~2_HOURS~ hours, and ~3_MIN~ minutes.

            AddButton(5, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 250, 150, 16, 1150433, EntryColor, false, false); // DELETE NOW
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (MaginciaLottoSystem.MessageQueue.ContainsKey(m_From))
                {
                    List<NewMaginciaMessage> messages = MaginciaLottoSystem.MessageQueue[m_From];

                    if (messages == null)
                        MaginciaLottoSystem.MessageQueue.Remove(m_From);
                    else
                    {
                        MaginciaLottoSystem.RemoveMessageFromQueue(m_From, m_Message);

                        if (MaginciaLottoSystem.HasMessageInQueue(m_From))
                            m_From.SendGump(new NewMaginciaMessageGump(m_From, messages[0]));
                    }
                }
            }
        }
    }
}