using System;
using Server;

namespace Server.Items
{
	public class BlazedLegs : LeatherLegs
	{
		public override int ArtifactRarity{ get{ return 15; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BlazedLegs()
		{
			Hue = 2993;
                        Name = "Blazed Legs";
                        Attributes.LowerManaCost = 8;
                        Attributes.AttackChance = 20;
                        Attributes.BonusStr = 35;
			FireBonus = 20;
         Attributes.WeaponDamage = 25;
         Attributes.WeaponSpeed = 25;
			ColdBonus = 20;
                        PoisonBonus = 20;
                        PhysicalBonus = 20;
                        EnergyBonus = 15;
		}

		public BlazedLegs( Serial serial ) : base( serial )
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