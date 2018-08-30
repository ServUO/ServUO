using System;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "a dog corpse" )]
	public class BloodHound : BaseCreature
	{
		[Constructable]
		public BloodHound() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a blood hound";
			Body = 0xD9;
			Hue = 342;
			BaseSoundID = 0x85;

			SetStr( 47, 67 );
			SetDex( 48, 63 );
			SetInt( 49, 67 );

			SetHits( 47, 52 );
			SetMana( 0 );

			SetDamage( 8, 13 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 20, 35 );

			SetSkill( SkillName.MagicResist, 22.1, 47.0 );
			SetSkill( SkillName.Tactics, 69.2, 71.0 );
			SetSkill( SkillName.Wrestling, 49.2, 61.0 );

			Fame = 0;
			Karma = 300;

			VirtualArmor = 21;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 98.3;
		}

		private DateTime m_NextRevealTime;

		// Min/max seconds until next Reveal
		public double RevealMinDelay{ get{ return 25.0; } }
		public double RevealMaxDelay{ get{ return 40.0; } }

		public override void OnThink()
		{
			if( DateTime.Now >= m_NextRevealTime )
			{
				DoReveal();
				m_NextRevealTime = DateTime.Now + TimeSpan.FromSeconds( RevealMinDelay + (Utility.RandomDouble() * RevealMaxDelay) );
			}
		}
	
		public void DoReveal()
		{
			ArrayList list = new ArrayList();

			foreach ( Mobile m in this.GetMobilesInRange( 8 ) )
			{
				int h = m.Skills[SkillName.Hiding].Fixed;
				if ( m == this || !CanBeHarmful( m ) || !m.Alive || !InLOS( m ) || h < 100 )
					continue;

				else
					list.Add( m );
			}

			foreach ( Mobile m in list )
			{
				m.RevealingAction();
			}
		}


		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

		public BloodHound(Serial serial) : base(serial)
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