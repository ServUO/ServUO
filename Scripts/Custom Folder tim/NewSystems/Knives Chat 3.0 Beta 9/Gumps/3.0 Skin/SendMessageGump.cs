using System;
using System.Collections;
using Server;
using Server.HuePickers;

namespace Knives.Chat3
{
    public class SendMessageGump : GumpPlus
    {
        #region Class Definitions

        private Mobile c_From, c_To;
        private Message c_Reply;
        private string c_Text, c_Subject;
        private MsgType c_MsgType;

        #endregion

        #region Constructors

        public SendMessageGump(Mobile from, Mobile to, string txt, Message reply, MsgType type)
            : base(from, 200, 200)
        {
            from.CloseGump(typeof(SendMessageGump));

            Override = true;

            c_From = from;
            c_To = to;
            c_Text = txt;
            c_Subject = "";
            c_Reply = reply;
            c_MsgType = type;

            if (c_Reply != null)
            {
                if (c_Reply.Subject.IndexOf("RE:") != 0)
                    c_Subject = "RE: " + c_Reply.Subject;
                else
                    c_Subject = c_Reply.Subject;
            }
        }

        #endregion

        #region Methods

        protected override void BuildGump()
        {
            int width = Data.GetData(Owner).ExtraPm ? 400 : 300;
            int y = 10;
            int field = Data.GetData(Owner).ExtraPm ? 300 : 150;

            if (c_MsgType == MsgType.System)
                AddHtml(0, y, width, "<CENTER>" + General.Local(94));
            else if (c_MsgType == MsgType.Staff)
                AddHtml(0, y, width, "<CENTER>" + General.Local(256));
            else
                AddHtml(0, y, width, "<CENTER>" + General.Local(62) + " " + c_To.RawName);

            AddImage(width / 2 - 120, y + 2, 0x39);
            AddImage(width / 2 + 90, y + 2, 0x3B);

            if (Data.GetData(Owner).Recording == this)
            {
                AddHtml(30, y+=20, width-60, 25, c_Subject, true, false);
                AddHtml(20, y+=30, width-40, field, c_Text, true, true);
                AddHtml(0, y+=(field+20), width, "<CENTER>" + General.Local(63));
            }
            else
            {
                AddTextField(30, y+=20, width - 60, 21, Data.GetData(Owner).MsgC, 0xBBC, "Subject", c_Subject);
                AddTextField(20, y+=30, width - 40, field, Data.GetData(Owner).MsgC, 0xBBC, "Text", c_Text);

                y+=(field+15);

                if(Data.GetData(Owner).ExtraPm)
                    AddButton(20, y, 0x2333, "Record", new GumpCallback(Record));

                AddButton(50, y, Data.GetData(Owner).ExtraPm ? 0x25E4 : 0x25E8, Data.GetData(Owner).ExtraPm ? 0x25E5 : 0x25E9, "ExtraPm", new GumpCallback(ExtraPm));
            }

            AddImage(width / 2 - 10, y, 0x2342, Data.GetData(Owner).MsgC);
            AddButton(width / 2 - 6, y + 4, 0x2716, "Channel Color", new GumpCallback(Color));
            AddHtml(width - 85, y, 50, General.Local(252));
            AddButton(width-100, y+3, 0x2716, "Send", new GumpCallback(Send));

            AddBackgroundZero(0, 0, width, y + 30, Data.GetData(Owner).DefaultBack);
        }

        #endregion

        #region Responses

        private void ExtraPm()
        {
            Data.GetData(Owner).ExtraPm = !Data.GetData(Owner).ExtraPm;
            NewGump();
        }

        public void AddText(string txt)
        {
            c_Text += txt;
            NewGump();
        }

        private void Save()
        {
            c_Subject = GetTextField("Subject");
            c_Text = GetTextField("Text");
        }

        private void Record()
        {
            Save();

            if (c_Subject.Trim() == "")
                c_Subject = "No Subject";
 
            Data.GetData(Owner).Recording = this;
            Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(65));

            NewGump();
        }

        private void Send()
        {
            if( Data.GetData(Owner).Recording == null )
                Save();

            if (c_Text.Trim() == "")
            {
                Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(66));
                NewGump();
                return;
            }

            if (c_Subject.Trim() == "")
                c_Subject = "No subject";

            if (!TrackSpam.LogSpam(Owner, "Message", TimeSpan.FromSeconds(Data.MsgSpam)))
            {
                Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(97));
                NewGump();
                return;
            }

            if (Data.GetData(Owner).Recording == this)
                Data.GetData(Owner).Recording = null;

            if (Data.FilterMsg)
            {
                c_Text = Filter.FilterText(Owner, c_Text, false);
                c_Subject = Filter.FilterText(Owner, c_Subject, false);
            }

            if (c_MsgType == MsgType.System)
            {
                foreach (Data data in Data.Datas.Values)
                {
                    data.AddMessage(new Message(Owner, c_Subject, c_Text, MsgType.System));
                    General.PmNotify(data.Mobile);
                }
            }
            else if (c_MsgType == MsgType.Staff)
            {
                foreach (Data data in Data.Datas.Values)
                {
                    if (data.Mobile.AccessLevel != AccessLevel.Player)
                    {
                        data.AddMessage(new Message(Owner, c_Subject, c_Text, MsgType.Staff));
                        General.PmNotify(data.Mobile);
                    }
                }
            }
            else
            {
                Data.GetData(c_To).AddMessage(new Message(Owner, c_Subject, c_Text, MsgType.Normal));
                General.PmNotify(c_To);

                Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(67) + " " + c_To.RawName);
                if (Data.GetData(c_To).Status != OnlineStatus.Online)
                    Owner.SendMessage(Data.GetData(Owner).SystemC, c_To.RawName + ": " + Data.GetData(c_To).AwayMsg);
            }

            if (Data.LogPms)
                Logging.LogPm(String.Format(DateTime.Now + " <Mail> {0} to {1}: {2}", Owner.RawName, (c_To == null ? "All" : c_To.RawName), c_Text));
 
            foreach( Data data in Data.Datas.Values)
                if (data.Mobile.AccessLevel >= c_From.AccessLevel && ((data.GlobalM && !data.GIgnores.Contains(c_From)) || data.GListens.Contains(c_From)))
                    data.Mobile.SendMessage(data.GlobalMC, String.Format("(Global) <Mail> {0} to {1}: {2}", Owner.RawName, (c_To == null ? "All" : c_To.RawName), c_Text ));
        }

        private void Color()
        {
            Owner.SendHuePicker(new InternalPicker(this));
        }

        #endregion

        #region Internal Classes

        private class InternalPicker : HuePicker
        {
            private GumpPlus c_Gump;

            public InternalPicker(GumpPlus g)
                : base(0x1018)
            {
                c_Gump = g;
            }

            public override void OnResponse(int hue)
            {
                Data.GetData(c_Gump.Owner).MsgC = hue;

                c_Gump.NewGump();
            }
        }

        #endregion
    }
}