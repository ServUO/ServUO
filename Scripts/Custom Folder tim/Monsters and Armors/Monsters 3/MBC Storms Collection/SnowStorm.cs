/*
 * Created by Mr. Bill Creations
 * Date: 2/11/2008
 * Time: 7:48 PM
 * 
*/
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a snow storm corpse" )]
	public class SnowStorm : BaseCreature
	{
		[Constructable]
		public SnowStorm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a snow storm";
			Body = 0x23D;
			BaseSoundID = 40;
			Hue = 0x47E;
			
			SetStr( 163, 178 );
			SetDex( 83, 93 );
			SetInt( 36, 48 );
			
			SetHits( 98, 107 );
			
			SetDamage( 6, 9 );
			
			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 80 );
			
			SetResistance( ResistanceType.Physical, 23, 28 );
			SetResistance( ResistanceType.Fire, 5, 8 );
			SetResistance( ResistanceType.Cold, 30, 35 );
			SetResistance( ResistanceType.Poison, 13, 17 );
			SetResistance( ResistanceType.Energy, 13, 17 );
			
			SetSkill( SkillName.MagicResist, 25.1, 32.5 );
			SetSkill( SkillName.Tactics, 40.1, 50.0 );
			SetSkill( SkillName.Wrestling, 40.1, 50.0 );
			
			Fame = 2500;
			Karma = -2500;
			
			VirtualArmor = 20;
						
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 99.1;
			
			PackItem( new IronOre( 3 ) );
			PackItem( new BlackPearl( 3 ) );
		}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.None; } }
		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return Utility.RandomList( 2, 3 ); } }
		
		public SnowStorm( Serial serial ) : base( serial )
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

