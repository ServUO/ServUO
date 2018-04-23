using System;
using Server;

namespace Server.Items
{
	public class BlazedFemaleChest : StuddedBustierArms
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BlazedFemaleChest()
		{
			Hue = 2993;
                        Name = "Blazed Chest";
			Attributes.Luck = 200;
			Attributes.DefendChance = 15;
			Attributes.LowerRegCost = 50;
                        Attributes.LowerManaCost = 30;
			ArmorAttributes.MageArmor = 1;
         Attributes.WeaponDamage = 25;
         Attributes.WeaponSpeed = 25;
                        FireBonus = 20;
			ColdBonus = 20;
                        PoisonBonus = 20;
                        PhysicalBonus = 20;
                        EnergyBonus = 20;
		}

		public BlazedFemaleChest( Serial serial ) : base( serial )
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