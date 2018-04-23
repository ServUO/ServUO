using System;
using Server;

namespace Server.Items
{
	public class GhostlyShield : BaseShield
	{
		public override int BasePhysicalResistance{ get{ return 0; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 4; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 0; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override int AosStrReq{ get{ return 20; } }

		public override int ArmorBase{ get{ return 7; } }

		[Constructable]
		public GhostlyShield() : base( 0x1B73 )
		{
			Weight = 5.0;
			Hue = 16385;
			Name = "Ghostly Shield";
			Attributes.SpellChanneling = 1;
			SkillBonuses.SetValues( 0, SkillName.SpiritSpeak, 10.0 );
		}

		public GhostlyShield( Serial serial ) : base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}
	}
}
