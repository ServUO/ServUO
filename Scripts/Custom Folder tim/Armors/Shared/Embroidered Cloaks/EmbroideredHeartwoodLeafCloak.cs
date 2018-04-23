using System;
using Server;

namespace Server.Items
{
	public class EmbroideredHeartwoodLeafCloak : BaseOuterTorso
	{
		public override int LabelNumber{ get{ return 1094901; } } // Embroidered Heartwood Leaf Cloak [Replica]

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public EmbroideredHeartwoodLeafCloak() : base( 0x2684 )
		{
			Name = "Embroidered Heartwood Leaf Cloak [Replica]";
			Hue = 1193;
			StrRequirement = 0;

			SkillBonuses.Skill_1_Name = SkillName.Veterinary;
			SkillBonuses.Skill_1_Value = 5;
		}

		public EmbroideredHeartwoodLeafCloak( Serial serial ) : base( serial )
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
