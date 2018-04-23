//==============================================//
// Base Created by Dupre					//
// Masterfully Modified (to be a garden)--by DarkJustin from The Bluegrass Shard-- Where Tweaking is life... :-p							//
//==============================================//

using System; 
using Server; 
using Server.Gumps; 
using Server.Network;
using Server.Items;
using Server.Mobiles;

namespace Server.Gumps
{   
public class GardenDGump : Gump
{
public GardenDGump( GardenDestroyer gardendestroyer, Mobile owner) : base( 150,75 ) 
{
m_GardenDestroyer = gardendestroyer;
owner.CloseGump( typeof( GardenDGump ) );
this.Closable=false;
this.Disposable=false;
this.Dragable=true;
this.Resizable=false;
this.AddPage(0);
this.AddBackground(0, 0, 445, 250, 9200);
this.AddBackground(10, 10, 425, 160, 3500);
this.AddLabel(95, 30, 195, @"* Do you want to destroy your garden? *");
this.AddLabel(60, 70, 1359, @"I hope you took heed to my warning, and removed any");
this.AddLabel(60, 90, 1359, @"items from your Garden Secure before you decide to");
this.AddLabel(60, 110, 1359, @"destroy your ENTIRE garden.");
this.AddLabel(107, 205, 172, @"Destroy");
this.AddLabel(270, 205, 32, @"Don't Destroy");
AddButton( 115, 180, 4023, 4024, 1, GumpButtonType.Reply, 0 );
AddButton( 295, 180, 4017, 4018, 0, GumpButtonType.Reply, 0 );
}

private GardenDestroyer m_GardenDestroyer; 

public override void OnResponse( NetState state, RelayInfo info ) //Function for GumpButtonType.Reply Buttons 
      { 

         Mobile from = state.Mobile; 

         switch ( info.ButtonID ) 
         { 
            case 0: //Case uses the ActionIDs defenied above. Case 0 defenies the actions for the button with the action id 0 
            { 
		   //Cancel
       	   from.SendMessage( "Your choose not to destroy your garden." ); 
               break; 
            } 

            case 1: //Case uses the ActionIDs defenied above. Case 0 defenies the actions for the button with the action id 0 
            { 
               
               //RePack 
			m_GardenDestroyer.Delete();
			from.AddToBackpack( new GardenDeed() ); 
			from.SendMessage( "You destroyed your garden, and placed the creation tools back in your back pack." ); 
			break;
            } 
	    }
      }}
   
public class GardenGump : Gump
{
public GardenGump( Mobile owner ) : base( 150,75 )
{
this.Closable=false;
this.Disposable=false;
this.Dragable=true;
this.Resizable=false;
this.AddPage(0);
this.AddBackground(0, 0, 445, 395, 9200);
this.AddBackground(10, 10, 425, 375, 3500);
this.AddLabel(40, 140, 1359, @"These gardening tools can ONLY be used by you.");
this.AddLabel(140, 200, 195, @"* WARNING - TAKE CARE *");
this.AddLabel(40, 240, 1359, @"When you destroy the garden, the ENTIRE contents of the");
this.AddLabel(40, 260, 1359, @"Gardening Secure are deleted, and are none retrievable,");
this.AddLabel(40, 280, 1359, @"even by a GameMaster or Adminstrator, you've been warned!");
this.AddButton(310, 330, 9723, 9724, (int)Buttons.Button1, GumpButtonType.Reply, 0);
this.AddLabel(135, 50, 195, @"* You have placed your garden. *");
this.AddLabel(40, 80, 1359, @"In your garden you will find a 'Clean Up' sign, this is used");
this.AddLabel(40, 100, 1359, @"to destroy your garden when you have finished using it.");
this.AddLabel(40, 120, 1359, @"You will also find inside the garden, a Garden Secure.");
this.AddLabel(140, 335, 32, @"I have read the WARNING");
}

public enum Buttons
{
Button1,
}
}
}
