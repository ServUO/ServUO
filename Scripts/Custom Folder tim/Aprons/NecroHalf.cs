using System; 
using Server; 

namespace Server.Items
{ 
	public class NecroHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public NecroHalf()
		{
			Name = "The Necromancer";
			Hue = 1172;

			Attributes.BonusInt = 15;
                        SkillBonuses.SetValues( 0, SkillName.Necromancy, 10.0 );
			Attributes.CastSpeed = 1;
			
		}

		public NecroHalf( Serial serial ) : base( serial )
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