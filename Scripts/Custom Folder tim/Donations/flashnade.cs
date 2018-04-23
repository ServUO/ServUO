//Bwahahah. imagine a set of all the effects.
using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections;
using System.Reflection;
using Server.Network;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
	public enum FlashType : byte
	{
		FadeOut,
		FadeIn,
		LightFlash,
		FadeInOut

	}
	public sealed class FlashEffect : Packet
	{

		public FlashEffect( FlashType flashType ) : base( 0x70, 28 )
		{
			m_Stream.Write( ( byte)4 );//effectType
			m_Stream.Write( ( int )0 );//fromSerial
			m_Stream.Write( ( int )0 );//toSerial
			m_Stream.Write( ( ushort )flashType );//in regular 0x70 ItemID is here
			m_Stream.Fill( 16 );
		}
	}
    public class flashnade : Item
    {
		private Timer m_CountDown = null;
		
		public Timer CountDown
		{ 
			get 
			{ 
				return m_CountDown; 
			} 
			set 
			{ 
				m_CountDown = value; 
			} 
		}
		
		
		
		private bool m_running = false;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public bool running 
		{ 
			get 
			{ 
				return m_running; 
			} 
			set 
			{ 
				m_running = value; 
			} 
		}
		
		private int m_range = 10;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int range 
		{ 
			get 
			{ 
				return m_range; 
			} 
			set 
			{ 
				m_range = value; 
			} 
		}
		
		[Constructable]
		public flashnade() : base( 3850 )	
		{
			Name = "FlashNade";
			Movable = true;
            Visible = true;
		
        }
		public override void OnDoubleClick( Mobile m )
		{
			if ( !IsChildOf( m.Backpack ) )
			{
				m.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				if(m_running == false)
					m.Target = new thrownade( this );
			}	
		}


        public flashnade(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
			
			writer.Write( m_running );
			writer.Write( (int)m_range );
			
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			m_running = reader.ReadBool();
			m_range = reader.ReadInt();
			
			if(m_running)
			this.Delete();
        }	
    }
	
	public class thrownade : Target
	{
		private flashnade m_flashnade;

		public thrownade( flashnade fn ) : base( -1, false, TargetFlags.None )
		{
			m_flashnade = fn;

		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			
			IPoint3D p = targeted as IPoint3D;

			if ( p == null )
				return;

			Map map = from.Map;

			if ( map == null )
				return;
				
			SpellHelper.GetSurfaceTop( ref p );
			
			m_flashnade.MoveToWorld( new Point3D( p ), map );
			m_flashnade.Movable = false;
			Effects.SendMovingEffect( from, m_flashnade, m_flashnade.ItemID, 7, 0, false, false, m_flashnade.Hue, 0 );
			m_flashnade.CountDown = new countdown_timer( from, m_flashnade, 5  ); 
			m_flashnade.CountDown.Start();
			m_flashnade.running = true;
			
		}
	}
	public class countdown_timer : Timer
	{
		private flashnade m_flashnade;
		private int m_count;
		private Mobile m_from;
		public countdown_timer(Mobile from, flashnade fn, int count ) : base( TimeSpan.FromSeconds( 1 )  )
		{
			m_flashnade = fn;
			m_count = count;
			m_from = from;
			Priority = TimerPriority.TwoFiftyMS;
		}

		protected override void OnTick()
		{
			if ( m_flashnade.Deleted )
			{
				Stop();
				return;
			}
			
			if(m_flashnade.running == false)
			{
				Stop();
				return;
			}
			
			if(m_count > 1)
			{
				m_count--;
				
				m_flashnade.PublicOverheadMessage( MessageType.Regular, 957, false, m_count.ToString() );
				
				m_flashnade.CountDown = new countdown_timer( m_from, m_flashnade, m_count ); 
				m_flashnade.CountDown.Start();
				Stop();
				return;
			}
			
			
			IPooledEnumerable eable = m_flashnade.Map.GetMobilesInRange( m_flashnade.Location, m_flashnade.range );
			foreach ( object o in eable )
			{
				if ( o is Mobile )
				{
					Mobile m = (Mobile)o;
					if(m_from != m)
					{
						NetState ns = m.NetState;
						
						Packet animPacket = Packet.Acquire( new FlashEffect( FlashType.FadeInOut ) );
						
						if( m!= null && m is PlayerMobile)
						ns.Send( animPacket );
					}
					else if(m_from == m)
					{
						NetState ns = m.NetState;
						
						Packet animPacket = Packet.Acquire( new FlashEffect( FlashType.LightFlash ) );
						
						if( m!= null && m is PlayerMobile)
						ns.Send( animPacket );
					}
				}
			}
			
			
			m_flashnade.Delete();
			Stop();
			return;
		}
	}
}