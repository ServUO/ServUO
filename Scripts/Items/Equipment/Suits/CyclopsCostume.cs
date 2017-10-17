using System;
using Server;

namespace Server.Items
{
	public class CyclopsCostume : BaseCostume
	{
        [Constructable]
        public CyclopsCostume() : base()
        {
            this.CostumeBody = 75;
        }
		
		public override int LabelNumber
        {
            get
            {
                return 1114234;
            }
        }// cyclops costume

		public CyclopsCostume( Serial serial ) : base( serial )
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
