using System;
using Server.Items;

namespace Server.Items
{
	public class EtoileBleue : GoldRing
	{
		public override int LabelNumber{ get{ return 1080238; } } // Thank you Paradyme
		public override SetItem SetID{ get{ return SetItem.Luck; } }
		public override int Pieces{ get{ return 2; } }
		[Constructable]
		public EtoileBleue() : base()
		{

			Weight = 1.0;
	
			SetHue = 0x554;  //a guess
			
			Attributes.Luck = 150;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;

			SetAttributes.Luck = 100;
			SetAttributes.RegenHits = 2;
			SetAttributes.RegenMana = 2;
			SetAttributes.CastSpeed = 1;
			SetAttributes.CastRecovery = 4;
		}

		public EtoileBleue( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
