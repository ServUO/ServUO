using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
	public class SerpentsHoldMage : BaseRanger 
	{ 

		[Constructable] 
		public SerpentsHoldMage() : base( AIType.AI_Mage, FightMode.Weakest, 10, 5, 0.1, 0.2 ) 
		{ 
			Title = "the Mage"; 

			AddItem( new Boots() );
			AddItem( new WizardsHat(237) );
			AddItem( new Cloak(237) );
			LeatherGloves gloves = new LeatherGloves();
			gloves.Hue = 2301;
			AddItem(gloves);
			AddItem( new BodySash(237) );
			LeatherLegs legs = new LeatherLegs();
			legs.Hue = 2301;
			AddItem(legs);

			SetStr( 1000, 1000 );
			SetDex( 250, 250 );
			SetInt( 250, 250 );

			SetSkill( SkillName.MagicResist, 120.0,120.0 );
			SetSkill( SkillName.Magery, 120.0, 120.0 );
			SetSkill( SkillName.Tactics, 65.0, 87.5 );
			SetSkill( SkillName.Wrestling, 120.0,120.0 );

			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
				
				FemaleLeatherChest chest = new FemaleLeatherChest();
				chest.Hue = 2301;
				AddItem(chest);
				AddItem( new PlainDress(2301) );


			}
			else 
			{ 
				Body = 400; 			
				Name = NameList.RandomName( "male" ); 
				
				LeatherChest chest = new LeatherChest();
				chest.Hue = 2301;
				AddItem(chest);
				LeatherArms arms = new LeatherArms();
				arms.Hue = 2301;
				AddItem(arms);
				AddItem( new Robe(2301) );


			}

			Utility.AssignRandomHair( this );
		}


		public SerpentsHoldMage( Serial serial ) : base( serial ) 
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