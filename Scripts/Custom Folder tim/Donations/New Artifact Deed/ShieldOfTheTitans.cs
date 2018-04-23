using System;
using Server;

namespace Server.Items
{
	public class ShieldOfTheTitans : BronzeShield
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ShieldOfTheTitans()
		{
			Name = "Shield Of The Titans";
			Hue = 1165;
			Attributes.BonusStr = 5;
			ArmorAttributes.SelfRepair = 5;
			Attributes.ReflectPhysical = 20;
			Attributes.DefendChance = 20;
			PhysicalBonus = 15;
		}

		public ShieldOfTheTitans( Serial serial ) : base( serial )
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