
using System;
using Server;

namespace Server.Items
{
	public class SilverCrowBracelet : GoldBracelet
	{
		public override int ArtifactRarity{ get{ return 62; } }

		[Constructable]
		public SilverCrowBracelet()
		{
			Weight = 1.0; 
            		Name = "Silver Crow Bracelet"; 
            		Hue = 1953;

            //Attributes.AttackChance = 10;
            //Attributes.BonusStr = 10;
            //Attributes.BonusDex = 10;
            //Attributes.BonusMana = 10;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 3;
			Attributes.DefendChance = 10;
            //Attributes.Luck = 100;
            //Attributes.RegenMana = 3;
			//Attributes.WeaponSpeed = 15;

			SkillBonuses.SetValues( 0, SkillName.Swords, 20.0 );

			Resistances.Energy = 5;
			Resistances.Fire = 2;
			Resistances.Physical = 10;

			
		}

		public SilverCrowBracelet( Serial serial ) : base( serial )
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