using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DecietBrazier : Item
	{
		public TimeSpan m_TimeDelay = TimeSpan.FromMinutes(2);
		public int m_iSpawnRange = 7;
		public int m_iEventRange = 6;
		
		private bool m_bShowWarning = true;
		private DateTime m_TimeToCanSpawn;

		private static Type[] m_tMonsters = new Type[]
		{
			typeof( HeadlessOne ),
			typeof( Wraith ),
			typeof( Ogre ),
			typeof( Troll ),
			typeof( Wyvern ),
			typeof( GiantSpider ),
			typeof( Slime ),
			typeof( Mongbat ),
			typeof( DireWolf ),
			typeof( Lizardman ),
			typeof( Orc ),
			typeof( Skeleton ),
			typeof( EarthElemental ),
			typeof( Ettin ),
			typeof( Ratman ),
			typeof( Gazer ),
			typeof( Gargoyle ),
			typeof( Harpy ),
			typeof( Lich ),
			typeof( Nightmare ),
			typeof( FireSteed ),
			typeof( Drake ),
			typeof( Dragon ),
			typeof( Zombie )
	};
		
		#region CommandProperties
		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Delay
		{
			get { return m_TimeDelay; }
			set { m_TimeDelay = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpawnRange
		{
			get { return m_iSpawnRange; }
			set { m_iSpawnRange = value; }
		}
		#endregion

		[Constructable]
		public DecietBrazier() : base( 0xE31 )
		{
			Movable = false;
			Light = LightType.Circle225;
			Weight = 20.0;
			SetDelay();
		}

		private void SetDelay()
		{
			m_TimeToCanSpawn = DateTime.Now + m_TimeDelay;
		}

		public Point3D GetSpawnPosition()
		{
			Map map = Map;

			if ( map == null )
				return Location;

			for ( int i = 0; i < 10; i++ )
			{
				int x = Location.X + (Utility.Random( (m_iSpawnRange * 2) + 1 ) - m_iSpawnRange);
				int y = Location.Y + (Utility.Random( (m_iSpawnRange * 2) + 1 ) - m_iSpawnRange);
				int z = Map.GetAverageZ( x, y );

				if ( Map.CanSpawnMobile( new Point2D( x, y ), this.Z ) )
					return new Point3D( x, y, this.Z );
				else if ( Map.CanSpawnMobile( new Point2D( x, y ), z ) )
					return new Point3D( x, y, z );
			}
			return this.Location;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}

			if( DateTime.Now > m_TimeToCanSpawn )
			{
				BaseCreature creature = (BaseCreature) Activator.CreateInstance( m_tMonsters[Utility.Random(m_tMonsters.Length)] );
				Point3D loc = GetSpawnPosition();
				creature.MoveToWorld(loc, Map);
				creature.Home = loc;

				Effects.SendLocationParticles( EffectItem.Create( creature.Location, creature.Map, EffectItem.DefaultDuration ), 0x3709, 10, 30, 5052 );
				Effects.PlaySound( creature.Location, creature.Map, 0x225 );

				m_bShowWarning = true;
				SetDelay();
			}
			else
				from.SendLocalizedMessage(500759); // The brazier fizzes and pops, but nothing seems to happen.
		}

		public override bool HandlesOnMovement{ get{ return true; } }

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if( m.Player )
			{
				bool inOldRange = Utility.InRange( oldLocation, Location, m_iEventRange );
				bool inNewRange = Utility.InRange( m.Location, Location, m_iEventRange );

				if ( inNewRange && !inOldRange && m_bShowWarning )
				{
					this.PublicOverheadMessage(Network.MessageType.Regular, 905, 500761); // Heed this warning well, and use this brazier at your own peril.
					m_bShowWarning = false;
				}
			}
		}

		public DecietBrazier( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			writer.Write( (bool) m_bShowWarning );
			writer.Write( (TimeSpan) m_TimeDelay );
			writer.Write( (int) m_iSpawnRange );
			writer.Write( (DateTime) m_TimeToCanSpawn );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			m_bShowWarning = reader.ReadBool();
			m_TimeDelay = reader.ReadTimeSpan();
			m_iSpawnRange = reader.ReadInt();
			m_TimeToCanSpawn = reader.ReadDateTime();

			SetDelay();
		}
	}
}