using System;
using Server;

namespace Server.Items
{
	public class Chestofthemagi : LeatherChest
	{
		public override int ArtifactRarity{ get{ return 146; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public Chestofthemagi()
		{
			Hue = 1153;
                        Name = "Chest Of The Magi";
			Attributes.Luck = 112;
			Attributes.DefendChance = 5;
			Attributes.LowerRegCost = 50;
         Attributes.BonusMana = 50;
         Attributes.BonusHits = 35;
			Attributes.RegenMana = 15;
                        Attributes.LowerManaCost = 50;
			ArmorAttributes.MageArmor = 1;
         Attributes.CastSpeed = 1;
         Attributes.SpellDamage = 15;
         Attributes.CastRecovery = 1;
                        FireBonus = 25;
			ColdBonus = 25;
                        PoisonBonus = 25;
                        PhysicalBonus = 25;
                        EnergyBonus = 25;
		}

		public Chestofthemagi( Serial serial ) : base( serial )
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