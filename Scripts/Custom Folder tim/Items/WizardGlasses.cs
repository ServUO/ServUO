using System;

namespace Server.Items
{
	public class WizardGlasses : BaseEarrings
	{
		[Constructable]
		public WizardGlasses () : base( 0x2FB8 )
		{
			Weight = 1;
                        Name = "Wizard Glasses";
                        Layer = Layer.Neck;
                        Hue = 1154;
                        LootType = LootType.Blessed;
		}

		public WizardGlasses ( Serial serial ) : base( serial )
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