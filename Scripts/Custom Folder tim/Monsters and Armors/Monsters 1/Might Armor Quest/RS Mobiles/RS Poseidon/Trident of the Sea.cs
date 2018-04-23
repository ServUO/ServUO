//Created with Maraks Script Creator 4
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class TridentoftheSea : Pitchfork
  {
public override int ArtifactRarity{ get{ return 666; } }
		public override int OldMinDamage{ get{ return 25; } }
		public override int AosMinDamage{ get{ return 25; } }
		public override int OldMaxDamage{ get{ return 30; } }
		public override int AosMaxDamage{ get{ return 30; } }


      [Constructable]
		public TridentoftheSea()
		{
          Name = "Trident of the Sea";
	Hue = 2242;
      WeaponAttributes.HitLeechHits = 50;
      WeaponAttributes.HitLeechMana = 50;
      WeaponAttributes.HitLeechStam = 50;
      WeaponAttributes.SelfRepair = 5;
      Attributes.AttackChance = 50;
      Attributes.BonusDex = 10;
      Attributes.BonusHits = 10;
      Attributes.BonusInt = 10;
      Attributes.BonusMana = 10;
      Attributes.BonusStam = 10;
      Attributes.DefendChance = 50;
      Attributes.WeaponDamage = 50;
      Attributes.WeaponSpeed = 50;
      Attributes.BonusMana = 10;
		}

		public TridentoftheSea( Serial serial ) : base( serial )
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
