using System;
using Server;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class MonsterContractEntry : IComparable //#01
	{
		public int CompareTo( object obj )
		{
			return MonsterContractType.Get[Monster].Name.CompareTo( MonsterContractType.Get[((MonsterContractEntry)obj).Monster].Name);
		}
			
		private int m_monster;
		private int reward;
		private int m_amount;
		private int m_killed;
		
		public int Monster
		{
			get{ return m_monster; }
			set{ m_monster = value; }
		}
		
		public int Reward
		{
			get{ return reward; }
			set{ reward = value; }
		}
		
		public int AmountToKill
		{
			get{ return m_amount; }
			set{ m_amount = value; }
		}
		
		public int AmountKilled
		{
			get{ return m_killed; }
			set{ m_killed = value; }
		}
		
		public MonsterContractEntry( int monster, int ak, int atk, int gpreward )
		{
			Monster = monster;
			AmountToKill = atk;
			AmountKilled = ak;
			Reward = gpreward;
		}
		
		
		public MonsterContractEntry( GenericReader reader )
		{ 
			Deserialize( reader );
		} 

		public void Serialize( GenericWriter writer ) 
		{ 
			writer.Write( (int) 0 ); // version 
		
			writer.Write( m_monster );
			writer.Write( reward );
			writer.Write( m_amount );
			writer.Write( m_killed );
		}

		public void Deserialize( GenericReader reader ) 
		{ 
			int version = reader.ReadInt(); 
			
			m_monster = reader.ReadInt();
			reward = reader.ReadInt();
			m_amount = reader.ReadInt();
			m_killed = reader.ReadInt();

            if ( m_killed > m_amount )
                m_killed = m_amount;
		}
	}
}
