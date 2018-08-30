using System;
using Server;

namespace Server.Items
{
	public class PillagerCap : SkullCap
	{


		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public PillagerCap()
		{
			Name = "Pillager Cap";
			Hue = 1658;
			SkillBonuses.SetValues( 0, SkillName.Stealth, 20.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealing, 20.0 );
			SkillBonuses.SetValues( 2, SkillName.Hiding, 20.0 );
			Attributes.DefendChance = 10;
			Attributes.AttackChance = 10;
			Attributes.WeaponDamage = 10;
			Attributes.WeaponSpeed = 10;
			Attributes.ReflectPhysical = 10;
		}

		public PillagerCap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			}
		}
	}
