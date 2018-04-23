// Scripted by Neptune
using System;
using Server;

namespace Server.Items
{
	public class MegamiChest : LeatherChest
	{
		public override int ArtifactRarity{ get{ return 619; } }

		public override int InitMinHits{ get{ return 300; } }
		public override int InitMaxHits{ get{ return 300; } }

		[Constructable]
		public MegamiChest() 
		{
			Weight = 0.0; 
            		Name = "Chest Of Rebirth"; 
            		Hue = 2946;
			ItemID = 7180;
			Attributes.AttackChance = Utility.Random( 1, 20 );
			Attributes.BonusDex = Utility.Random( 1, 20 );
			Attributes.BonusHits = Utility.Random( 1, 20 );
			Attributes.BonusInt = Utility.Random( 1, 20 );
			Attributes.BonusMana = Utility.Random( 1, 20 );
			Attributes.BonusStam = Utility.Random( 1, 20 );
			Attributes.BonusStr = Utility.Random( 1, 20 );
			Attributes.CastRecovery = Utility.Random( 1, 10 );
			Attributes.CastSpeed = Utility.Random( 1, 10 );
			Attributes.DefendChance = Utility.Random( 1, 20 );
			Attributes.LowerManaCost = Utility.Random( 1, 20 );
			Attributes.Luck = Utility.Random( 1, 2000 );
			Attributes.ReflectPhysical = Utility.Random( 1, 20 );
			Attributes.RegenHits = Utility.Random( 1, 20 );
			Attributes.RegenMana = Utility.Random( 1, 20 );
			Attributes.RegenStam = Utility.Random( 1, 20 );
			Attributes.SpellDamage = Utility.Random( 1, 20 );
			Attributes.WeaponDamage = Utility.Random( 1, 20 );
			Attributes.WeaponSpeed = Utility.Random( 1, 20 );

			ArmorAttributes.MageArmor = 1;

			


		}

		public MegamiChest( Serial serial ) : base( serial )
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
