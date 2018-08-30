using System;
using Server;

namespace Server.Items
{
	public class TurtleGuard : HeaterShield
	{

		public override int ArtifactRarity{ get{ return 90; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public TurtleGuard()
		{
                        ItemID = 0x2B01;
                        Name = "Turtle Guard";
                        Weight = 1;
			            Hue = 2871;

	      		    Attributes.AttackChance = 15;
              		Attributes.DefendChance = 10;
              		Attributes.Luck = 300;
              		Attributes.NightSight = 1;
              		Attributes.ReflectPhysical = 10;
              		Attributes.SpellChanneling = 1;
              		Attributes.SpellDamage = 25;
              		Attributes.WeaponDamage = 15;
              		ArmorAttributes.DurabilityBonus = 10;
              		ArmorAttributes.LowerStatReq = 30;
              		ArmorAttributes.SelfRepair = 5;
              		ColdBonus = 14;
              		EnergyBonus = 14;
              		FireBonus = 14;
              		PhysicalBonus = 14;
              		PoisonBonus = 14;
              		StrBonus = 10;
              		LootType = LootType.Blessed; 

		}

		public TurtleGuard( Serial serial ) : base( serial )
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