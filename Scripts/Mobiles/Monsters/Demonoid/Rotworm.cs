using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Items;
using Server.Engines.Quests;
//using Server.Engines.Loyalty;

namespace Server.Mobiles
{
	[CorpseName( "a rotworm corpse" )]
	[TypeAlias( "Server.Mobiles.RotWorm" )]
	public class Rotworm : BaseCreature
	{
		[Constructable]
		public Rotworm()
			: base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.25, 0.5 )
		{
			Name = "a rotworm";
			Body = 732;

			SetStr( 200, 300 );
			SetDex( 80 );
			SetInt( 15, 20 );

			SetHits( 200, 250 );
			SetStam( 50 );

			SetDamage( 1, 5 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 25, 35 );
			SetResistance( ResistanceType.Poison, 65, 75 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.MagicResist, 25.0 );
			SetSkill( SkillName.Tactics, 25.0 );
			SetSkill( SkillName.Wrestling, 50.0 );

			Fame = 500;
			Karma = -500;

            QLPoints = 10;

			switch ( Utility.Random( 10 ) )
			{
				case 0: PackItem( new LeftArm() ); break;
				case 1: PackItem( new RightArm() ); break;
				case 2: PackItem( new Torso() ); break;
				case 3: PackItem( new Bone() ); break;
				case 4: PackItem( new RibCage() ); break;
				case 5: PackItem( new RibCage() ); break;
				case 6: PackItem( new BonePile() ); break;
			}
		}

		public Rotworm( Serial serial )
			: base( serial )
		{
		}

		public override int GetAngerSound() { return 0x62D; }
		public override int GetIdleSound() { return 0x62D; }
		public override int GetAttackSound() { return 0x62A; }
		public override int GetHurtSound() { return 0x62C; }
		public override int GetDeathSound() { return 0x62B; }

		//public override LoyaltyGroup LoyaltyGroupEnemy { get { return LoyaltyGroup.GargoyleQueen; } }
		//public override int LoyaltyPointsAward { get { return 10; } }

		public override int Meat { get { return 2; } }
		//public override MeatType MeatType { get { return MeatType.Rotworm; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override void OnKilledBy( Mobile mob )
		{
			base.OnKilledBy( mob );

			if ( mob is PlayerMobile && 0.2 > Utility.RandomDouble() )
			{
				PlayerMobile pm = mob as PlayerMobile;

				if ( QuestHelper.HasQuest<Missing>( pm ) )
				{
					// As the rotworm dies, you find and pickup a scroll case. Inside the scroll case is parchment. The scroll case crumbles to dust.
					pm.SendLocalizedMessage( 1095146 );

					pm.AddToBackpack( new ArielHavenWritofMembership() );
				}
			}
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			CandlewoodTorch torch = m.FindItemOnLayer( Layer.TwoHanded ) as CandlewoodTorch;

			if ( torch != null && torch.Burning )
				BeginFlee( TimeSpan.FromSeconds( 5.0 ) );
		}

		private static Dictionary<Mobile, BloodDiseaseTimer> m_BloodDiseaseTable = new Dictionary<Mobile, BloodDiseaseTimer>();

		public static bool IsDiseased( Mobile m )
		{
			return m_BloodDiseaseTable.ContainsKey( m );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( !m_BloodDiseaseTable.ContainsKey( attacker ) && this.InRange( attacker, 1 ) && 0.25 > Utility.RandomDouble()/* && !FontOfFortune.HasBlessing( attacker, FontOfFortune.BlessingType.Protection )*/ )
			{
				// The rotworm has infected you with blood disease!!
				attacker.SendLocalizedMessage( 1111672, "", 0x25 );

				attacker.PlaySound( 0x213 );
				Effects.SendTargetParticles( attacker, 0x373A, 1, 15, 0x26B9, EffectLayer.Head );

				BloodDiseaseTimer timer = new BloodDiseaseTimer( attacker );
				timer.Start();

				m_BloodDiseaseTable.Add( attacker, timer );
			}
		}

		public override void OnAfterMove( Point3D oldLocation )
		{
			base.OnAfterMove( oldLocation );

			if ( Hits < HitsMax && 0.25 > Utility.RandomDouble() )
			{
				Corpse toAbsorb = null;

				foreach ( Item item in Map.GetItemsInRange( Location, 1 ) )
				{
					if ( item is Corpse )
					{
						Corpse c = (Corpse) item;

						if ( c.ItemID == 0x2006 )
						{
							toAbsorb = c;
							break;
						}
					}
				}

				if ( toAbsorb != null )
				{
					if ( toAbsorb.Owner == null || toAbsorb.Owner.Player )
					{
						// * The rotworm attempts to absorb the remains, but cannot! *
						PublicOverheadMessage( MessageType.Regular, 0x3B2, 1111666 );
					}
					else
					{
						toAbsorb.ProcessDelta();
						toAbsorb.SendRemovePacket();
						toAbsorb.ItemID = Utility.Random( 0xECA, 9 ); // bone graphic
						toAbsorb.Hue = 0;
						toAbsorb.Direction = Direction.North;
						toAbsorb.ProcessDelta();

						Hits = HitsMax;

						// * The rotworm absorbs the fleshy remains of the corpse *
						PublicOverheadMessage( MessageType.Regular, 0x3B2, 1111667 );
					}
				}
			}
		}

		private class BloodDiseaseTimer : Timer
		{
			private const int MaxCount = 8;

			private int m_Count;
			private Mobile m_Victim;

			public BloodDiseaseTimer( Mobile m )
				: base( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.0 ) )
			{
				m_Victim = m;
			}

			protected override void OnTick()
			{
				if ( m_Count == MaxCount || m_Victim.Deleted || !m_Victim.Alive || m_Victim.IsDeadBondedPet )
				{
					// You no longer feel sick.
					m_Victim.SendLocalizedMessage( 1111673 );

					m_BloodDiseaseTable.Remove( m_Victim );
					Stop();
				}
				else if ( m_Count > 0 )
				{
					AOS.Damage( m_Victim, Utility.RandomMinMax( 10, 20 ), 0, 0, 0, 100, 0 );
					m_Victim.Combatant = null;
				}

				m_Count++;
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}
}
