#region AuthorHeader
//
//	Claim System version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Xanthos.Utilities;

namespace Xanthos.Claim
{
    public class LootBag : Bag
    {
		private List<Type> m_TypesToLoot;
		private object m_SyncRoot;

		public List<Type> TypesToLoot
		{
			get { return m_TypesToLoot; }
		}

		public object SyncRoot
		{
			get { return m_SyncRoot; }
		}

		[Constructable]
		public LootBag() : base()
		{
			m_SyncRoot = new object();
			Weight = 0.0;
			Hue = 1266;
			Name = "loot bag";
			LootType = LootType.Blessed;
			//LootType = ClaimConfig.LootBagBlessed ? LootType.Blessed : LootType.Regular;
			

			m_TypesToLoot = new List<Type>( ClaimConfig.TypesToLoot );
			PruneNulls( m_TypesToLoot );
		}

		public LootBag( Serial serial ) : base( serial )
		{
			m_SyncRoot = new object();
		}

		public static bool TypeIsLootable( Container bag, Item item )
		{
			if ( item == null )
				return false;

			if ( ClaimConfig.LootArtifacts && Misc.IsArtifact( item ) )
				return true;

			Type itemType = item.GetType();

			if ( null != bag && bag is LootBag && !bag.Deleted )
			{
				lock ( ( (LootBag)bag ).SyncRoot )
				{
					foreach ( Type lootType in ( (LootBag)bag ).TypesToLoot )
					{
						if ( null != lootType && ( itemType == lootType || itemType.IsSubclassOf( lootType ) ) )
							return true;
					}
				}
			}
			else
			{
				foreach ( Type lootType in ClaimConfig.TypesToLoot )
				{
					if ( null != lootType && ( itemType == lootType || itemType.IsSubclassOf( lootType ) ) )
						return true;
				}
			}

			return false;
		}

		private void PruneNulls( List<Type> list )
		{
			// Clean out any null entries
			for ( int i = 0; i < list.Count;  )
			{
				if ( null == list[ i ] )
					list.RemoveAt( i );
				else
					i++;
			}
		}

		internal void AddType( Type type )
		{
			lock ( m_SyncRoot )
				m_TypesToLoot.Add( type );
		}

		internal void RemoveTypeAt( int index )
		{
			lock ( m_SyncRoot )
				m_TypesToLoot.RemoveAt( index );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			
			PruneNulls( m_TypesToLoot );

			// Version 1
			writer.Write( m_TypesToLoot.Count );
			foreach ( Type type in m_TypesToLoot )
			{
				writer.Write( type.FullName );
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
					break;
				case 1:
					int count = reader.ReadInt();
					m_TypesToLoot = new List<Type>( count );

					for ( int i = count; i > 0; i-- )
					{
						try
						{
							String str = reader.ReadString();
							Type type = Type.GetType( str );

							if ( null != type )
								m_TypesToLoot.Add( type );
						}
						catch { }
					}
					break;
			}
		}
	}
}
