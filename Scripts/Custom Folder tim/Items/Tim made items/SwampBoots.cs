//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/25/2017 12:31:01 AM
//=================================================

using System;
using Server;

namespace Server.Items
{
	public class SwampBoots : BaseArmor
	{
		public override int ArtifactRarity{ get{ return 50; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public SwampBoots() : base (5899)
		{
			Name = "Armored Swamp Boots";
			Hue = 65;
			
			LootType = LootType.Regular;
			this.Weight = 4;

			Attributes.BonusStr = 6;
			Attributes.BonusDex = 9;
			Attributes.DefendChance = 10;
			Attributes.AttackChance = 7;
			Attributes.ReflectPhysical = 15;
			Attributes.RegenHits = 10;
			PhysicalBonus = 3;
			FireBonus = 4;
			ColdBonus = 9;
			PoisonBonus = 10;
			EnergyBonus = 5;
		}


		public SwampBoots( Serial serial ) : base( serial )
		{
		}

		public override ArmorMaterialType MaterialType
		{
			get
			{
				return ArmorMaterialType.Cloth;
			}
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
