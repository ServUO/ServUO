using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a sphinx corpse" )]
	public class Sphinx : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return Utility.RandomBool() ? WeaponAbility.MortalStrike : WeaponAbility.BleedAttack;
		}

		[Constructable]
		public Sphinx() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a sphinx";
			Body = 788;
			BaseSoundID = 0x2A7;

			SetStr( 190, 213 );
			SetDex( 125, 144 );
			SetInt( 214, 239 );

			SetHits( 1340, 1496 );

			SetDamage( 17, 24 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 95, 100 );
			SetResistance( ResistanceType.Energy, 80, 90 );

			SetSkill( SkillName.Meditation, 100.0 );
			SetSkill( SkillName.Poisoning, 81.2, 92.5 );
			SetSkill( SkillName.MagicResist, 97.0, 102.6 );
			SetSkill( SkillName.Tactics, 94.7, 100.0 );
			SetSkill( SkillName.Wrestling, 80.0 );
			SetSkill( SkillName.Magery, 22.3, 44.2 );

			Fame = -20000;
			Karma = -20000;

			VirtualArmor = 50;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
			AddLoot( LootPack.Gems, Utility.Random( 8, 12 ) );
			AddLoot( LootPack.MedScrolls, Utility.Random( 2, 3 ) );
		}
		
		public override int Hides{ get{ return 8; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Meat{ get{ return 1; } }
		public override bool AutoDispel{ get{ return true; } }
		public override bool BardImmune{ get{ return !Core.AOS; } }
		public override bool Unprovokable{ get{ return Core.AOS; } }
		public override bool Uncalmable{ get{ return Core.AOS; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return (0.8 >= Utility.RandomDouble() ? Poison.Lesser : Poison.Regular); } }

		public override int TreasureMapLevel{ get{ return 5; } }

               public override void OnDeath(Container c)
                {
                        c.DropItem(new BankCheck(10));
                        if (0.05 > Utility.RandomDouble())
                {
                        c.DropItem(new BoneBox());
                }
                        base.OnDeath(c);
                }

		public Sphinx( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 1200 )
				BaseSoundID = 0x2A7;
		}
	}
}