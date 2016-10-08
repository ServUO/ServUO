using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
	public class ConfirmCallbackGump : Gump
	{
		public Action<Mobile, object> ConfirmCallback { get; set; }
		public Action<Mobile, object> CloseCallback { get; set; }

        public Mobile User { get; set; }

		public object Title { get; set; }
		public object Body { get; set; }
		public object State { get; set; }
		public string Arguments { get; set; }
		
        public ConfirmCallbackGump(PlayerMobile user, object title, object body, object state, string args = null, Action<Mobile, object> confirm = null, Action<Mobile, object> close = null, int x = 20, int y = 20)
            : base(x, y)
		{
            User = user;

			Title = title;
			Body = body;
			State = state;
			Arguments = args;
			
			ConfirmCallback = confirm;
			CloseCallback = close;

            AddGumpLayout();
		}
		
		public void AddGumpLayout()
		{
			AddImageTiled( 0, 0, 348, 262, 0xA8E );
			AddAlphaRegion( 0, 0, 348, 262 );
			AddImage( 0, 15, 0x27A8 ); 
			AddImageTiled( 0, 30, 17, 200, 0x27A7 );
			AddImage( 0, 230, 0x27AA ); 
			AddImage( 15, 0, 0x280C ); 
			AddImageTiled( 30, 0, 300, 17, 0x280A );
			AddImage( 315, 0, 0x280E ); 
			AddImage( 15, 244, 0x280C ); 
			AddImageTiled( 30, 244, 300, 17, 0x280A );
			AddImage( 315, 244, 0x280E ); 
			AddImage( 330, 15, 0x27A8 ); 
			AddImageTiled( 330, 30, 17, 200, 0x27A7 );
			AddImage( 330, 230, 0x27AA ); 
			AddImage( 333, 2, 0x2716 ); 
			AddImage( 333, 248, 0x2716 ); 
			AddImage( 2, 248, 0x2716 ); 
			AddImage( 2, 2, 0x2716 );             

			if (Title is int) 
			   AddHtmlLocalized( 25, 25, 200, 20, (int)Title, 0x7D00, false, false );
			else  if (Title is string)
			   AddHtml( 25, 25, 200, 20, String.Format("<basefont color=#FF0000>{0}", (string)Title), false, false );

			AddImage( 25, 45, 0xBBF ); 

			if (Body is int)
			{
				if(Arguments != null)
					AddHtmlLocalized( 25, 55, 300, 120, (int)Body, Arguments, 0xFFFFFF, false, false );
				else
					AddHtmlLocalized( 25, 55, 300, 120, (int)Body, 0xFFFFFF, false, false );
			}
			else if(Body is string)
			   AddHtml( 25, 55, 300, 120, String.Format("<BASEFONT COLOR=#FFFFFF>{0}</BASEFONT>", (string)Body), false, false );

			AddRadio( 25, 175, 0x25F8, 0x25FB, true, 1);
			AddRadio( 25, 210, 0x25F8, 0x25FB, false, 2);

			AddHtmlLocalized( 60, 180, 280, 20, 1074976, 0xFFFFFF, false, false );
			AddHtmlLocalized( 60, 215, 280, 20, 1074977, 0xFFFFFF, false, false );

            AddButton(265, 220, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0);
		}

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID != 1)
                return;

            bool confirm = info.IsSwitched(1);

            if (confirm)
            {
                if (ConfirmCallback != null)
                    ConfirmCallback(User, State);
            }
            else if (CloseCallback != null)
            {
                CloseCallback(User, State);
            }
        }
	}
}