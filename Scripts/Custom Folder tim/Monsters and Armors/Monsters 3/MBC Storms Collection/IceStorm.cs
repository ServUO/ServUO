/*
 * Created by Mr. Bill Creations
 * Date: 2/11/2008
 * Time: 6:02 AM
 * 
*/
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "an ice storm corpse" )]
	public class IceStorm : BaseCreature
	{
		[Constructable]
		public IceStorm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ice storm";
			Body = 0x23D;
			BaseSoundID = 40;
			Hue = 0x47F;
			
			SetStr( 78, 93 );
			SetDex( 48, 58 );
			SetInt( 88, 96 );
			
			SetHits( 47, 56 );
			
			SetDamage( 5, 11 );
			
			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Cold, 75 );
			
			SetResistance( ResistanceType.Physical, 18, 23 );
			SetResistance( ResistanceType.Fire, 3, 5 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 10, 15 );
			SetResistance( ResistanceType.Energy, 10, 15 );
			
			SetSkill( SkillName.EvalInt, 5.3, 30.0 );
			SetSkill( SkillName.Magery, 5.3, 30.0 );
			SetSkill( SkillName.MagicResist, 15.1, 40.0 );
			SetSkill( SkillName.Tactics, 35.1, 50.0 );
			SetSkill( SkillName.Wrestling, 30.1, 50.0 );
			
			Fame = 2000;
			Karma = -2000;
			
			VirtualArmor = 20;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 79.1;
			
			PackItem( new BlackPearl() );
			PackReg( 3 );
		}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.Gems, 2 );
		}
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.None; } }
		public override bool BleedImmune{ get{ return true; } }
		
		public IceStorm( Serial serial ) : base( serial )
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

