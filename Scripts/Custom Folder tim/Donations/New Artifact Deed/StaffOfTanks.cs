using System;
using Server;

namespace Server.Items
{
	public class StaffOfTanks : GnarledStaff
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public StaffOfTanks()
		{
			Name = "Staff Of Tanks";
			Hue = 1111;
			// TODO: MageWeapon -0
			Attributes.SpellChanneling = 1;
			Attributes.CastSpeed = 1;
			Attributes.WeaponDamage = 45;
			Attributes.WeaponSpeed = 25;
			WeaponAttributes.HitHarm = 35;
		}

		

		public StaffOfTanks( Serial serial ) : base( serial )
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