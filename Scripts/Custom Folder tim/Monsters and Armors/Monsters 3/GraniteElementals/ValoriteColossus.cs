using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a colossus corpse" )]
	public class ValoriteColossus : BaseCreature
	{

		[Constructable]
		public ValoriteColossus() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			// TODO: Gas attack
			Name = "a valorite colossus";
			Hue = 0x8AB;
			Body = 0x33D;
			BaseSoundID = 268;

			SetStr( 226, 255 );
			SetDex( 126, 145 );
			SetInt( 71, 92 );

			SetHits( 236, 353 );

			SetDamage( 15, 19 );

			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Cold, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.MagicResist, 50.1, 95.0 );
			SetSkill( SkillName.Tactics, 60.1, 100.0 );
			SetSkill( SkillName.Wrestling, 60.1, 100.0 );

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 38;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Gems, 4 );
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );
			
			ValoriteGranite granite = new ValoriteGranite();
   			granite.Amount = 10;
   			c.DropItem(granite);

		}

		public override bool AutoDispel{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }

		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)from;

				if ( bc.Controlled || bc.BardTarget == this )
					damage = 0; // Immune to pets and provoked creatures
			}
			else if ( from != null )
			{
				int hitback = damage;
				AOS.Damage( from, this, hitback, 100, 0, 0, 0, 0 );
			}
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = true; // Every spell is reflected back to the caster
		}
		public override void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
			if ( 0.5 >= Utility.RandomDouble() )
			{
				double positionChance = Utility.RandomDouble();
				BaseArmor armor;

				if ( positionChance < 0.07 )
					armor = to.NeckArmor as BaseArmor;
				else if ( positionChance < 0.14 )
					armor = to.HandArmor as BaseArmor;
				else if ( positionChance < 0.28 )
					armor = to.ArmsArmor as BaseArmor;
				else if ( positionChance < 0.43 )
					armor = to.HeadArmor as BaseArmor;
				else if ( positionChance < 0.65 )
					armor = to.LegsArmor as BaseArmor;
				else
					armor = to.ChestArmor as BaseArmor;

				if ( armor != null )
				{
					int ruin = Utility.RandomMinMax( 1, 4 );
					armor.HitPoints -= ruin;
				}
			}
		}
		public ValoriteColossus( Serial serial ) : base( serial )
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
