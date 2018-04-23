using System;
using Server;
using Server.Mobiles;

namespace Carding.Mobiles
{
	[CorpseName( "a mustang corpse" )]
	public class Mustang : BaseMount
	{
		[ CommandProperty( AccessLevel.GameMaster ) ]
		public string MustangID
		{
			set
			{
				if ( string.IsNullOrEmpty(value) )
					return;

				MustangInfo info = MustangCollection.FromString( value );

				if ( info != null )
					info.ApplyTo( this );
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public string SkillLimit
		{
			set
			{
				if ( string.IsNullOrEmpty(value) )
					return;

				string[] vals = value.Split( '-' );

				if ( vals.Length != 2 )
					return;

				int min = -1;
				int max = -1;

				try
				{
					min = int.Parse( vals[ 0 ] );
					max = int.Parse( vals[ 1 ] );
				}
				catch
				{
					return;
				}

				MustangInfo info = MustangCollection.FromSkills( min, max );

				if ( info != null )
					info.ApplyTo( this );
			}
		}

		[Constructable]
		public Mustang() : this( MustangCollection.Randomize() )
		{
		}

		[ Constructable ]
		public Mustang( int minSkill, int maxSkill ) : this( MustangCollection.FromSkills( minSkill, maxSkill ) )
		{
		}

		[Constructable]
		public Mustang( string name ) : this( MustangCollection.FromString( name ) )
		{
		}

        public Mustang(MustangInfo info) : base(info.Name, 0xCC, 0x3EA2, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
			BaseSoundID = 0xA8;

			SetStr( 100, 180 );
			SetDex( 90, 130 );
			SetInt( 6, 10 );

			SetHits( 100, 180 );
			SetMana( 0 );

			SetDamage( 6, 8 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 40.0 );
			SetSkill( SkillName.Tactics, 40.0 );
			SetSkill( SkillName.Wrestling, 50.0 );

			Fame = 600;
			Karma = -266;

			Tamable = true;
			ControlSlots = 2;

			info.ApplyTo( this );
		}

		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }


		public Mustang( Serial serial ) : base( serial )
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

				//0xC8, 0x3E9F - Light brown
				//0xE2, 0x3EA0 - Light Grey
				//0xE4, 0x3EA1 - Brown/Grey with light nose
				//0xCC, 0x3EA2 - Dark Brown