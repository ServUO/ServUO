// Scripted by Sarenbou
using System;
using Server;

namespace Server.Items
{
	public class AtlantisCrown : Bandana
	{
		public override int ArtifactRarity{ get{ return 400; } }

		public override int InitMinHits{ get{ return 30000; } }
		public override int InitMaxHits{ get{ return 30000; } }

		[Constructable]
		public AtlantisCrown()
		{
			Weight = 0.0; 
            		Name = "Atlantis Crown"; 
            		Hue = 2716;
			ItemID = 11025;
			Attributes.AttackChance = 50;
			Attributes.BonusDex = 40;
			Attributes.BonusInt = 30;
			Attributes.BonusStr = 35;
			Attributes.CastRecovery = 2;
			Attributes.CastSpeed = 2;
			Attributes.DefendChance = 50;
			Attributes.LowerManaCost = 20;
			Attributes.RegenHits = 25;
			Attributes.RegenMana = 25;
			Attributes.RegenStam = 25;
			Attributes.WeaponDamage = 20;
			Attributes.WeaponSpeed = 15;



		}

		public AtlantisCrown( Serial serial ) : base( serial )
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
