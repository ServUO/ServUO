// Scripted by Grim. Ask me before you edit my scripts please.
using System;
using Server;

namespace Server.Items
{
	public class SkullOfJupiter : DragonHelm
	{
		public override int ArtifactRarity{ get{ return 75; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public SkullOfJupiter()
		{
			Name = " Skull Of Jupiter";
			Hue = 1152;
			Attributes.BonusStr = 30;
			Attributes.BonusDex = 13;
			ArmorAttributes.MageArmor = 1;
			Attributes.WeaponDamage = 10;
			Attributes.WeaponSpeed = 21;
			FireBonus = 80;
			PhysicalBonus = 30;
			ColdBonus = 80;
			EnergyBonus = 80;
			PoisonBonus = 80;
			
			LootType = LootType.Blessed;
		}

		public SkullOfJupiter( Serial serial ) : base( serial )
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
