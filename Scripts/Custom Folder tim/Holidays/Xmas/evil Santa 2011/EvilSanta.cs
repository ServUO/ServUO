using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Fifth;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	public class EvilSanta : BaseCreature
	{

		public override WeaponAbility GetWeaponAbility()
		{
			return Utility.RandomBool() ? WeaponAbility.MortalStrike : WeaponAbility.WhirlwindAttack;
		}

		public override bool IgnoreYoungProtection { get { return Core.ML; } }

		[Constructable]
		public EvilSanta() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4  )
		{
			Name = " The Evil Santa";
			Title = "of Chaos";
			Body = 400;
			Hue = 33770;
			BaseSoundID = 343;

			SetStr( 500, 625 );
			SetDex( 300, 550 );
			SetInt( 1000, 2000 );

			SetHits( 10000 );
			SetStam( 500, 700 );

			SetDamage( 50, 120 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 60, 70 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 97.6, 100.0 );
  

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 70;



                 FacialHairItemID = 8267;
                  FacialHairHue = 1153;

			LongPants legs = new LongPants();
			legs.Hue = 1157;
			legs.Movable = false;
			AddItem( legs );

			FancyShirt chest = new FancyShirt();
			chest.Hue = 1157;
			chest.Movable = false;
			AddItem( chest );

			LeatherGloves gloves = new LeatherGloves();
			gloves.Hue = 1153;
			gloves.Movable = false;
			AddItem( gloves );

			ElvenBoots boots = new ElvenBoots();
			boots.Hue = 1153;
			boots.Movable = false;
			AddItem( boots );

                  Item hair = new Item( Utility.RandomList( 8252 ) );
			hair.Hue = 1153;
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem( hair );
			
			BodySash bodysash = new BodySash();
			bodysash.Hue = 1153;
			bodysash.Movable = false;
			AddItem ( bodysash );
			
			HalfApron halfapron = new HalfApron();
			halfapron.Hue = 1153;
			halfapron.Movable = false;
			AddItem ( halfapron );

			Cloak cloak = new Cloak();
			cloak.Hue = 1153;
			cloak.Movable = false;
			AddItem ( cloak );
					
			PackGold( 6000, 10000);

         }

            public override void OnDeath( Container c )
		{
			base.OnDeath( c );
                // idea is to drop the Statue - Kenshin.
            int drop = Utility.Random(1, 1);

            for (int i = 0; i < drop; i++)

                if(Utility.RandomDouble() < 0.5) // 40% chance of getting this piece - Kenshin.
                    c.DropItem(new SantaStatuette()); 
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 3 );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override double AutoDispelChance{ get{ return 1.0; } }
		public override bool BardImmune{ get{ return !Core.SE; } }
		public override bool Unprovokable{ get{ return Core.SE; } }
		public override bool Uncalmable{ get{ return Core.SE; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }

		public override bool ShowFameTitle{ get{ return false; } }
		public override bool ClickTitle{ get{ return false; } }

		public void Polymorph( Mobile m )
		{
			if ( !m.CanBeginAction( typeof( PolymorphSpell ) ) || !m.CanBeginAction( typeof( IncognitoSpell ) ) || m.IsBodyMod )
				return;

			IMount mount = m.Mount;

			if ( mount != null )
				mount.Rider = null;

			if ( m.Mounted )
				return;

			if ( m.BeginAction( typeof( PolymorphSpell ) ) )
			{
				Item disarm = m.FindItemOnLayer( Layer.OneHanded );

				if ( disarm != null && disarm.Movable )
					m.AddToBackpack( disarm );

				disarm = m.FindItemOnLayer( Layer.TwoHanded );

				if ( disarm != null && disarm.Movable )
					m.AddToBackpack( disarm );

				m.BodyMod = 999;
				m.HueMod = 0;

				new ExpirePolymorphTimer( m ).Start();
			}
		}

		private class ExpirePolymorphTimer : Timer
		{
			private Mobile m_Owner;

			public ExpirePolymorphTimer( Mobile owner ) : base( TimeSpan.FromMinutes( 3.0 ) )
			{
				m_Owner = owner;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if ( !m_Owner.CanBeginAction( typeof( PolymorphSpell ) ) )
				{
					m_Owner.BodyMod = 0;
					m_Owner.HueMod = -1;
					m_Owner.EndAction( typeof( PolymorphSpell ) );
				}
			}
		}

		public void SpawnEvilMinionone( Mobile target )
		{
			Map map = this.Map;

			if ( map == null )
				return;

			int humans = 0;

			foreach ( Mobile m in this.GetMobilesInRange( 10 ) )
			{
				if ( m is EvilMinionone || m is EvilMiniontwo || m is EvilMinionone )
					++humans;
			}

			if ( humans < 16 )
			{
				PlaySound( 0x3D );

				int newhumans = Utility.RandomMinMax( 3, 6 );

				for ( int i = 0; i < newhumans; ++i )
				{
					BaseCreature human;

					switch ( Utility.Random( 5 ) )
					{
						default:
						case 0: case 1:	human = new EvilMinionone(); break;
						case 2: case 3:	human = new EvilMiniontwo(); break;
						case 4:			human = new EvilMinionone(); break;
					}

					human.Team = this.Team;

					bool validLocation = false;
					Point3D loc = this.Location;

					for ( int j = 0; !validLocation && j < 10; ++j )
					{
						int x = X + Utility.Random( 3 ) - 1;
						int y = Y + Utility.Random( 3 ) - 1;
						int z = map.GetAverageZ( x, y );

						if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
							loc = new Point3D( x, y, Z );
						else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
							loc = new Point3D( x, y, z );
					}

					human.MoveToWorld( loc, map );
					human.Combatant = target;
				}
			}
		}

		public void DoSpecialAbility( Mobile target )
		{
			if ( target == null || target.Deleted ) //sanity
				return;
			if ( 0.6 >= Utility.RandomDouble() ) // 60% chance to polymorph attacker into a Minion
				Polymorph( target );

			if ( 0.2 >= Utility.RandomDouble() ) // 20% chance to more Minion
				SpawnEvilMinionone( target );

			if ( Hits < 500 && !IsBodyMod ) // Evil Santa is low on life, polymorph into a Minion
				Polymorph( this );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			DoSpecialAbility( attacker );
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			DoSpecialAbility( defender );
		}

		public EvilSanta( Serial serial ) : base( serial )
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