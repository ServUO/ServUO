using System;
using Server;
using System.Collections; 
using Server.Items;

namespace Server.Mobiles
{
	public class LordBritish : BaseHealer
	{
		public override bool CanTeach { get { return true; } }

		public override bool CheckTeach( SkillName skill, Mobile from )
		{
			if ( !base.CheckTeach( skill, from ) )
			{
				return false;
			}

			return (skill == SkillName.Forensics) || (skill == SkillName.Healing) || (skill == SkillName.SpiritSpeak) || (skill == SkillName.Swords);
		}

		[Constructable]
		public LordBritish()
		{

			Name = "Lord British";

			Frozen = true;

			SetSkill( SkillName.Forensics, 80.0, 100.0 );
			SetSkill( SkillName.SpiritSpeak, 80.0, 100.0 );
			SetSkill( SkillName.Swords, 80.0, 100.0 );
		}

		public override void InitOutfit()
		{
			AddItem( new LordBritishSuit());

			Item cape = new Cloak();		
			cape.Hue = 38;
			cape.Movable = false;

			AddItem( cape );
		}

		public override bool IsActiveVendor { get { return true; } }
		public override bool IsInvulnerable { get { return true; } }

		public LordBritish( Serial serial ) : base( serial )
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

			if ( Core.AOS && NameHue == 0x35 )
			{
				NameHue = -1;
			}
		}
	}
}