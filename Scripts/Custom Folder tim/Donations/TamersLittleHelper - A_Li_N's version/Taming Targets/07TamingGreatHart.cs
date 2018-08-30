//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingGreatHart corpse" )]
	public class TamingGreatHart : BaseTamingCreature
	{
		[Constructable]
		public TamingGreatHart( Mobile from ) : base( from )
		{
			Name = "a Taming Great Hart";
			Body = 0xEA;
			BaseSoundID = 0x82;

			SetStr( 59 );
			SetDex( 59 );
			SetInt( 59 );

			SetHits( 59 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 59.0 );
			SetSkill( SkillName.Tactics, 59.0 );
			SetSkill( SkillName.Wrestling, 59.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = 59.1;
		}

		public TamingGreatHart(Serial serial) : base(serial)
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

