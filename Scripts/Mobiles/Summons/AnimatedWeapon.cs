using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "the remains of a broken weapon" )]
	public class AnimatedWeapon : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 125.0; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public AnimatedWeapon() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Animated Weapon";
			Body = 692;

			SetStr( 150 );
			SetDex( 150 );
			SetInt( 100 );

            SetHits(180);

			SetDamage( 14, 21 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Poison, 100 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 45, 55 );
			SetResistance( ResistanceType.Cold, 45, 55 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 90.1, 100.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			VirtualArmor = 58;
			ControlSlots = 4;
		}

		public override int GetAttackSound()
		{
			return 0x64A;
		}

		public override int GetHurtSound()
		{
			return 0x64A;
		}

        public override double GetFightModeRanking(Mobile m, FightMode acqType, bool bPlayerOnly)
        {
            return (m.Str + m.Skills[SkillName.Tactics].Value) / Math.Max(GetDistanceToSqrt(m), 1.0);
        }

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Parasitic; } } // Immune to poison?

		public AnimatedWeapon( Serial serial ) : base( serial )
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