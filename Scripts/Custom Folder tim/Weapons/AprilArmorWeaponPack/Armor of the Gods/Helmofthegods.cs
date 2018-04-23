//Made By Makoro Shimoro
using System;
using Server;

namespace Server.Items
{
	public class HelmOfTheGods : PlateHelm
	{
		//public override int LabelNumber{ get{ return 1061096; } } // Helm of Insight
		public override int ArtifactRarity{ get{ return 15; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public HelmOfTheGods()
		{
			Hue = 1159;
			Name = "Helm Of The Gods";
			Attributes.BonusDex = 10;
			Attributes.BonusStam = 5;
			Attributes.RegenStam = 2;
			Attributes.AttackChance = 5;
			PhysicalBonus = 10;
			FireBonus = 10;
			ColdBonus = 10;
			PoisonBonus = 10;
			EnergyBonus = 10;			
		}

		public HelmOfTheGods( Serial serial ) : base( serial )
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
