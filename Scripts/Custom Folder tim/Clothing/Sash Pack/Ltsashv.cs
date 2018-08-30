
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x153B, 0x153C )]
	public class Ltsashv : BaseMiddleTorso
	{
 		public override void GetProperties(ObjectPropertyList list)
		{
		base.GetProperties(list); list.Add(1075085);
		}

              	[Constructable]
              	public Ltsashv() : base ( 0x1541 )
		{
			Weight = 2.0;
			Name = "Lieutenant of the Valorian Militia";
			Hue = 1445;

			Attributes.RegenHits = 2;
			Attributes.RegenMana = 2;
			Attributes.RegenStam = 2; 
		}

		public Ltsashv( Serial serial ) : base( serial )
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
