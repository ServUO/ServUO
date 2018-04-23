using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an oil elemental corpse" )]
	public class OilElemental : BaseCreature
	{
		[Constructable]
		public OilElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an oil elemental";
			Body = 0x9E;
			BaseSoundID = 278;
			Hue = 2051;

			SetStr( 226, 255 );
			SetDex( 86, 105 );
			SetInt( 171, 195 );

			SetHits( 136, 153 );

			SetDamage( 8, 13 );

			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Poison, 50 );
			SetDamageType( ResistanceType.Fire, 25 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 70, 80 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Anatomy, 30.3, 60.0 );
			SetSkill( SkillName.EvalInt, 70.1, 85.0 );
			SetSkill( SkillName.Magery, 70.1, 85.0 );
			SetSkill( SkillName.MagicResist, 60.1, 75.0 );
			SetSkill( SkillName.Tactics, 80.1, 90.0 );
			SetSkill( SkillName.Wrestling, 70.1, 90.0 );

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 40;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Average );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override double HitPoisonChance{ get{ return 0.6; } }

		public override int TreasureMapLevel{ get{ return Core.AOS ? 2 : 3; } }

		public OilElemental( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 263 )
				BaseSoundID = 278;

			if ( Body == 13 )
				Body = 0x9E;

			if ( Hue == 0x4001 )
				Hue = 0;
		}
	}
}