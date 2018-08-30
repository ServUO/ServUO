//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingSheep corpse" )]
	public class TamingSheep : BaseTamingCreature
	{
		[Constructable]
		public TamingSheep( Mobile from ) : base( from )
		{
			Name = "a Taming Sheep";
			Body = 0xCF;
			BaseSoundID = 0xD6;

			SetStr( 11 );
			SetDex( 11 );
			SetInt( 11 );

			SetHits( 11 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 11.0 );
			SetSkill( SkillName.Tactics, 11.0 );
			SetSkill( SkillName.Wrestling, 11.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = 11.1;
		}

		public TamingSheep(Serial serial) : base(serial)
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

