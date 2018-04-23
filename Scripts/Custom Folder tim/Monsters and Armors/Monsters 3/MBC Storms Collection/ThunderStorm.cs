/*
 * Created by Mr. Bill Creations
 * Date: 2/10/2008
 * Time: 7:52 AM
 * 
 */
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a thunder stom corpse" )]
	public class ThunderStorm : BaseCreature
	{
		[Constructable]
		public ThunderStorm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a thunder storm";
			Body = 0x23D;
			BaseSoundID = 40;
			Hue = 0x4001;
			
			SetStr( 63, 78 );
			SetDex( 83, 93 );
			SetInt( 51, 63 );
			
			SetHits( 38, 47 );
			
			SetDamage( 4, 5 );
			
			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 40 );
			SetDamageType( ResistanceType.Energy, 40 );
			
			SetResistance( ResistanceType.Physical, 18, 23 );
			SetResistance( ResistanceType.Fire, 8, 13 );
			SetResistance( ResistanceType.Cold, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 13, 18 );
			
			SetSkill( SkillName.EvalInt, 30.1, 37.5 );
			SetSkill( SkillName.Magery, 30.1, 37.5 );
			SetSkill( SkillName.MagicResist, 30.1, 37.5 );
			SetSkill( SkillName.Tactics, 30.1, 40.0 );
			SetSkill( SkillName.Wrestling, 30.1, 40.0 );
			
			Fame = 2250;
			Karma = -2250;
			
			VirtualArmor = 20;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 79.1;
			
			PackItem( new SulfurousAsh( 4 ) );
		}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.LowScrolls );
		}
		
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.None; } }
		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		
		public ThunderStorm( Serial serial ) : base( serial )
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



