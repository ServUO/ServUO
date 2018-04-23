using System;
using Server;

namespace Server.Items
{
	public class BlazedGorget: LeatherGorget
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BlazedGorget()
		{
			Hue = 2993; 
                        Name = "Blazed Gorget";
                        Attributes.LowerManaCost = 15;
                        Attributes.BonusInt = 5;
			Attributes.BonusStr = 10;
			Attributes.RegenHits = 15;
			Attributes.RegenStam = 5;
			FireBonus = 20;
			ColdBonus = 20;
         Attributes.WeaponDamage = 25;
         Attributes.WeaponSpeed = 25;
                        PoisonBonus = 20;
                        PhysicalBonus = 20;
                        EnergyBonus = 15;
		}

		public BlazedGorget( Serial serial ) : base( serial )
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