using System;
using Server;
using Server.Items;
				
namespace Server.Mobiles
{
	[CorpseName( "Corpse Of A Evo Armor Guardian" )]
	public class EvoArmorGuardian : BaseCreature
	{
		public override bool ShowFameTitle{ get{ return false; } }

		[Constructable]
		public EvoArmorGuardian() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Evo Guardian";
			Body = 311;
			Hue = 0;
                        BaseSoundID = 1072;
				 
			SetStr( 500, 500 );
			SetDex( 400, 400 );
			SetInt( 200, 250 );
				 
			SetHits( 3500, 5000 );
				 
			SetDamage( 20, 35 );
				 
			SetDamageType( ResistanceType.Physical, 55 );
			SetDamageType( ResistanceType.Cold, 55 );
			SetDamageType( ResistanceType.Fire, 55 );
				 
			SetResistance( ResistanceType.Physical, 50, 100 );
			SetResistance( ResistanceType.Energy, 50, 100 );
			SetResistance( ResistanceType.Poison, 50, 100 );
			SetResistance( ResistanceType.Cold, 50, 100 );
			SetResistance( ResistanceType.Fire, 50, 100 );
				 
			SetSkill( SkillName.Wrestling, 95.1, 100.0 );
			SetSkill( SkillName.Anatomy, 95.1, 100.0 );
			SetSkill( SkillName.MagicResist, 95.1, 100.0 );
			SetSkill( SkillName.Swords, 95.1, 100.0 );
			SetSkill( SkillName.Tactics, 95.1, 100.0 );
			SetSkill( SkillName.Parry, 95.1, 100.0 );
			SetSkill( SkillName.Focus, 95.1, 100.0 );
				 
			Fame = 25000;
			Karma = -25000;
				 
			VirtualArmor = 40;
                  

                        PackGold( 1000, 6000 );
	
		}
			
		public override bool AlwaysAttackable{ get{ return true; } }
 
		public override void GenerateLoot()
		{
			 AddLoot( LootPack.Rich, 2 );
		}
                public override void OnDeath(Container c)
		{
			switch ( Utility.Random( 30 )) 
			{ 
				case 0: c.AddItem( new armsofevolution() ); break; 
				case 1: c.AddItem( new chestofevolution() ); break;
				case 2: c.AddItem( new glovesofevolution() ); break;
                                case 3: c.AddItem( new helmofevolution() ); break;
                                case 4: c.AddItem( new legsofevolution() ); break;
                                case 5: c.AddItem( new femalechestofevolution() ); break;
                                case 6: c.AddItem( new gorgetofevolution() ); break;
			}
			base.OnDeath( c );
			
		}
			
		public EvoArmorGuardian( Serial serial ) : base( serial )
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
