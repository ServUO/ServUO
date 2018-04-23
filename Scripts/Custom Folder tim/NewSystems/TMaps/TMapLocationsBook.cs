//Trammel Treasure Map Locations Book
//by henry_r
//02/16/08
//based on uo.stratics.com treasure map archive
//special thanks to Joeku for help with the weblink
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
   public class TMapLocationsBook : Item 
   { 
      [Constructable] 
      public TMapLocationsBook() : base( 0x2D50 )
      { 
         Movable = true; 
         Hue = 51;          
         Weight = 0.0;  
         Name = " Trammel Treasure Map Locations"; 
         LootType = LootType.Blessed;
 
      } 

      public TMapLocationsBook( Serial serial ) : base( serial ) 
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



                  else if ( m.Region is Server.Regions.Jail )
         		{
                  	m.SendLocalizedMessage( 1041530, "", 0x35 ); // You'll need a better jailbreak plan then that!
                        return false;
         		}


                       else if ( Server.Factions.Sigil.ExistsOn( m ) )
				{
					m.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil.
                                        return false;
				}


         else if ( m.Spell != null ) 
         { 
            m.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment. 
            return false; 
         } 
         else 
         { 
            m.CloseGump( typeof( MapLocationsGump ) ); 
            m.SendGump( new MapLocationsGump( m ) ); 

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
         new TMList("Maps 1-17", "Maps 1-17", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 961, 506, 0 ), "1" ),
               new TMEntry( new Point3D( 1162, 189, 6 ), "2" ),
               new TMEntry( new Point3D( 1315, 317, 22 ), "3" ),
               new TMEntry( new Point3D( 1469, 230, 16 ), "4" ),
               new TMEntry( new Point3D( 1504, 364, 0 ), "5" ),
               new TMEntry( new Point3D( 2672, 392, 15 ), "6" ),
               new TMEntry( new Point3D( 2741, 435, 15 ), "7" ),
               new TMEntry( new Point3D( 2770, 345, 15 ), "8" ),
               new TMEntry( new Point3D( 2781, 289, 15 ), "9" ),
               new TMEntry( new Point3D( 2836, 233, 0 ), "10" ),
               new TMEntry( new Point3D( 3014, 250, 0 ), "11" ),
               new TMEntry( new Point3D( 3082, 202, 4 ), "12" ),
               new TMEntry( new Point3D( 1028, 1181, 0 ), "13" ),  
               new TMEntry( new Point3D( 1318, 889, 0 ), "14" ),
               new TMEntry( new Point3D( 1414, 771, 0 ), "15" ),
               new TMEntry( new Point3D( 1530, 753, 16 ), "16" ),
               new TMEntry( new Point3D( 1555, 806, 0 ), "17" )
            } ); 
       
     public static readonly TMList MapLocations2 =
         new TMList("Maps 18-34", "Maps 18-34", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 1510, 968, 0 ), "18" ),
               new TMEntry( new Point3D( 1561, 1058, 0 ), "19" ),
               new TMEntry( new Point3D( 1510, 1071, 0 ), "20" ),
               new TMEntry( new Point3D( 2339, 645, 0 ), "21" ),
               new TMEntry( new Point3D( 2350, 689, 0 ), "22" ),
               new TMEntry( new Point3D( 2396, 723, 0 ), "23" ),
               new TMEntry( new Point3D( 2433, 767, 0 ), "24" ),
               new TMEntry( new Point3D( 2643, 853, 0 ), "25" ),
               new TMEntry( new Point3D( 2458, 1042, 0 ), "26" ),
               new TMEntry( new Point3D( 2517, 1066, 0 ), "27" ),
               new TMEntry( new Point3D( 2338, 1159, 5 ), "28" ),
               new TMEntry( new Point3D( 2391, 1155, 0 ), "29" ),
               new TMEntry( new Point3D( 3246, 246, 4 ), "30" ),
               new TMEntry( new Point3D( 3403, 238, 0 ), "31" ),
               new TMEntry( new Point3D( 3376, 458, 9 ), "32" ),
               new TMEntry( new Point3D( 3369, 638, 5 ), "33" ),
               new TMEntry( new Point3D( 199, 1460, 0 ), "34" )
                
            } );

        public static readonly TMList MapLocations3 =
         new TMList("Maps 35-51", "Maps 35-51", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 207, 1444, 0 ), "35" ),
               new TMEntry( new Point3D( 349, 1565, 0 ), "36" ),
               new TMEntry( new Point3D( 582, 1451, 0 ), "37" ),
               new TMEntry( new Point3D( 360, 1337, 6 ), "38" ),
               new TMEntry( new Point3D( 620, 1706, 0 ), "39" ),
               new TMEntry( new Point3D( 963, 1859, 0 ), "40" ),
               new TMEntry( new Point3D( 979, 1850, 22 ), "41" ), 
               new TMEntry( new Point3D( 970, 1894, 0 ), "42" ),
               new TMEntry( new Point3D( 970, 1884, 0 ), "43" ),
               new TMEntry( new Point3D( 978, 1880, 0 ), "44" ),
               new TMEntry( new Point3D( 1017, 1859, 0 ), "45" ),
               new TMEntry( new Point3D( 1034, 1877, 0 ), "46" ),
               new TMEntry( new Point3D( 1042, 1904, 5 ), "47" ),
               new TMEntry( new Point3D( 1042, 1960, 0 ), "48" ),
               new TMEntry( new Point3D( 1038, 1976, 0 ), "49" ),
               new TMEntry( new Point3D( 1024, 1991, 0 ), "50" ),
               new TMEntry( new Point3D( 974, 1992, 0 ), "51" )
                
            } );

         public static readonly TMList MapLocations4 =
         new TMList("Maps 52-68", "Maps 52-68", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 989, 1993, 0 ), "52" ),
               new TMEntry( new Point3D( 450, 2054, 0 ), "53" ),
               new TMEntry( new Point3D( 478, 2043, 8 ), "54" ), 
               new TMEntry( new Point3D( 492, 2027, 5 ), "55" ),
               new TMEntry( new Point3D( 468, 2087, 8 ), "56" ),
               new TMEntry( new Point3D( 466, 2100, 5 ), "57" ),
               new TMEntry( new Point3D( 1651, 2030, 0 ), "58" ),
               new TMEntry( new Point3D( 1689, 1992, 0 ), "59" ),
               new TMEntry( new Point3D( 1709, 1964, 5 ), "60" ),
               new TMEntry( new Point3D( 1725, 1999, 0 ), "61" ),
               new TMEntry( new Point3D( 1732, 2017, 0 ), "62" ),
               new TMEntry( new Point3D( 1742, 2028, 0 ), "63" ),
               new TMEntry( new Point3D( 1753, 2020, 0 ), "64" ),
               new TMEntry( new Point3D( 2034, 1942, 0 ), "65" ),
               new TMEntry( new Point3D( 2054, 1963, 0 ), "66" ), 
               new TMEntry( new Point3D( 2065, 1979, 0 ), "67" ),
               new TMEntry( new Point3D( 2058, 1990, 6 ), "68" )
                
            } );

         public static readonly TMList MapLocations5 =
         new TMList("Maps 69-85", "Maps 69-85", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 2070, 1990, 0 ), "69" ),
               new TMEntry( new Point3D( 2062, 1962, 0 ), "70" ),
               new TMEntry( new Point3D( 2098, 1976, 0 ), "71" ),
               new TMEntry( new Point3D( 2089, 1987, 0 ), "72" ),
               new TMEntry( new Point3D( 2093, 2006, 0 ), "73" ),
               new TMEntry( new Point3D( 2188, 1991, 0 ), "74" ),
               new TMEntry( new Point3D( 1426, 2405, 5 ), "75" ),
               new TMEntry( new Point3D( 1434, 2381, 5 ), "76" ),
               new TMEntry( new Point3D( 1470, 2340, 5 ), "77" ),
               new TMEntry( new Point3D( 1451, 2301, 0 ), "78" ),
               new TMEntry( new Point3D( 1436, 2294, 0 ), "79" ),
               new TMEntry( new Point3D( 1438, 2217, 0 ), "80" ),
               new TMEntry( new Point3D( 1467, 2181, 0 ), "81" ),
               new TMEntry( new Point3D( 1464, 2246, 5 ), "82" ),
               new TMEntry( new Point3D( 1478, 2273, 5 ), "83" ),
               new TMEntry( new Point3D( 1562, 2312, 5 ), "84" ),
               new TMEntry( new Point3D( 1546, 2223, 10 ), "85" )
                
            } );

         public static readonly TMList MapLocations6 =
         new TMList("Maps 86-102", "Maps 86-102", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 1518, 2214, 5 ), "86" ),
               new TMEntry( new Point3D( 1533, 2189, 5 ), "87" ),
               new TMEntry( new Point3D( 1522, 2150, 0 ), "88" ),
               new TMEntry( new Point3D( 1541, 2115, 5 ), "89" ),
               new TMEntry( new Point3D( 1594, 2193, 0 ), "90" ),
               new TMEntry( new Point3D( 1618, 2236, 0 ), "91" ),
               new TMEntry( new Point3D( 1654, 2268, 5 ), "92" ),
               new TMEntry( new Point3D( 1724, 2288, 5 ), "93" ),
               new TMEntry( new Point3D( 1773, 2321, 5 ), "94" ),
               new TMEntry( new Point3D( 1758, 2333, 0 ), "95" ),
               new TMEntry( new Point3D( 1765, 2431, 5 ), "96" ),
               new TMEntry( new Point3D( 1702, 2318, 5 ), "97" ),
               new TMEntry( new Point3D( 1654, 2304, 0 ), "98" ),
               new TMEntry( new Point3D( 2061, 2144, 0 ), "99" ),
               new TMEntry( new Point3D( 2104, 2124, 0 ), "100" ),
               new TMEntry( new Point3D( 2098, 2101, 0 ), "101" ),
               new TMEntry( new Point3D( 2129, 2108, 0 ), "102" )
                
            } );

         public static readonly TMList MapLocations7 =
         new TMList("Maps 103-119", "Maps 103-119", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 2153, 2121, 0 ), "103" ),
               new TMEntry( new Point3D( 2186, 2143, 0 ), "104" ),
               new TMEntry( new Point3D( 2177, 2151, 0 ), "105" ),
               new TMEntry( new Point3D( 2161, 2149, 0 ), "106" ),
               new TMEntry( new Point3D( 2130, 2133, 0 ), "107" ),
               new TMEntry( new Point3D( 2123, 2121, 0 ), "108" ),
               new TMEntry( new Point3D( 2647, 2167, 5 ), "109" ),
               new TMEntry( new Point3D( 2628, 2221, 6 ), "110" ),
               new TMEntry( new Point3D( 2642, 2289, 7 ), "111" ),
               new TMEntry( new Point3D( 2682, 2291, 5 ), "112" ),
               new TMEntry( new Point3D( 2727, 2309, 0 ), "113" ),
               new TMEntry( new Point3D( 2781, 2294, 6 ), "114" ),
               new TMEntry( new Point3D( 2804, 2255, 0 ), "115" ),
               new TMEntry( new Point3D( 2850, 2252, 5 ), "116" ),
               new TMEntry( new Point3D( 2957, 2150, 53 ), "117" ),
               new TMEntry( new Point3D( 2967, 2171, 36 ), "118" ),
               new TMEntry( new Point3D( 2952, 2177, 52 ), "119" )
                
            } );

         public static readonly TMList MapLocations8 =
         new TMList("Maps 120-136", "Maps 120-136", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 2955, 2200, 46 ), "120" ),
               new TMEntry( new Point3D( 2932, 2240, 5 ), "121" ),
               new TMEntry( new Point3D( 95, 2505, 0 ), "122" ),
               new TMEntry( new Point3D( 1025, 2702, 0 ), "123" ),
               new TMEntry( new Point3D( 1290, 2735, 0 ), "124" ),
               new TMEntry( new Point3D( 1382, 2840, 0 ), "125" ),
               new TMEntry( new Point3D( 1390, 2985, 0 ), "126" ),
               new TMEntry( new Point3D( 1414, 3059, 0 ), "127" ),
               new TMEntry( new Point3D( 1647, 2462, 5 ), "128" ),
               new TMEntry( new Point3D( 1563, 2705, 0 ), "129" ),
               new TMEntry( new Point3D( 1671, 2808, 0 ), "130" ),
               new TMEntry( new Point3D( 1601, 3103, 0 ), "131" ),
               new TMEntry( new Point3D( 1665, 3063, 0 ), "132" ),
               new TMEntry( new Point3D( 1068, 3182, 0 ), "133" ),
               new TMEntry( new Point3D( 1075, 3156, 0 ), "134" ),
               new TMEntry( new Point3D( 1073, 3133, 0 ), "135" ),
               new TMEntry( new Point3D( 1090, 3110, 0 ), "136" )
                
            } );

         public static readonly TMList MapLocations9 =
         new TMList("Maps 137-153", "Maps 137-153", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 1093, 3132, 0 ), "137" ),
               new TMEntry( new Point3D( 1096, 3179, 0 ), "138" ),
               new TMEntry( new Point3D( 1129, 3403, 0 ), "139" ),
               new TMEntry( new Point3D( 1162, 3468, 0 ), "140" ),
               new TMEntry( new Point3D( 1127, 3499, 0 ), "141" ),
               new TMEntry( new Point3D( 1136, 3446, 0 ), "142" ),
               new TMEntry( new Point3D( 2014, 3269, 0 ), "143" ),
               new TMEntry( new Point3D( 2040, 3427, 0 ), "144" ),
               new TMEntry( new Point3D( 2096, 3384, 0 ), "145" ),
               new TMEntry( new Point3D( 2149, 3362, 10 ), "146" ),
               new TMEntry( new Point3D( 2370, 3428, 3 ), "147" ),
               new TMEntry( new Point3D( 2342, 3482, 3 ), "148" ),
               new TMEntry( new Point3D( 2360, 3507, 3 ), "149" ),
               new TMEntry( new Point3D( 2387, 3506, 3 ), "150" ),
               new TMEntry( new Point3D( 2467, 3580, 3 ), "151" ),
               new TMEntry( new Point3D( 2481, 3623, 3 ), "152" ),
               new TMEntry( new Point3D( 2527, 3585, 0 ), "153" )
                
            } );

         public static readonly TMList MapLocations10 =
         new TMList("Maps 154-170", "Maps 154-170", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 2534, 3609, 0 ), "154" ),
               new TMEntry( new Point3D( 2797, 3452, 0 ), "155" ), 
               new TMEntry( new Point3D( 2803, 3489, 0 ), "156" ),
               new TMEntry( new Point3D( 2792, 3520, 0 ), "157" ),
               new TMEntry( new Point3D( 2831, 3510, 0 ), "158" ),
               new TMEntry( new Point3D( 2989, 3606, 15 ), "159" ),
               new TMEntry( new Point3D( 3055, 3602, 0 ), "160" ),
               new TMEntry( new Point3D( 2154, 3983, 3 ), "161" ),
               new TMEntry( new Point3D( 2144, 3985, 0 ), "162" ),
               new TMEntry( new Point3D( 2140, 3941, 3 ), "163" ),
               new TMEntry( new Point3D( 2157, 3924, 3 ), "164" ),
               new TMEntry( new Point3D( 2152, 3951, 3 ), "165" ),
               new TMEntry( new Point3D( 2162, 3988, 3 ), "166" ),
               new TMEntry( new Point3D( 2452, 3942, 0 ), "167" ),
               new TMEntry( new Point3D( 2421, 3929, 3 ), "168" ),
               new TMEntry( new Point3D( 2414, 3920, 3 ), "169" ),
               new TMEntry( new Point3D( 2421, 3901, 3 ), "170" )
                
            } );

         public static readonly TMList MapLocations11 =
         new TMList("Maps 171-187", "Maps 171-187", Map.Trammel, new TMEntry[] 
            { 
               new TMEntry( new Point3D( 2481, 3908, 6 ), "171" ),
               new TMEntry( new Point3D( 2512, 3899, 3 ), "172" ),
               new TMEntry( new Point3D( 2515, 3919, 0 ), "173" ),
               new TMEntry( new Point3D( 2512, 3962, 6 ), "174" ),
               new TMEntry( new Point3D( 2527, 3982, 0 ), "175" ),
               new TMEntry( new Point3D( 2516, 3998, 3 ), "176" ),
               new TMEntry( new Point3D( 4476, 3282, 0 ), "177" ),
               new TMEntry( new Point3D( 4477, 3230, 0 ), "178" ),
               new TMEntry( new Point3D( 4465, 3210, 0 ), "179" ),
               new TMEntry( new Point3D( 4425, 3152, 0 ), "180" ),
               new TMEntry( new Point3D( 4420, 3117, 0 ), "181" ),
               new TMEntry( new Point3D( 4449, 3130, 0 ), "182" ),   
               new TMEntry( new Point3D( 4454, 3418, 0 ), "183" ),
               new TMEntry( new Point3D( 4501, 3108, 0 ), "184" ),
               new TMEntry( new Point3D( 4513, 3104, 0 ), "185" ),
               new TMEntry( new Point3D( 4470, 3188, 0 ), "186" ),
               new TMEntry( new Point3D( 4507, 3227, 0 ), "187" )
                
            } );

         public static readonly TMList MapLocations12 =
         new TMList("Maps 188-200", "Maps 188-200", Map.Trammel, new TMEntry[] 
            {
               new TMEntry( new Point3D( 4495, 3242, 0 ), "188" ),
               new TMEntry( new Point3D( 4462, 3369, 0 ), "189" ),
               new TMEntry( new Point3D( 4694, 3486, 0 ), "190" ),
               new TMEntry( new Point3D( 3477, 2761, 35 ), "191" ),
               new TMEntry( new Point3D( 3426, 2723, 45 ), "192" ),
               new TMEntry( new Point3D( 3418, 2675, 50 ), "193" ),
               new TMEntry( new Point3D( 3533, 2471, 10 ), "194" ),
               new TMEntry( new Point3D( 3511, 2421, 55 ), "195" ),
               new TMEntry( new Point3D( 3568, 2402, 11 ), "196" ),
               new TMEntry( new Point3D( 3702, 2825, 21 ), "197" ),
               new TMEntry( new Point3D( 3594, 2826, 44 ), "198" ),
               new TMEntry( new Point3D( 3557, 2820, 24 ), "199" ),
               new TMEntry( new Point3D( 3541, 2784, 6 ), "200" )           
            } );
             	
		public static readonly TMList[] UORLists		= new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5, MapLocations6, MapLocations7, MapLocations8, MapLocations9, MapLocations10, MapLocations11, MapLocations12  };
		public static readonly TMList[] UORlistsYoung	= new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5, MapLocations6, MapLocations7, MapLocations8, MapLocations9, MapLocations10, MapLocations11, MapLocations12  };
		public static readonly TMList[] LBRLists		= new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5, MapLocations6, MapLocations7, MapLocations8, MapLocations9, MapLocations10, MapLocations11, MapLocations12  };
		public static readonly TMList[] LBRListsYoung	= new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5, MapLocations6, MapLocations7, MapLocations8, MapLocations9, MapLocations10, MapLocations11, MapLocations12  };
		public static readonly TMList[] AOSLists		= new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5, MapLocations6, MapLocations7, MapLocations8, MapLocations9, MapLocations10, MapLocations11, MapLocations12  };
		public static readonly TMList[] AOSListsYoung	= new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5, MapLocations6, MapLocations7, MapLocations8, MapLocations9, MapLocations10, MapLocations11, MapLocations12  };
		public static readonly TMList[] SELists		= new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5, MapLocations6, MapLocations7, MapLocations8, MapLocations9, MapLocations10, MapLocations11, MapLocations12  };
		public static readonly TMList[] SEListsYoung	= new TMList[] { MapLocations1, MapLocations2, MapLocations3, MapLocations4, MapLocations5, MapLocations6, MapLocations7, MapLocations8, MapLocations9, MapLocations10, MapLocations11, MapLocations12  };
		public static readonly TMList[] RedLists		= new TMList[] {  };
		public static readonly TMList[] SigilLists	= new TMList[] {  };
    } 

   public class MapLocationsGump : Gump 
   { 
      public static void Initialize() 
      { 
         CommandSystem.Register( "TMGump", AccessLevel.GameMaster, new CommandEventHandler( MapLocationsGump_OnCommand ) ); 
      } 

      private static void MapLocationsGump_OnCommand( CommandEventArgs e ) 
      { 
         e.Mobile.SendGump( new MapLocationsGump( e.Mobile ) ); 
      }

      private Mobile m_Mobile; 
      private TMList[] m_Lists; 

      public MapLocationsGump( Mobile mobile ) : base( 100, 100 ) 
      { 
         m_Mobile = mobile; 

         TMList[] checkLists; 

         if ( mobile.Player ) 
         { 
            if ( mobile.Kills >= 999999999 ) 
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

         AddButton( 10, 475, 4005, 4007, 2, GumpButtonType.Reply, 0 );
        AddLabel( 45, 475, 0x34, "Visit UO.STRATICS.COM Treasure Map Archive" );

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
         else if ( info.ButtonID == 2 ) // Launch Browser
         m_Mobile.LaunchBrowser( "http://uo.stratics.com/thb/info/maparchive/Archive1.shtml" );

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

	 if ( Server.Spells.SpellHelper.CheckCombat( m_Mobile ) ) 
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