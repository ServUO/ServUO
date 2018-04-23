//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingBakeKitsune corpse" )]
	public class TamingBakeKitsune : BaseTamingCreature
	{
		[Constructable]
		public TamingBakeKitsune( Mobile from ) : base( from )
		{
			Name = "a Taming Bake Kitsune";
			Body = 246;
			BaseSoundID = 0x4DD;

			SetStr( 90 );
			SetDex( 90 );
			SetInt( 90 );

			SetHits( 90 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 90.0 );
			SetSkill( SkillName.Tactics, 90.0 );
			SetSkill( SkillName.Wrestling, 90.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = 90.1;
		}

		public TamingBakeKitsune(Serial serial) : base(serial)
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




