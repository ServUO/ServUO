using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Items;
using Server.ContextMenus;

namespace Server.Mobiles
{
	public class PetChickenLizard : BaseCreature
	{				
		public override bool NoHouseRestrictions{ get{ return true; } }
		
		private DateTime m_Birth;
		
		[CommandProperty( AccessLevel.GameMaster)]
		public DateTime Birth
		{
			get { return m_Birth; }
			set { m_Birth = value; }
		} 
		
		[Constructable]
		public PetChickenLizard() : this( DateTime.MinValue, null, 0 )
		{
		}
		
		[Constructable]
		public PetChickenLizard( DateTime birth, string name, int hue ) : base( AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4 )
		{
			Name = "a pet chickenlizard";
            Title = "the ChickenLizard";
            Body = 716;
            BaseSoundID = 0x2EE;
			
			SetStr( 1, 5 );
			SetDex( 25, 30 );
			SetInt( 2 );
			
			SetHits( 1, Str );
			SetStam( 25, Dex );
			SetMana( 0 );

			SetResistance( ResistanceType.Physical, 2 );

			SetSkill( SkillName.MagicResist, 4 );
			SetSkill( SkillName.Tactics, 4 );
			SetSkill( SkillName.Wrestling, 4 );

			CantWalk = true;
			Blessed = true;
			
			if ( birth != DateTime.MinValue )
				m_Birth = birth;
			else
				m_Birth = DateTime.Now;
				
			if ( name != null )
				Name = name;
				
			if ( hue > 0 )
				Hue = hue;
		}
		
		public override void OnStatsQuery( Mobile from )
		{
			if ( from.Map == this.Map && Utility.InUpdateRange( this, from ) && from.CanSee( this ) )
			{
				BaseHouse house = BaseHouse.FindHouseAt( this );

				if ( house != null && house.IsCoOwner( from ) )
                    from.SendMessage("As the house owner, you may rename this ChickenLizard"); // As the house owner, you may rename this ChickenLizard.
					
				from.Send( new Server.Network.MobileStatus( from, this ) );
			}
		}

        public PetChickenLizard(Serial serial)
            : base(serial)
		{
		}				
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			int weeks = GetWeeks( m_Birth );
			
			if ( weeks == 1 )
				list.Add( 1072626 ); // 1 week old
			else if ( weeks > 1 )
				list.Add( 1072627, weeks.ToString() ); // ~1_AGE~ weeks old
		}
		
		public override bool CanBeRenamedBy( Mobile from )
		{
			if ( (int) from.AccessLevel > (int) AccessLevel.Player )
				return true;
		
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsCoOwner( from ) )
				return true;
			else
				return false;
		}	
				
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			
			writer.Write( (DateTime) m_Birth );
		}
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies; } }

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			m_Birth = reader.ReadDateTime();
		}
		
		public static int GetWeeks( DateTime birth )
		{
			TimeSpan span = DateTime.Now - birth;
			
			return (int) ( span.TotalDays / 7 );		
		}
	}	
}