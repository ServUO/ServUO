using Server;
using System;
using Server.Gumps;
using Server.Accounting;
using Server.Commands;
using Server.Network;

namespace Server.Gumps
{
    public class PasswordChanger : Gump
    {
    	
    	public static void Initialize()
		{
			CommandSystem.Register ( "password", AccessLevel.Player, new CommandEventHandler ( Password_OnCommand ) );
		}
    	
		public static void Password_OnCommand( CommandEventArgs e )
		{
			e.Mobile.CloseGump( typeof ( PasswordChanger ) );
			e.Mobile.SendGump( new PasswordChanger() );
		}
		
       public PasswordChanger() : base(0, 0)
       {
           this.Closable = true;
           this.Disposable = true;
           this.Dragable = false;
           this.Resizable = false;
           this.AddPage(0);

           this.AddBackground(133, 77, 414, 206, 3600);

           this.AddLabel(279, 97, 1178, @"Password Changer");
           this.AddLabel(160, 130, 1299, @"Current Password:");
           this.AddLabel(160, 160, 1299, @"New Password:");
           this.AddLabel(160, 190, 1299, @"Confirm New Password:");

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
            			m.SendMessage(1300, "Your password has not changed.");
            			return;
            		}
            	case 1:
            		{
            			string origPass = GetString( info, 1 );
            			string newPass = GetString( info, 2 );
            			string confirmNewPass = GetString( info, 3 );
            			
            			if ( newPass != confirmNewPass ) //Two "New password" fields do not match
            			{
            				m.SendMessage(37, "The 'Confirm New Password' value does not match the 'New Password'. Remember it is cAsE sEnSaTiVe. ");
            				m.CloseGump( typeof( PasswordChanger ) );
            				m.SendGump( new PasswordChanger() );
            				return;
            			}
            			
						for( int i = 0; i < newPass.Length; ++i )
						{
							if( !(char.IsLetterOrDigit( newPass[i] )) ) //Char is NOT a letter or digit
							{
								m.SendMessage(37, "Passwords may only consist of letters (A - Z) and Digits (0 - 9).");
								return;
							}
						}
         	
            			Account a = m.Account as Account;
            
            			if ( !(a.CheckPassword( origPass )) ) //"Current Password" value is incorrect
            			{
            				m.SendMessage(37, "The 'Current Password' value is incorrect. [ " + origPass + " ].");
            				m.CloseGump( typeof( PasswordChanger ) );
            				m.SendGump( new PasswordChanger() );
            				return;
            			}
            
	            		if ( (a.CheckPassword( origPass )) && (newPass == confirmNewPass) ) //Current password is correct, and 2 "New Password" fields match.
            			{
            				a.SetPassword( newPass );
            				m.SendMessage(68, "Your account ( " + a.Username + " ) password has been changed to '" + newPass + "'. Remember this!");
           	 			}
            			break;
            		}
            }
       }
    }
}
