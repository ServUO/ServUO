using System;
using Server;

namespace Server.Items
{
	public class DestroyingAngel : BaseReagent, ICommodity
	{
		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return (Core.ML); } }

		[Constructable]
		public DestroyingAngel() : this( 1 )
		{
		}

		[Constructable]
		public DestroyingAngel( int amount ) : base( 0xE1F, amount )
		{
			Hue = 0x290;
			Name = "destroying angel";
		}

		public DestroyingAngel( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}