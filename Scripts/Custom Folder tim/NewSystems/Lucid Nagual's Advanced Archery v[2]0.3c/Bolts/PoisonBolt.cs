using System;

namespace Server.Items
{
	public class PoisonBolt : Item, ICommodity
	{
		// string ICommodity.Description
		// {
			// get
			// {
				// return String.Format( Amount == 1 ? "{0} poisonarrow" : "{0} poisonarrows", Amount );
			// }
		// }
		
		int ICommodity.DescriptionNumber
        {
            get
            {
                return 3002030; // Poison
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }

		[Constructable]
		public PoisonBolt() : this( 1 )
		{
		}

		[Constructable]
		public PoisonBolt( int amount ) : base( 0x1BFB )
		{
			Name = "Poison Bolt";
			Hue = 69;
			Stackable = true;
			Weight = 0.1;
			Amount = amount;
		}

		public PoisonBolt( Serial serial ) : base( serial )
		{
		}

		// public override Item Dupe( int amount )
		// {
			// return base.Dupe( new PoisonBolt( amount ), amount );
		// }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
