using System;
using Server;

namespace Server.Items
{
	public class VoidWandererCostume : BaseCostume
	{
        public override string CreatureName { get { return "wanderer of the void"; } }

        [Constructable]
		public VoidWandererCostume() : base( )
		{
            this.CostumeBody = 316;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114286;
            }
        }// void wanderer costume

		public VoidWandererCostume( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
