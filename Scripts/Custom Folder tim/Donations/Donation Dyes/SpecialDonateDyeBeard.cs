using System;
using System.Text;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
	public class SpecialDonateDyeBeard : Item
	{
		[Constructable]
		public SpecialDonateDyeBeard() : base( 0xEFC )
		{
			Weight = 11.0;
			LootType = LootType.Regular;
			Name = "Special Donate Beard Dye";
			Hue = 1258;
		}

		public SpecialDonateDyeBeard( Serial serial ) : base( serial )
		{
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				from.CloseGump( typeof( SpecialDonateDyeBeardGump ) );
				from.SendGump( new SpecialDonateDyeBeardGump( this ) );
			}
			else
			{
				from.LocalOverheadMessage( MessageType.Regular, 906, 1019045 ); // I can't reach that.
			}

		}
	}

	public class SpecialDonateDyeBeardGump : Gump
	{
		private SpecialDonateDyeBeard m_SpecialDonateDyeBeard;

		private class SpecialDonateDyeBeardEntry
		{
			private string m_Name;
			private int m_HueStart;
			private int m_HueCount;

			public string Name
			{
				get
				{
					return m_Name;
				}
			}

			public int HueStart
			{
				get
				{
					return m_HueStart;
				}
			}

			public int HueCount
			{
				get
				{
					return m_HueCount;
				}
			}

			public SpecialDonateDyeBeardEntry( string name, int hueStart, int hueCount )
			{
				m_Name = name;
				m_HueStart = hueStart;
				m_HueCount = hueCount;
			}
		}

		private static SpecialDonateDyeBeardEntry[] m_Entries = new SpecialDonateDyeBeardEntry[]
			{
                                new SpecialDonateDyeBeardEntry( "*****", 1161, 1 ),
                                new SpecialDonateDyeBeardEntry( "*****", 1281, 1 ),
				new SpecialDonateDyeBeardEntry( "*****", 1258, 1 ),
				new SpecialDonateDyeBeardEntry( "*****", 3, 2 ),
                                new SpecialDonateDyeBeardEntry( "*****", 1282, 1 ),
                                new SpecialDonateDyeBeardEntry( "*****", 1195, 1 ),
				new SpecialDonateDyeBeardEntry( "*****", 1151, 1 ),
				new SpecialDonateDyeBeardEntry( "*****", 1175, 1 ),
				new SpecialDonateDyeBeardEntry( "*****", 1155, 1 ),
                                new SpecialDonateDyeBeardEntry( "*****", 1286, 1 ),
				new SpecialDonateDyeBeardEntry( "*****", 1166, 1 ),
				new SpecialDonateDyeBeardEntry( "*****", 1194, 1 )
		};

		public SpecialDonateDyeBeardGump( SpecialDonateDyeBeard dye ) : base( 0, 0 )
		{
			m_SpecialDonateDyeBeard = dye;

			AddPage( 0 );
			AddBackground( 150, 60, 350, 358, 2600 );
			AddBackground( 170, 104, 110, 270, 5100 );
			AddHtmlLocalized( 230, 75, 200, 20, 1011013, false, false );		// Hair Color Selection Menu
			AddHtmlLocalized( 235, 380, 300, 20, 1011014, false, false );		// Dye my hair this color!
			AddButton( 200, 380, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );        // DYE HAIR

			for ( int i = 0; i < m_Entries.Length; ++i )
			{
				AddLabel( 180, 109 + (i * 22), m_Entries[i].HueStart - 1, m_Entries[i].Name );
				AddButton( 257, 110 + (i * 22), 5224, 5224, 0, GumpButtonType.Page, i + 1 );
			}

			for ( int i = 0; i < m_Entries.Length; ++i )
			{
				SpecialDonateDyeBeardEntry e = m_Entries[i];

				AddPage( i + 1 );

				for ( int j = 0; j < e.HueCount; ++j )
				{
					AddLabel( 328 + ((j / 16) * 80), 102 + ((j % 16) * 17), e.HueStart + j - 1, "*****" );
					AddRadio( 310 + ((j / 16) * 80), 102 + ((j % 16) * 17), 210, 211, false, (i * 100) + j );
				}
			}
		}

		public override void OnResponse( NetState from, RelayInfo info )
		{
			if ( m_SpecialDonateDyeBeard.Deleted )
				return;

			Mobile m = from.Mobile;
			int[] switches = info.Switches;

			if ( !m_SpecialDonateDyeBeard.IsChildOf( m.Backpack ) )
			{
				m.SendLocalizedMessage( 1042010 ); //You must have the objectin your backpack to use it.
				return;
			}
        }
    }
		}
