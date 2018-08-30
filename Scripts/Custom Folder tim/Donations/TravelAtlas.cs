using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Items
{
	public class TravelAtlas : Item
	{
		[Flags]
		public enum OptFlags
		{
			None			= 0x0000,
			Trammel			= 0x0001,
			TramDungeons	= 0x0002,
			Felucca			= 0x0004,
			FelDungeons		= 0x0008,
			Custom      	= 0x0010,
			Ilshenar		= 0x0020,
			IlshenarShrines = 0x0040,
			Malas			= 0x0080,
			Tokuno			= 0x0100,
			AllowReds		= 0x0200,
			TerMur			= 0x0400,
			UseGlobal		= 0x0800
		}

		public static void Initialize()
		{
			
			if ( m_GlobalFlags == OptFlags.None )
			{
				SetOptFlag( ref m_GlobalFlags, OptFlags.Trammel, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.TramDungeons, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Felucca, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.FelDungeons, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Custom, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Ilshenar, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.IlshenarShrines, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Malas, Core.AOS );
				SetOptFlag( ref m_GlobalFlags, OptFlags.Tokuno, Core.SE );
				SetOptFlag( ref m_GlobalFlags, OptFlags.TerMur, true );
				SetOptFlag( ref m_GlobalFlags, OptFlags.UseGlobal, true );
			}

			GlobalEntries.Add("Trammel", new TAEntry[]
			{
				new TAEntry("Britain", new Point3D(1434, 1699, 2), Map.Trammel ),
				new TAEntry("Bucs Den", new Point3D(2705, 2162, 0), Map.Trammel ),
				new TAEntry("Cove", new Point3D(2237, 1214, 0), Map.Trammel ),
				new TAEntry("Delucia", new Point3D(5274, 3991, 37), Map.Trammel ),
				new TAEntry("New Haven", new Point3D(3500, 2571, 14), Map.Trammel ),
				new TAEntry("Haven", new Point3D(3626, 2611, 0), Map.Trammel ),
				new TAEntry("Jhelom", new Point3D(1417, 3821, 0), Map.Trammel ),
				new TAEntry("Magincia", new Point3D(3728, 2164, 20), Map.Trammel ),
				new TAEntry("Minoc", new Point3D(2525, 582, 0), Map.Trammel ),
				new TAEntry("Moonglow", new Point3D(4471, 1177, 0), Map.Trammel ),
				new TAEntry("Nujel'm", new Point3D(3770, 1308, 0), Map.Trammel ),
				new TAEntry("Papua", new Point3D(5729, 3208, -6), Map.Trammel ),
				new TAEntry("Serpents Hold", new Point3D(2895, 3479, 15), Map.Trammel ),
				new TAEntry("Skara Brae", new Point3D(596, 2138, 0), Map.Trammel ),
				new TAEntry("Trinsic", new Point3D(1823, 2821, 0), Map.Trammel ),
				new TAEntry("Vesper", new Point3D(2899, 676, 0), Map.Trammel ),
				new TAEntry("Wind", new Point3D(1361, 895, 0), Map.Trammel ),
				new TAEntry("Yew", new Point3D(542, 985, 0), Map.Trammel )
			});

			GlobalEntries.Add("Trammel Dungeons", new TAEntry[]
			{
				new TAEntry("Covetous", new Point3D(2498, 921, 0), Map.Trammel ),
				new TAEntry("Daemon Temple", new Point3D(4591, 3647, 80), Map.Trammel ),
				new TAEntry("Deceit", new Point3D(4111, 434, 5), Map.Trammel ),
				new TAEntry("Despise", new Point3D(1301, 1080, 0), Map.Trammel ),
				new TAEntry("Destard", new Point3D(1176, 2640, 2), Map.Trammel ),
				new TAEntry("Fire", new Point3D(2923, 3409, 8), Map.Trammel ),
				new TAEntry("Hythloth", new Point3D(4721, 3824, 0), Map.Trammel ),
				new TAEntry("Ice", new Point3D(1999, 81, 4), Map.Trammel ),
				new TAEntry("Ophidian Temple", new Point3D(5766, 2634, 43), Map.Trammel ),
				new TAEntry("Orc Caves", new Point3D(1017, 1429, 0), Map.Trammel ),
				new TAEntry("Shame", new Point3D(511, 1565, 0), Map.Trammel ),
				new TAEntry("Solen Hive", new Point3D(2607, 763, 0), Map.Trammel ),
				new TAEntry("Terathan Keep", new Point3D(5451, 3143, -60), Map.Trammel ),
				new TAEntry("Wrong", new Point3D(2043, 238, 10), Map.Trammel )
			});

			GlobalEntries.Add("Felucca", new TAEntry[]
			{
				new TAEntry("Britain", new Point3D(1434, 1699, 2), Map.Felucca ),
				new TAEntry("Bucs Den", new Point3D(2705, 2162, 0), Map.Felucca ),
				new TAEntry("Cove", new Point3D(2237, 1214, 0), Map.Felucca ),
				new TAEntry("Delucia", new Point3D(5274, 3991, 37), Map.Felucca ),
				new TAEntry("Jhelom", new Point3D(1417, 3821, 0), Map.Felucca ),
				new TAEntry("Magincia", new Point3D(3728, 2164, 20), Map.Felucca ),
				new TAEntry("Minoc", new Point3D(2525, 582, 0), Map.Felucca ),
				new TAEntry("Moonglow", new Point3D(4471, 1177, 0), Map.Felucca ),
				new TAEntry("Nujel'm", new Point3D(3770, 1308, 0), Map.Felucca ),
				new TAEntry("Ocllo", new Point3D(3626, 2611, 0), Map.Felucca ),
				new TAEntry("Papua", new Point3D(5729, 3208, -6), Map.Felucca ),
				new TAEntry("Serpents Hold", new Point3D(2895, 3479, 15), Map.Felucca ),
				new TAEntry("Skara Brae", new Point3D(596, 2138, 0), Map.Felucca ),
				new TAEntry("Trinsic", new Point3D(1823, 2821, 0), Map.Felucca ),
				new TAEntry("Vesper", new Point3D(2899, 676, 0), Map.Felucca ),
				new TAEntry("Wind", new Point3D(1361, 895, 0), Map.Felucca ),
				new TAEntry("Yew", new Point3D(542, 985, 0), Map.Felucca )
			});

			GlobalEntries.Add("Felucca Dungeons", new TAEntry[]
			{
				new TAEntry("Covetous", new Point3D(2498, 921, 0), Map.Felucca ),
				new TAEntry("Daemon Temple", new Point3D(4591, 3647, 80), Map.Felucca ),
				new TAEntry("Deceit", new Point3D(4111, 434, 5), Map.Felucca ),
				new TAEntry("Despise", new Point3D(1301, 1080, 0), Map.Felucca ),
				new TAEntry("Destard", new Point3D(1176, 2640, 2), Map.Felucca ),
				new TAEntry("Fire", new Point3D(2923, 3409, 8), Map.Felucca ),
				new TAEntry("Hythloth", new Point3D(4721, 3824, 0), Map.Felucca ),
				new TAEntry("Ice", new Point3D(1999, 81, 4), Map.Felucca ),
				new TAEntry("Ophidian Temple", new Point3D(5766, 2634, 43), Map.Felucca ),
				new TAEntry("Orc Caves", new Point3D(1017, 1429, 0), Map.Felucca ),
				new TAEntry("Shame", new Point3D(511, 1565, 0), Map.Felucca ),
				new TAEntry("Solen Hive", new Point3D(2607, 763, 0), Map.Felucca ),
				new TAEntry("Terathan Keep", new Point3D(5451, 3143, -60), Map.Felucca ),
				new TAEntry("Wrong", new Point3D(2043, 238, 10), Map.Felucca )
			});

			GlobalEntries.Add("Custom Areas", new TAEntry[]//add locations to the Custom map here
			{
				new TAEntry("Gate Room", new Point3D(6079, 451, -22), Map.Felucca ),
				new TAEntry("Training Room", new Point3D(24, 1273, 0), Map.Malas ),
                //new TAEntry("The Expedition", new Point3D(5395, 1419, 0), Map.Trammel ),
                new TAEntry("Hue Room", new Point3D(5379, 1094, 0), Map.Trammel ),
				new TAEntry("Triple Champ Abyss", new Point3D(7037, 698, 28), Map.Felucca ),
				new TAEntry("The Arena", new Point3D(877, 1858, 0), Map.Felucca )

			});

			GlobalEntries.Add("Ilshenar", new TAEntry[]
			{
				new TAEntry("Ankh Dungeon", new Point3D(576, 1150, -100), Map.Ilshenar ),
				new TAEntry("Blood Dungeon", new Point3D(1747, 1171, -2), Map.Ilshenar ),
				new TAEntry("Exodus Dungeon", new Point3D(854, 778, -80), Map.Ilshenar ),
				new TAEntry("Gargoyle City", new Point3D(852, 602, -40), Map.Ilshenar ),
				new TAEntry("Lakeshire", new Point3D(1203, 1124, -25), Map.Ilshenar ),
				new TAEntry("Mistas", new Point3D(819, 1130, -29), Map.Ilshenar ),
				new TAEntry("Montor", new Point3D(1706, 205, 104), Map.Ilshenar ),
				new TAEntry("Rock Dungeon", new Point3D(1787, 572, 69), Map.Ilshenar ),
				new TAEntry("Savage Camp", new Point3D(1151, 659, -80), Map.Ilshenar ),
				new TAEntry("Sorceror's Dungeon", new Point3D(548, 462, -53), Map.Ilshenar ),
				new TAEntry("Spectre Dungeon", new Point3D(1363, 1033, -8), Map.Ilshenar ),
				new TAEntry("Spider Cave", new Point3D(1420, 913, -16), Map.Ilshenar ),
				new TAEntry("Wisp Dungeon", new Point3D(651, 1302, -58), Map.Ilshenar )
			});

			GlobalEntries.Add("Ilshenar Shrines", new TAEntry[]
			{
				new TAEntry("Compassion Shrine", new Point3D(1215, 467, -13), Map.Ilshenar ),
				new TAEntry("Honesty Shrine", new Point3D(722, 1366, -60), Map.Ilshenar ),
				new TAEntry("Honor Shrine", new Point3D(744, 724, -28), Map.Ilshenar ),
				new TAEntry("Humility Shrine", new Point3D(281, 1016, 0), Map.Ilshenar ),
				new TAEntry("Justice Shrine", new Point3D(987, 1011, -32), Map.Ilshenar ),
				new TAEntry("Sacrifice Shrine", new Point3D(1174, 1286, -30), Map.Ilshenar ),
				new TAEntry("Spirituality Shrine", new Point3D(1532, 1340, -3), Map.Ilshenar ),
				new TAEntry("Valor Shrine", new Point3D(528, 216, -45), Map.Ilshenar ),
				new TAEntry("Choas Shrine", new Point3D(1721, 218, 96), Map.Ilshenar )
			});

			GlobalEntries.Add("Malas", new TAEntry[]
			{
				new TAEntry("Doom", new Point3D(2368, 1267, -85), Map.Malas ),
				new TAEntry("Luna", new Point3D(1015, 527, -65), Map.Malas ),
				new TAEntry("Orc Fort 1", new Point3D(912, 215, -90), Map.Malas ),
				new TAEntry("Orc Fort 2", new Point3D(1678, 374, -50), Map.Malas ),
				new TAEntry("Orc Fort 3", new Point3D(1375, 621, -86), Map.Malas ),
				new TAEntry("Orc Fort 4", new Point3D(1184, 715, -89), Map.Malas ),
				new TAEntry("Orc Fort 5", new Point3D(1279, 1324, -90), Map.Malas ),
				new TAEntry("Orc Fort 6", new Point3D(1598, 1834, -107), Map.Malas ),
				new TAEntry("Ruined Temple", new Point3D(1598, 1762, -110), Map.Malas ),
				new TAEntry("Umbra", new Point3D(1997, 1386, -85), Map.Malas )
			});

			GlobalEntries.Add("Tokuno", new TAEntry[]
			{
				new TAEntry("Bushido Dojo", new Point3D(322, 430, 32), Map.Tokuno ),
				new TAEntry("Crane Marsh", new Point3D(203, 985, 18), Map.Tokuno ),
				new TAEntry("Fan Dancer's Dojo", new Point3D(970, 222, 23), Map.Tokuno ),
				new TAEntry("Isamu-Jima", new Point3D(1169, 998, 41), Map.Tokuno ),
				new TAEntry("Makoto-Jima", new Point3D(802, 1204, 25), Map.Tokuno ),
				new TAEntry("Homare-Jima", new Point3D(270, 628, 15), Map.Tokuno ),
				new TAEntry("Makoto Desert", new Point3D(724, 1050, 33), Map.Tokuno ),
				new TAEntry("Makoto Zento", new Point3D(741, 1261, 30), Map.Tokuno ),
				new TAEntry("Mt. Sho Castle", new Point3D(1234, 772, 3), Map.Tokuno ),
				new TAEntry("Valor Shrine", new Point3D(1044, 523, 15), Map.Tokuno ),
				new TAEntry("Yomotsu Mine", new Point3D(257, 786, 63), Map.Tokuno )
			});

			GlobalEntries.Add("Staff", new TAEntry[]//add locations to the staff map here
			{
				new TAEntry("Green Acres tram", new Point3D(5445, 1153, 0), Map.Trammel ),
				new TAEntry("Green Acres fel", new Point3D(5445, 1153, 0), Map.Felucca ),
				new TAEntry("Jail tram", new Point3D(5296, 1173, 0), Map.Trammel ),
				new TAEntry("Jail fel", new Point3D(5296, 1173, 0), Map.Felucca ),
				new TAEntry("Star Room tram", new Point3D(5146, 1774, 0), Map.Trammel ),
				new TAEntry("Star Room fel", new Point3D(5146, 1774, 0), Map.Felucca )
			});

			GlobalEntries.Add("TerMur", new TAEntry[]//add locations to the TerMur map here
			{
				new TAEntry("East Refuge", new Point3D(1112, 3619, -45), Map.TerMur ),
				new TAEntry("Fisherman Village", new Point3D(640, 3059, 38), Map.TerMur ),
				new TAEntry("Holy City", new Point3D(996, 3869, -42), Map.TerMur ),
				new TAEntry("Royal City", new Point3D(851, 3525, -38), Map.TerMur )
				
			});
		}
			
		public static int GenerateTravelAtlas()
		{
			int gen = 0;

			if (GetOptFlag( m_GlobalFlags, OptFlags.Trammel )) gen += GenerateEntry( "Trammel" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.TramDungeons )) gen += GenerateEntry( "Trammel Dungeons" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Felucca )) gen += GenerateEntry( "Felucca" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.FelDungeons )) gen += GenerateEntry( "Felucca Dungeons" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Custom)) gen += GenerateEntry( "Custom Areas" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Ilshenar)) gen += GenerateEntry( "Ilshenar" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.IlshenarShrines)) gen += GenerateEntry( "Ilshenar Shrines" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Malas) && Core.AOS) gen += GenerateEntry( "Malas" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.Tokuno) && Core.SE) gen += GenerateEntry( "Tokuno" );
			if (GetOptFlag( m_GlobalFlags, OptFlags.TerMur)) gen += GenerateEntry( "TerMur" );

			gen += GenerateEntry( "Staff" );

			return gen;
		}		

		private static int GenerateEntry( string map )
		{
			TAEntry[] me = (TAEntry[])GlobalEntries[map];
			if ( me != null )
			{
				for (int i = 0; i < me.Length; i++)
					new TravelAtlas().MoveToWorld( me[i].Destination, me[i].Map );
				return me.Length;
			}
			return 0;
		}

		public static Hashtable GlobalEntries = new Hashtable();
		private OptFlags m_Flags;
		private static OptFlags m_GlobalFlags;

		[CommandProperty(AccessLevel.Administrator)]
		public bool Trammel{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Trammel ); } set{ SetOptFlag( OptFlags.Trammel, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool TramDungeons{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.TramDungeons ); } set{ SetOptFlag( OptFlags.TramDungeons, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Felucca{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Felucca ); } set{ SetOptFlag( OptFlags.Felucca, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool FelDungeons{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.FelDungeons ); } set{ SetOptFlag( OptFlags.FelDungeons, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Custom{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Custom ); } set{ SetOptFlag( OptFlags.Custom, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Ilshenar{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Ilshenar ); } set{ SetOptFlag( OptFlags.Ilshenar, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool IlshenarShrines{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.IlshenarShrines ); } set{ SetOptFlag( OptFlags.IlshenarShrines, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Malas{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Malas ); } set{ SetOptFlag( OptFlags.Malas, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Tokuno{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.Tokuno ); } set{ SetOptFlag( OptFlags.Tokuno, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool AllowReds{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.AllowReds ); } set{ SetOptFlag( OptFlags.AllowReds, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool TerMur{ get{ return GetOptFlag( (UseGlobal ? m_GlobalFlags : m_Flags), OptFlags.TerMur ); } set{ SetOptFlag( OptFlags.TerMur, value ); } }

		[CommandProperty(AccessLevel.Administrator)]
		public bool UseGlobal{ get{ return GetOptFlag( m_Flags, OptFlags.UseGlobal ); } set{ SetOptFlag( ref m_Flags, OptFlags.UseGlobal, value ); } }

		public void SetOptFlag( OptFlags toSet, bool value )
		{
			if ( UseGlobal )
			{
				if ( value )
					m_GlobalFlags |= toSet;
				else
					m_GlobalFlags &= ~toSet;
			}
			else
			{
				if ( value )
					m_Flags |= toSet;
				else
					m_Flags &= ~toSet;
			}
		}

		public static void SetOptFlag( ref OptFlags flags, OptFlags toSet, bool value )
		{
			if ( value )
				flags |= toSet;
			else
				flags &= ~toSet;
		}

		public static bool GetOptFlag( OptFlags flags, OptFlags flag )
		{
			return ( (flags & flag) != 0 );
		}

		[Constructable]
		public TravelAtlas() : this ( (int)m_GlobalFlags )
		{
		}

		[Constructable]
		public TravelAtlas( int flags ) : base( 0x22C5 )
		{
			Movable = true;
			LootType = LootType.Blessed;
			Hue = 38;
			Name = "Travel Atlas";
			Light = LightType.Circle300;
			m_Flags = (OptFlags)flags;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.Player )
				return;
			UseTeleporter(from);
		}

		public bool UseTeleporter( Mobile m )
		{
			if ( m.Criminal )
				m.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
			else if ( Server.Spells.SpellHelper.CheckCombat( m ) )
				m.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
			else if ( m.Spell != null )
				m.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment.
			else
			{
				m.CloseGump( typeof( TravelAtlasGump ) );
				m.SendGump( new TravelAtlasGump( m, this, 0 ) );

				if ( !m.Hidden || m.AccessLevel == AccessLevel.Player )
					Effects.PlaySound( m.Location, m.Map, 0x20E );
				return true;
			}
			return false;
		}

		public TravelAtlas( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			//version 2
			writer.Write( (int) m_Flags );
			writer.Write( (int) m_GlobalFlags );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch (version)
			{
				case 2:
				{
					m_Flags = (OptFlags)reader.ReadInt();
					m_GlobalFlags = (OptFlags)reader.ReadInt();
					break;
				}
				case 1:
				{
					SetOptFlag( ref m_Flags, OptFlags.IlshenarShrines, reader.ReadBool() );
					goto case 0;
				}
				case 0:
				{
					SetOptFlag( ref m_Flags, OptFlags.Trammel, reader.ReadBool() );
					SetOptFlag( ref m_Flags, OptFlags.TramDungeons, reader.ReadBool() );
					SetOptFlag( ref m_Flags, OptFlags.Felucca, reader.ReadBool() );
					SetOptFlag( ref m_Flags, OptFlags.FelDungeons, reader.ReadBool() );
					SetOptFlag( ref m_Flags, OptFlags.Custom, reader.ReadBool() );
					SetOptFlag( ref m_Flags, OptFlags.Ilshenar, reader.ReadBool() );
					SetOptFlag( ref m_Flags, OptFlags.Malas, reader.ReadBool() );
					SetOptFlag( ref m_Flags, OptFlags.Tokuno, reader.ReadBool() );
					SetOptFlag( ref m_Flags, OptFlags.AllowReds, reader.ReadBool() );
					SetOptFlag( ref m_Flags, OptFlags.TerMur, reader.ReadBool() );
					UseGlobal = false;
					break;
				}
			}
		}
	}

	public class TravelAtlasGump : Gump
	{
		private TravelAtlas m_TravelAtlas;
		private int m_Page;
		private bool m_Reds, m_HasLBR, m_HasAOS, m_HasSE;

		public TravelAtlasGump( Mobile from, TravelAtlas TA, int page ) : base( 100, 100 )
		{
			ClientFlags flags = (from.NetState == null ? ClientFlags.None : from.NetState.Flags);

			m_TravelAtlas = TA;
			m_Page = page;
			 m_HasLBR = ((int)flags & 0x04) != 0;
   			m_HasAOS = ((int)flags & 0x08) != 0;
   			m_HasSE = ((int)flags & 0x10) != 0;
			m_Reds = (from.Kills < 5 || TA.AllowReds);

			//Did they press an invalid button or supply an invalid argument?
			if ( page < 0 || page > 11 )
				page = 0;

			AddPage( 0 );
			AddBackground( 0, 0, 660, 404, 3500 ); //white

			AddImage(267, 10, 5528); // map
			AddImage(45, 30, 5609); // globe

			AddHtmlLocalized( 10, 10, 200, 20, 1012011, false, false ); // Pick your destination:

			int p = 1;

			if ( TA.Trammel && m_Reds )
			{
				GenerateMapListing( 1 );
				AddPageButton( "Trammel", Map.Trammel, p++, 1 );
			}

			if ( TA.TramDungeons && m_Reds )
			{
				GenerateMapListing( 2 );
				AddPageButton( "Trammel Dungeons", Map.Trammel, p++, 2 );
			}

			if ( TA.Felucca )
			{
				GenerateMapListing( 3 );
				AddPageButton( "Felucca", Map.Felucca, p++, 3 );
			}

			if ( TA.FelDungeons )
			{
				GenerateMapListing( 4 );
				AddPageButton( "Felucca Dungeons", Map.Felucca, p++, 4 );
			}

			if ( TA.Custom && m_Reds && Core.AOS && m_HasAOS )
			{
				GenerateMapListing( 5 );
				AddPageButton( "<basefont color=#0000FF>Custom <basefont color=#FF0000>Areas</basefont>", null, p++, 5 );
			}

			if ( TA.Ilshenar && m_Reds && m_HasLBR )
			{
				GenerateMapListing( 6 );
				AddPageButton( "Ilshenar", Map.Ilshenar, p++, 6 );
			}

			if ( TA.IlshenarShrines && m_Reds && m_HasLBR )
			{
				GenerateMapListing( 7 );
				AddPageButton( "Ilshenar Shrines", Map.Ilshenar, p++, 7 );
			}

			if ( TA.Malas && m_Reds && Core.AOS && m_HasAOS )
			{
				GenerateMapListing( 8 );
				AddPageButton( "Malas", Map.Malas, p++, 8 );
			}

			if ( TA.Tokuno && m_Reds && Core.SE && m_HasSE )
			{
				GenerateMapListing( 9 );
				AddPageButton( "Tokuno", Map.Tokuno, p++, 9 );
			}

			if ( from.AccessLevel > AccessLevel.Player )
			{
				GenerateMapListing( 10 );
				AddPageButton( "Staff Only", null, p++, 10 );
			}

			if ( TA.TerMur )
			{
				GenerateMapListing( 11 );
				AddPageButton( "TerMur", null, p++, 11 );
			}
		}

		private void AddPageButton( string text, Map map, int offset, int page )
		{
			string label;
			if ( map != null )
				label = String.Format( "<basefont color=#{0}>{1}</basefont>", MapHue( map ), text );
			else
				label = text;
			AddHtml( 30, 100 + ((offset - 1) * 25), 150, 20, label, false, false );
			AddButton( 10, 100 + ((offset - 1) * 25), 2117, 2118, page, GumpButtonType.Reply, 0 );
		}

		private static TAEntry GetEntry( string name, int id )
		{
			TAEntry[] me = (TAEntry[])TravelAtlas.GlobalEntries[name];

			if ( me != null )
			{
				if ( id < 0 || id >= me.Length )
					id = 0;
				return me[id];
			}

			return null;
		}

		private void GenerateMapListing( int page )
		{
			if ( m_Page == 0 )
				m_Page = page;
			else if ( page != m_Page )
				return;

			string name = m_Entries[page-1];

			TAEntry[] me = (TAEntry[])TravelAtlas.GlobalEntries[name];
			if ( me == null )
				return;

			int offset = m_Page * 100;
			bool gates = name == "Custom Areas";
			for (int i = 0, l = 0; i < me.Length; i++ )
			{
				TAEntry entry = me[i];

				if ( ( (gates || name == "Felucca") && entry.Map == Map.Felucca && !m_TravelAtlas.Felucca) )
					continue;
				else if ( (gates || name == "Trammel") && entry.Map == Map.Trammel && (!m_TravelAtlas.Trammel || !m_Reds))
					continue;
				else if ( entry.Map == Map.Ilshenar && (!m_TravelAtlas.Ilshenar || !m_HasLBR || !m_Reds))
					continue;
				else if (entry.Map == Map.Malas && (!Core.AOS || !m_HasAOS || !m_TravelAtlas.Malas || !m_Reds))
					continue;
				else if (entry.Map == Map.Tokuno && (!Core.SE || !m_HasSE || !m_TravelAtlas.Tokuno || !m_Reds))
					continue;
				else
				{
					string label = String.Format( "<basefont color=#{0}>{1}</basefont>", MapHue( entry.Map ), entry.Name );
					AddHtml( 180, 20+(l*20), 150, 20, label, false, false );
					AddButton( 145, 20+(l*20), 4015, 4016, (i+offset), GumpButtonType.Reply, 0 );
					l++;
				}
			}
		}

		private string MapHue( Map map )
		{
			if ( map == null )
				return "101010";
			if ( map == Map.Felucca )
				return "FF0000";
			else if ( map == Map.Trammel )
				return "0000FF";
			else if ( map == Map.Ilshenar )
				return "008000";
			else if ( map == Map.Malas )
				return "FFFFFF";
			else
				return "FF00FF";
		}

		private static string[] m_Entries = new string[]
		{
			"Trammel", "Trammel Dungeons", "Felucca", "Felucca Dungeons",
			"Custom Areas", "Ilshenar", "Ilshenar Shrines", "Malas",
			"Tokuno", "Staff", "TerMur"
		};

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( info.ButtonID <= 0 || from == null || from.Deleted || m_TravelAtlas == null || m_TravelAtlas.Deleted )
				return;

			int id = info.ButtonID / 100;
			int count = info.ButtonID % 100;

			if ( id == 0 && count < 12 )
			{
				from.SendGump( new TravelAtlasGump( from, m_TravelAtlas, count ) );
				return;
			}

			//Invalid checks
			if ( id < 1 || id > 11 || (id == 10 && from.AccessLevel < AccessLevel.GameMaster) )
				id = 1;

			string name = m_Entries[id-1];

			TAEntry entry = GetEntry( name, count );

			bool gates = name == "Custom Areas";

			if ( entry == null )
				from.SendMessage( "Error: Invalid Button Response - No Map Entries" );
			else if ( ( (gates || name == "Felucca") && entry.Map == Map.Felucca && !m_TravelAtlas.Felucca) )
				from.SendMessage( "Error: Invalid Button Response - Felucca Disabled" );
			else if ( (gates || name == "Trammel") && entry.Map == Map.Trammel && (!m_TravelAtlas.Trammel || !m_Reds))
				from.SendMessage( "Error: Invalid Button Response - Trammel Disabled" );
			else if ( (name == "Ilshenar" ) && entry.Map == Map.Ilshenar && (!m_TravelAtlas.Ilshenar || !m_HasLBR || !m_Reds))
				from.SendMessage( "Error: Invalid Button Response - Ilshenar Disabled" );
			else if (entry.Map == Map.Malas && (!Core.AOS || !m_HasAOS || !m_TravelAtlas.Malas || !m_Reds))
				from.SendMessage( "Error: Invalid Button Response - Malas Disabled" );
			else if (entry.Map == Map.Tokuno && (!Core.SE || !m_HasSE || !m_TravelAtlas.Tokuno || !m_Reds))
				from.SendMessage( "Error: Invalid Button Response - Tokuno Disabled" );
			else if ( !from.InRange( m_TravelAtlas.GetWorldLocation(), 1 ) || from.Map != m_TravelAtlas.Map )
				from.SendLocalizedMessage( 1019002 ); // You are too far away to use the gate.
			else if ( from.Criminal )
				from.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
			else if ( Server.Spells.SpellHelper.CheckCombat( from ) )
				from.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
			else if ( from.Spell != null )
				from.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment.
			else if ( from.Map == entry.Map && from.InRange( entry.Destination, 1 ) )
				from.SendLocalizedMessage( 1019003 ); // You are already there.
			else
			{
				BaseCreature.TeleportPets( from, entry.Destination, entry.Map );

				from.Combatant = null;

				from.MoveToWorld( entry.Destination, entry.Map );

				if ( !from.Hidden || from.AccessLevel == AccessLevel.Player )
					Effects.PlaySound( entry.Destination, entry.Map, 0x1FE );
			}
		}
	}

	public class TAEntry
	{
		private string m_Name;
		private Point3D m_Destination;
		private Map m_Map;

		public string Name{ get{ return m_Name; } }
		public Point3D Destination{ get{ return m_Destination; } }
		public Map Map{ get{ return m_Map; } }

		public TAEntry( string name, Point3D p, Map map)
		{
			m_Name = name;
			m_Destination = p;
			m_Map = map;
		}
	}
}