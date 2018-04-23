
using System;
using Server;

namespace Server.Items
{
	public class Blackknightsbracelet : GoldBracelet
	{
		public override int ArtifactRarity{ get{ return 20; } }

		[Constructable]
		public Blackknightsbracelet()
		{
			Weight = 1.0; 
            		Name = "Black Knights Bracelet"; 
            		Hue = 4455;

			Attributes.AttackChance = 30;
			Attributes.BonusStr = 20;
			Attributes.CastSpeed = 3;
			Attributes.DefendChance = 20;
			Attributes.Luck = 10;
			Attributes.RegenMana = 10;
			Attributes.WeaponSpeed = 15;

			SkillBonuses.SetValues( 0, SkillName.Swords, 20.0 );

			Resistances.Energy = 5;
			Resistances.Fire = 2;
			Resistances.Physical = 10;

			
		}

		public Blackknightsbracelet( Serial serial ) : base( serial )
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