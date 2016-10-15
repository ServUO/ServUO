using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class MagicalDoor : Item
	{
        [Constructable]
        public MagicalDoor() : this(Utility.RandomList(7905, 7914, 7923, 7932))
        {
        }

		public MagicalDoor(int id) : base( id )
		{
		}

        public override int LabelNumber { get { return 1112410; } } //Magical Door [Replica]

        public override int ItemID
        {
            get
            {
                if (Parent == null && Map != Map.Internal)
                    return base.ItemID - 8;

                return base.ItemID;
            }
            set 
            {
                base.ItemID = value;
            }
        }

		public MagicalDoor( Serial serial ) : base( serial )
		{
		}

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
