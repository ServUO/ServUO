using System; 
using Server; 
using Server.Gumps; 
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Gumps
{ 
   public class CharonGump : Gump 
   { 
      public static void Initialize() 
      {
          CommandSystem.Register("CharonGump", AccessLevel.GameMaster, new CommandEventHandler(CharonGump_OnCommand)); 
      } 

      private static void CharonGump_OnCommand( CommandEventArgs e ) 
      { 
         e.Mobile.SendGump( new CharonGump( e.Mobile ) ); 
      } 

      public CharonGump( Mobile owner ) : base( 50,50 ) 
      { 
//----------------------------------------------------------------------------------------------------

				AddPage( 0 );
			AddImageTiled(  54, 33, 369, 400, 2624 );
			AddAlphaRegion( 54, 33, 369, 400 );

			AddImageTiled( 416, 39, 44, 389, 203 );
//--------------------------------------Window size bar--------------------------------------------
			
			AddImage( 97, 49, 9005 );
			AddImageTiled( 58, 39, 29, 390, 10460 );
			AddImageTiled( 412, 37, 31, 389, 10460 );
			AddLabel( 140, 60, 0x34, "Titan Quest" );
			

			AddHtml( 107, 140, 300, 230, "<BODY>" +
//----------------------/----------------------------------------------/
"<BASEFONT COLOR=Blue><I>* Charon stares into your eyes. *</I><br><br>" +
"<BASEFONT Color=Blue> Youve come to slay Hades !!<br><br>" + 
"<BASEFONT COLOR=Blue><I>* If you wish to do battle with him you will have to do something for me. *</I><br><br>" +
"<BASEFONT COLOR=Blue> I want you to find Crockett Scarr <br><br>" +
"<BASEFONT COLOR=Blue> He made a pact with some demons and must be stopped<br><br>" +
"<BASEFONT COLOR=Blue> You will find Crockett outside one of the Ilshenar Dungeons.<br><br>" +
"<BASEFONT COLOR=Blue><I>* You realize that defeating this Crockett person is the only way you can face Hades *</I><br>" + 
"</BODY>", false, true);
			
			AddImage( 430, 9, 10441);
			AddImageTiled( 40, 38, 17, 391, 9263 );
			AddImage( 6, 25, 10421 );
			AddImage( 34, 12, 10420 );
			AddImageTiled( 94, 25, 342, 15, 10304 );
			AddImageTiled( 40, 427, 415, 16, 10304 );
			AddImage( -10, 314, 10402 );
			AddImage( 56, 150, 10411 );
			AddImage( 155, 120, 2103 );
			AddImage( 136, 84, 96 );

			AddButton( 225, 390, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0 ); 

//--------------------------------------------------------------------------------------------------------------
      } 

      public override void OnResponse( NetState state, RelayInfo info ) //Function for GumpButtonType.Reply Buttons 
      { 
         Mobile from = state.Mobile; 

         switch ( info.ButtonID ) 
         { 
            case 0: //Case uses the ActionIDs defenied above. Case 0 defenies the actions for the button with the action id 0 
            { 
               //Cancel 
               from.SendMessage( "Please return my helmet!!!" );
               break; 
            } 

         }
      }
   }
}