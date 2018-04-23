using System;

namespace Server.Items
{
	public class ArmorPiercingBolt : Item, ICommodity
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
                return 1028860; // Armor Pierce
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
		public ArmorPiercingBolt() : this( 1 )
		{
		}

		[Constructable]
		public ArmorPiercingBolt( int amount ) : base( 0x1BFB )
		{
			Name = "Armor Piercing Bolt";
			Hue = 1153;
			Stackable = true;
			Weight = 0.1;
			Amount = amount;
		}

		public ArmorPiercingBolt( Serial serial ) : base( serial )
		{
		}

		// public override Item Dupe( int amount )
		// {
			// return base.Dupe( new ArmorPiercingBolt( amount ), amount );
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
