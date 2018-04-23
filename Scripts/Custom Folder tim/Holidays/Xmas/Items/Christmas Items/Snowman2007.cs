using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[Flipable( 0x2328, 0x2329 )]
	public class Snowman2007 : Item, IDyable
	{
		public static string GetRandomTitle()
		{
			// All hail OSI staff
			string[] titles = new string[]
				{
					/*  1 */ "Crow",
					/*  2 */ "Destiny",
					/*  3 */ "Barak",
					/*  4 */ "Lurch",
					/*  5 */ "Lolth",
					/*  6 */ "Sunny",
					/*  7 */ "Alaraic",
					/*  8 */ "Kegan",
					/*  9 */ "Kordra",
					///* 10 */ "Earia",
					///* 11 */ "Foster",
					///* 12 */ "Gonzo",
					///* 13 */ "Haan",
					///* 14 */ "Halona",
					///* 15 */ "Hugo",
					///* 16 */ "Hyacinth",
					///* 17 */ "Imirian",
					///* 18 */ "Jinsol",
					///* 19 */ "Liciatia",
					///* 20 */ "Loewen",
					///* 21 */ "Loke",
					///* 22 */ "Magnus",
					///* 23 */ "Maleki",
					///* 24 */ "Morpheus",
					///* 25 */ "Obberron",
					///* 26 */ "Odee",
					///* 27 */ "Orbeus",
					///* 28 */ "Pax",
					///* 29 */ "Phields",
					///* 30 */ "Pigpen",
					///* 31 */ "Platinum",
					///* 32 */ "Polpol",
					///* 33 */ "Prume",
					///* 34 */ "Quinnly",
					///* 35 */ "Ragnarok",
					///* 36 */ "Rend",
					///* 37 */ "Roland",
					///* 38 */ "RyanM",
					///* 39 */ "Screach",
					///* 40 */ "Seraph",
					///* 41 */ "Silvani",
					///* 42 */ "Sherbear",
					///* 43 */ "SkyWalker",
					///* 44 */ "Snark",
					///* 45 */ "Sowl",
					///* 46 */ "Spada",
					///* 47 */ "Starblade",
					///* 48 */ "Tenacious",
					///* 49 */ "Tnez",
					///* 50 */ "Wasia",
					///* 51 */ "Zilo",
					///* 52 */ "Zippy",
					///* 53 */ "Zoer"
				};

			if ( titles.Length > 0 )
				return titles[Utility.Random( titles.Length )];

			return null;
		}

		private string m_Title;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Title
		{
			get{ return m_Title; }
			set{ m_Title = value; InvalidateProperties(); }
		}

		[Constructable]
		public Snowman2007() : this( Utility.RandomDyedHue(), GetRandomTitle() )
		{
		}

		[Constructable]
		public Snowman2007( int hue ) : this( hue, GetRandomTitle() )
		{
		}

		[Constructable]
		public Snowman2007( string title ) : this( Utility.RandomDyedHue(), title )
		{
		}

		[Constructable]
		public Snowman2007( int hue, string title ) : base( Utility.Random( 0x2328, 2 ) )
		{
			Weight = 10.0;
			Hue = hue;
			LootType = LootType.Blessed;

			m_Title = title;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Title != null )
				list.Add( 1062841, m_Title ); // ~1_NAME~ the Snowman
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public Snowman2007( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (string) m_Title );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Title = reader.ReadString();
					break;
				}
			}

			Utility.Intern( ref m_Title );
		}
	}
}