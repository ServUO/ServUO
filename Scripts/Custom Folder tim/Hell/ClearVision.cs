using System;
using Server;

namespace Server.Items
{
	public class ClearVision : BaseEquipableLight
	{
		public override int LitItemID{ get { return 0xA12; } }
		public override int UnlitItemID{ get { return 0xF6B; } }

		public override int LitSound{ get { return 0x54; } }
		public override int UnlitSound{ get { return 0x4BB; } }
		
		[Constructable]
		public ClearVision() : base( 0xF6B )
		{
			if ( Burnout )
				Duration = TimeSpan.FromMinutes( 3000 );
			else
				Duration = TimeSpan.Zero;

			Burning = false;
			Light = LightType.Circle300;
                        Hue = 1266;
			Weight = 1.0;
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile && Burning )
				Mobiles.MeerMage.StopEffect( (Mobile)parent, true );
		}

		public override void Ignite()
		{
			base.Ignite();

			if ( Parent is Mobile && Burning )
				Mobiles.MeerMage.StopEffect( (Mobile)Parent, true );
		}

		public ClearVision( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 2.0 )
				Weight = 1.0;
		}
	}
}