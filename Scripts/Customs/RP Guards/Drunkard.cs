using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
	public class Drunkard : BaseRanger
	{ 

		[Constructable] 
		public Drunkard() : base( AIType.AI_Melee, FightMode.Closest, 15, 1, 0.1, 0.2 ) 
		{ 
			Title = "the Drunkard"; 

			SetStr( 1500, 1500 );
			SetDex( 150, 150 );
			SetInt( 61, 75 );

			SetSkill( SkillName.MagicResist, 120.0, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0, 120.0 );
			SetSkill( SkillName.Tactics,120.0, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0,120.0 );

			AddItem( new Bandana(Utility.RandomBlueHue()));
			AddItem( new FancyShirt(Utility.RandomRedHue()));

			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
								
				AddItem( new Skirt(Utility.RandomRedHue()));
				AddItem( new ThighBoots());
				
			}
			else 
			{ 
				Body = 400; 			
				Name = NameList.RandomName( "male" ); 

				AddItem( new LongPants(Utility.RandomRedHue()));
				AddItem( new Boots());
				
			}

			Utility.AssignRandomHair( this );
		}

		public Drunkard( Serial serial ) : base( serial ) 
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