using Server;
using System;
using Server.Gumps;
using Server.Accounting;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class SetEMail : Gump
    {
    	
    	public static void Initialize()
		{
			CommandSystem.Register ( "setemail", AccessLevel.Player, new CommandEventHandler ( Email_OnCommand ) );
		}
    	
		public static void Email_OnCommand( CommandEventArgs e )
		{
			Account acct = e.Mobile.Account as Account;
			string MailString = acct.GetTag( "EMailRecieved" );
			if( MailString != null )
			{
				e.Mobile.SendMessage( "E-Mail has already been set" );
			}
			else
			{
			e.Mobile.CloseGump( typeof ( SetEMail ) );
			e.Mobile.SendGump( new SetEMail() );
			}
		}
		
       public SetEMail() : base(0, 0)
       {
           this.Closable = true;
           this.Disposable = true;
           this.Dragable = true;
           this.Resizable = false;
           this.AddPage(0);

           this.AddBackground(133, 77, 414, 206, 3600);

           this.AddLabel(279, 97, 1150, @"Set E-Mail");
           this.AddLabel(160, 130, 1150, @"Magic Word:"); //1299
           this.AddLabel(160, 160, 1150, @"E-Mail:");
           this.AddLabel(160, 190, 1150, @"Confirm E-Mail:");

           this.AddButton(306, 230, 247, 248, 1, GumpButtonType.Reply, 0);

           this.AddAlphaRegion(321, 190, 200, 20);
           this.AddAlphaRegion(321, 160, 200, 20);
           this.AddAlphaRegion(321, 130, 200, 20);

           this.AddTextEntry( 321, 130, 200, 20, 1175, 1, "");
           this.AddTextEntry( 321, 160, 200, 20, 1175, 2, "");
           this.AddTextEntry( 321, 190, 200, 20, 1175, 3, "");
       }
       
       private string GetString(RelayInfo info, int id)
	   {
			TextRelay t = info.GetTextEntry(id);
			return (t == null ? null : t.Text.Trim());
	   }
       
       public override void OnResponse(NetState sender, RelayInfo info)
       {
            Mobile m = sender.Mobile;
            switch (info.ButtonID)
            {
            	case 0:
            		{
            			m.SendMessage(1278, "Your e-mail has not been set.");
            			return;
            		}
            	case 1:
            		{
            			string MagicWord = GetString( info, 1 );
            			string EMAIL = GetString( info, 2 );
            			string confirmEMAIL = GetString( info, 3 );

				Account acct = m.Account as Account;
				
            			if ( EMAIL.Length <= 6 || MagicWord.Length <= 3 || EMAIL.Length > 40 || MagicWord.Length > 40 )
				{
					acct.RemoveTag( "EMAIL" );
					m.CloseGump( typeof( SetEMail ) );
					m.SendGump( new SetEMail() );
					m.SendMessage(37, "Your e-mail must be at least 7 letters or numbers.  Magic Word must be at least 4 letters or numbers.");
					return;
				}				

            			if ( EMAIL != confirmEMAIL ) 
            			{
            				m.SendMessage(37, "The 'E-mail' values do not match. Remember it is cAsE sEnSaTiVe. ");
            				m.CloseGump( typeof( SetEMail ) );
            				m.SendGump( new SetEMail() );
            				return;
            			}
            			

	            		if ( EMAIL != null && MagicWord != null) 
            			{
					
					acct.SetTag( "EMailRecieved", " ( " + EMAIL + " ) " );
					acct.SetTag( "MagicWord", " ( " + MagicWord + " ) " );
					
            				m.SendMessage(68, "Your Magic Word is '" + MagicWord + "' and your e-mail is '" + EMAIL + "'. Remember this!");
           	 		}
				
            			break;
            		}
            }
       }
    }
}