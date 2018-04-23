using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class KamiNarisIndestructableDoubleAxe : DoubleAxe, ITokunoDyable
  {
public override int ArtifactRarity{ get{ return 15; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

      [Constructable]
		public KamiNarisIndestructableDoubleAxe()
		{
          Name = "Kami-Naris Indestructable Double Axe";
          Hue = 1161;
      WeaponAttributes.DurabilityBonus = 100;
      WeaponAttributes.HitFireArea = 60;
      WeaponAttributes.HitHarm = 100;
      WeaponAttributes.HitLeechHits = 30;
      WeaponAttributes.HitLeechStam = 25;
      WeaponAttributes.HitLightning = 25;
      WeaponAttributes.SelfRepair = 20;
      Attributes.WeaponDamage = 50;
      Attributes.WeaponSpeed = 20;
		}

		public KamiNarisIndestructableDoubleAxe( Serial serial ) : base( serial )
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
