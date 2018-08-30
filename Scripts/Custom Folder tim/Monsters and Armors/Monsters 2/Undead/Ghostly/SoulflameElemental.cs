using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a soulflame elemental corpse" )]
	public class SoulflameElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public SoulflameElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a soulflame elemental";
			Body = 15;
			BaseSoundID = 838;
			Hue = 1287;
			
			SetStr( 146, 148 );
			SetDex( 166, 185 );
			SetInt( 181, 185 );

			SetHits( 276, 293 );

			SetDamage( 16, 28 );

			SetDamageType( ResistanceType.Physical, 85 );
			SetDamageType( ResistanceType.Fire, 105 );

			SetResistance( ResistanceType.Physical, 85, 105 );
			SetResistance( ResistanceType.Fire, 100, 120 );
			SetResistance( ResistanceType.Cold, 100, 120 );
			SetResistance( ResistanceType.Poison, 60, 80 );
			SetResistance( ResistanceType.Energy, 100, 120 );

			SetSkill( SkillName.EvalInt, 100.1, 115.0 );
			SetSkill( SkillName.Magery, 80.1, 105.0 );
			SetSkill( SkillName.MagicResist, 85.2, 105.0 );
			SetSkill( SkillName.Tactics, 80.1, 100.0 );
			SetSkill( SkillName.Wrestling, 70.1, 100.0 );

			Fame = 8500;
			Karma = -8500;

			VirtualArmor = 40;
			ControlSlots = 4;

			PackItem( new SulfurousAsh( 3 ) );

			AddItem( new LightSource() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Gems );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 2; } }

		public SoulflameElemental( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 274 )
				BaseSoundID = 838;
		}
	}
}
