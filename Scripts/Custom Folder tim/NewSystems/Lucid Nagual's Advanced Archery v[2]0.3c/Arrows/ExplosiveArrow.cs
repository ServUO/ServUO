using System;

namespace Server.Items
{
	public class ExplosiveArrow : Item, ICommodity
	{
		// string ICommodity.Description
		// {
			// get
			// {
				// return String.Format( Amount == 1 ? "{0} explosivearrow" : "{0} explosivearrows", Amount );
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
		public ExplosiveArrow() : this( 1 )
		{
		}

		[Constructable]
		public ExplosiveArrow( int amount ) : base( 0xF3F )
		{
			Stackable = true;
			Name = "Explosive Arrow";
			Hue = 32;
			Weight = 0.1;
			Amount = amount;
		}

		public ExplosiveArrow( Serial serial ) : base( serial )
		{
		}

		// public override Item Dupe( int amount )
		// {
			// return base.Dupe( new ExplosiveArrow( amount ), amount );
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