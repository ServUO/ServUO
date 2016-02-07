using System;

namespace Server.Gumps
{
    public class BaseConfirmGump : Gump
    {
        public BaseConfirmGump()
            : base(120, 50)
        { 
            this.Closable = false;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
		
            this.AddPage(0);
			
            this.AddImageTiled(0, 0, 348, 262, 0xA8E);
            this.AddAlphaRegion(0, 0, 348, 262);
            this.AddImage(0, 15, 0x27A8); 
            this.AddImageTiled(0, 30, 17, 200, 0x27A7);
            this.AddImage(0, 230, 0x27AA); 
            this.AddImage(15, 230, 0x280C); 
            this.AddImageTiled(30, 0, 300, 17, 0x280A);
            this.AddImage(315, 0, 0x280E); 
            this.AddImage(15, 244, 0x280C); 
            this.AddImageTiled(30, 244, 300, 17, 0x280A);
            this.AddImage(315, 244, 0x280E); 
            this.AddImage(330, 15, 0x27A8); 
            this.AddImageTiled(330, 30, 17, 200, 0x27A7);
            this.AddImage(330, 230, 0x27AA); 
            this.AddImage(333, 2, 0x2716); 
            this.AddImage(315, 248, 0x2716); 
            this.AddImage(2, 248, 0x2716); 
            this.AddImage(2, 2, 0x2716); 			
            this.AddHtmlLocalized(25, 25, 200, 20, this.TitleNumber, 0x7D00, false, false);
            this.AddImage(25, 40, 0xBBF); 
            this.AddHtmlLocalized(25, 55, 300, 120, this.LabelNumber, 0xFFFFFF, false, false);
			
            this.AddRadio(25, 175, 0x25F8, 0x25FB, true, (int)Buttons.Break);
            this.AddRadio(25, 210, 0x25F8, 0x25FB, false, (int)Buttons.Close);
			
            this.AddHtmlLocalized(60, 180, 280, 20, 1074976, 0xFFFFFF, false, false);
            this.AddHtmlLocalized(60, 215, 280, 20, 1074977, 0xFFFFFF, false, false);
			
            this.AddButton(265, 220, 0xF7, 0xF8, (int)Buttons.Confirm, GumpButtonType.Reply, 0);
        }

        private enum Buttons
        {
            Close,
            Break,
            Confirm
        }
        public virtual int TitleNumber
        {
            get
            {
                return 1075083;
            }
        }// <center>Warning!</center>
        public virtual int LabelNumber
        {
            get
            {
                return 1074975;
            }
        }// Are you sure you wish to select this?
        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        { 
            if (info.ButtonID == (int)Buttons.Confirm)
            {
                if (info.IsSwitched((int)Buttons.Break))
                    this.Confirm(state.Mobile);
                else
                    this.Refuse(state.Mobile);
            }
        }

        public virtual void Confirm(Mobile from)
        { 
        }

        public virtual void Refuse(Mobile from)
        {
        }
    }
}