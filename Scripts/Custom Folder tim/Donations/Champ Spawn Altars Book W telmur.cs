//Map Locations Book
//by henry_r--- Changed by REDSNOW
//03/14/12

using System; 
using System.Collections;
using System.Text; 
using Server; 
using Server.Gumps; 
using Server.Network; 
using Server.Mobiles;
using Server.Commands;


namespace Server.Items 
{ 
   public class ChampSpawnAltarLocations : Item 
   { 
      [Constructable] 
      public ChampSpawnAltarLocations() : base( 0x2D50 )
      { 
         Movable = true; 
         Hue = 1152; //Icy Blue          
         Weight = 0.0;  
         Name = " Champ Spawn Altar Locations"; 
         LootType = LootType.Blessed;
 
      }

      public ChampSpawnAltarLocations(Serial serial)
          : base(serial) 
      { 
      } 

      public override void OnDoubleClick( Mobile from ) 
      { 
         if ( !from.Player ) 
            return; 

         if ( from.InRange( GetWorldLocation(), 1 ) ) 
            UseGate( from ); 
         else 
            from.SendLocalizedMessage( 500446 ); // That is too far away. 
      } 

      public override bool OnMoveOver( Mobile m ) 
      { 
         return !m.Player || UseGate( m ); 
      } 

      public bool UseGate( Mobile m ) 
      { 
         if ( m.Criminal ) 
         { 
            m.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily. 
            return false; 
         } 
         else if ( Server.Spells.SpellHelper.CheckCombat( m ) ) 
         { 
            m.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle?? 
            return false; 
         } 

                  else if ( Server.Misc.WeightOverloading.IsOverloaded(  m ) )
			{
				m.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
				return false;
			}
                               
                       
         else if ( m.Spell != null ) 
         { 
            m.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment. 
            return false; 
         } 
         else 
         {
             m.CloseGump(typeof(ChampSpawnAltarLocationsGump));
             m.SendGump(new ChampSpawnAltarLocationsGump(m)); 

            //Effects.PlaySound( m.Location, m.Map, 0x20E ); 
            return true; 
         } 
      } 

      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 

         writer.Write( (int) 0 ); // version 
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 

         int version = reader.ReadInt(); 
      } 

   public class TMEntry 
   { 
      private Point3D m_Location; 
      private string m_Text; 

      public Point3D Location 
      { 
         get 
         { 
            return m_Location; 
         } 
      } 

      public string Text 
      { 
         get 
         { 
            return m_Text; 
         } 
      } 

      public TMEntry( Point3D loc, string text ) 
      { 
         m_Location = loc; 
         m_Text = text; 
      } 
   } 

   public class TMList 
   { 
      private string m_Text, m_SelText; 
      private Map m_Map; 
      private TMEntry[] m_Entries; 

      public string Text 
      { 
         get 
         { 
            return m_Text; 
         } 
      } 

      public string SelText 
      { 
         get 
         { 
            return m_SelText; 
         } 
      } 

      public Map Map 
      { 
         get 
         { 
            return m_Map; 
         } 
      } 

      public TMEntry[] Entries 
      { 
         get 
         { 
            return m_Entries; 
         } 
      } 

      public TMList( string text, string selText, Map map, TMEntry[] entries ) 
      { 
         m_Text = text; 
         m_SelText = selText; 
         m_Map = map; 
         m_Entries = entries; 
      } 

      public static readonly TMList MapLocations1 = 
         new TMList( "Felucca Champ Altars", "Felucca Champ Altars", Map.Felucca, new TMEntry[] //Fel including T2
            { 
               new TMEntry( new Point3D( 5179, 708, 5 ), "Deceit" ),  
               new TMEntry( new Point3D( 5557, 824, 50 ), "Despise" ),   
               new TMEntry( new Point3D( 5259, 837, 46 ), "Destard" ), 
               new TMEntry( new Point3D( 5815, 1351, -13 ), "Fire" ),
               new TMEntry( new Point3D( 5191, 1606, 5 ), "Terathan Keep" ), 
               new TMEntry( new Point3D( 5511, 2361, 25 ), "Ice Land T2" ), 
               new TMEntry( new Point3D( 6038, 2400, 31 ), "East Ice Land T2" ), 
               new TMEntry( new Point3D( 5550, 2641, 0 ), "South Oasis T2" ),
               new TMEntry( new Point3D( 5637, 2917, 22 ), "West of Fire T2" ),
               new TMEntry( new Point3D( 6035, 2944, 37 ), "Terra Sanctum T2" ), 
               new TMEntry( new Point3D( 5267, 3172, 89 ), "Marble Passage T2" ), 
               new TMEntry( new Point3D( 5281, 3368, 36 ), "Darwin Thicket T2" ), 
               new TMEntry( new Point3D( 5954, 3475, 10 ), "Harpers Bog T2" ), 
               new TMEntry( new Point3D( 5207, 3637, 5 ), "City of the Dead T2" ),
               new TMEntry( new Point3D( 5559, 3757, 6 ), "Vesper Passage T2" ),
               new TMEntry( new Point3D( 5982, 3882, 5 ), "South Khaldun T2" ),
               new TMEntry( new Point3D( 5724, 3991, 27 ), "Tortoise Lagoon T2" )  
            } ); 
       
