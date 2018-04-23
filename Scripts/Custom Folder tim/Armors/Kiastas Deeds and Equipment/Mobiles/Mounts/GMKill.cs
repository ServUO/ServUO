using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Kiasta;

namespace Server.Kiasta.Mobiles
{
	[CorpseName( "a horse corpse" )]
	[TypeAlias( "Server.Mobiles.BrownHorse", "Server.Mobiles.DirtyHorse", "Server.Mobiles.GrayHorse", "Server.Mobiles.TanHorse" )]
	public class GMKill : BaseMount
	{
		private static int[] m_IDs = new int[]
			{
				0xC8, 0x3E9F,
				0xE2, 0x3EA0,
				0xE4, 0x3EA1,
				0xCC, 0x3EA2
			};

		[Constructable]
		public GMKill() : this( "a horse" )
		{
		}

		[Constructable]
		public GMKill( string name ) : base( name, 0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			int random = Utility.Random( 4 );

			Body = m_IDs[random * 2];
			ItemID = m_IDs[random * 2 + 1];
			BaseSoundID = 0xA8;

			SetStr( 2200, 9800 );
			SetDex( 5600, 7500 );
			SetInt( 600, 1000 );

			SetHits( 28000, 45000 );
			SetMana( 0 );

			SetDamage( 3000, 4000 );

			SetDamageType( ResistanceType.Physical, 10000 );

			SetResistance( ResistanceType.Physical, 15000, 20000 );

			SetSkill( SkillName.MagicResist, 250.0, 300.0 );
			SetSkill( SkillName.Tactics, 300.0, 450.0 );
			SetSkill( SkillName.Wrestling, 300.0, 450.0 );

			Fame = 300;
			Karma = 300;

			Tamable = true;
			ControlSlots = 100;
            MinTameSkill = 300;

            PackItem(Kiasta.Settings.Misc.BlackDragoonBag);
            PackItem(Kiasta.Settings.Misc.BlueDragoonBag);
            PackItem(Kiasta.Settings.Misc.GreenDragoonBag);
            PackItem(Kiasta.Settings.Misc.RandomDragoonColorBag);
            PackItem(Kiasta.Settings.Misc.RedDragoonBag);
            PackItem(Kiasta.Settings.Misc.WhiteDragoonBag);
            PackItem(Kiasta.Settings.Misc.YellowDragoonBag);
            PackItem(Kiasta.Settings.Misc.ItemStatDeedsBag);
            PackItem(Kiasta.Settings.Misc.SlayerDeedsBag);
		}

        /*public override void GenerateLoot()
        {
            
        }*/

		//public override int Meat{ get{ return 3; } }
		//public override int Hides{ get{ return 10; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public GMKill( Serial serial ) : base( serial )
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
