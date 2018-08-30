using System;
using Server;

namespace Server.Items
{
	public class LeechSword : VikingSword
	{
		public override int ArtifactRarity{ get{ return 10; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public LeechSword()
		{
			Name = "Leech Sword";
			Hue = 32;
			WeaponAttributes.HitLeechHits = 50;
			WeaponAttributes.HitLeechMana = 50;
			WeaponAttributes.HitLeechStam = 50;
			Attributes.WeaponSpeed = 10;
			Attributes.WeaponDamage = 50;
		}

		public LeechSword( Serial serial ) : base( serial )
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