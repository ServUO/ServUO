
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x153B, 0x153C )]
	public class Ltsash : BaseMiddleTorso
	{
 		public override void GetProperties(ObjectPropertyList list)
		{
		base.GetProperties(list); list.Add(1075085);
		}

              	[Constructable]
              	public Ltsash() : base ( 0x1541 )
		{
			Weight = 2.0;
			Name = "Lieutenant of the Britannian Royal Guard";
			Hue = 232;

			Attributes.BonusInt = 5;
			Attributes.LowerRegCost = 10;
			Attributes.RegenMana = 2; 
		}

		public Ltsash( Serial serial ) : base( serial )
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
