using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "the remains of a rising colossus" )]
	public class RisingColossus : BaseCreature
	{
        private int m_DispelDifficulty;
        public override double DispelDifficulty{ get{ return m_DispelDifficulty; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public RisingColossus(int level) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.5 )
		{
            int statbonus = ((level * 3) / 4);
            int hitsbonus = ((level * 9) / 8);
            int skillvalue = level / 2;

            this.Name = "a rising colossus";
            this.Body = 829;

            this.SetHits(200 + hitsbonus);

            this.SetStr(600 + statbonus);
            this.SetDex(30 + statbonus);
            this.SetInt(50 + statbonus);

            this.SetDamage(level / 12, level / 10);

            this.SetDamageType( ResistanceType.Physical, 100 );

			this.SetResistance( ResistanceType.Physical, 65, 70 );
			this.SetResistance( ResistanceType.Fire, 50, 55 );
			this.SetResistance( ResistanceType.Cold, 50, 55 );
			this.SetResistance( ResistanceType.Poison, 100 );
			this.SetResistance( ResistanceType.Energy, 65, 70 );

            this.SetSkill(SkillName.MagicResist, skillvalue);
            this.SetSkill(SkillName.Tactics, skillvalue);
            this.SetSkill(SkillName.Wrestling, skillvalue);
            this.SetSkill(SkillName.Anatomy, skillvalue);
            this.SetSkill(SkillName.Mysticism, skillvalue);

            this.VirtualArmor = 58;
			this.ControlSlots = 5;

            m_DispelDifficulty = 40 + (level * 2);
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
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public RisingColossus( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

            writer.Write((int)m_DispelDifficulty);
        }

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            m_DispelDifficulty = reader.ReadInt();
        }
	}
}