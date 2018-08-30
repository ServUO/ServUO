using System;
using Server;

namespace Server.Items
{
	public class MagesBrace : GoldBracelet
	{
		public override int ArtifactRarity{ get{ return 11; } }

		[Constructable]
		public MagesBrace()
		{
			Name = "Mages Brace";
			Hue = 34;
			Attributes.CastRecovery = 1;
			Attributes.CastSpeed = 1;
			Attributes.LowerManaCost = 20;
			Attributes.LowerRegCost = 20;
		}

		public MagesBrace( Serial serial ) : base( serial )
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