/*
 * Created by Mr. Bill Creations
 * Date: 2/10/2008
 * Time: 11:20 PM
 * 
 */
 using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a blood storm corpse" )]
	public class BloodStorm : BaseCreature
	{
		[Constructable]
		public BloodStorm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a blood storm";
			Body = 0x23D;
			BaseSoundID = 40;
			Hue = 0x23;
			
			SetStr( 263, 308 );
			SetDex( 33, 43 );
			SetInt( 113, 175 );
			
			SetHits( 158, 185 );
			
			SetDamage( 9, 14 );
			
			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Poison, 50 );
			SetDamageType( ResistanceType.Energy, 50 );
			
			SetResistance( ResistanceType.Physical, 28, 33 );
			SetResistance( ResistanceType.Fire, 10, 15 );
			SetResistance( ResistanceType.Cold, 20, 25 );
			SetResistance( ResistanceType.Poison, 25, 30 );
			SetResistance( ResistanceType.Energy, 15, 20 );
			
			SetSkill( SkillName.EvalInt, 42.6, 50.0 );
			SetSkill( SkillName.Magery, 42.6, 50.0 );
			SetSkill( SkillName.Meditation, 5.2, 25.0 );
			SetSkill( SkillName.MagicResist, 40.1, 47.5 );
			SetSkill( SkillName.Tactics, 40.1, 50.0 );
			SetSkill( SkillName.Wrestling, 40.1, 50.0 );
			
			Fame = 6250;
			Karma = -6250;
			
			VirtualArmor = 30;
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 79.1;
			
		}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );			
		}
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.None; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		
		public BloodStorm( Serial serial ) : base( serial )
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

