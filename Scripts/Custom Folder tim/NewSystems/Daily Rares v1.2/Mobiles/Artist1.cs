using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using System.Collections.Generic;
namespace Server.Mobiles
{
	public class Artist1 : BaseCreature
	{
		public override bool AlwaysAttackable{ get{ return true; } }
		public override bool InitialInnocent{ get{ return true; } }

		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
		public Artist1() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the artist";
			Hue = Utility.RandomSkinHue();

			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				AddItem( new Skirt( Utility.RandomNeutralHue() ) );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
			}

			SetStr( 86, 100 );
			SetDex( 81, 95 );
			SetInt( 61, 75 );

			SetDamage( 10, 23 );

			SetSkill( SkillName.Tactics, 65.0, 87.5 );
			SetSkill( SkillName.Wrestling, 15.0, 37.5 );

			Fame = 1000;
			Karma = 1000;

			AddItem( new HalfApron());

			switch ( Utility.Random( 2 ) )
			{
				case 0: AddItem( new FancyShirt( Utility.RandomNeutralHue() ) ); break;
				case 1: AddItem( new Shirt( Utility.RandomNeutralHue() ) ); break;
			}

			if ( Utility.Random( 100 ) < 2 )
			{
				Sandals sandals = new Sandals();
				sandals.Hue = 1;
				AddItem( sandals );
			}
			else
			{
				switch ( Utility.Random( 3 ) )
				{
					case 0: AddItem( new Sandals( Utility.RandomNeutralHue() ) );; break;
					case 1: AddItem( new Sandals( Utility.RandomNondyedHue() ) ); break;
					case 2: AddItem( new Shoes() );; break;
				}
			}

			PackItem( new PaintsAndBrush() );
			//AddItem( Server.Items.Hair.GetRandomHair( Female ) );
		      Utility.AssignRandomHair( this );
            }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public Artist1( Serial serial ) : base( serial )
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