//#01 ordre alphabétique

using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Prompts;
using Server.Mobiles;
using Server.ContextMenus;

namespace Server.Items
{
	public class MonsterContractBook : Item
	{		
		private ArrayList m_Entries;

		public ArrayList Entries
		{
			get{ return m_Entries; }
		}

		[Constructable]
		public MonsterContractBook() : base( 0x2259 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			Hue = 1204;
			Name= "Hunter Bod Book";
			m_Entries = new ArrayList();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
				from.LocalOverheadMessage( Network.MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			else if ( m_Entries.Count == 0 )
				from.SendLocalizedMessage( 1062381 ); // The book is empty.
			else if ( from is PlayerMobile )
				from.SendGump( new MonsterContractBookGump( (PlayerMobile)from, this ) );
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is MonsterContract )
			{
				MonsterContract MC = dropped as MonsterContract;
				if ( !IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1062385 ); // You must have the book in your backpack to add deeds to it.
					return false;
				}
				else if ( m_Entries.Count < 20 )
				{
					m_Entries.Add( new MonsterContractEntry( MC.Monster, MC.AmountKilled, MC.AmountToKill, MC.Reward ) );
					
					m_Entries.Sort();//#01
					
					InvalidateProperties();

					from.SendLocalizedMessage( 1062386 ); // Deed added to book.

					if ( from is PlayerMobile )
						from.SendGump( new MonsterContractBookGump( (PlayerMobile)from, this ) );

					dropped.Delete();
					return true;
				}
				else
				{
					from.SendLocalizedMessage( 1062387 ); // The book is full of deeds.
					return false;
				}
			}

			from.SendMessage( "This is not a valid contract.");
			return false;
		}

		public MonsterContractBook( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.WriteEncodedInt( (int) m_Entries.Count );

			for ( int i = 0; i < m_Entries.Count; ++i )
			{
				MonsterContractEntry obj = m_Entries[i] as MonsterContractEntry;
				
				if( obj != null )
					obj.Serialize( writer );
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					int count = reader.ReadEncodedInt();

					m_Entries = new ArrayList( count );

					for ( int i = 0; i < count; ++i )
						m_Entries.Add( new MonsterContractEntry( reader ) );
					break;
				}
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1062344, m_Entries.Count.ToString() ); // Deeds in book: ~1_val~

		}
	}
}
