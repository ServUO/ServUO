using System;
using Server;

namespace Server.Items
{
	public class SkitteringHopperCostume : BaseCostume
	{
        public override string CreatureName { get { return "skittering hopper"; } }

        [Constructable]
		public SkitteringHopperCostume() : base( )
		{
            this.CostumeBody = 302;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114240;
            }
        }// skittering hopper costume

		public SkitteringHopperCostume( Serial serial ) : base( serial )
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
