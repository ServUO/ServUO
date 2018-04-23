//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 6/25/2017 12:31:01 AM
//=================================================

using System;
using Server;

namespace Server.Items
{
	public class DarkLordWrap : BaseArmor
	{
		public override int ArtifactRarity{ get{ return 50; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public DarkLordWrap() : base (7939)
		{
			Name = "Dark Lord's Wrap";
			Hue = 2155;
			
			LootType = LootType.Regular;
			this.Weight = 4;

			Attributes.BonusStr = 10;
			Attributes.BonusDex = 10;
			Attributes.DefendChance = 15;
			Attributes.AttackChance = 15;
			Attributes.ReflectPhysical = 25;
			Attributes.RegenHits = 10;
			PhysicalBonus = 10;
			FireBonus = 10;
			ColdBonus = 10;
			PoisonBonus = 10;
			EnergyBonus = 10;

			this.SkillBonuses.SetValues(0, SkillName.Swords, 10.0);
			this.SkillBonuses.SetValues(1, SkillName.Anatomy, 10);
			this.SkillBonuses.SetValues(2, SkillName.Tactics, 10);
			this.SkillBonuses.SetValues(2, SkillName.Healing, 10);
		}


		public DarkLordWrap( Serial serial ) : base( serial )
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
