using System;
using Server;

namespace Server.Items
{

[Flipable( 0x27A1, 0x27EC )]
	public class MageVest : BaseMiddleTorso
	{
		public override int ArtifactRarity{ get{ return 27; } }

		[Constructable]
		public MageVest() : base( 0x27A1 )
		{
			Weight = 3.0;
            Name = "Mage Vest {Grove Artifact}";
			Hue = 1366;
			
			Attributes.RegenMana = 3;
			Attributes.Luck = 100;
			Attributes.BonusHits = 5;
			Attributes.SpellDamage = 10;
			Attributes.CastRecovery = 2;
            Attributes.CastSpeed = 1; 
			SkillBonuses.SetValues( 0, SkillName.Magery, 20.0 );
		}

		public MageVest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}