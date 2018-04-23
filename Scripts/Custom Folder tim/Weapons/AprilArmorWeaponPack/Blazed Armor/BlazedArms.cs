using System;
using Server;

namespace Server.Items
{
	public class BlazedArms : BoneArms
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BlazedArms()
		{
			Hue = 2993;
                        Name = " Blazed Bone Arms";
			SkillBonuses.SetValues( 0, SkillName.Magery, 15.0 );
			Attributes.SpellDamage = 20;
			ArmorAttributes.MageArmor = 1;
                        Attributes.LowerRegCost = 50;
                        FireBonus = 20;
         Attributes.WeaponDamage = 25;
         Attributes.WeaponSpeed = 25;
			ColdBonus = 20;
                        PoisonBonus = 20;
                        PhysicalBonus = 20;
                        EnergyBonus = 20;
		}

		public BlazedArms( Serial serial ) : base( serial )
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