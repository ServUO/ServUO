#region AuthorHeader
//This script is the string version of publicmoongate.cs script found on the RunUO forums.  
//I simply, renamed it, changed it to moveable and changed the pic to that of a hued bodbook.
//Many of the locations were copied from Traveling Books script by Broze The Newb        
//Modified by Ashlar, beloved of Morrigan
// Moded by Icon
/*
 * 
 * Modded 10MAR2016 by Tukaram.  Changed book from basic runebook locations to treasure map locations. 
 * Mostly the same locations - there are a couple in Fel that no longer exist in Tram. I have seen the
 * non-existing locations in Tram spawn on maps still... bummer.  Runes 197, 198 & 200 are on non-existing 
 * landmass in Trammel (New Haven). Rune 195 in Tram is blocked by a building. So I did not include those 
 * in the Tram book.
 * 
 * 
*/
#endregion AuthorHeader


using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Commands;


namespace Server.Items
{
   public class FeluccaTMapAltas : Item
   {
      [Constructable]
      public FeluccaTMapAltas() : base( 0x22C5 ) //old id is 0x2259
      {
         Movable = true;
         //Light = LightType.Circle300;
         //Hue = 322;
         Weight = 3;  
         Name = "Felucca T Map Atlas";
         LootType = LootType.Blessed;
 
      }

      public FeluccaTMapAltas(Serial serial)
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
             m.CloseGump(typeof(FeluccaTMapAltasGump));
             m.SendGump(new FeluccaTMapAltasGump(m));

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

   public class PMEntry
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

      public PMEntry( Point3D loc, string text )
      {
         m_Location = loc;
         m_Text = text;
      }
   }

   public class PMList
   {
      private string m_Text, m_SelText;
      private Map m_Map;
      private PMEntry[] m_Entries;

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

      public PMEntry[] Entries
      {
         get
         {
            return m_Entries;
         }
      }

      public PMList( string text, string selText, Map map, PMEntry[] entries )
      {
         m_Text = text;
         m_SelText = selText;
         m_Map = map;
         m_Entries = entries;
      }

