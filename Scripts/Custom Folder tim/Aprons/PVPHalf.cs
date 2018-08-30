using System; 
using Server; 

namespace Server.Items
{ 
	public class PVPHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public PVPHalf()
                {
			Name = "-PWNED-";
			Hue = 1975;

			Attributes.BonusDex = 5;
			Attributes.BonusStr = 5;
			Attributes.BonusInt = 5;

			Resistances.Physical = 5;
			Resistances.Fire = 5;
			Resistances.Cold = 5;
			Resistances.Poison = 5;
			Resistances.Energy = 5;
                        Attributes.WeaponSpeed = 25;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
                        Attributes.RegenHits = 5;
		}

		public PVPHalf( Serial serial ) : base( serial )

		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
} 