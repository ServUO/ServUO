using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
	public class MinocGuard : BaseRanger
	{ 

		[Constructable] 
		public MinocGuard() : base( AIType.AI_Melee, FightMode.Closest, 15, 1, 0.1, 0.2 ) 
		{ 
			Title = "the Knight"; 

			SetStr( 1750, 1750 );
			SetDex( 150, 150 );
			SetInt( 61, 75 );

			SetSkill( SkillName.MagicResist, 120.0, 120.0 );
			SetSkill( SkillName.Swords, 120.0, 120.0 );
			SetSkill( SkillName.Tactics,120.0, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0,120.0 );

			AddItem( new Longsword() );
			HeaterShield twohanded = new HeaterShield();
			twohanded.Hue = 1175;
			AddItem(twohanded);
			PlateLegs legs = new PlateLegs();
			legs.Hue = 1175;
			AddItem(legs);
			PlateArms arms = new PlateArms();
			arms.Hue = 1175;
			AddItem(arms);
			PlateGorget neck = new PlateGorget();
			neck.Hue = 1175;
			AddItem(neck);
			AddItem( new Boots() );
			PlateHelm helm = new PlateHelm();
			helm.Hue = 1175;
			AddItem(helm);
			AddItem( new Cloak(248) );
			PlateGloves gloves = new PlateGloves();
			gloves.Hue = 1175;
			AddItem(gloves);
			AddItem( new BodySash(248) );

			if ( Female = Utility.RandomBool() ) 
			{ 
				Body = 401; 
				Name = NameList.RandomName( "female" );
								
				FemalePlateChest chest = new FemalePlateChest();
				chest.Hue = 1175;
				AddItem(chest);
				
			}
			else 
			{ 
				Body = 400; 					
				Name = NameList.RandomName( "male" ); 

				PlateChest chest = new PlateChest();
				chest.Hue = 1175;
				AddItem(chest);
				
			}

			Utility.AssignRandomHair( this );
		}

		public MinocGuard( Serial serial ) : base( serial ) 
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