//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingMountainGoat corpse" )]
	public class TamingMountainGoat : BaseTamingCreature
	{
		[Constructable]
		public TamingMountainGoat( Mobile from ) : base( from )
		{
			Name = "a Taming Mountain Goat";
			Body = 88;
			BaseSoundID = 0x99;

			SetStr( 10 );
			SetDex( 10 );
			SetInt( 10 );

			SetHits( 3 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 10.0 );
			SetSkill( SkillName.Tactics, 10.0 );
			SetSkill( SkillName.Wrestling, 10.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = -0.9;
		}

		public TamingMountainGoat(Serial serial) : base(serial)
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