     public static readonly TMList MapLocations2 =
         new TMList("Ilshenar", "Ilshenar", Map.Ilshenar, new TMEntry[] //IlSH
            { 
               new TMEntry( new Point3D( 382, 328, -45 ), "Valor" ),
               new TMEntry( new Point3D( 462, 926, -82 ), "Humility" ), 
               new TMEntry( new Point3D( 1645, 1108, -7 ), "Spirituality" ), 
               new TMEntry( new Point3D( 2212, 1260, 10 ), "Twisted Weald" )
                             
            } );

       public static readonly TMList MapLocations3 = 
             new TMList( "Malas Champ", "Malas Champ", Map.Malas, new TMEntry[] //Malas
            { 
               new TMEntry( new Point3D( 175, 1629, -7 ), "Bedlam Crypt" ),
			   new TMEntry( new Point3D( 2367, 1128, -85 ), "Undead Stadium" )

			} );

         public static readonly TMList MapLocations4 = 
         new TMList( "Tokuno Champ", "Tokuno Champ", Map.Tokuno, new TMEntry[] //Tokuno
            { 
               new TMEntry( new Point3D( 949, 435, 14 ), "Sleeping Dragon" )
       
                
            });

         public static readonly TMList MapLocations5 =
         new TMList("Tel Mur Champs", "Tel Mur Champs", Map.Felucca, new TMEntry[] //Tel Mur
            { 
               new TMEntry( new Point3D( 7000, 1004, 5 ), "Primeval Lich" ),
               new TMEntry( new Point3D( 6993, 734, 77 ), "Abyssal Champ" )
        //       new TMEntry( new Point3D( 2099, 1976, 0 ), "XXXX" ),
        //       new TMEntry( new Point3D( 2090, 1987, 0 ), "XXXX" ),
        //       new TMEntry( new Point3D( 2094, 2006, 0 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1426, 2405, 5 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1426, 2405, 5 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1434, 2381, 5 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1470, 2340, 5 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1451, 2301, 0 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1437, 2294, 0 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1439, 2217, 0 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1467, 2181, 0 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1464, 2246, 5 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1478, 2273, 5 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1562, 2312, 5 ), "XXXX" ),
        //       new TMEntry( new Point3D( 1546, 2223, 10 ), "XXXX" )
                
        //    } );

                 
            });

