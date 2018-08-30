using System;

namespace Server.Items
{
	public class LightningArrowAmmo : Item, ICommodity
	{
		// string ICommodity.Description
		// {
			// get
			// {
				// return String.Format( Amount == 1 ? "{0} lightningarrow" : "{0} lightningarrows", Amount );
			// }
		// }
		
		int ICommodity.DescriptionNumber
        {
            get
            {
                return 1015190; // Lightning
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
		public LightningArrowAmmo() : this( 1 )
		{
		}

		[Constructable]
		public LightningArrowAmmo( int amount ) : base( 0xF3F )
		{
			Stackable = true;
			Name = "Lightning Arrow";
			Hue = 1174;
			Weight = 0.1;
			Amount = amount;
		}

		public LightningArrowAmmo( Serial serial ) : base( serial )
		{
		}

		// public override Item Dupe( int amount )
		// {
			// return base.Dupe( new LightningArrowAmmo( amount ), amount );
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
