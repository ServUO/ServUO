using System;
using Server;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	[Flipable( 0x14EF, 0x14F0 )]
	public class MonsterContract : Item
	{
		private int m_monster;
		private int reward;
		private int m_amount;
		private int m_killed;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Monster
		{
			get{ return m_monster; }
			set{ m_monster = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Reward
		{
			get{ return reward; }
			set{ reward = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int AmountToKill
		{
			get{ return m_amount; }
			set{ m_amount = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int AmountKilled
		{
			get{ return m_killed; }
			set{ m_killed = value; }
		}
		
		[Constructable]
		public MonsterContract() : base( 0x14EF )
		{
			Weight = 1;
			Movable = true;
			LootType = LootType.Blessed;
			Monster = MonsterContractType.Random();
			AmountToKill = Utility.Random( 10 ) + 5;
			int price = MonsterContractType.Get[Monster].Rarety;
			double scalar = Utility.RandomDouble();
			if(scalar < 0.6)scalar = 0.6;
			Reward = (int)((price * (price/2)) * scalar) * AmountToKill;
			Name = "Contract: " + AmountToKill + " " + MonsterContractType.Get[Monster].Name;
			AmountKilled = 0;
		}
		
		[Constructable]
		public MonsterContract( int monster, int atk, int gpreward ) : base( 0x14F0 )
		{
			Weight = 1;
			Movable = true;
			LootType = LootType.Blessed;
			Monster = monster;
			AmountToKill = atk;
			Reward = gpreward;
			Name = "Contract: " + AmountToKill + " " + MonsterContractType.Get[Monster].Name;
			AmountKilled = 0;
		}
		
		[Constructable]
		public MonsterContract( int monster, int ak, int atk, int gpreward ) : this( monster,atk,gpreward )
		{
			AmountKilled = ak;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if( IsChildOf( from.Backpack ) )
			{
				from.SendGump( new MonsterContractGump( from, this ) );
			}
			else
			{
				from.SendLocalizedMessage( 1047012 ); // This contract must be in your backpack to use it
			}
		}
		
		public MonsterContract( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			
			writer.Write( m_monster );
			writer.Write( reward );
			writer.Write( m_amount );
			writer.Write( m_killed );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			m_monster = reader.ReadInt();
			reward = reader.ReadInt();
			m_amount = reader.ReadInt();
			m_killed = reader.ReadInt();
			LootType = LootType.Blessed;
		}
	}
}
