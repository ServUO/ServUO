//Based on three scripts from RunUO Distro 1.0  Thanks RunUO!!!!
//Created by Ashlar, beloved of Morrigan
using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a TamingAlligator corpse" )]
	public class TamingAlligator : BaseTamingCreature
	{
		[Constructable]
		public TamingAlligator( Mobile from ) : base( from )
		{
			Name = "a Taming Alligator";
			Body = 0xCA;
			BaseSoundID = 660;

			SetStr( 47 );
			SetDex( 47 );
			SetInt( 47 );

			SetHits( 47 );
			SetMana( 0 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 1, 5 );

			SetSkill( SkillName.MagicResist, 47.0 );
			SetSkill( SkillName.Tactics, 47.0 );
			SetSkill( SkillName.Wrestling, 47.0 );

			Fame = 150;
			Karma = 0;

			VirtualArmor = 2;

			MinTameSkill = 47.1;
		}

		public TamingAlligator(Serial serial) : base(serial)
		{
		}

		public override int GetAttackSound()
		{
			return 0x82;
		}

		public override int GetHurtSound()
		{
			return 0x83;
		}

		public override int GetDeathSound()
		{
			return 0x84;
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





