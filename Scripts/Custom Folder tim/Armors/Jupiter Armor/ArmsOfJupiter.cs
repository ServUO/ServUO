// Scripted by Grim. Ask me before you edit my scripts please.
using System;
using Server;

namespace Server.Items
{
	public class ArmsOfJupiter : DragonArms
	{
		public override int ArtifactRarity{ get{ return 71; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ArmsOfJupiter()
		{
			Weight = 1.0; 
            		Name = "Arms Of Jupiter"; 
            		Hue = 1152;


			Attributes.CastRecovery = 2;
			Attributes.CastSpeed = 2;
			Attributes.DefendChance = 30;
			Attributes.ReflectPhysical = 25;
			Attributes.SpellChanneling = 1;

			ArmorAttributes.DurabilityBonus = 5;
			ArmorAttributes.MageArmor = 1;
			ArmorAttributes.SelfRepair = 17;

			ColdBonus = 25;
			DexBonus = 25;
			EnergyBonus = 25;
			FireBonus = 25;
			IntRequirement = 28;
			PhysicalBonus = 15;
			StrBonus = 10;
			StrRequirement = 80;

			LootType = LootType.Blessed;

		}

		public ArmsOfJupiter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize( GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
