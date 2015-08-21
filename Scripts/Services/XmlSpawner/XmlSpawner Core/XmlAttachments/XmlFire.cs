using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;

namespace Server.Engines.XmlSpawner2
{
	public class XmlFire : XmlAttachment
	{
		private int m_Damage = 0;
		private TimeSpan m_Refractory = TimeSpan.FromSeconds(5);    // 5 seconds default time between activations
		private DateTime m_EndTime;
		private int proximityrange = 5;                 // default movement activation from 5 tiles away

		[CommandProperty( AccessLevel.GameMaster )]
		public int Damage { get{ return m_Damage; } set { m_Damage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Refractory { get { return m_Refractory; } set { m_Refractory  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range { get { return proximityrange; } set { proximityrange  = value; } }

		// These are the various ways in which the message attachment can be constructed.
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlFire(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlFire(int damage)
		{
			m_Damage = damage;
		}

		[Attachable]
		public XmlFire(int damage, double refractory)
		{
			m_Damage = damage;
			Refractory = TimeSpan.FromSeconds(refractory);

		}
        
		[Attachable]
		public XmlFire(int damage, double refractory, double expiresin)
		{
			m_Damage = damage;
			Expiration = TimeSpan.FromMinutes(expiresin);
			Refractory = TimeSpan.FromSeconds(refractory);
		}


		// note that this method will be called when attached to either a mobile or a weapon
		// when attached to a weapon, only that weapon will do additional damage
		// when attached to a mobile, any weapon the mobile wields will do additional damage
		public override void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
		{
			// if it is still refractory then return
			if(DateTime.UtcNow < m_EndTime) return;

			int damage = 0;

			if(m_Damage > 0)
				damage = Utility.Random(m_Damage);

			if(defender != null && attacker != null && damage > 0)
			{
				attacker.MovingParticles( defender, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160 );
				attacker.PlaySound( 0x15E );

				SpellHelper.Damage( TimeSpan.Zero, defender, attacker, damage, 0, 100, 0, 0, 0 );
			    
				m_EndTime = DateTime.UtcNow + Refractory;
			}
		}
        
		public override bool HandlesOnMovement { get { return true; } }

		public override void OnMovement(MovementEventArgs e )
		{
			base.OnMovement(e);
		    
			if(e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player) return;

			if(AttachedTo is Item && (((Item)AttachedTo).Parent == null) && Utility.InRange( e.Mobile.Location, ((Item)AttachedTo).Location, proximityrange ))
			{
				OnTrigger(null, e.Mobile);
			} 
			else
				return;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 1 );
			// version 1
			writer.Write(proximityrange);
			// version 0
			writer.Write(m_Damage);
			writer.Write(m_Refractory);
			writer.Write(m_EndTime - DateTime.UtcNow);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch(version)
			{
				case 1:
					Range = reader.ReadInt();
					goto case 0;
				case 0:
					// version 0
					m_Damage = reader.ReadInt();
					Refractory = reader.ReadTimeSpan();
					TimeSpan remaining = reader.ReadTimeSpan();
					m_EndTime = DateTime.UtcNow + remaining;
					break;
			}
		}

		public override string OnIdentify(Mobile from)
		{
			string msg = null;

			if(Expiration > TimeSpan.Zero)
			{
				msg = String.Format("Fire Damage {0} expires in {1} mins", m_Damage, Expiration.TotalMinutes);
			} 
			else
			{
				msg = String.Format("Fire Damage {0}",m_Damage);
			}
            
			if(Refractory > TimeSpan.Zero)
			{
				return String.Format("{0} : {1} secs between uses",msg, Refractory.TotalSeconds);
			} 
			else
				return msg;
		}
		
		public override void OnTrigger(object activator, Mobile m)
		{
			if(m == null ) return;

			// if it is still refractory then return
			if(DateTime.UtcNow < m_EndTime) return;

			int damage = 0;

			if(m_Damage > 0)
				damage = Utility.Random(m_Damage);

			if(damage > 0)
			{
				m.MovingParticles( m, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160 );
				m.PlaySound( 0x15E );
				SpellHelper.Damage( TimeSpan.Zero, m, damage, 0, 100, 0, 0, 0 );
			}

			m_EndTime = DateTime.UtcNow + Refractory;

		}    
	}
}
