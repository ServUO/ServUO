using System;
using Server;

namespace Server.Items
{
	public class TurtleCloak : Cloak
	{
		public override int ArtifactRarity{ get{ return 18; } }
		
		[Constructable]
		public TurtleCloak()
		{
			Name = "Turtle Cloak";
			Hue = 2871;
			LootType = LootType.Blessed;

			Resistances.Cold = 11;
			Resistances.Energy = 13;
			Resistances.Fire = 9;
			Resistances.Physical = 10;
			Resistances.Poison = 8;
			Attributes.BonusDex = 15;
            Attributes.BonusHits = 14;
            Attributes.BonusInt = 15;
            Attributes.BonusMana = 16;
            Attributes.BonusStam = 15;
            Attributes.CastRecovery = 4;
            Attributes.CastSpeed = 6;
		}

		public TurtleCloak( Serial serial ) : base( serial )
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