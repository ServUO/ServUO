using System;
using Server;

namespace Server.Items
{
	public class OakwoodSpirit : BaseTalisman
	{
		public override int LabelNumber{ get{ return 1075034; } } // Bloodwood Spirit
		public override bool ForceShowName{ get{ return true; } }

		[Constructable]
		public OakwoodSpirit() : base( 0x2F5A )
		{
			Name = "Oakwood Spirit";
			Hue = 2010;
			MaxChargeTime = 1200;

			Removal = TalismanRemoval.Damage;
			Blessed = GetRandomBlessed();
			Protection = GetRandomProtection( false );

			SkillBonuses.SetValues( 0, SkillName.Hiding, 10.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealth, 5.0 );
		}

		public OakwoodSpirit( Serial serial ) :  base( serial )
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
