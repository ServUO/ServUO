using System;
using Server;

namespace Server.Items
{
	public class FireElementalCostume : BaseCostume
	{
        public override string CreatureName { get { return "fire elemental"; } }

        [Constructable]
		public FireElementalCostume() : base( )
		{
            this.CostumeBody = 15;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114224;
            }
        }// fire elemental costume

		public FireElementalCostume( Serial serial ) : base( serial )
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
