//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingGrizzlyBear corpse" )]
	public class TamingGrizzlyBear : BaseTamingCreature
	{
		[Constructable]
		public TamingGrizzlyBear( Mobile from ) : base( from )
		{
			Name = "a Taming Grizzly Bear";
			Body = 212;
			BaseSoundID = 0xA3;

			SetStr( 65 );
			SetDex( 65 );
			SetInt( 65 );

			SetHits( 65 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 65.0 );
			SetSkill( SkillName.Tactics, 65.0 );
			SetSkill( SkillName.Wrestling, 65.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = 65.1;
		}

		public TamingGrizzlyBear(Serial serial) : base(serial)
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


