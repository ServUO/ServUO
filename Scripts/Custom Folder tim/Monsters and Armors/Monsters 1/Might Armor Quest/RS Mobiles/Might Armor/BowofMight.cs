using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class BowofMight : CompositeBow
	{

		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Gold ); } }

		public override int AosStrengthReq{ get{ return 50; } }
		public override int OldStrengthReq{ get{ return 50; } }
		public override int AosMinDamage{ get{ return 40; } }
		public override int OldMinDamage{ get{ return 40; } }
		public override int AosMaxDamage{ get{ return 40; } }
		public override int OldMaxDamage{ get{ return 40; } }

		public override int InitMinHits{ get{ return 250; } }
		public override int InitMaxHits{ get{ return 250; } }

		[Constructable]
		public BowofMight() : base()
		{
			Weight = 5.0;
			Name = "Bow of Might";
			Hue = 2022;
			Layer = Layer.OneHanded;
                        WeaponAttributes.HitLeechHits = 100;
      			WeaponAttributes.HitLeechMana = 100;
      			WeaponAttributes.HitLeechStam = 100;
      			WeaponAttributes.SelfRepair = 5;
      			Attributes.AttackChance = 50;
      			Attributes.BonusDex = 20;
      			Attributes.BonusHits = 20;
      			Attributes.BonusInt = 20;
      			Attributes.BonusMana = 20;
      			Attributes.BonusStam = 20;
      			Attributes.DefendChance = 50;
      			Attributes.WeaponDamage = 80;
      			Attributes.WeaponSpeed = 80;
      			Attributes.BonusMana = 10;
			LootType = LootType.Regular;
			
		}

		public BowofMight( Serial serial ) : base( serial )
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