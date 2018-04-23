using System;
using Server;

namespace Server.Items
{
	public class TunicOfMight : StuddedChest
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public TunicOfMight()
		{
			Name = "Tunic Of Might";
			Hue = 1000;
			Attributes.BonusStr = 25;
			ArmorAttributes.SelfRepair = 5;
			Attributes.ReflectPhysical = 20;
			PhysicalBonus = 15;
		}

		public TunicOfMight( Serial serial ) : base( serial )
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