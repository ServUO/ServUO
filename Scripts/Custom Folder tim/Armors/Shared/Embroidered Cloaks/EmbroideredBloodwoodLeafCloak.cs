using System;
using Server;

namespace Server.Items
{
	public class EmbroideredBloodwoodLeafCloak : BaseOuterTorso
	{
		public override int LabelNumber{ get{ return 1094901; } } // Embroidered Frostwood Leaf Cloak [Replica]
		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override bool CanFortify{ get{ return false; } }

		[Constructable]
		public EmbroideredBloodwoodLeafCloak() : base( 0x2684 )
		{
			Name = "Embroidered Bloodwood Leaf Cloak [Replica]";
			Hue = 1157;
			StrRequirement = 0;

			SkillBonuses.Skill_1_Name = SkillName.SpiritSpeak;
			SkillBonuses.Skill_1_Value = 5;
		}

		public EmbroideredBloodwoodLeafCloak( Serial serial ) : base( serial )
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
