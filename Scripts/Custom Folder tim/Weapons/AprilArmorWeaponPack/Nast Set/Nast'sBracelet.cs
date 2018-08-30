using System;
using Server;

namespace Server.Items
{
	public class NastsBracelet : GoldBracelet
	{
		public override int ArtifactRarity{ get{ return 5000; } }

		[Constructable]
		public NastsBracelet()
		{
                                                   Name = "Nasts Bracelet";
			               Hue = Utility.RandomList(38,203);
                                                   LootType = LootType.Blessed;
			               Attributes.ReflectPhysical = 1000;
                                                   Attributes.CastRecovery = 100;
                                                   Attributes.CastSpeed = 100;
			               Attributes.NightSight = 1;
                                                   Attributes.Luck = 200;
                                                   Attributes.LowerRegCost = 100;
                                                   Attributes.LowerManaCost = 100;
                                        Attributes.RegenHits = 100;
                                        Attributes.RegenMana = 100;
                                        Attributes.RegenStam = 100;
		}

		public NastsBracelet( Serial serial ) : base( serial )
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