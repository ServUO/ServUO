using System;
using Server;

namespace Server.Items
{
	public class CyclopsCostume : BaseCostume
	{
        [Constructable]
        public CyclopsCostume() : base()
        {
            Name = "a Cyclops halloween Costume";
            this.CostumeBody = 75;
        }

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
