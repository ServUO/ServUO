using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
	public class MoonglowGuard : BaseRanger
	{ 

		[Constructable] 
		public MoonglowGuard() : base( AIType.AI_Melee, FightMode.Closest, 15, 1, 0.1, 0.2 ) 
		{ 
			Title = "the Swordsman"; 

			SetStr( 1500, 1500 );
			SetDex( 150, 150 );
			SetInt( 61, 75 );

			SetSkill( SkillName.MagicResist, 120.0, 120.0 );
			SetSkill( SkillName.Swords, 120.0, 120.0 );
			SetSkill( SkillName.Tactics,120.0, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0,120.0 );

			AddItem( new Broadsword() );
			AddItem( new MetalShield() );
			AddItem( new StuddedGloves() );
			AddItem( new Boots() );
			AddItem( new Bandana(1309) );
			AddItem( new Robe(1309) );
			AddItem( new StuddedChest());
			AddItem( new StuddedLegs());
			AddItem( new StuddedArms());

			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
								
				//AddItem( new FemalePlateChest() );
				
			}
			else 
			{ 
				Body = 400; 			
				Name = NameList.RandomName( "male" ); 

				//AddItem(new PlateChest());
				
			}

			Utility.AssignRandomHair( this );
		}

		public MoonglowGuard( Serial serial ) : base( serial ) 
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