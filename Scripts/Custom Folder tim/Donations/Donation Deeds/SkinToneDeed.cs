using System;
using System.Text;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
	public class SkinToneDeed : Item
	{
		public override string DefaultName{ get{ return "a skin toning deed"; } }

		[Constructable]
		public SkinToneDeed() : base( 5360 )
		{
			Weight = 1.0;
			LootType = LootType.Regular;
		}

		public SkinToneDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( SkinToneDeedGump ) );
				from.SendGump( new SkinToneDeedGump( this ) );
			}
			else
				from.SendLocalizedMessage( 1042010 );
	}

	public class SkinToneDeedGump : Gump
	{
		private SkinToneDeed m_SkinToneDeed;

		private class SkinToneDeedEntry
		{
			private string m_Name;
			private int m_HueStart;
			private int m_HueCount;

			public string Name{ get{ return m_Name; } }
			public int HueStart{ get{ return m_HueStart; } }
			public int HueCount{ get{ return m_HueCount; } }

			public SkinToneDeedEntry( string name, int hueStart, int hueCount )
			{
				m_Name = name;
				m_HueStart = hueStart;
				m_HueCount = hueCount;
			}
		}

		private static SkinToneDeedEntry[] m_Entries = new SkinToneDeedEntry[]
			{
				new SkinToneDeedEntry( "*****", 1002, 15 ),
				new SkinToneDeedEntry( "*****", 1016, 15 ),
				new SkinToneDeedEntry( "*****", 1031, 15 ),
				new SkinToneDeedEntry( "*****", 1046, 11 ),
		};

		public SkinToneDeedGump( SkinToneDeed dye ) : base( 0, 0 )
		{
			m_SkinToneDeed = dye;

			AddPage( 0 );
			AddBackground( 150, 60, 350, 358, 2600 );
			AddBackground( 170, 104, 110, 270, 5100 );
			AddHtml( 230, 75, 200, 20, @"Select a skin tone", false, false );
			AddHtml( 235, 380, 300, 20, @"Change my skin tone", false, false );
			AddButton( 200, 380, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );

			for ( int i = 0; i < m_Entries.Length; ++i )
			{
				AddLabel( 180, 109 + (i * 22), m_Entries[i].HueStart - 1, m_Entries[i].Name );
				AddButton( 257, 110 + (i * 22), 5224, 5224, 0, GumpButtonType.Page, i + 1 );
			}

			for ( int i = 0; i < m_Entries.Length; ++i )
			{
				SkinToneDeedEntry e = m_Entries[i];

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
			if ( m_SkinToneDeed == null || m_SkinToneDeed.Deleted )
				return;

			Mobile m = from.Mobile;
			int[] switches = info.Switches;

			if ( m_SkinToneDeed.IsChildOf( m.Backpack ) )
			{
				if ( info.ButtonID != 0 && switches.Length > 0 )
				{
					// To prevent this from being exploited, the hue is abstracted into an internal list

					int entryIndex = switches[0] / 100;
					int hueOffset = switches[0] % 100;

					if ( entryIndex >= 0 && entryIndex < m_Entries.Length )
					{
						SkinToneDeedEntry e = m_Entries[entryIndex];

						if ( hueOffset >= 0 && hueOffset < e.HueCount )
						{
							m_SkinToneDeed.Consume();
							int hue = e.HueStart + hueOffset;

							m.Hue = hue | 0x8000;

							m.SendMessage( "Your skin tone has changed." );
							m.PlaySound( 0x4E );
						}
					}
				}
			}
			else
				m.SendLocalizedMessage( 1042010 ); //You must have the objectin your backpack to use it.
		}
	}
}}