using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
	public class DeluciaGuard : BaseRanger
	{ 

		[Constructable] 
		public DeluciaGuard() : base( AIType.AI_Melee, FightMode.Closest, 15, 1, 0.1, 0.2 ) 
		{ 
			Title = "the Fencer"; 

			SetStr( 1750, 1750 );
			SetDex( 150, 150 );
			SetInt( 61, 75 );

			SetSkill( SkillName.MagicResist, 120.0, 120.0 );
			SetSkill( SkillName.Fencing, 120.0, 120.0 );
			SetSkill( SkillName.Tactics,120.0, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0,120.0 );

			AddItem( new WarFork() );
			AddItem( new BronzeShield() );
			AddItem( new Boots() );
			AddItem( new NorseHelm() );
			AddItem( new Cloak(667) );
			AddItem( new RingmailGloves() );
			AddItem( new BodySash(653) );

			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
								
				AddItem( new LeatherSkirt() );
				AddItem( new FemalePlateChest() );
				
			}
			else 
			{ 
				Body = 400; 			
				Name = NameList.RandomName( "male" ); 
				
				AddItem( new RingmailLegs());
				AddItem( new RingmailChest());
				AddItem( new RingmailArms());
				AddItem( new StuddedGorget());	
			
			}

			Utility.AssignRandomHair( this );
		}

		public DeluciaGuard( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 
		} 
	} 
}   