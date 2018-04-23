using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class NDRoboBlaster : HeavyCrossbow
	{

		public override int DefMaxRange{ get{ return 20; } }

		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( BlasterAmmo ); } }

		public override int AosStrengthReq{ get{ return 50; } }
		public override int OldStrengthReq{ get{ return 50; } }
		public override int AosMinDamage{ get{ return 20; } }
		public override int OldMinDamage{ get{ return 20; } }
		public override int AosMaxDamage{ get{ return 25; } }
		public override int OldMaxDamage{ get{ return 25; } }

		public override int InitMinHits{ get{ return 250; } }
		public override int InitMaxHits{ get{ return 250; } }

		[Constructable]
		public NDRoboBlaster() : base()
		{
			Weight = 5.0;
			Name = "Robo Blaster";
			Layer = Layer.OneHanded;
                        Attributes.AttackChance = 500;
			Attributes.DefendChance = 250;
			Attributes.ReflectPhysical = 100;
			WeaponAttributes.HitFireball = 100;
			WeaponAttributes.HitFireArea = 100;
			WeaponAttributes.HitLeechMana = 10;
			WeaponAttributes.HitLeechStam = 10;
			WeaponAttributes.HitLeechHits = 10;
			WeaponAttributes.HitHarm = 100;
			WeaponAttributes.HitColdArea = 100;
			WeaponAttributes.SelfRepair = 50;
			LootType = LootType.Regular;
			Hue = 1985;
		}

		public NDRoboBlaster( Serial serial ) : base( serial )
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