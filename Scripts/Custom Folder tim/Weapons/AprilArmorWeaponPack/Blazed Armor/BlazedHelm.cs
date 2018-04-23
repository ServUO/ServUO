using System;
using Server;

namespace Server.Items
{
	public class BlaazedHelm : BoneHelm
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BlaazedHelm()
		{
			Hue = 2993;
                        Name = "Blazed Helmet";
			Attributes.RegenHits = 2;
			Attributes.RegenStam = 3;
			Attributes.WeaponDamage = 25;
                        Attributes.CastSpeed = 1;
                        Attributes.CastRecovery = 1;
                        Attributes.BonusStr = 25;
                        FireBonus = 20;
         Attributes.WeaponDamage = 25;
         Attributes.WeaponSpeed = 25;
			ColdBonus = 20;
                        PoisonBonus = 20;
                        PhysicalBonus = 20;
                        EnergyBonus = 20;
			
		}

		public BlaazedHelm( Serial serial ) : base( serial )
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