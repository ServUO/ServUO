using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Gumps;
using Server.Multis;
using Server.ContextMenus;

namespace Server.Items
{

	public class PlentifulHarvest : Item, ISecurable
	{
		private static readonly TimeSpan m_SpawnTime = TimeSpan.FromHours( 12.0 );

		private int m_fruits;
		private DateTime m_NextSpawnTime;
		private SpawnTimer m_SpawnTimer;

		private SecureLevel m_Level;

		[CommandProperty( AccessLevel.GameMaster )]
		public SecureLevel Level
		{
			get{ return m_Level; }
			set{ m_Level = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int fruits
		{
			get{ return m_fruits; }
			set
			{
				if ( value >= 10 )
				{
					m_fruits = 10;

					StopSpawnTimer();
				}
				else
				{
					if ( value <= 0 )
						m_fruits = 0;
					else
						m_fruits = value;

					StartSpawnTimer( m_SpawnTime );
				}

				InvalidateProperties();
			}
		}

		[Constructable]
		public PlentifulHarvest() : base( 0x993 )
		{
			Name = "Plentiful Harvest";
			Weight = 10.0;
			LootType = LootType.Blessed;

			m_fruits = 0;
			StartSpawnTimer( TimeSpan.FromMinutes( 1.0 ) );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( " {0} Fruits To Harvest",  fruits.ToString() ); // fruits:  ~1_COUNT~
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			SetSecureLevelEntry.AddTo( from, this, list );
		}

		private void StartSpawnTimer( TimeSpan delay )
		{
			StopSpawnTimer();

			m_SpawnTimer = new SpawnTimer( this, delay );
			m_SpawnTimer.Start();

			m_NextSpawnTime = DateTime.Now + delay;
		}

		private void StopSpawnTimer()
		{
			if ( m_SpawnTimer != null )
			{
				m_SpawnTimer.Stop();
				m_SpawnTimer = null;
			}
		}

		private class SpawnTimer : Timer
		{
			private PlentifulHarvest m_PHarvest;

			public SpawnTimer( PlentifulHarvest PHarvest, TimeSpan delay ) : base( delay )
			{
				m_PHarvest = PHarvest;

				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				if ( m_PHarvest.Deleted )
					return;

				m_PHarvest.m_SpawnTimer = null;
				m_PHarvest.fruits++;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
			else if ( fruits > 0 )
			{
				from.AddToBackpack( new BagOfFruit( fruits ) );
				fruits = 0;
			}
		}

		public PlentifulHarvest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.WriteEncodedInt( (int) m_fruits );
			writer.WriteDeltaTime( (DateTime) m_NextSpawnTime );
			writer.WriteEncodedInt( (int) m_Level );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_fruits = reader.ReadEncodedInt();
			m_NextSpawnTime = reader.ReadDeltaTime();
			m_Level = (SecureLevel) reader.ReadEncodedInt();

			if ( m_fruits < 10 )
				StartSpawnTimer( m_NextSpawnTime - DateTime.Now );
		}
	}

	public class BagOfFruit : Bag
	{
		[Constructable]
		public BagOfFruit() : this( 1 )
		{
		}

		[Constructable]
		public BagOfFruit( int amount )
		{
			Name = "Bag of Fruit";			

			DropItem( new Apple	   ( amount ) );
			DropItem( new Banana	   ( amount ) );
			DropItem( new Grapes       ( amount ) );

		}

		public BagOfFruit( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}