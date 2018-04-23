using System;
using Server.Items;

namespace Server.Items
{

[FlipableAttribute( 0x2B68, 0x315F )]
	public class WarriorsBelt : BaseWaist
	{
		public override int ArtifactRarity{ get{ return 11; } }


		[Constructable]
		public WarriorsBelt( ) : base( 0x2B68 )
		{
			Weight = 4.0;
            Name = "Warriors Belt {Grove Artifact}";
            Hue = 1366;

			Attributes.DefendChance = 10;
			Attributes.SpellDamage = 10;
			Attributes.ReflectPhysical = 5;
			Attributes.RegenHits = 2;
			Attributes.Luck = 100;
			
			SkillBonuses.SetValues( 0, SkillName.Swords, 20.0 );
			SkillBonuses.SetValues( 1, SkillName.EvalInt, 15.0 );
		}

		public WarriorsBelt( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}
