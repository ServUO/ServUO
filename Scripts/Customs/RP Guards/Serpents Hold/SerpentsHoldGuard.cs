using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
	public class SerpentsHoldGuard : BaseRanger
	{ 

		[Constructable] 
		public SerpentsHoldGuard() : base( AIType.AI_Melee, FightMode.Closest, 15, 1, 0.1, 0.2 ) 
		{ 
			Title = "the Knight"; 

			SetStr( 1750, 1750 );
			SetDex( 150, 150 );
			SetInt( 61, 75 );

			SetSkill( SkillName.MagicResist, 120.0, 120.0 );
			SetSkill( SkillName.Swords, 120.0, 120.0 );
			SetSkill( SkillName.Tactics,120.0, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0,120.0 );

			Longsword onehanded = new Longsword();
			onehanded.Hue = 2301;
			AddItem(onehanded);
			HeaterShield twohanded = new HeaterShield();
			twohanded.Hue = 2301;
			AddItem(twohanded);
			DragonLegs legs = new DragonLegs();
			legs.Hue = 2301;
			AddItem(legs);
			DragonArms arms = new DragonArms();
			arms.Hue = 2301;
			AddItem(arms);
			AddItem( new Boots(2301) );
			DragonHelm helm = new DragonHelm();
			helm.Hue = 2301;
			AddItem(helm);
			AddItem( new Cloak(237) );
			DragonGloves gloves = new DragonGloves();
			gloves.Hue = 2301;
			AddItem(gloves);
			AddItem( new BodySash(237) );

			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
								
				DragonChest chest = new DragonChest();
				chest.Hue = 2301;
				AddItem(chest);
				
			}
			else 
			{ 
				Body = 400; 			
				Name = NameList.RandomName( "male" ); 

				DragonChest chest = new DragonChest();
				chest.Hue = 2301;
				AddItem(chest);
				
			}

			Utility.AssignRandomHair( this );
		}

		public SerpentsHoldGuard( Serial serial ) : base( serial ) 
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