//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingBrownBear corpse" )]
	public class TamingBrownBear : BaseTamingCreature
	{
		[Constructable]
		public TamingBrownBear( Mobile from ) : base( from )
		{
			Name = "a Taming Brown Bear";
			Body = 167;
			BaseSoundID = 0xA3;

			SetStr( 41 );
			SetDex( 41 );
			SetInt( 41 );

			SetHits( 41 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 41.0 );
			SetSkill( SkillName.Tactics,41.0 );
			SetSkill( SkillName.Wrestling, 41.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = 41.1;
		}

		public TamingBrownBear(Serial serial) : base(serial)
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




