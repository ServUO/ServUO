using System;
using Server;

namespace Server.Items
{
	public class HammerOfJustice : WarHammer
	{
		public override int ArtifactRarity{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public HammerOfJustice()
		{
			Name = "Hammer Of Justice";
			Hue = 1176;
			WeaponAttributes.HitLowerDefend = 50;
			WeaponAttributes.HitEnergyArea = 100;
			Attributes.AttackChance = 15;
			Attributes.WeaponDamage = 50;
		}

		public HammerOfJustice( Serial serial ) : base( serial )
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