using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class GreaterDragon1 : BaseCreature
	{
		public override bool StatLossAfterTame { get { return true; } }

		[Constructable]
		public GreaterDragon1 () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.3, 0.5 )
		{
			Name = "an enormious dragon";
			Body = Utility.RandomList( 826);
			BaseSoundID = 362;
            double chance = Utility.RandomDouble() * 23301;

            if (chance <= 1)
                Hue = 0x480;
            else if (chance < 50)
                Hue = Utility.RandomList( 0x497, 0x482 );
            else if (chance < 500)
                Hue = Utility.RandomList( 0x4D6, 0x65F, 0x724, 0x527, 0x585 );       

			SetStr( 1025, 1425 );
			SetDex( 81, 148 );
			SetInt( 475, 675 );

			SetHits( 1000, 1600 );
			SetStam( 120, 135 );

			SetDamage( 24, 33 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 60, 85 );
			SetResistance( ResistanceType.Fire, 65, 90 );
			SetResistance( ResistanceType.Cold, 40, 55 );
			SetResistance( ResistanceType.Poison, 40, 60 );
			SetResistance( ResistanceType.Energy, 50, 75 );

			SetSkill( SkillName.Meditation, 0 );
			SetSkill( SkillName.EvalInt, 110.0, 140.0 );
			SetSkill( SkillName.Magery, 110.0, 140.0 );
			SetSkill( SkillName.Poisoning, 0 );
			SetSkill( SkillName.Anatomy, 0 );
			SetSkill( SkillName.MagicResist, 110.0, 140.0 );
			SetSkill( SkillName.Tactics, 110.0, 140.0 );
			SetSkill( SkillName.Wrestling, 115.0, 145.0 );

			Fame = 22000;
			Karma = -15000;

			VirtualArmor = 60;

			Tamable = true;
			ControlSlots = 4;
			MinTameSkill = 104.7;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 4 );
			AddLoot( LootPack.Gems, 8 );
		}

		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return !Controlled; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return 19; } }
		public override int Hides{ get{ return 30; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Scales{ get{ return 7; } }
		public override ScaleType ScaleType{ get{ return ( Body == 12 ? ScaleType.Yellow : ScaleType.Red ); } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override bool CanAngerOnTame { get { return true; } }

		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.BleedAttack;
		}

		public GreaterDragon1( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			SetDamage( 24, 33 );

			if( version == 0 )
			{
				Server.SkillHandlers.AnimalTaming.ScaleStats( this, 0.50 );
				Server.SkillHandlers.AnimalTaming.ScaleSkills( this, 0.80, 0.90 ); // 90% * 80% = 72% of original skills trainable to 90%
				Skills[SkillName.Magery].Base = Skills[SkillName.Magery].Cap; // Greater dragons have a 90% cap reduction and 90% skill reduction on magery
			}
		}
	}
}
