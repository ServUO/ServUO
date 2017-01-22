using System;
using Server.Items;

namespace Server.Items
{
	public class WorkableGlass : Item, ICommodity
	{
		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return true; } }

        public override int LabelNumber { get { return 1154170; } } // workable glass

		[Constructable]
		public WorkableGlass() : this( 1 )
		{
		}

		[Constructable]
		public WorkableGlass( int amount ) : base( 19328 )
		{
			Stackable = true;
			Weight = 1.0;
		}

        public WorkableGlass(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}