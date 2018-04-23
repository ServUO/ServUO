using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class Corsair : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }

		[Constructable]
		public Corsair() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "male" );
			SpeechHue = Utility.RandomDyedHue();
			Title = "the corsair";
			Hue = Utility.RandomSkinHue();
			Body = 0x190;

			SetStr( 86, 100 );
			SetDex( 101, 125 );
			SetInt( 81, 105 );

			SetDamage( 12, 24 );

			SetSkill( SkillName.Fencing, 66.0, 97.5 );
			SetSkill( SkillName.Macing, 65.0, 87.5 );
			SetSkill( SkillName.MagicResist, 25.0, 47.5 );
			SetSkill( SkillName.Swords, 65.0, 87.5 );
			SetSkill( SkillName.Tactics, 65.0, 87.5 );
			SetSkill( SkillName.Wrestling, 15.0, 37.5 );

			Fame = 1000;
			Karma = -1000;

			AddItem( new Boots( Utility.RandomNeutralHue() ) );
			AddItem( new FancyShirt( Utility.RandomBirdHue() ) );
			AddItem( new Bandana( Utility.RandomBirdHue() ) );
			AddItem( new TattsukeHakama( Utility.RandomBirdHue() ) );

			switch ( Utility.Random( 3 ))
			{
				case 0: AddItem( new Pickaxe() ); break;
				case 1: AddItem( new Cutlass() ); break;
				case 2: AddItem( new Broadsword() ); break;
			}

			Utility.AssignRandomHair( this );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public Corsair( Serial serial ) : base( serial )
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