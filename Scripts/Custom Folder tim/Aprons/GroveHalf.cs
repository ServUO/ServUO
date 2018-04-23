using System; 
using Server; 

namespace Server.Items
{ 
	public class GroveHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public GroveHalf()

		{
			Name = "Grove Apron";
			Hue = 1153;

			Attributes.AttackChance = 10;
            Attributes.Luck = 250;
			Attributes.DefendChance = 10;
		}

		public GroveHalf( Serial serial ) : base( serial )
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