         public static readonly TMList[] UORLists = new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5 };
         public static readonly TMList[] UORlistsYoung = new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5 };
         public static readonly TMList[] LBRLists = new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5 };
         public static readonly TMList[] LBRListsYoung = new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5 };
         public static readonly TMList[] AOSLists = new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5 };
         public static readonly TMList[] AOSListsYoung = new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5 };
         public static readonly TMList[] SELists = new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5 };
         public static readonly TMList[] SEListsYoung = new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5 }; 
		 public static readonly TMList[] RedLists		= new TMList[] {  };
		 public static readonly TMList[] SigilLists	    = new TMList[] {  };
    } 

   public class ChampSpawnAltarLocationsGump : Gump 
   { 
      public static void Initialize() 
      {
          CommandSystem.Register("CSALGump", AccessLevel.GameMaster, new CommandEventHandler(ChampSpawnAltarLocationsGump_OnCommand)); 
      }

      private static void ChampSpawnAltarLocationsGump_OnCommand(CommandEventArgs e) 
      {
          e.Mobile.SendGump(new ChampSpawnAltarLocationsGump(e.Mobile)); 
      }

      private Mobile m_Mobile; 
      private TMList[] m_Lists;

      public ChampSpawnAltarLocationsGump(Mobile mobile)
          : base(100, 100) 
      { 
         m_Mobile = mobile; 

         TMList[] checkLists; 

         if ( mobile.Player ) 
         { 
            if ( mobile.Kills >= 5 ) 
            { 
               checkLists = TMList.RedLists; 
            } 
            else 
            { 
               int flags = mobile.NetState == null ? 0 : (int)mobile.NetState.Flags; 

               if ( Core.AOS && (flags & 0x8) != 0 ) 
                  checkLists = TMList.AOSLists; 
               else if ( (flags & 0x4) != 0 ) 
                  checkLists = TMList.LBRLists; 
               else 
                  checkLists = TMList.UORLists; 
            } 
         } 
         else 
         { 
            checkLists = TMList.AOSLists; 
         } 

         m_Lists = new TMList[checkLists.Length]; 

         for ( int i = 0; i < m_Lists.Length; ++i ) 
            m_Lists[i] = checkLists[i]; 

         for ( int i = 0; i < m_Lists.Length; ++i ) 
         { 
            if ( m_Lists[i].Map == mobile.Map ) 
            { 
               TMList temp = m_Lists[i]; 

               m_Lists[i] = m_Lists[0]; 
               m_Lists[0] = temp; 

               break; 
            } 
         } 

         AddPage( 0 ); 

         AddBackground( 0, 0, 380, 500, 9200 ); 

         AddButton( 10, 345, 4005, 4007, 1, GumpButtonType.Reply, 0 ); 
         AddHtmlLocalized( 45, 345, 140, 25, 1011036, false, false ); // OKAY 

         AddButton( 10, 370, 4005, 4007, 0, GumpButtonType.Reply, 0 ); 
         AddHtmlLocalized( 45, 370, 140, 25, 1011012, false, false ); // CANCEL 

        // AddButton( 10, 475, 4005, 4007, 2, GumpButtonType.Reply, 0 );
        //AddLabel( 45, 475, 0x34, "Visit UO.STRATICS.COM Treasure Map Archive" );

         AddHtmlLocalized( 5, 5, 200, 20, 1012011, false, false ); // Pick your destination: 

         for ( int i = 0; i < checkLists.Length; ++i ) 
         { 
            AddButton( 10, 35 + (i * 25), 2117, 2118, 0, GumpButtonType.Page, Array.IndexOf( m_Lists, checkLists[i] ) + 1 ); 
            AddHtml( 30, 35 + (i * 25), 150, 20, checkLists[i].Text, false, false ); 
         } 

         for ( int i = 0; i < m_Lists.Length; ++i ) 
            RenderPage( i, Array.IndexOf( checkLists, m_Lists[i] ) ); 
      } 

      private void RenderPage( int index, int offset ) 
      { 
         TMList list = m_Lists[index]; 

         AddPage( index + 1 ); 

         AddButton( 10, 35 + (offset * 25), 2117, 2118, 0, GumpButtonType.Page, index + 1 ); 
         AddHtml( 30, 35 + (offset * 25), 150, 20, list.SelText, false, false ); 

         TMEntry[] entries = list.Entries; 

         for ( int i = 0; i < entries.Length; ++i ) 
         { 
            AddRadio( 200, 35 + (i * 25), 210, 211, false, (index * 100) + i ); 
            AddHtml( 225, 35 + (i * 25), 150, 20, entries[i].Text, false, false ); 
         } 
      } 

      public override void OnResponse( NetState state, RelayInfo info ) 
      { 
         if ( info.ButtonID == 0 ) // Cancel 
            return; 
         else if ( m_Mobile.Deleted || m_Mobile.Map == null ) 
            return; 
         
         int[] switches = info.Switches; 

         if ( switches.Length == 0 ) 
            return; 

         int switchID = switches[0]; 
         int listIndex = switchID / 100; 
         int listEntry = switchID % 100; 

         if ( listIndex < 0 || listIndex >= m_Lists.Length ) 
            return; 

         TMList list = m_Lists[listIndex]; 

         if ( listEntry < 0 || listEntry >= list.Entries.Length ) 
            return; 

         TMEntry entry = list.Entries[listEntry]; 

         if ( m_Mobile.Player && m_Mobile.Kills >= 5 && list.Map != Map.Trammel ) 
         { 
            m_Mobile.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there. 
         } 
         else if ( m_Mobile.Criminal ) 
         { 
            m_Mobile.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily. 
         } 
         else if ( Server.Spells.SpellHelper.CheckCombat( m_Mobile ) ) 
         { 
            m_Mobile.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle?? 
         } 
         else if ( m_Mobile.Spell != null ) 
         { 
            m_Mobile.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment. 
         } 
         else if ( m_Mobile.Map == list.Map && m_Mobile.InRange( entry.Location, 1 ) ) 
         { 
            m_Mobile.SendLocalizedMessage( 1019003 ); // You are already there. 
         } 
         else 
         { 
            BaseCreature.TeleportPets( m_Mobile, entry.Location, list.Map ); 

            m_Mobile.Combatant = null; 
            m_Mobile.Warmode = false; 
            m_Mobile.Map = list.Map; 
            m_Mobile.Location = entry.Location; 
         } 
      } 
   } 
 } }