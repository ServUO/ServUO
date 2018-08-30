using System;
using Server;

namespace Server.Items
{
	public class ArmsOfReplentishment : LeatherArms
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ArmsOfReplentishment()
		{
			Name = "Arms Of Replentishment";
			Hue = 52;
			Attributes.RegenHits = 10;
			Attributes.RegenStam = 5;
			Attributes.BonusHits = 15;
			Attributes.RegenMana = 10;
		}

		public ArmsOfReplentishment( Serial serial ) : base( serial )
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