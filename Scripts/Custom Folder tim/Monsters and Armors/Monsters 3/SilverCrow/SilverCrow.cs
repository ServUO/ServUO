using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a Silver Crow corpse" )]
	public class SilverCrow : BaseCreature
	{
		[Constructable]
		public SilverCrow () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.0, 0.2 )
		{
			Name = "Silver Crow";
			Body = 5;
			BaseSoundID = 0x2EE;;
                        Hue = 1953;

			SetStr( 725, 800 );
			SetDex( 177, 255 );
			SetInt( 131, 150 );

			SetHits( 8250, 10399 );

			SetDamage( 40, 48 );

			SetDamageType( ResistanceType.Physical, 60 );
			SetDamageType( ResistanceType.Fire, 50 );
			SetDamageType( ResistanceType.Energy, 80 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 95, 100 );
			SetResistance( ResistanceType.Cold, 65, 75 );
			SetResistance( ResistanceType.Poison, 65, 75 );
			SetResistance( ResistanceType.Energy, 65, 75 );

			SetSkill( SkillName.Anatomy, 110.1, 126.0 );
			SetSkill( SkillName.EvalInt, 110.1, 115.0 );
			SetSkill( SkillName.Magery, 100.0, 100.5 );
			SetSkill( SkillName.Meditation, 118.1, 122.0 );
			SetSkill( SkillName.MagicResist, 100.5, 130.0 );
			SetSkill( SkillName.Tactics, 115.1, 120.0 );
			SetSkill( SkillName.Wrestling, 105.1, 110.0 );

			Fame = -10000;
			Karma = -15000;

			VirtualArmor = 60;

			switch ( Utility.Random( 2 ))
			{
				case 0: PackItem( new ValoriteIngot( 50 ) ); break;
				case 1: PackItem( new Bone( 100 ) ); break;
				//case 2: PackItem( new Bloodmoss( 3 ) ); break;
				//case 3: PackItem( new Garlic( 3 ) ); break;
				//case 4: PackItem( new MandrakeRoot( 3 ) ); break;
				//case 5: PackItem( new Nightshade( 3 ) ); break;
				//case 6: PackItem( new SulfurousAsh( 3 ) ); break;
				//case 7: PackItem( new Ginseng( 3 ) ); break;
			}

		      	PackItem( new Broadsword() );
		      	PackGold( 5100, 5500 );
                        //PackItem( new LootTokenCheck( 2500 ) ); 

                         switch ( Utility.Random( 100 ) ) 
                        { 
                          case 0: PackItem( new SilverCrowArms( ) ); 
                          break; 
                          case 1: PackItem( new SilverCrowGorget( ) ); 
                          break; 
                          case 2: PackItem( new SilverCrowGloves( ) ); 
                          break; 
                          case 3: PackItem( new SilverCrowFChest( ) ); 
                          break; 
                          case 4: PackItem( new SilverCrowScythe( ) ); 
                          break; 
                          case 5: PackItem( new SilverCrowLegs( ) ); 
                          break; 
                          case 6: PackItem( new SilverCrowChest( ) ); 
                          break; 
                          case 7: PackItem( new SilverCrowRing( ) ); 
                          break;
                          case 8: PackItem( new SilverCrowHelm() );
                          break;
			  case 9: PackItem( new SilverCrowBracelet() );
                          break;
                     }
                  		
	      }
		public override bool CanRummageCorpses{ get{ return false; } }
            public override bool BardImmune{ get{ return false; } }
            public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return 5; } }
		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Feathers{ get{ return 100; } }

		public SilverCrow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}