
//////////////////////
//Created by KyleMan//
//////////////////////
using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class Vegeta : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }
		
		[Constructable]
		public Vegeta() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			SpeechHue = Utility.RandomDyedHue();
			Title = "the prince of all saiyans";
			Hue = 33770;

			{
				Body = 0x190;
				Name = "Vegeta";
			}

			SetStr( 250, 255 );
			SetDex( 100, 125 );
			SetInt( 61, 75 );

			SetDamage( 15, 23 );

			SetHits( 2500, 3500 );

			SetSkill( SkillName.Fencing, 88.8, 97.5 );
			SetSkill( SkillName.Macing, 99.9, 110.0 );
			SetSkill( SkillName.MagicResist, 25.0, 47.5 );
			SetSkill( SkillName.Swords, 65.0, 87.5 );
			SetSkill( SkillName.Tactics, 99.9, 110.0 );
			SetSkill( SkillName.Wrestling, 15.0, 37.5 );
			SetSkill( SkillName.Anatomy, 80.0, 90.1 );
			SetSkill( SkillName.Parry, 80.0, 100.0 );
			SetSkill( SkillName.Lumberjacking, 80.0, 100.0 );

			Fame = 10000;
			Karma = -10000;

			PackItem( new VegetasDeath() );

			AddItem( new VegetasPants() );
			AddItem( new VegetasOutterShirt() );
			AddItem( new VegetasUnderShirt() );
			AddItem( new VegetasBoots() );
			AddItem( new VegetasAxe() ); //Change to AddItem( new VegetasAxeUnblessed() ); for the axe to be in loot.

			AddItem( new ShortHair( 1 ) );
		}
									
		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss );
		}

		public override bool AlwaysMurderer{ get{ return true; } }

		public Vegeta( Serial serial ) : base( serial )
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