      public static readonly PMList Maps117 =
         new PMList( "Maps 1-17", "Maps 1-17", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 962, 506, 0 ), "Map 1 NWNW1" ), 
               new PMEntry( new Point3D( 1163,189, 6 ), "Map 2 NWNW2" ), 
               new PMEntry( new Point3D( 1316, 317, 22 ), "Map 3 NWNEa1" ), 
               new PMEntry( new Point3D( 1469, 232, 16 ), "Map 4 NWNEa2" ), 
               new PMEntry( new Point3D( 1505, 364, 0 ), "Map 5 NWNEa3" ), 
               new PMEntry( new Point3D( 2673, 392, 15 ), "Map 6 NENWb1" ), 
               new PMEntry( new Point3D( 2742, 435, 15 ), "Map 7 NENWb2" ), 
               new PMEntry( new Point3D( 2771, 345, 15 ), "Map 8 NENWb3" ), 
               new PMEntry( new Point3D( 2782, 289, 15 ), "Map 9 NENWb4" ),  
               new PMEntry( new Point3D( 2837, 233, 0 ), "Map 10 NENWb5" ),  
               new PMEntry( new Point3D( 3015, 250, 0 ), "Map 11 NENWb6" ),  
               new PMEntry( new Point3D( 3083, 202, 4 ), "Map 12 NENWb7" ),  
               new PMEntry( new Point3D( 1029, 1181, 0 ), "Map 13 NWSWb1" ),  
               new PMEntry( new Point3D( 1319, 889, 0 ), "Map 14 NWNEc1" ),  
               new PMEntry( new Point3D( 1415, 771, 0 ), "Map 15 NWNEc2" ),  
               new PMEntry( new Point3D( 1531, 753, 16 ), "Map 16 NWNEc3" ),  
               new PMEntry( new Point3D( 1556, 806, 0 ), "Map 17 NWNEc4" )  
            } );

      public static readonly PMList Maps1834 =
         new PMList( "Maps 18-34", "Maps 18-34", Map.Felucca, new PMEntry[]
                {
               new PMEntry( new Point3D( 1511, 968, 0 ), "Map 18 NWNEc5" ), 
               new PMEntry( new Point3D( 1562, 1058, 0 ), "Map 19 NWNEc7" ), 
               new PMEntry( new Point3D( 1511, 1071, 0 ), "Map 20 NWNEc6" ), 
               new PMEntry( new Point3D( 2340, 645, 0 ), "Map 21 NWNEb1" ), 
               new PMEntry( new Point3D( 2351, 689, 0 ), "Map 22 NWNEb2" ), 
               new PMEntry( new Point3D( 2397, 723, 0 ), "Map 23 NWNEb3" ), 
               new PMEntry( new Point3D( 2434, 767, 0 ), "Map 24 NWNEb4" ), 
               new PMEntry( new Point3D( 2644, 851, 0 ), "Map 25 NENWa1" ), 
               new PMEntry( new Point3D( 2459, 1042, 0 ), "Map 26 NWSEa4" ),  
               new PMEntry( new Point3D( 2518, 1066, 0 ), "Map 27 NWSEa3" ),  
               new PMEntry( new Point3D( 2339, 1159, 5 ), "Map 28 NWSEa1" ),  
               new PMEntry( new Point3D( 2393, 1154, 0 ), "Map 29 NWSEa2" ),  
               new PMEntry( new Point3D( 3247, 246, 4 ), "Map 30 NENWb8" ),  
               new PMEntry( new Point3D( 3404, 238, 0 ), "Map 31 NENWb10" ),  
               new PMEntry( new Point3D( 3377, 458, 9 ), "Map 32 NENWb9" ),  
               new PMEntry( new Point3D( 3370, 638, 5 ), "Map 33 NENWb11" ),  
               new PMEntry( new Point3D( 200, 1460, 0 ), "Map 34 NWSWa4" ) 
                } );

      public static readonly PMList Maps3551 =
         new PMList("Maps 35-51", "Maps 35-51", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 208, 1444, 0 ), "Map 35 NWSWa3" ), 
               new PMEntry( new Point3D( 360, 1337, 0 ), "Map 36 NWSWa2" ), 
               new PMEntry( new Point3D( 582, 1453, 0 ), "Map 37 NWSWa1" ), 
               new PMEntry( new Point3D( 349, 1565, 0 ), "Map 38 NWSWa5" ), 
               new PMEntry( new Point3D( 620, 1706, 0 ), "Map 39 NWSWa6" ), 
               new PMEntry( new Point3D( 963, 1859, 0 ), "Map 40 NWSWb3" ), 
               new PMEntry( new Point3D( 980, 1850, 0 ), "Map 41 NWSWb2" ), 
               new PMEntry( new Point3D( 970, 1894, 0 ), "Map 42 NWSWb6" ), 
               new PMEntry( new Point3D( 970, 1884, 0 ), "Map 43 NWSWb5" ),  
               new PMEntry( new Point3D( 978, 1880, 0 ), "Map 44 NWSWb4" ),  
               new PMEntry( new Point3D( 1018, 1860, 0 ), "Map 45 NWSWb14" ),  
               new PMEntry( new Point3D( 1035, 1877, 0 ), "Map 46 NWSWb13" ),  
               new PMEntry( new Point3D( 1043, 1904, 0 ), "Map 47 NWSWb12" ),  
               new PMEntry( new Point3D( 1043, 1960, 0 ), "Map 48 NWSWb11" ),  
               new PMEntry( new Point3D( 1039, 1976, 0 ), "Map 49 NWSWb10" ),  
               new PMEntry( new Point3D( 1025, 1991, 0 ), "Map 50 NWSWb9" ),  
               new PMEntry( new Point3D( 975, 1992, 0 ), "Map 51 NWSWb7" ) 
            } );

      public static readonly PMList Maps5268 =
         new PMList("Maps 52-68", "Maps 52-68", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 990, 1993, 0 ), "Map 52 NWSWb8" ), 
               new PMEntry( new Point3D( 451, 2053, 0 ), "Map 53 SWNWa3" ), 
               new PMEntry( new Point3D( 477, 2044, 7 ), "Map 54 SWNWa2" ), 
               new PMEntry( new Point3D( 493, 2027, 3 ), "Map 55 SWNWa1" ), 
               new PMEntry( new Point3D( 469, 2087, 8 ), "Map 56 SWNWa4" ), 
               new PMEntry( new Point3D( 467, 2100, 5 ), "Map 57 SWNWa5" ), 
               new PMEntry( new Point3D( 1658, 2030, 0 ), "Map 58 NWSEb1" ), 
               new PMEntry( new Point3D( 1690, 1993, 0 ), "Map 59 NWSEb2" ), 
               new PMEntry( new Point3D( 1710, 1964, 0 ), "Map 60 NWSEb3" ),  
               new PMEntry( new Point3D( 1726, 1999, 0 ), "Map 61 NWSEb4" ),  
               new PMEntry( new Point3D( 1733, 2017, 0 ), "Map 62 NWSEb5" ),  
               new PMEntry( new Point3D( 1743, 2028, 0 ), "Map 63 NWSEb6" ),  
               new PMEntry( new Point3D( 1754, 2020, 0 ), "Map 64 NWSEb7" ),  
               new PMEntry( new Point3D( 2035, 1942, 0 ), "Map 65 NWSEb8" ),  
               new PMEntry( new Point3D( 2055, 1963, 0 ), "Map 66 NWSEb9" ),  
               new PMEntry( new Point3D( 2066, 1979, 0 ), "Map 67 NWSEb13" ),  
               new PMEntry( new Point3D( 2059, 1990, 0 ), "Map 68 NWSEb14" ) 
                } );

      public static readonly PMList Maps6985 =
         new PMList("Maps 69-85", "Maps 69-85", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 2071, 2007, 0 ), "Map 69 NWSEb15" ), 
               new PMEntry( new Point3D( 2063, 1962, 0 ), "Map 70 NWSEb10" ), 
               new PMEntry( new Point3D( 2099, 1976, 0 ), "Map 71 NWSEb12" ), 
               new PMEntry( new Point3D( 2090, 1987, 0 ), "Map 72 NWSEb11" ), 
               new PMEntry( new Point3D( 2094, 2006, 0 ), "Map 73 NWSEb16" ), 
               new PMEntry( new Point3D( 2189, 1991, 0 ), "Map 74 NWSEb17" ), 
               new PMEntry( new Point3D( 1427, 2405, 5 ), "Map 75 SWNEa1" ), 
               new PMEntry( new Point3D( 1435, 2381, 5 ), "Map 76 SWNEa2" ), 
               new PMEntry( new Point3D( 1471, 2340, 5 ), "Map 77 SWNEa3" ),  
               new PMEntry( new Point3D( 1452, 2302, 0 ), "Map 78 SWNEa5" ),  
               new PMEntry( new Point3D( 1437, 2294, 0 ), "Map 79 SWNEa6" ),  
               new PMEntry( new Point3D( 1439, 2217, 0 ), "Map 80 SWNEa9" ),  
               new PMEntry( new Point3D( 1467, 2182, 0 ), "Map 81 SWNEa10" ),  
               new PMEntry( new Point3D( 1465, 2246, 5 ), "Map 82 SWNEa8" ),  
               new PMEntry( new Point3D( 1479, 2273, 5 ), "Map 83 SWNEa7" ),  
               new PMEntry( new Point3D( 1563, 2312, 5 ), "Map 84 SWNEa4" ),  
               new PMEntry( new Point3D( 1547, 2223, 10 ), "Map 85 SWNEa15" ) 
            } );

      public static readonly PMList Maps86102 =
         new PMList("Maps 86-102", "Maps 86-102", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 1519, 2214, 5 ), "Map 86 SWNEa11" ), 
               new PMEntry( new Point3D( 1534, 2189, 5 ), "Map 87 SWNEa14" ), 
               new PMEntry( new Point3D( 1523, 2150, 0 ), "Map 88 SWNEa12" ), 
               new PMEntry( new Point3D( 1542, 2115, 5 ), "Map 89 SWNEa13" ), 
               new PMEntry( new Point3D( 1595, 2193, 0 ), "Map 90 SWNEa16" ), 
               new PMEntry( new Point3D( 1619, 2236, 0 ), "Map 91 SWNEa17" ), 
               new PMEntry( new Point3D( 1655, 2268, 5 ), "Map 92 SWNEa18" ), 
               new PMEntry( new Point3D( 1725, 2288, 5 ), "Map 93 SWNEa20" ), 
               new PMEntry( new Point3D( 1774, 2321, 5 ), "Map 94 SWNEa22" ),  
               new PMEntry( new Point3D( 1759, 2333, 0 ), "Map 95 SWNEa23" ),  
               new PMEntry( new Point3D( 1766, 2431, 5 ), "Map 96 SWNEa24" ),  
               new PMEntry( new Point3D( 1703, 2318, 5 ), "Map 97 SWNEa21" ),  
               new PMEntry( new Point3D( 1655, 2304, 0 ), "Map 98 SWNEa19" ),  
               new PMEntry( new Point3D( 2063, 2144, 0 ), "Map 99 SWNEc1" ),  
               new PMEntry( new Point3D( 2104, 2124, 0 ), "Map 100 SWNEc2" ),  
               new PMEntry( new Point3D( 2098, 2101, 0 ), "Map 101 SWNEc3" ),  
               new PMEntry( new Point3D( 2129, 2108, 0 ), "Map 102 SWNEc5" )   
              });

      public static readonly PMList Maps103119 =
         new PMList("Maps 103-119", "Maps 103-119", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 2153, 2121, 0 ), "Map 103 SWNEc7" ), 
               new PMEntry( new Point3D( 2186, 2144, 0 ), "Map 104 SWNEc9" ), 
               new PMEntry( new Point3D( 2177, 2151, 0 ), "Map 105 SWNEc10" ), 
               new PMEntry( new Point3D( 2161, 2149, 0 ), "Map 106 SWNEc8" ), 
               new PMEntry( new Point3D( 2130, 2133, 0 ), "Map 107 SWNEc6" ), 
               new PMEntry( new Point3D( 2123, 2121, 0 ), "Map 108 SWNEc4" ), 
               new PMEntry( new Point3D( 2647, 2167, 5 ), "Map 109 SENWa1" ), 
               new PMEntry( new Point3D( 2628, 2221, 6 ), "Map 110 SENWa2" ), 
               new PMEntry( new Point3D( 2642, 2289, 7 ), "Map 111 SENWa3" ),  
               new PMEntry( new Point3D( 2682, 2291, 5 ), "Map 112 SENWa4" ),  
               new PMEntry( new Point3D( 2727, 2309, 0 ), "Map 113 SENWa5" ),  
               new PMEntry( new Point3D( 2781, 2294, 6 ), "Map 114 SENWa6" ),  
               new PMEntry( new Point3D( 2807, 2255, 0 ), "Map 115 SENWa7" ),  
               new PMEntry( new Point3D( 2850, 2252, 5 ), "Map 116 SENWa8" ),  
               new PMEntry( new Point3D( 2957, 2150, 53 ), "Map 117 SENWa12" ),  
               new PMEntry( new Point3D( 2967, 2171, 36 ), "Map 118 SENWa11" ),  
               new PMEntry( new Point3D( 2952, 2177, 52 ), "Map 119 SENWa13" )
            });

      public static readonly PMList Maps120136 =
         new PMList("Maps 120-136", "Maps 120-136", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 2955, 2200, 46 ), "Map 120 SENWa10" ), 
               new PMEntry( new Point3D( 2932, 2240, 5 ), "Map 121 SENWa9" ), 
               new PMEntry( new Point3D( 958, 2505, 0 ), "Map 122 SWNWb1" ), 
               new PMEntry( new Point3D( 1025, 2702, 0 ), "Map 123 SWNWb2" ), 
               new PMEntry( new Point3D( 1290, 2735, 0 ), "Map 124 SWNWb3" ), 
               new PMEntry( new Point3D( 1382, 2840, 0 ), "Map 125 SWNEb1" ), 
               new PMEntry( new Point3D( 1390, 2985, 0 ), "Map 126 SWNEb2" ), 
               new PMEntry( new Point3D( 1414, 3059, 0 ), "Map 127 SWNEb3" ), 
               new PMEntry( new Point3D( 1647, 2642, 5 ), "Map 128 SWNEa25" ),  
               new PMEntry( new Point3D( 1563, 2705, 0 ), "Map 129 SWNEa26" ),  
               new PMEntry( new Point3D( 1671, 2808, 0 ), "Map 130 SWNEa27" ),  
               new PMEntry( new Point3D( 1601, 3013, 0 ), "Map 131 SWNEb4" ),  
               new PMEntry( new Point3D( 1665, 3063, 0 ), "Map 132 SWNEb5" ),  
               new PMEntry( new Point3D( 1068, 3182, 0 ), "Map 133 SWSWa5" ),  
               new PMEntry( new Point3D( 1075, 3156, 0 ), "Map 134 SWSWa4" ),  
               new PMEntry( new Point3D( 1073, 3133, 0 ), "Map 135 SWSWa3" ),  
               new PMEntry( new Point3D( 1090, 3110, 0 ), "Map 136 SWSWa1" ) 
             });

      public static readonly PMList Maps137153 =
         new PMList("Maps 137-153", "Maps 137-153", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 1093, 3132, 0 ), "Map 137 SWSWa2" ), 
               new PMEntry( new Point3D( 1096, 3179, 0 ), "Map 138 SWSWa6" ), 
               new PMEntry( new Point3D( 1129, 3403, 0 ), "Map 139 SWSWb1" ), 
               new PMEntry( new Point3D( 1162, 3468, 0 ), "Map 140 SWSWb3" ), 
               new PMEntry( new Point3D( 1127, 3499, 0 ), "Map 141 SWSWb4" ), 
               new PMEntry( new Point3D( 1136, 3446, 0 ), "Map 142 SWSWb2" ), 
               new PMEntry( new Point3D( 2014, 3269, 0 ), "Map 143 SWSEa1" ), 
               new PMEntry( new Point3D( 2040, 3427, 0 ), "Map 144 SWSEa4" ), 
               new PMEntry( new Point3D( 2096, 3384, 0 ), "Map 145 SWSEa3" ),  
               new PMEntry( new Point3D( 2149, 3362, 10 ), "Map 146 SWSEa2" ),  
               new PMEntry( new Point3D( 2370, 3428, 3 ), "Map 147 SWSEc1" ),  
               new PMEntry( new Point3D( 2342, 3482, 3 ), "Map 148 SWSEc2" ),  
               new PMEntry( new Point3D( 2360, 3507, 3 ), "Map 149 SWSEc3" ),  
               new PMEntry( new Point3D( 2387, 3506, 3 ), "Map 150 SWSEc4" ),  
               new PMEntry( new Point3D( 2467, 3580, 3 ), "Map 151 SWSEe1" ),  
               new PMEntry( new Point3D( 2481, 3623, 3 ), "Map 152 SWSEe3" ),  
               new PMEntry( new Point3D( 2527, 3585, 0 ), "Map 153 SWSEe2" ) 
            });

      public static readonly PMList Maps154170 =
         new PMList("Maps 154-170", "Maps 154-170", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 2534, 3609, 0 ), "Map 154 SWSEe4" ), 
               new PMEntry( new Point3D( 2797, 3452, 0 ), "Map 155 SESW1" ), 
               new PMEntry( new Point3D(2803, 3488, 0 ), "Map 156 SESW2" ), 
               new PMEntry( new Point3D( 2792, 3520, 0 ), "Map 157 SESW3" ), 
               new PMEntry( new Point3D( 2831, 3510, 0 ), "Map 158 SESW4" ), 
               new PMEntry( new Point3D( 2989, 3606, 15 ), "Map 159 SESW5" ), 
               new PMEntry( new Point3D( 3035, 3602, 0 ), "Map 160 SESW6" ), 
               new PMEntry( new Point3D( 2154, 3983, 3 ), "Map 161 SWSEd5" ), 
               new PMEntry( new Point3D( 2144, 3985, 0 ), "Map 162 SWSEd4" ),  
               new PMEntry( new Point3D( 2140, 3941, 3 ), "Map 163 SWSEd3" ),  
               new PMEntry( new Point3D( 2157, 3924, 3 ), "Map 164 SWSEd2" ),  
               new PMEntry( new Point3D( 2152, 3951, 3 ), "Map 165 SWSEd1" ),  
               new PMEntry( new Point3D( 2162, 3988, 3 ), "Map 166 SWSEd6" ),  
               new PMEntry( new Point3D( 2452, 3942, 0 ), "Map 167 SWSEb10" ),  
               new PMEntry( new Point3D( 2421, 3929, 3 ), "Map 168 SWSEb9" ),  
               new PMEntry( new Point3D( 2414, 3920, 3 ), "Map 169 SWSEb8" ),  
               new PMEntry( new Point3D( 2436, 3902, 0 ), "Map 170 SWSEb7" ) 
            });

      public static readonly PMList Maps171187 =
         new PMList("Maps 171-187", "Maps 171-187", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 2481, 3908, 6 ), "Map 171 SWSEb6" ), 
               new PMEntry( new Point3D( 2512, 3899, 3 ), "Map 172 SWSEb5" ), 
               new PMEntry( new Point3D( 2512, 3919, 0 ), "Map 173 SWSEb4" ), 
               new PMEntry( new Point3D( 2512, 3962, 6 ), "Map 174 SWSEb3" ), 
               new PMEntry( new Point3D( 2527, 3982, 0 ), "Map 175 SWSEb2" ), 
               new PMEntry( new Point3D( 2516, 3998, 3 ), "Map 176 SWSEb1" ), 
               new PMEntry( new Point3D( 4476, 3282, 0 ), "Map 177 SESE9" ), 
               new PMEntry( new Point3D( 4477, 3230, 0 ), "Map 178 SESE8" ), 
               new PMEntry( new Point3D( 4465, 3210, 0 ), "Map 179 SESE7" ),  
               new PMEntry( new Point3D( 4425, 3152, 0 ), "Map 180 SESE6" ),  
               new PMEntry( new Point3D( 4420, 3117, 0 ), "Map 181 SESE1" ),  
               new PMEntry( new Point3D( 4449, 3130, 0 ), "Map 182 SESE2" ),  
               new PMEntry( new Point3D( 4453, 3148, 0 ), "Map 183 SESE3" ),  
               new PMEntry( new Point3D( 4501, 3108, 0 ), "Map 184 SESE4" ),  
               new PMEntry( new Point3D( 4513, 3104, 0 ), "Map 185 SESE5" ),  
               new PMEntry( new Point3D( 4470, 3188, 0 ), "Map 186 SESE10" ),  
               new PMEntry( new Point3D( 4507, 3227, 0 ), "Map 187 SESE11" ) 
            });

      public static readonly PMList Maps188200 =
  new PMList("Maps 188-200", "Maps 188-200", Map.Felucca, new PMEntry[]
            {
               new PMEntry( new Point3D( 4495, 3242, 0 ), "Map 188 SESE12" ), 
               new PMEntry( new Point3D( 4642, 3369, 0 ), "Map 189 SESE13" ), 
               new PMEntry( new Point3D( 4694, 3486, 0 ), "Map 190 SESE14" ), 
               new PMEntry( new Point3D( 3477, 2761, 35 ), "Map 191 SENWb6" ), 
               new PMEntry( new Point3D( 3426, 2723, 49 ), "Map 192 SENWb5" ), 
               new PMEntry( new Point3D( 3418, 2675, 50 ), "Map 193 SENWb4" ), 
               new PMEntry( new Point3D( 3533, 2471, 10 ), "Map 194 SENWb3" ),
               new PMEntry( new Point3D( 3511, 2421, 48 ), "Map 195 SENWb2" ),
               new PMEntry( new Point3D( 3568, 2402, 11 ), "Map 196 SENWb1" ), 
               new PMEntry( new Point3D( 3702, 2825, 21 ), "Map 197 SENWb10" ), 
               new PMEntry( new Point3D( 3594, 2826, 44 ), "Map 198 SENWb9" ), 
               new PMEntry( new Point3D( 3557, 2820, 24 ), "Map 199 SENWb8" ), 
               new PMEntry( new Point3D( 3541, 2785, 5 ), "Map 200 SENWb7" )  

            });





        //Here you can edit what facets to show on the TravelBooks... For Tmaps I show all, just easier not to edit out section.
                public static readonly PMList[] UORLists        = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
                public static readonly PMList[] UORlistsYoung   = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
                public static readonly PMList[] LBRLists        = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
                public static readonly PMList[] LBRListsYoung   = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
                public static readonly PMList[] AOSLists        = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
                public static readonly PMList[] AOSListsYoung   = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
                public static readonly PMList[] SELists         = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
                public static readonly PMList[] SEListsYoung    = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
                public static readonly PMList[] RedLists        = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
                public static readonly PMList[] SigilLists      = new PMList[] { Maps117, Maps1834, Maps3551, Maps5268, Maps6985, Maps86102, Maps103119, Maps120136, Maps137153, Maps154170, Maps171187, Maps188200 };
    }

   public class FeluccaTMapAltasGump : Gump
   {
      public static void Initialize()
      {
         CommandSystem.Register( "Pod2", AccessLevel.GameMaster, new CommandEventHandler( FeluccaTMapAltasGump_OnCommand ) );
      }

      private static void FeluccaTMapAltasGump_OnCommand( CommandEventArgs e )
      {
         e.Mobile.SendGump( new FeluccaTMapAltasGump( e.Mobile ) );
      }

      private Mobile m_Mobile;
      private PMList[] m_Lists;

      public FeluccaTMapAltasGump(Mobile mobile)
          : base(100, 100)
      {
         m_Mobile = mobile;

         PMList[] checkLists;

         if ( mobile.Player )
         {
            if ( mobile.Kills >= 5 )
            {
               checkLists = PMList.RedLists;
            }
           // else
           // {
//               int flags = mobile.NetState == null ? 0 : mobile.NetState.Flags;

           //    if ( Core.AOS && (flags & 0x8) != 0 )
           //       checkLists = PMList.AOSLists;
           //    else if ( (flags & 0x4) != 0 )
           //       checkLists = PMList.LBRLists;
           //    else
           //       checkLists = PMList.UORLists;
          //  }
        // }
         else
         {
            checkLists = PMList.AOSLists;
         }

         m_Lists = new PMList[checkLists.Length];

         for ( int i = 0; i < m_Lists.Length; ++i )
            m_Lists[i] = checkLists[i];

         for ( int i = 0; i < m_Lists.Length; ++i )
         {
            if ( m_Lists[i].Map == mobile.Map )
            {
               PMList temp = m_Lists[i];

               m_Lists[i] = m_Lists[0];
               m_Lists[0] = temp;

               break;
            }
         }

         AddPage( 0 );

         AddBackground( 0, 0, 380, 480, 9200 ); //5054 is invis. 2600 is not bad

         AddButton( 10, 425, 4005, 4007, 1, GumpButtonType.Reply, 0 );
         AddHtmlLocalized( 45, 425, 140, 25, 1011036, false, false ); // OKAY

         AddButton( 10, 450, 4005, 4007, 0, GumpButtonType.Reply, 0 );
         AddHtmlLocalized( 45, 450, 140, 25, 1011012, false, false ); // CANCEL

         AddHtmlLocalized( 5, 5, 200, 20, 1012011, false, false ); // Pick your destination:

         for ( int i = 0; i < checkLists.Length; ++i )
         {
            AddButton( 10, 35 + (i * 25), 2117, 2118, 0, GumpButtonType.Page, Array.IndexOf( m_Lists, checkLists[i] ) + 1 );
            AddHtml( 30, 35 + (i * 25), 150, 20, checkLists[i].Text, false, false );
         }

         for ( int i = 0; i < m_Lists.Length; ++i )
            RenderPage( i, Array.IndexOf( checkLists, m_Lists[i] ) );
      }}

      private void RenderPage( int index, int offset )
      {
         PMList list = m_Lists[index];

         AddPage( index + 1 );

         AddButton( 10, 35 + (offset * 25), 2117, 2118, 0, GumpButtonType.Page, index + 1 );
         AddHtml( 30, 35 + (offset * 25), 150, 20, list.SelText, false, false );

         PMEntry[] entries = list.Entries;

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

         PMList list = m_Lists[listIndex];

         if ( listEntry < 0 || listEntry >= list.Entries.Length )
            return;

         PMEntry entry = list.Entries[listEntry];

         if ( m_Mobile.Player && m_Mobile.Kills >= 5 && list.Map != Map.Felucca )
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
