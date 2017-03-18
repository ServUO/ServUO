using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Network;
using Server.Gumps;
using Server.Multis;

namespace Server.Items
{
	public class Beehive : Item, ISecurable
	{
		private static readonly TimeSpan m_SpawnTime = TimeSpan.FromHours( 4.0 );

		private int m_Honeypots;
		private DateTime m_NextSpawnTime;
		private SpawnTimer m_SpawnTimer;
		private Item m_Bees;

		private SecureLevel m_Level;

		public override bool ForceShowProperties { get { return true; } }

		public override int LabelNumber { get { return 1080263; } } // Beehive

		[CommandProperty( AccessLevel.GameMaster )]
		public SecureLevel Level { get { return m_Level; } set { m_Level = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Honeypots
		{
			get { return m_Honeypots; }
			set
			{
				if ( value >= 10 )
				{
					m_Honeypots = 10;

					StopSpawnTimer();
				}
				else
				{
					if ( value <= 0 )
						m_Honeypots = 0;
					else
						m_Honeypots = value;

					StartSpawnTimer( m_SpawnTime );
				}

				InvalidateProperties();
			}
		}

		[Constructable]
		public Beehive()
			: base( 2330 )
		{
			Weight = 10.0;

			m_Honeypots = 0;
			StartSpawnTimer( TimeSpan.FromMinutes( 1.0 ) );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1080202, Honeypots.ToString() ); // Honeypots: ~1_COUNT~
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

			m_NextSpawnTime = DateTime.UtcNow + delay;
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
			private Beehive m_Beehive;

			public SpawnTimer( Beehive beehive, TimeSpan delay )
				: base( delay )
			{
				m_Beehive = beehive;

			}

			protected override void OnTick()
			{
				if ( m_Beehive.Deleted )
				{
					return;
				}

				m_Beehive.m_SpawnTimer = null;
				m_Beehive.Honeypots++;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
			else if ( Honeypots > 0 )
			{
				from.AddToBackpack( new JarHoney() );
				Honeypots--;
			}
		}

		public override void OnLockDownChange()
		{
			if ( IsLockedDown )
			{
				if ( m_Bees == null )
				{
					m_Bees = new Item( 2331 );
					m_Bees.MoveToWorld( Location, Map );
					m_Bees.Movable = false;
				}
			}
			else
			{
				if ( m_Bees != null )
				{
					m_Bees.Delete();
					m_Bees = null;
				}
			}
		}

		public Beehive( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 1 ); // version

			writer.Write( (Item) m_Bees );
			writer.WriteEncodedInt( (int) m_Honeypots );
			writer.WriteDeltaTime( (DateTime) m_NextSpawnTime );
			writer.WriteEncodedInt( (int) m_Level );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 1:
					{
						m_Bees = reader.ReadItem();
						goto case 0;
					}
				case 0:
					{
						m_Honeypots = reader.ReadEncodedInt();
						m_NextSpawnTime = reader.ReadDeltaTime();
						m_Level = (SecureLevel) reader.ReadEncodedInt();
						break;
					}
			}

			if ( m_Honeypots < 10 )
				StartSpawnTimer( m_NextSpawnTime - DateTime.UtcNow );
		}
	}
}
