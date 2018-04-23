//=================================================
//This script was created by Gizmo's Uo Quest Maker
//This script was created on 5/29/2016 1:07:06 PM
//=================================================

using System;
using Server;

namespace Server.Items
{
	public class MirrorShield : BronzeShield
	{
		public override int ArtifactRarity{ get{ return 50; } }
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public MirrorShield()
		{
			Name = "Mirror Shield";
			Hue = 100;
			Weight = 2;
			SkillBonuses.SetValues( 0, SkillName.Swords, 10 );
			Attributes.AttackChance = 30;
			Attributes.DefendChance = 30;
			Attributes.WeaponDamage = 30;
			Attributes.SpellChanneling = 1;
			Attributes.WeaponSpeed = 30;
			ArmorAttributes.SelfRepair = 5;
			PhysicalBonus = 15;
			FireBonus = 25;
		}

		public MirrorShield( Serial serial ) : base( serial )
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
