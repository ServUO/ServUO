using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
	public class CoveArcher : BaseRanger
	{ 


		[Constructable] 
		public CoveArcher() : base( AIType.AI_Archer, FightMode.Weakest, 15, 5, 0.1, 0.2 ) 
		{ 
			Title = "the Archer"; 

			AddItem( new Bow() );
			AddItem( new Boots() );
			AddItem( new Bandana(632) );
			AddItem( new Cloak(632) );
			AddItem( new StuddedGloves() );
			AddItem( new BodySash(1309) );

			SetStr( 1200, 1200 );
			SetDex( 250, 250 );
			SetInt( 61, 75 );

			SetSkill( SkillName.Anatomy, 120.0, 120.0 );
			SetSkill( SkillName.Archery, 120.0, 120.0 );
			SetSkill( SkillName.Tactics, 120.0, 120.0 );
			SetSkill( SkillName.MagicResist, 120.0, 120.0 );

			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
				
				AddItem( new LeatherSkirt() );
				AddItem( new FemaleStuddedChest() );
				
			}
			else 
			{ 
				Body = 400; 			
				Name = NameList.RandomName( "male" );

				AddItem( new StuddedChest());
				AddItem( new StuddedLegs());
				AddItem( new StuddedArms());

			}
			
			Utility.AssignRandomHair( this );
		}

		public CoveArcher( Serial serial ) : base( serial ) 
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