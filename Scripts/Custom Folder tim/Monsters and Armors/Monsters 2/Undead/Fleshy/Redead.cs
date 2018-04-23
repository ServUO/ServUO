using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a redead corpse" )]
	public class Redead : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ParalyzingBlow;
		}

		[Constructable]
		public Redead() : base( AIType.AI_Melee,FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a redead";
			Body = 3;
			BaseSoundID = 0x3E9;
			Hue = 2313;

			SetStr( 80, 100 );
			SetDex( 101, 105 );
			SetInt( 66, 85 );

			SetHits( 105, 108 );

			SetDamage( 12, 17 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 65, 75 );
			SetResistance( ResistanceType.Cold, 65, 75 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.MagicResist, 65.1, 70.0 );
			SetSkill( SkillName.Tactics, 87.6, 92.5 );
			SetSkill( SkillName.Wrestling, 80.1, 90.0 );

			Fame = 8500;
			Karma = -8500;

			VirtualArmor = 27;
		}

		public override int TreasureMapLevel{ get{ return 1; } }

		public Redead( Serial serial ) : base( serial )
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