/*
 * Created by Mr. Bill Creations
 * Date: 2/11/2008
 * Time: 7:13 PM
 * 
*/
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a sea storm corpse" )]
	public class SeaStorm : BaseCreature
	{
		[Constructable]
		public SeaStorm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a sea storm";
			Body = 0x23D;
			BaseSoundID = 40;
			Hue = 0x60;
			
			SetStr( 63, 78 );
			SetDex( 33, 43 );
			SetInt( 51, 63 );
			
			SetHits( 38, 47 );
			
			SetDamage( 4, 10 );
			
			SetDamageType( ResistanceType.Physical, 100 );
			
			SetResistance( ResistanceType.Physical, 18, 23 );
			SetResistance( ResistanceType.Fire, 5, 14 );
			SetResistance( ResistanceType.Cold, 5, 14 );
			SetResistance( ResistanceType.Poison, 30, 35 );
			SetResistance( ResistanceType.Energy, 3, 5 );
			
			SetSkill( SkillName.EvalInt, 30.1, 37.5 );
			SetSkill( SkillName.Magery, 30.1, 37.5 );
			SetSkill( SkillName.MagicResist, 50.1, 57.5 );
			SetSkill( SkillName.Tactics, 25.1, 35.0 );
			SetSkill( SkillName.Wrestling, 25.1, 35.0 );
			
			Fame = 2250;
			Karma = -2250;
			
			VirtualArmor = 20;
			CanSwim = true;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 79.1;
			
			PackItem( new BlackPearl( 3 ) );
		}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Potions );
		}
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.None; } }
		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		
		public SeaStorm( Serial serial ) : base( serial )
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

