using System;
using Server;

namespace Server.Items
{
	public class GiantPixieCostume : BaseCostume
	{
        public override string CreatureName { get { return "giant pixie"; } }

        [Constructable]
		public GiantPixieCostume() : base( )
		{
            this.CostumeBody = 176;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114244;
            }
        }// giant pixie costume

		public GiantPixieCostume( Serial serial ) : base( serial )
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
