
using System;
using Server;

namespace Server.Items
{
	public class SilverCrowGorget : PlateGorget 
	{
		public override int ArtifactRarity{ get{ return 62; } }

		public override int InitMinHits{ get{ return 300; } }
		public override int InitMaxHits{ get{ return 300; } }

		[Constructable]
		public SilverCrowGorget()
		{
			Weight = 3.0; 
            		Name = "Silver Crow Gorget"; 
            		Hue = 21953;

			Attributes.BonusMana = 5;
			Attributes.DefendChance = 10;
			Attributes.Luck = 100;
			Attributes.RegenMana = 4;
			Attributes.RegenStam = 3;

			ArmorAttributes.SelfRepair = 10;

			ColdBonus = 15;
			EnergyBonus = 15;
			FireBonus = 15;
			PhysicalBonus = 15;
			PoisonBonus = 15;
			StrRequirement = 55;

		}

		public SilverCrowGorget( Serial serial ) : base( serial )
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