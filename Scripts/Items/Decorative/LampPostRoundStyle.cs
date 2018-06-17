using System;
using Server;

namespace Server.Items
{
	public class LampPostRoundStyle : BaseLight
	{
		public override int LabelNumber { get { return 1071089; } } // Lamp Post (Round Style)

		public override int LitItemID { get { return 0xB22; } }
		public override int UnlitItemID { get { return 0xB23; } }

		[Constructable]
		public LampPostRoundStyle()
			: base( 0xB23 )
		{
			Weight = 10.0;
			LootType = LootType.Blessed;

			Light = LightType.Circle300;

			Duration = TimeSpan.Zero;
		}

		public LampPostRoundStyle( Serial serial )
			: base( serial )
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

			/*int version = */
			reader.ReadInt();
		}
	}
}