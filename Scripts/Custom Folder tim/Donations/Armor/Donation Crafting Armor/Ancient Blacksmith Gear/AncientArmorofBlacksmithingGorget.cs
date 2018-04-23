//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 12/21/2016 4:31:37 PM
//=================================================

using System;
using Server;

namespace Server.Items
{
	public class AncientArmorofBlacksmithingGorget : LeatherGorget
	{
		public override int ArtifactRarity{ get{ return 55; } }
		public override int InitMinHits{ get{ return 85; } }
		public override int InitMaxHits{ get{ return 155; } }

		[Constructable]
		public AncientArmorofBlacksmithingGorget()
		{
			Name = "Ancient Armor of Blacksmithing Gorget";
            Hue = 1109;
			Weight = 3;
			SkillBonuses.SetValues( 0, SkillName.ArmsLore, 10 );
			SkillBonuses.SetValues( 1, SkillName.Blacksmith, 10 );
			Attributes.AttackChance = 5;
			Attributes.DefendChance = 10;
			Attributes.BonusDex = 5;
			Attributes.BonusStr = 5;
			Attributes.Luck = 300;
			Attributes.ReflectPhysical = 10;
			Attributes.NightSight = 1;
			ArmorAttributes.SelfRepair = 6;
			PhysicalBonus = 7;
			ColdBonus = 7;
			FireBonus = 7;
			PoisonBonus = 7;
			EnergyBonus = 7;
		}

		public AncientArmorofBlacksmithingGorget( Serial serial ) : base( serial )
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
