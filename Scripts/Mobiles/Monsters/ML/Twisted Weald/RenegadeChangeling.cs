using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Spells;
using Server.Spells.Third;
using Server.Spells.Sixth;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a renegade changeling corpse" )]
	public class RenegadeChangeling : Changeling
	{
		[Constructable]
		public RenegadeChangeling()
		{
			Name = "a renegade changeling";
			Body = 264;
            BaseSoundID = 0x470;

			SetStr( 180, 200 );
			SetDex( 300, 330 );
			SetInt( 500, 550 );

			SetHits( 2500, 3000 );
			SetStam( 212, 262 );
			SetMana( 317, 399 );

			SetDamage( 22, 24 );

			SetDamageType( ResistanceType.Physical, 100 );
			
			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Wrestling, 120.0, 130.0 );
            SetSkill(SkillName.Tactics, 120.0, 130.0);
            SetSkill(SkillName.MagicResist, 120.0, 150.0);
			SetSkill( SkillName.Magery, 110, 120 );
            SetSkill(SkillName.EvalInt, 110, 120);
            SetSkill(SkillName.Meditation, 110, 120);
			
			for(int i = 0; i < Utility.RandomMinMax(1, 7); i++)
            {
                PackItem(Loot.RandomScroll(0, Loot.RegularScrollTypes.Length, SpellbookType.Regular));
            }

			PackItem( new Arrow( 35 ) );
			PackItem( new Bolt( 25 ) );			
			PackGem( 2 );

            Fame = 18900;
            Karma = -18900;
		}

        public RenegadeChangeling(Serial serial)
            : base(serial)
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 3 );
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

