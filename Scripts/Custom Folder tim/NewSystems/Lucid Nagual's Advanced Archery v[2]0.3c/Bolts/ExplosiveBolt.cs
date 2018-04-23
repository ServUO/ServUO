using System;

namespace Server.Items
{
	public class ExplosiveBolt : Item, ICommodity
	{
		// string ICommodity.Description
		// {
			// get
			// {
				// return String.Format( Amount == 1 ? "{0} explosivebolt" : "{0} explosivebolts", Amount );
			// }
		// }
		
		int ICommodity.DescriptionNumber
        {
            get
            {
                return 1116351; // Explosive
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
		public ExplosiveBolt() : this( 1 )
		{
		}

		[Constructable]
		public ExplosiveBolt( int amount ) : base( 0x1BFB )
		{
			Name = "Explosive Bolt";
			Hue = 32;
			Stackable = true;
			Weight = 0.1;
			Amount = amount;
		}

		public ExplosiveBolt( Serial serial ) : base( serial )
		{
		}

		// public override Item Dupe( int amount )
		// {
			// return base.Dupe( new ExplosiveBolt( amount ), amount );
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
