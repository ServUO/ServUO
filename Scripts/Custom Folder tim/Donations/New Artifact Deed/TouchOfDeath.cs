using System;
using Server;

namespace Server.Items
{
	public class TouchOfDeath : BoneGloves
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public TouchOfDeath()
		{
			Name = "Touch Of Death";
			Hue = 1175;
			Attributes.WeaponDamage = 15;
			Attributes.ReflectPhysical = 30;
			ArmorAttributes.MageArmor = 1;
			PhysicalBonus = 12;
		}

		public TouchOfDeath( Serial serial ) : base( serial )
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