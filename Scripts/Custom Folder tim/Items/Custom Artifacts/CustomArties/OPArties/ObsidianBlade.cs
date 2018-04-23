using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class ObsidianBlade : BladedStaff, ITokunoDyable
  {
public override int ArtifactRarity{ get{ return 19; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

      [Constructable]
		public ObsidianBlade()
		{
          Name = "Obsidian Blade";
	Hue = 2051;
      WeaponAttributes.HitLeechHits = 25;
      WeaponAttributes.HitLeechMana = 25;
      WeaponAttributes.HitLeechStam = 25;
      WeaponAttributes.HitPhysicalArea = 100;
      WeaponAttributes.UseBestSkill = 1;
      Attributes.WeaponDamage = 50;
      Attributes.WeaponSpeed = 30;
		}

		public ObsidianBlade( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
