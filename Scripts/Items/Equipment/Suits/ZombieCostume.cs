using System;
using Server;

namespace Server.Items
{
	public class ZombieCostume : BaseCostume
	{
        public override string CreatureName { get { return "zombie"; } }

        [Constructable]
		public ZombieCostume() : base( )
		{
            this.CostumeBody = 3;
		}
		
		public override int LabelNumber
        {
            get
            {
                return 1114222;
            }
        }// zombie costume

		public ZombieCostume( Serial serial ) : base( serial )
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
