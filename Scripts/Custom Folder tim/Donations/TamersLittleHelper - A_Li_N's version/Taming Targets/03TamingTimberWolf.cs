//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingTimberWolf corpse" )]
	public class TamingTimberWolf : BaseTamingCreature
	{
		[Constructable]
		public TamingTimberWolf( Mobile from ) : base( from )
		{
			Name = "a Taming Timber Wolf";
			Body = 225;
			BaseSoundID = 0xE5;

			SetStr( 23 );
			SetDex( 23 );
			SetInt( 23 );

			SetHits( 23 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 23.1 );
			SetSkill( SkillName.Tactics, 23.1 );
			SetSkill( SkillName.Wrestling, 23.1 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = 23.1;
		}

		public TamingTimberWolf(Serial serial) : base(serial)
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


