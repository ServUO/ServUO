using System;
using Server;

namespace Server.Items
{
	public class TurtleRobe : Robe
	{
		public override int ArtifactRarity{ get{ return 20; } }
		
		[Constructable]
		public TurtleRobe()
		{
			Name = "Turtle Robe";
			Hue = 2871;
			LootType = LootType.Blessed;

			Resistances.Cold = 15;
			Resistances.Energy = 14;
			Resistances.Fire = 11;
			Resistances.Physical = 12;
			Resistances.Poison = 10;
			Attributes.BonusDex = 15;
            Attributes.BonusHits = 14;
            Attributes.BonusInt = 15;
            Attributes.BonusMana = 16;
            Attributes.BonusStam = 15;
            Attributes.CastRecovery = 4;
            Attributes.CastSpeed = 6;
		}

		public TurtleRobe( Serial serial ) : base( serial )
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