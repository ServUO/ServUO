using System; 
using Server; 

namespace Server.Items
{ 
	public class ThiefHalf : HalfApron
	{
                public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public ThiefHalf()
		{
			Name = "The Thief";
			Hue = 1363;
                        SkillBonuses.SetValues( 0, SkillName.Stealing, 10.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealth, 10.0 );
                        Attributes.AttackChance = 5;

		}

		public ThiefHalf( Serial serial ) : base( serial )
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