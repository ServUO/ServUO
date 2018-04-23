using System;
using Server;

namespace Server.Items
{
	public class MageRoyal : LeatherGorget
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public MageRoyal()
		{
			Name = "Mage Royal";
			Hue = 11;
			Attributes.CastRecovery = 3;
			Attributes.CastSpeed = 1;
			Attributes.BonusInt = 8;
			Attributes.RegenStam = 3;
			Attributes.LowerRegCost = 50;
		}

		public MageRoyal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}