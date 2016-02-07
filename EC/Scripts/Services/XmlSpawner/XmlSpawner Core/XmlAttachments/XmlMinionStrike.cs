using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{
	public class XmlMinionStrike : XmlAttachment
	{
		private int m_Chance = 5;       // 5% chance by default
		private TimeSpan m_Refractory = TimeSpan.FromSeconds(5);    // 5 seconds default time between activations
		private DateTime m_EndTime;
		private string m_Minion = "Drake";
		private ArrayList MinionList = new ArrayList();

		[CommandProperty( AccessLevel.GameMaster )]
		public int Chance { get{ return m_Chance; } set { m_Chance = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Refractory { get { return m_Refractory; } set { m_Refractory  = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Minion { get { return m_Minion; } set { m_Minion  = value; } }


		// These are the various ways in which the message attachment can be constructed.
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlMinionStrike(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlMinionStrike(string minion)
		{
			m_Minion = minion;
			Expiration = TimeSpan.FromMinutes(30);
		}
        
		[Attachable]
		public XmlMinionStrike(string minion,int chance )
		{
			m_Chance = chance;
			m_Minion = minion;
			Expiration = TimeSpan.FromMinutes(30);
		}

		[Attachable]
		public XmlMinionStrike(string minion, int chance, double refractory)
		{
			m_Chance = chance;
			Refractory = TimeSpan.FromSeconds(refractory);
			Expiration = TimeSpan.FromMinutes(30);
			m_Minion = minion;

		}
        
		[Attachable]
		public XmlMinionStrike(string minion, int chance, double refractory, double expiresin)
		{
			m_Chance = chance;
			Expiration = TimeSpan.FromMinutes(expiresin);
			Refractory = TimeSpan.FromSeconds(refractory);
			m_Minion = minion;
		}
        
		public override void OnAttach()
		{
			base.OnAttach();

			if(AttachedTo is Mobile)
			{
				Mobile m = AttachedTo as Mobile;
				Effects.PlaySound( m, m.Map, 516 );
			}
		}


		// note that this method will be called when attached to either a mobile or a weapon
		// when attached to a weapon, only that weapon will do additional damage
		// when attached to a mobile, any weapon the mobile wields will do additional damage
		public override void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
		{

			// if it is still refractory then return
			if(DateTime.UtcNow < m_EndTime) return;

			if(m_Chance <= 0 || Utility.Random(100) > m_Chance)
				return;

			if(defender != null && attacker != null)
			{

				// spawn a minion
				object o = null;
				try
				{
					o = Activator.CreateInstance( SpawnerType.GetType(m_Minion) );
				} 
				catch{}

				if(o is BaseCreature)
				{
					BaseCreature b = o as BaseCreature;
					b.MoveToWorld(attacker.Location, attacker.Map);

					if(attacker is PlayerMobile)
					{
						b.Controlled = true;
						b.ControlMaster = attacker;
					}

					b.Combatant = defender;

					// add it to the list of controlled mobs
					MinionList.Add(b);
				} 
				else
				{
					if(o is Item)
						((Item)o).Delete();
					if(o is Mobile)
						((Mobile)o).Delete();
					// bad minion specification so delete the attachment
					Delete();
				}

				m_EndTime = DateTime.UtcNow + Refractory;
			}
		}
        
		public override void OnDelete()
		{
			base.OnDelete();

			if(AttachedTo is Mobile)
			{
				Mobile m = AttachedTo as Mobile;
				if(!m.Deleted)
				{
					Effects.PlaySound( m, m.Map, 958 );
				} 
			}

			// delete the minions
			foreach(BaseCreature b in MinionList)
			{
				if(b != null && !b.Deleted)
					b.Delete();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0
			writer.Write(m_Chance);
			writer.Write(m_Minion);
			writer.Write(m_Refractory);
			writer.Write(m_EndTime - DateTime.UtcNow);
			writer.Write(MinionList.Count);
			foreach(BaseCreature b in MinionList)
				writer.Write(b);

		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			m_Chance = reader.ReadInt();
			m_Minion = reader.ReadString();
			Refractory = reader.ReadTimeSpan();
			TimeSpan remaining = reader.ReadTimeSpan();
			m_EndTime = DateTime.UtcNow + remaining;
			int nminions = reader.ReadInt();
			for(int i = 0;i<nminions;i++)
			{
				BaseCreature b = (BaseCreature)reader.ReadMobile();
				MinionList.Add(b);
			}
		}

		public override string OnIdentify(Mobile from)
		{
			string msg = null;

			if(Expiration > TimeSpan.Zero)
			{
				msg = String.Format("Minion : {0} {1}% chance expires in {2} mins", m_Minion, Chance, Expiration.TotalMinutes);
			} 
			else
			{
				msg = String.Format("Minion : {0}",m_Minion);
			}

			if(Refractory > TimeSpan.Zero)
			{
				return String.Format("{0} : {1} secs between uses",msg, Refractory.TotalSeconds);
			} 
			else
				return msg;
		}
	}
}
