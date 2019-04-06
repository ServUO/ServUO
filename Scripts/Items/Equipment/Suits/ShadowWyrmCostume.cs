using System;
using Server;

namespace Server.Items
{
	public class ShadowWyrmCostume : BaseCostume
	{
        public override string CreatureName { get { return "shadow wyrm"; } }

        [Constructable]
		public ShadowWyrmCostume() : base( )
		{
            CostumeBody = 106;
            CostumeHue = 0;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114009;
            }
        }// shadow wyrm halloween costume

		public ShadowWyrmCostume( Serial serial ) : base( serial )
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
