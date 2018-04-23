using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
[CorpseName( "Nag's corpse" )]
	public class NagOne : BaseCreature
	{
	        [Constructable]
		public NagOne() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 173;
			Name = "Nag";
                        Hue = 1194;

			BaseSoundID = 0x183;

			SetStr( 1505, 1600 );
			SetDex( 102, 300 );
			SetInt( 402, 600 );

			SetHits( 3000 );
			SetStam( 105, 600 );

			SetDamage( 21, 33 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 75, 80 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 60, 70 );

			SetSkill( SkillName.MagicResist, 70.7, 140.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 97.6, 100.0 );
			SetSkill( SkillName.Poisoning, 97.6, 100.0 );   

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 110;

			Tamable = false;

			if( Utility.RandomDouble() <= 0.01 ) PackItem( new NagsThrone() );

			switch( Utility.Random( 10 ) )
			{
				case 0: PackItem( new LeftArm() ); break;
				case 1: PackItem( new RightArm() ); break;
				case 2: PackItem( new Torso() ); break;
				case 3: PackItem( new Bone() ); break;
				case 4: PackItem( new RibCage() ); break;
				case 5: PackItem( new RibCage() ); break;
				case 6: PackItem( new BonePile() ); break;
				case 7: PackItem( new BonePile() ); break;
				case 8: PackItem( new BonePile() ); break;
				case 9: PackItem( new BonePile() ); break;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.UltraRich, 4 );
			AddLoot( LootPack.Gems, 5 );
			AddLoot( LootPack.MedScrolls, 4 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override bool AutoDispel{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			// TODO: Web ability
		}

		public NagOne( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}