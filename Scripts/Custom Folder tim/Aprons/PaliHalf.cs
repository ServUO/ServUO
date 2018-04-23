using System; 
using Server; 

namespace Server.Items
{ 
	public class PaliHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public PaliHalf()
		{
			Name = "The Paladin";
			Hue = 1176;

			Attributes.BonusStr = 10;
			Attributes.BonusInt = 10;
                        SkillBonuses.SetValues( 0, SkillName.Chivalry, 10.0 );
			Resistances.Energy = 10;

			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
		}

		public PaliHalf( Serial serial ) : base( serial )
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