/*
 * Created by Mr. Bill Creations
 * Date: 9/22/2006
 * Time: 1:40 PM
 *  
*/
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a toxic storm corpse" )]
	public class ToxicStorm : BaseCreature
	{
		[Constructable]
		public ToxicStorm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a toxic storm";
			Body = 0x23D;
			BaseSoundID = 40;
			Hue = 0x38;
			
			SetStr( 163, 178 );
			SetDex( 33, 43 );
			SetInt( 136, 148 );
			
			SetHits( 98, 107 );
			
			SetDamage( 5, 8 );
			
			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Fire, 50 );
			SetDamageType( ResistanceType.Energy, 25 );
			
			SetResistance( ResistanceType.Physical, 23, 28 );
			SetResistance( ResistanceType.Fire, 20, 25 );
			SetResistance( ResistanceType.Cold, 10, 15 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 15, 20 );
			
			SetSkill( SkillName.Anatomy, 15.2, 30.0 );
			SetSkill( SkillName.EvalInt, 35.1, 42.5 );
			SetSkill( SkillName.Magery, 35.1, 42.5 );
			SetSkill( SkillName.MagicResist, 30.1, 37.5 );
			SetSkill( SkillName.Poisoning, 30.1, 40.0 );
			SetSkill( SkillName.Tactics, 40.1, 45.0 );
			SetSkill( SkillName.Wrestling, 35.1, 45.0 );
			
			Fame = 5000;
			Karma = -5000;
			
			VirtualArmor = 20;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 79.1;
			
			PackItem( new NoxCrystal( 4 ) );
		}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );			
		}
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.None; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override Poison HitPoison{ get{ return Poison.Regular; } }
		public override double HitPoisonChance{ get{ return 0.3; } }
		
		public ToxicStorm( Serial serial ) : base( serial )
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


