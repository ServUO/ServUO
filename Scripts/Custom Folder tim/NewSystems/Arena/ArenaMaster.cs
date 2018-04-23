using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "an Arena Champion corpse" )]
	public class ArenaMaster : BaseCreature
	{
		[Constructable]
		public ArenaMaster() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Arena Champion";
			Body = 400;

			SetStr( 600 );
			SetDex( 500 );
			SetInt( 4000 );

			SetHits( 3000 );
			SetMana( 5000 );
			SetStam( 300 );

			SetDamage( 50, 60 );

			SetDamageType( ResistanceType.Fire, 50 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 90);
			SetResistance( ResistanceType.Fire, 90);
			SetResistance( ResistanceType.Cold, 90);
			SetResistance( ResistanceType.Poison, 90);
			SetResistance( ResistanceType.Energy, 90);

			SetSkill( SkillName.MagicResist, 150.0 );
			SetSkill( SkillName.Tactics, 150.0 );
			SetSkill( SkillName.MagicResist, 150.0 );
			SetSkill( SkillName.Magery, 150.0 );
			SetSkill( SkillName.EvalInt, 150.0 );
			SetSkill( SkillName.Meditation, 150.0);
			SetSkill( SkillName.Focus, 150.0);
			SetSkill( SkillName.Wrestling, 150.0);

			VirtualArmor = 60;

			Item hat = new HatOfTheMagi();			
			hat.Hue = 1109;
			hat.Movable = false;
			
			Item gloves = new InquisitorsResolution();			
			gloves.Hue = 1109;
			gloves.Movable = false;
	
			Item sandals = new Sandals();		
			sandals.Hue = 1109;
			sandals.Movable = false;

			Item shroud = new HoodedShroudOfShadows();
			shroud.Movable = false;

			Item staffofpower = new StaffOfPower();
			staffofpower.Hue = 38;
			staffofpower.Movable = false;

			AddItem( hat );
			AddItem( gloves );	
			AddItem( shroud );
			AddItem( sandals );
			AddItem( staffofpower ); 
                
        }

		public override bool AlwaysMurderer { get { return true; } }
		public override bool Unprovokable{ get{ return true; } }
		public override bool Uncalmable{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 1; }
                 
		}

		public override bool OnBeforeDeath()
		{
			this.Say( "No!  I Cannot Be Defeated!!!" );
		{
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseHealer )
				{
					BaseHealer bc = (BaseHealer)m;
					
					if ( bc is LordBritish )
						list.Add( bc );
				}
			}
			foreach ( LordBritish wild in list )
				wild.Delete();
		}		

			LordBritish lb = new LordBritish();			
			lb.MoveToWorld( new Point3D( 2368, 1127, -90 ), Map.Malas );
			lb.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
			lb.PlaySound( 0x208 );
			lb.Say( "All Hail, A New Champion!!!!" );

			ArenaTreasureChest ac = new ArenaTreasureChest();			
			ac.MoveToWorld( new Point3D( 2369, 1127, -90 ), Map.Malas );

			ArenaExitMoongate ax = new ArenaExitMoongate();			
			ax.MoveToWorld( new Point3D( 2354, 1127, -90 ), Map.Malas );

			return true;
		}
		
		public void SpawnLichs( Mobile target )
		{
			Map map = this.Map;

			if ( map == null )
			{
				return;
			}

			this.Say( "I Am The Champion!!!" );

			int newLichs = Utility.RandomMinMax( 2, 4 );

			for ( int i = 0; i < newLichs; ++i )
			{
				Lich lich = new Lich();

				lich.Team = this.Team;
				lich.FightMode = FightMode.Closest;

				bool validLocation = false;
				Point3D loc = this.Location;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 3 ) - 1;
					int y = Y + Utility.Random( 3 ) - 1;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
					{
						loc = new Point3D( x, y, Z );
					}
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
					{
						loc = new Point3D( x, y, z );
					}
				}

				lich.MoveToWorld( loc, map );
				lich.Combatant = target;
			}
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( 0.1 >= Utility.RandomDouble() )
			{
				SpawnLichs( attacker );
			}
		}
		
		public ArenaMaster( Serial serial ) : base( serial )
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