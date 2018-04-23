using System;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
	public class EarringOfProtectionGump : Gump
	{
		//public override int TypeID { get { return 0x2341; } }

		private Item m_Token;

		public EarringOfProtectionGump( Item token )
			: base( 60, 36 )
		{
			m_Token = token;

			AddPage( 0 );

			AddBackground( 0, 0, 273, 324, 0x13BE );
			AddImageTiled( 10, 10, 253, 20, 0xA40 );
			AddImageTiled( 10, 40, 253, 244, 0xA40 );
			AddImageTiled( 10, 294, 253, 20, 0xA40 );
			AddAlphaRegion( 10, 10, 253, 304 );

			AddButton( 10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 296, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL

			AddHtmlLocalized( 14, 12, 273, 20, 1071175, 0x7FFF, false, false ); // Please select your item

			AddPage( 1 );

			for ( int i = 0, y = 49; i < Earrings.Length; i++, y += 24 )
			{
				AddButton( 19, y, 0x845, 0x846, 100 + i, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 44, y - 2, 213, 20, 1071091 + i, 0x7FFF, false, false );
			}
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( m_Token.Deleted || !m_Token.IsChildOf( sender.Mobile.Backpack ) )
				return;

			int idx = info.ButtonID - 100;

			if ( idx >= 0 && idx < Earrings.Length )
			{
				EarringEntry entry = Earrings[idx];

				Item earrings = (Item) Activator.CreateInstance( entry.Type );

				sender.Mobile.PlaceInBackpack( earrings );

				m_Token.Delete();
			}
		}

		private static EarringEntry[] Earrings = new EarringEntry[]
			{
				new EarringEntry( 1071091, typeof( EarringOfProtectionPhysical ) ),
				new EarringEntry( 1071092, typeof( EarringOfProtectionFire ) ),
				new EarringEntry( 1071093, typeof( EarringOfProtectionCold ) ),
				new EarringEntry( 1071094, typeof( EarringOfProtectionPoison ) ),
				new EarringEntry( 1071095, typeof( EarringOfProtectionEnergy ) ),
			};

		private class EarringEntry
		{
			private int m_Cliloc;
			private Type m_Type;

			public int Cliloc { get { return m_Cliloc; } }
			public Type Type { get { return m_Type; } }

			public EarringEntry( int cliloc, Type type )
			{
				m_Cliloc = cliloc;
				m_Type = type;
			}
		}
	}

	public class EarringOfProtectionPhysical : GoldEarrings
	{
		public override int LabelNumber { get { return 1071091; } } // Earring of Protection (Physical)

		[Constructable]
		public EarringOfProtectionPhysical()
		{
			Weight = 1.0;
			Hue = 0;

			Resistances.Physical = 2;

			LootType = LootType.Blessed;
		}

		public EarringOfProtectionPhysical( Serial serial )
			: base( serial )
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

			/*int version = */
			reader.ReadInt();
		}
	}

	public class EarringOfProtectionFire : GoldEarrings
	{
		public override int LabelNumber { get { return 1071092; } } // Earring of Protection (Fire)

		[Constructable]
		public EarringOfProtectionFire()
		{
			Weight = 1.0;
			Hue = 1260;

			Resistances.Fire = 2;

			LootType = LootType.Blessed;
		}

		public EarringOfProtectionFire( Serial serial )
			: base( serial )
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

			/*int version = */
			reader.ReadInt();
		}
	}

	public class EarringOfProtectionCold : GoldEarrings
	{
		public override int LabelNumber { get { return 1071093; } } // Earring of Protection (Cold)

		[Constructable]
		public EarringOfProtectionCold()
		{
			Weight = 1.0;
			Hue = 1266;

			Resistances.Cold = 2;

			LootType = LootType.Blessed;
		}

		public EarringOfProtectionCold( Serial serial )
			: base( serial )
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

			/*int version = */
			reader.ReadInt();
		}
	}

	public class EarringOfProtectionPoison : GoldEarrings
	{
		public override int LabelNumber { get { return 1071094; } } // Earring of Protection (Poison)

		[Constructable]
		public EarringOfProtectionPoison()
		{
			Weight = 1.0;
			Hue = 1272;

			Resistances.Poison = 2;

			LootType = LootType.Blessed;
		}

		public EarringOfProtectionPoison( Serial serial )
			: base( serial )
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

			/*int version = */
			reader.ReadInt();
		}
	}

	public class EarringOfProtectionEnergy : GoldEarrings
	{
		public override int LabelNumber { get { return 1071095; } } // Earring of Protection (Energy)

		[Constructable]
		public EarringOfProtectionEnergy()
		{
			Weight = 1.0;
			Hue = 1278;

			Resistances.Energy = 2;

			LootType = LootType.Blessed;
		}

		public EarringOfProtectionEnergy( Serial serial )
			: base( serial )
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

			/*int version = */
			reader.ReadInt();
		}
	}
}