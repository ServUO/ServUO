using System; 
using Server; 

namespace Server.Items
{ 
	public class MageHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public MageHalf()
		{
			Name = "The Mage";
			Hue = 1152;

			Attributes.BonusInt = 25;

			Attributes.CastSpeed = 2;
			Attributes.CastRecovery = 2;
		}

		public MageHalf( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
} 