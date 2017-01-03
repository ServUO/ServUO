using System;
using Server;

namespace Server.Items
{
	public class StainedGlassLamp : BaseLight
	{
		public override int LitItemID
		{
			get
			{
                return 0x4C51;
			}
		}

		public override int UnlitItemID
		{
			get
			{
                return 0x4C50;
			}
		}

		[Constructable]
        public StainedGlassLamp()
            : base(0x4C50)
		{
			if ( Burnout )
				Duration = TimeSpan.FromMinutes( 60 );
			else
				Duration = TimeSpan.Zero;

			Burning = false;
            Light = LightType.Circle225;
			Weight = 5.0;
		}

        public StainedGlassLamp(Serial serial)
            : base(serial)
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
		}
	}
}