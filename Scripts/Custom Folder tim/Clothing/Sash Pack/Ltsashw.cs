
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x153B, 0x153C )]
	public class Ltsashw : BaseMiddleTorso
	{
 		public override void GetProperties(ObjectPropertyList list)
		{
		base.GetProperties(list); list.Add(1075085);
		}

              	[Constructable]
              	public Ltsashw() : base ( 0x1541 )
		{
			Weight = 2.0;
			Name = "Warrior's Sash";
			Hue = 1282;

			Attributes.AttackChance = 10;
			Attributes.DefendChance = 10; 
		}

		public Ltsashw( Serial serial ) : base( serial )
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
