using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using System.Collections;

namespace Server.Engines.XmlSpawner2
{
	public class XmlEnemyMastery : XmlAttachment
	{
		private int m_Chance = 20;       // 20% chance by default
		private int m_PercentIncrease = 50;
		private string m_Enemy;
		private Type m_EnemyType;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Chance { get{ return m_Chance; } set { m_Chance = value; } }
        
		[CommandProperty( AccessLevel.GameMaster )]
		public int PercentIncrease { get{ return m_PercentIncrease; } set { m_PercentIncrease = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Enemy 
		{ 
			get { return m_Enemy; }
			set 
			{ 
				m_Enemy  = value; 
				// look up the type
				m_EnemyType = SpawnerType.GetType(m_Enemy);
			}
		}


		// These are the various ways in which the message attachment can be constructed.
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlEnemyMastery(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlEnemyMastery(string enemy)
		{
			Enemy = enemy;
		}
        
		[Attachable]
		public XmlEnemyMastery(string enemy,int increase )
		{
			m_PercentIncrease = increase;
			Enemy = enemy;
		}

		[Attachable]
		public XmlEnemyMastery(string enemy,int chance, int increase )
		{
			m_Chance = chance;
			m_PercentIncrease = increase;
			Enemy = enemy;
		}

		[Attachable]
		public XmlEnemyMastery(string enemy, int chance, int increase, double expiresin)
		{
			m_Chance = chance;
			m_PercentIncrease = increase;
			Expiration = TimeSpan.FromMinutes(expiresin);
			Enemy = enemy;
		}
        
		public override void OnAttach()
		{
			base.OnAttach();

			if(AttachedTo is Mobile)
			{
				Mobile m = AttachedTo as Mobile;
				Effects.PlaySound( m, m.Map, 516 );
				m.SendMessage(String.Format("You gain the power of Enemy Mastery over {0}",Enemy));
			}
		}


		// note that this method will be called when attached to either a mobile or a weapon
		// when attached to a weapon, only that weapon will do additional damage
		// when attached to a mobile, any weapon the mobile wields will do additional damage
		public override void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
		{
			if(m_Chance <= 0 || Utility.Random(100) > m_Chance)
				return;

			if(defender != null && attacker != null && m_EnemyType != null)
			{

				// is the defender the correct type?
				if(defender.GetType() == m_EnemyType || defender.GetType().IsSubclassOf(m_EnemyType))
				{
					defender.Damage( (int) (damageGiven*PercentIncrease/100), attacker );
				}
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
					m.SendMessage(String.Format("Your power of Enemy Mastery over {0} fades..",Enemy));
				} 
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 );
			// version 0
			writer.Write(m_PercentIncrease);
			writer.Write(m_Chance);
			writer.Write(m_Enemy);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			// version 0
			m_PercentIncrease = reader.ReadInt();
			m_Chance = reader.ReadInt();
			Enemy = reader.ReadString();
		}

		public override string OnIdentify(Mobile from)
		{
			string msg = null;

			if(Expiration > TimeSpan.Zero)
			{
				msg = String.Format("Enemy Mastery : +{3}% damage vs {0}, {1}%, hitchance expires in {2} mins", m_Enemy, Chance, Expiration.TotalMinutes, PercentIncrease);
			} 
			else
			{
				msg = String.Format("Enemy Mastery : +{2}% damage vs {0}, {1}% hitchance",m_Enemy, Chance, PercentIncrease);
			}

			return msg;
		}
	}
}
