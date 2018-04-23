using System; 
using Server; 

namespace Server.Items
{ 
	public class LuckyHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public LuckyHalf()
                {

			Name = "-Your Lucky-";
			Hue = 1177;
			
			Attributes.Luck = 1000;
          
		}

		public LuckyHalf( Serial serial ) : base( serial )
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