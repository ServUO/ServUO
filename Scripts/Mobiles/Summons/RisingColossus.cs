using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "the remains of a rising colossus" )]
	public class RisingColossus : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 125.0; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public RisingColossus() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.5 )
		{
			Name = "Rising Colossus";
			Body = 829;

			SetStr( 780 );
			SetDex( 210 );
			SetInt( 230 );

            SetHits( 470 );

			SetDamage( 19, 24 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 65, 70 );
			SetResistance( ResistanceType.Fire, 50, 55 );
			SetResistance( ResistanceType.Cold, 50, 55 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 65, 70 );

			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
            SetSkill(SkillName.Tactics, 120.0);
			SetSkill( SkillName.Wrestling, 120.0 );

			VirtualArmor = 58;
			ControlSlots = 5;
		}

        public override double GetFightModeRanking(Mobile m, FightMode acqType, bool bPlayerOnly)
        {
            return (m.Int + m.Skills[SkillName.Magery].Value) / Math.Max(GetDistanceToSqrt(m), 1.0);
        }

		public override int GetAttackSound()
		{
			return 0x627;
		}

		public override int GetHurtSound()
		{
			return 0x629;
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } } // Immune to poison?

		public RisingColossus( Serial serial ) : base( serial )
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