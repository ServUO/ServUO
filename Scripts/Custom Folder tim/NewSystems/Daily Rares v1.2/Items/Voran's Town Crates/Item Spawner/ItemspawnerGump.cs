
using System; 
using System.Collections; 
using Server.Network; 
using Server.Gumps; 
using System.Collections.Generic;

namespace Server.Items 
{ 
   public class ItemSpawnerGump : Gump 
   { 
      private ItemSpawner m_Spawner; 

      public ItemSpawnerGump( ItemSpawner spawner ) : base( 50, 50 ) 
      { 
         m_Spawner = spawner; 

         AddPage( 0 ); 

         AddBackground( 0, 0, 260, 371, 5054 ); 

         AddLabel( 95, 1, 0, "Items List" ); 

         AddButton( 5, 347, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0 ); 
         AddLabel( 38, 347, 0x384, "Cancel" ); 

         AddButton( 5, 325, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0 ); 
         AddLabel( 38, 325, 0x384, "Okay" ); 

         AddButton( 110, 347, 0xFA8, 0xFAA, 2, GumpButtonType.Reply, 0 ); 
         AddLabel( 143, 347, 0x384, "Total Respawn" ); 

         for ( int i = 0;  i < 13; i++ ) 
         { 
            AddButton( 5, ( 22 * i ) + 20, 0xFA5, 0xFA7, 3 + (i * 2), GumpButtonType.Reply, 0 ); 
            AddButton( 38, ( 22 * i ) + 20, 0xFA2, 0xFA4, 4 + (i * 2), GumpButtonType.Reply, 0 ); 

            AddImageTiled( 71, ( 22 * i ) + 20, 159, 23, 0x52 ); 
            AddImageTiled( 72, ( 22 * i ) + 21, 157, 21, 0xBBC ); 

            string str = ""; 

            if ( i < spawner.ItemsName.Count ) 
            { 
               str = (string)spawner.ItemsName[i]; 
                
               int count = m_Spawner.CountItems( str ); 

               AddLabel( 232, ( 22 * i ) + 20, 0, count.ToString() ); 
            } 

            AddTextEntry( 75, ( 22 * i ) + 21, 154, 21, 0, i, str ); 
         } 
      } 

      public List<string> CreateArray( RelayInfo info, Mobile from ) 
      { 
         List<string> itemsName = new List<string>(); 

         for ( int i = 0;  i < 13; i++ ) 
         { 
            TextRelay te = info.GetTextEntry( i ); 

            if ( te != null ) 
            { 
               string str = te.Text; 

               if ( str.Length > 0 ) 
               { 
                  str = str.Trim(); 

                  Type type = ItemSpawnerType.GetType( str ); 
                   
                   
                  if ( type != null ) 
                     itemsName.Add( str ); 
                  else 
                     from.SendMessage( "{0} is not a valid type name.", str ); 
               } 
            } 
         } 

         return itemsName; 
      } 
       
      public override void OnResponse( NetState state, RelayInfo info ) 
      { 
         if ( m_Spawner.Deleted ) 
            return; 

         switch ( info.ButtonID ) 
         { 
            case 0: // Closed 
            { 
               break; 
            } 
            case 1: // Okay 
            { 
               m_Spawner.ItemsName = CreateArray( info, state.Mobile ); 

               break; 
            } 
            case 2: // Complete respawn 
            { 
               m_Spawner.Respawn(); 

               break; 
            } 
            default: 
            { 
               int buttonID = info.ButtonID - 3; 
               int index = buttonID / 2; 
               int type = buttonID % 2; 

               TextRelay entry = info.GetTextEntry( index ); 

               if ( entry != null && entry.Text.Length > 0 ) 
               { 
                  if ( type == 0 ) // Spawn item 
                     m_Spawner.Spawn( entry.Text ); 
                  else // Remove items 
                     m_Spawner.RemoveItems( entry.Text ); 

                  m_Spawner.ItemsName = CreateArray( info, state.Mobile ); 
               } 

               break; 
            } 
         } 
      } 
   } 
} 
