//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingGiantToad corpse" )]
	public class TamingGiantToad : BaseTamingCreature
	{
		[Constructable]
		public TamingGiantToad( Mobile from ) : base( from )
		{
			Name = "a Taming Giant Toad";
			Body = 80;
			BaseSoundID = 0x26B;

			SetStr( 80 );
			SetDex( 80 );
			SetInt( 80 );

			SetHits( 80 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 80.0 );
			SetSkill( SkillName.Tactics, 80.0 );
			SetSkill( SkillName.Wrestling, 80.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = 80.1;
		}

		public TamingGiantToad(Serial serial) : base(serial)
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



