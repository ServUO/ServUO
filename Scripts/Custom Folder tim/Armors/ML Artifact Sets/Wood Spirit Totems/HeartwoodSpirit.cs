using System;
using Server;

namespace Server.Items
{
	public class HeartwoodSpirit : BaseTalisman
	{
		public override int LabelNumber{ get{ return 1075034; } } // Bloodwood Spirit
		public override bool ForceShowName{ get{ return true; } }

		[Constructable]
		public HeartwoodSpirit() : base( 0x2F5A )
		{
			Name = "Heartwood Spirit";
			Hue = 1193;
			MaxChargeTime = 1200;

			Removal = TalismanRemoval.Damage;
			Blessed = GetRandomBlessed();
			Protection = GetRandomProtection( false );

			SkillBonuses.SetValues( 0, SkillName.Healing, 10.0 );
			SkillBonuses.SetValues( 1, SkillName.Veterinary, 5.0 );
		}

		public HeartwoodSpirit( Serial serial ) :  base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && ( Protection == null || Protection.IsEmpty ) )
				Protection = GetRandomProtection( false );
		}
	}
}
