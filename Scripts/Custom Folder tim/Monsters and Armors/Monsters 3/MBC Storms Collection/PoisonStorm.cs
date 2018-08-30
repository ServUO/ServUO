/*
 * Created by Mr. Bill Creations
 * Date: 2/11/2008
 * Time: 6:47 AM
 * 
*/
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a poison storm corpse" )]
	public class PoisonStorm : BaseCreature
	{
		[Constructable]
		public PoisonStorm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a poison storm";
			Body = 0x23D;
			BaseSoundID = 40;
			Hue = 0x48;
			
			SetStr( 213, 258 );
			SetDex( 83, 93 );
			SetInt( 181, 218 );
			
			SetHits( 128, 155 );
			
			SetDamage( 6, 9 );
			
			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Poison, 90 );
			
			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 10, 15 );
			SetResistance( ResistanceType.Cold, 10, 15 );
			SetResistance( ResistanceType.Poison, 50 );
			SetResistance( ResistanceType.Energy, 20, 25 );
			
			SetSkill( SkillName.EvalInt, 40.1, 47.5 );
			SetSkill( SkillName.Magery, 40.1, 47.5 );
			SetSkill( SkillName.Meditation, 40.1, 60.0 );
			SetSkill( SkillName.Poisoning, 45.1, 50.0 );
			SetSkill( SkillName.MagicResist, 42.6, 57.5 );
			SetSkill( SkillName.Tactics, 40.1, 50.0 );
			SetSkill( SkillName.Wrestling, 35.1, 45.0 );
			
			Fame = 6250;
			Karma = -6250;
			
			VirtualArmor = 35;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 99.1;
			
			PackItem( new Nightshade( 4 ) );
			PackItem( new LesserPoisonPotion() );
		}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls );
		}
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.None; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override double HitPoisonChance{ get{ return 0.75; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		
		public PoisonStorm( Serial serial ) : base( serial )
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

