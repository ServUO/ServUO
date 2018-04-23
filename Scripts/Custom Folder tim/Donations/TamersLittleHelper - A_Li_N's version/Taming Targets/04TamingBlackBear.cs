//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingBlackBear corpse" )]
	public class TamingBlackBear : BaseTamingCreature
	{
		[Constructable]
		public TamingBlackBear( Mobile from ) : base( from )
		{
			Name = "a Taming Black Bear";
			Body = 211;
			BaseSoundID = 0xA3;

			SetStr( 35 );
			SetDex( 35 );
			SetInt( 35 );

			SetHits( 35 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 35.0 );
			SetSkill( SkillName.Tactics, 35.0 );
			SetSkill( SkillName.Wrestling, 35.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = 35.1;
		}

		public TamingBlackBear(Serial serial) : base(serial)
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



