using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
	public class MoonglowMage : BaseRanger 
	{ 

		[Constructable] 
		public MoonglowMage() : base( AIType.AI_Mage, FightMode.Weakest, 10, 5, 0.1, 0.2 ) 
		{ 
			Title = "the Mage"; 

			AddItem( new Boots() );
			AddItem( new WizardsHat(1309) );
			AddItem( new Robe(1309) );
			AddItem( new LeatherGloves() );
			AddItem( new LeatherLegs());

			SetStr( 1000, 1000 );
			SetDex( 250, 250 );
			SetInt( 250, 250 );

			SetSkill( SkillName.MagicResist, 120.0,120.0 );
			SetSkill( SkillName.Magery, 120.0, 120.0 );
			SetSkill( SkillName.Tactics, 120.0, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0,120.0 );

			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
				
				AddItem( new FemaleLeatherChest() );
				


			}
			else 
			{ 
				Body = 400; 			
				Name = NameList.RandomName( "male" ); 
				
				AddItem( new LeatherChest());
				AddItem( new LeatherArms());
				


			}

			Utility.AssignRandomHair( this );
		}


		public MoonglowMage( Serial serial ) : base( serial ) 
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