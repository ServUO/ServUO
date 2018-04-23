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

	public class CaptainBloodyRum : Item, ISecurable
	{
		private static readonly TimeSpan m_SpawnTime = TimeSpan.FromHours( 4.0 );

		private int m_bottles;
		private DateTime m_NextSpawnTime;
		private SpawnTimer m_SpawnTimer;

		private SecureLevel m_Level;

		public override int LabelNumber{ get{ return 1062913; } } // Rose of Trinsic

		[CommandProperty( AccessLevel.GameMaster )]
		public SecureLevel Level
		{
			get{ return m_Level; }
			set{ m_Level = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int bottles
		{
			get{ return m_bottles; }
			set
			{
				if ( value >= 10 )
				{
					m_bottles = 10;

					StopSpawnTimer();
				}
				else
				{
					if ( value <= 0 )
						m_bottles = 0;
					else
						m_bottles = value;

					StartSpawnTimer( m_SpawnTime );
				}

				InvalidateProperties();
			}
		}

		[Constructable]
		public CaptainBloodyRum() : base( 0x9C4 )
		{
			Name = "Captain Bloody's Rum";			
Weight = 1.0;
			LootType = LootType.Blessed;
			Hue = 1109;

			m_bottles = 10;
			StartSpawnTimer( TimeSpan.FromMinutes( 1.0 ) );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( bottles.ToString() ); // bottles:  ~1_COUNT~
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
			private CaptainBloodyRum m_Rose;

			public SpawnTimer( CaptainBloodyRum rose, TimeSpan delay ) : base( delay )
			{
				m_Rose = rose;

				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				if ( m_Rose.Deleted )
					return;

				m_Rose.m_SpawnTimer = null;
				m_Rose.bottles++;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
			else if ( bottles > 0 )
			{
				from.AddToBackpack( new CaptainBloodyRumbottle( bottles ) );
				bottles = 0;
			}
		}

		public CaptainBloodyRum( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.WriteEncodedInt( (int) m_bottles );
			writer.WriteDeltaTime( (DateTime) m_NextSpawnTime );
			writer.WriteEncodedInt( (int) m_Level );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_bottles = reader.ReadEncodedInt();
			m_NextSpawnTime = reader.ReadDeltaTime();
			m_Level = (SecureLevel) reader.ReadEncodedInt();

			if ( m_bottles < 10 )
				StartSpawnTimer( m_NextSpawnTime - DateTime.Now );
		}
	}

	public class CaptainBloodyRumbottle : Item
	{
		public override int LabelNumber{ get{ return 1062926; } } // bottle of the Rose of Trinsic

		[Constructable]
		public CaptainBloodyRumbottle() : this( 1 )
		{
		}

		[Constructable]
		public CaptainBloodyRumbottle( int amount ) : base( 0x99f )
		{
			Name = "A Bottle Of Captain Bloody's Rum";			
Stackable = true;
			Amount = amount;

			Weight = 1.0;
			Hue = 1109;
		}

		

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042038 ); // You must have the object in your backpack to use it.
			}
			else if ( from.GetStatMod( "CaptainBloodyRumbottle" ) != null )
			{
				from.SendLocalizedMessage( 1062927 ); // You have eaten one of these recently and eating another would provide no benefit.
			}
			else
			{
				from.PlaySound( 0x1EE );
				from.AddStatMod( new StatMod( StatType.Dex, "CaptainBloodyRumbottle", 25, TimeSpan.FromMinutes( 15.0 ) ) );

				Consume();
			}
		}

		public CaptainBloodyRumbottle( Serial serial ) : base( serial )
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