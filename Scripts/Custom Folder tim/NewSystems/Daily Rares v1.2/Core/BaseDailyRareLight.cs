using System;
using Server;

namespace Server.Items
{
	public abstract class BaseDailyRareLight : BaseDailyRare
	{
		private Timer m_Timer;
		private DateTime m_End;
		private bool m_BurntOut = false;
		private bool m_Burning = false;
		private bool m_Protected = false;
		private TimeSpan m_Duration = TimeSpan.Zero;

		public abstract int LitItemID{ get; }
		
		public virtual int UnlitItemID{ get { return 0; } }
		public virtual int BurntOutItemID{ get { return 0; } }
		
		public virtual int LitSound{ get { return 0x47; } }
		public virtual int UnlitSound{ get { return 0x3be; } }
		public virtual int BurntOutSound{ get { return 0x4b8; } }

		public static readonly bool Burnout = false;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Burning
		{
			get { return m_Burning; }
			set
			{
				if ( m_Burning != value )
				{
					m_Burning = true;
					DoTimer( m_Duration );
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BurntOut
		{
			get { return m_BurntOut; }
			set { m_BurntOut = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Protected
		{
			get { return m_Protected; }
			set { m_Protected = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Duration
		{
			get
			{
				if ( m_Duration != TimeSpan.Zero && m_Burning )
				{
					return m_End - DateTime.Now;
				}
				else
					return m_Duration;
			}

			set { m_Duration = value; }
		}

		[Constructable]
		public BaseDailyRareLight( int itemID ) : base( itemID )
		{
		}

		public BaseDailyRareLight( Serial serial ) : base( serial )
		{
		}

		public virtual void PlayLitSound()
		{
			if ( LitSound != 0 )
			{
				Point3D loc = GetWorldLocation();
				Effects.PlaySound( loc, Map, LitSound );
			}					
		}
		
		public virtual void PlayUnlitSound()
		{
			int sound = UnlitSound;

			if ( m_BurntOut && BurntOutSound != 0 )
				sound = BurntOutSound;

			
			if ( sound != 0 )
			{
				Point3D loc = GetWorldLocation();
				Effects.PlaySound( loc, Map, sound );
			}					
		}

		public virtual void Ignite()
		{
			if ( !m_BurntOut )
			{
				PlayLitSound();

				m_Burning = true;
				ItemID = LitItemID;
				DoTimer( m_Duration );
			}
		}

		public virtual void Douse()
		{
			m_Burning = false;
			
			if ( m_BurntOut && BurntOutItemID != 0 )
				ItemID = BurntOutItemID;
			else
				ItemID = UnlitItemID;

			if ( m_BurntOut )
				m_Duration = TimeSpan.Zero;
			else if ( m_Duration != TimeSpan.Zero )
				m_Duration = m_End - DateTime.Now;

			if ( m_Timer != null )
				m_Timer.Stop();

			PlayUnlitSound();
		}

		public virtual void Burn()
		{
			m_BurntOut = true;
			Douse();
		}

		private void DoTimer( TimeSpan delay )
		{
			m_Duration = delay;
			
			if ( m_Timer != null )
				m_Timer.Stop();

			if ( delay == TimeSpan.Zero )
				return;

			m_End = DateTime.Now + delay;

			m_Timer = new InternalTimer( this, delay );
			m_Timer.Start();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_BurntOut )
				return;

			if ( m_Protected && from.AccessLevel == AccessLevel.Player )
				return;

			if ( !from.InRange( this.GetWorldLocation(), 2 ) )
				return;

			if ( m_Burning )
			{
				if ( UnlitItemID != 0 )
					Douse();
			}
			else
			{
				Ignite();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write( m_BurntOut );
			writer.Write( m_Burning );
			writer.Write( m_Duration );
			writer.Write( m_Protected );

			if ( m_Burning && m_Duration != TimeSpan.Zero )
				writer.WriteDeltaTime( m_End );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_BurntOut = reader.ReadBool();
					m_Burning = reader.ReadBool();
					m_Duration = reader.ReadTimeSpan();
					m_Protected = reader.ReadBool();

					if ( m_Burning && m_Duration != TimeSpan.Zero )
						DoTimer( reader.ReadDeltaTime() - DateTime.Now );

					break;
				}
			}
		}

		private class InternalTimer : Timer
		{
			private BaseDailyRareLight m_Light;

			public InternalTimer( BaseDailyRareLight light, TimeSpan delay ) : base( delay )
			{
				m_Light = light;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				if ( m_Light != null && !m_Light.Deleted )
					m_Light.Burn();
			}
		}
	}
}
