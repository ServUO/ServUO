/*
 * Created by Mr. Bill Creations
 * Date: 2/10/2008
 * Time: 11:35 PM
 * 
 */
 using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a fire storm corpse" )]
	public class FireStorm : BaseCreature
	{
		[Constructable]
		public FireStorm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a fire storm";
			Body = 0x23D;
			BaseSoundID = 40;
			Hue = 0x54E;
			
			SetStr( 63, 78 );
			SetDex( 83, 93 );
			SetInt( 51, 63 );
			
			SetHits( 38, 47 );
			
			SetDamage( 4, 5 );
			
			SetDamageType( ResistanceType.Physical, 13 );
			SetDamageType( ResistanceType.Fire, 38 );
			
			SetResistance( ResistanceType.Physical, 18, 23 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 3, 5 );
			SetResistance( ResistanceType.Poison, 15, 20 );
			SetResistance( ResistanceType.Energy, 15, 20 );
			
			SetSkill( SkillName.EvalInt, 30.1, 37.5 );
			SetSkill( SkillName.Magery, 30.1, 37.5 );
			SetSkill( SkillName.MagicResist, 37.6, 52.5 );
			SetSkill( SkillName.Tactics, 40.1, 50.0 );
			SetSkill( SkillName.Wrestling, 35.1, 50.0 );
			
			Fame = 2250;
			Karma = -2250;
			
			VirtualArmor = 20;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 79.1;
			
			PackItem( new SulfurousAsh( 3 ) );
			
			AddItem( new LightSource() );
	}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Gems );
		}
		
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.None; } }
		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		
		public FireStorm( Serial serial ) : base( serial )
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

