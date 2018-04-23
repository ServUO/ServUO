using System; 
using Server; 
using Server.Gumps; 
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Gumps
{ 
   public class MonsterContractDealerGump : Gump 
   { 
      public static void Initialize() 
      { 
         CommandSystem.Register( "MonsterContractDealerGump", AccessLevel.GameMaster, new CommandEventHandler( MonsterContractDealerGump_OnCommand ) ); 
      } 

      private static void MonsterContractDealerGump_OnCommand( CommandEventArgs e ) 
      { 
         e.Mobile.SendGump( new MonsterContractDealerGump( e.Mobile ) ); 
      } 

      public MonsterContractDealerGump( Mobile owner ) : base( 50,50 ) 
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
			AddLabel( 140, 60, 0x34, "Monster Contracts" );
			

			AddHtml( 107, 140, 300, 230, "<BODY>" +
//----------------------/----------------------------------------------/
"<BASEFONT COLOR=YELLOW>Hello there. Can I speak with you one second? I have a favor to ask and I wonder if you could help me.<BR><BR>I have in my possesion contracts to rid the land of evil and drive the monsters out. But alas I am too old to do this myself. But I will gladly give you the contracts and only keep a small commission for myself if you could fill the contracts for me.<BR>" +
"<BASEFONT COLOR=YELLOW>Will you help me fill my contracts?<BR><BR>Oh, Thank you my friend. The contract will tell you which monsters to purge from the lands." + "</BODY>", false, true);

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
               from.SendMessage( "The contract was placed in your bag. Thank you my friend for all your kindness!" );
               break; 
            } 

         }
      }
   }
}
