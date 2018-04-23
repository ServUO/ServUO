using System;
using Server;

namespace Server.Items
{
	public class Armsofthemagi : LeatherArms
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public Armsofthemagi()
		{
			Hue = 1153;
                        Name = " Arms Of The Magi";
			SkillBonuses.SetValues( 0, SkillName.Magery, 25.0 );
			Attributes.SpellDamage = 50;
			ArmorAttributes.MageArmor = 1;
                        Attributes.LowerRegCost = 50;
         Attributes.BonusHits = 35;
         Attributes.BonusMana = 50;
         Attributes.CastSpeed = 1;
         Attributes.SpellDamage = 25;
			Attributes.RegenMana = 15;
         Attributes.CastRecovery = 1;
                        FireBonus = 15;
			ColdBonus = 15;
                        PoisonBonus = 15;
                        PhysicalBonus = 15;
                        EnergyBonus = 15;
		}

		public Armsofthemagi( Serial serial ) : base( serial )
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