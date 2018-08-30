using System;
using Server;

namespace Server.Items
{
	public class EmbroideredAshLeafCloak : BaseOuterTorso
	{
		public override int LabelNumber{ get{ return 1094901; } } // Embroidered Oak Leaf Cloak [Replica]

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public EmbroideredAshLeafCloak() : base( 0x2684 )
		{
			Name = "Embroidered Ash Leaf Cloak [Replica]";
			Hue = 1191;
			StrRequirement = 0;

			SkillBonuses.SetValues( 1, Utility.RandomCombatSkill(), 5.0 );
		}

		public EmbroideredAshLeafCloak( Serial serial ) : base( serial )
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
