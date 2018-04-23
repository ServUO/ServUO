using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a rotting wolf corpse" )]
	[TypeAlias( "Server.Mobiles.RevenantWolf" )]
	public class RevenantWolf : BaseCreature
	{
		[Constructable]
		public RevenantWolf() : base( AIType.AI_Melee,FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a revenant wolf";
			Body = 23;
			BaseSoundID = 0xE5;
			Hue = 1743;

			SetStr( 96, 120 );
			SetDex( 121, 145 );
			SetInt( 36, 60 );

			SetHits( 58, 72 );
			SetMana( 0 );

			SetDamage( 14, 18 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 20, 25 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 5, 10 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 10, 15 );

			SetSkill( SkillName.MagicResist, 57.6, 75.0 );
			SetSkill( SkillName.Tactics, 50.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 80.0 );

			Fame = 8500;
			Karma = -8500;

			VirtualArmor = 22;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 83.1;
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 7; } }
		public override bool BleedImmune{ get{ return true; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }

		public RevenantWolf(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}