using System;
using Server;

namespace Server.Items
{
	public class DrakeCostume : BaseCostume
	{
		[Constructable]
		public DrakeCostume() : base( )
		{
            this.CostumeBody = 60;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114245;
            }
        }// drake costume

		public DrakeCostume( Serial serial ) : base( serial )
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
