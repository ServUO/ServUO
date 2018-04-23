using System;
using Server;
using System.Collections; 
using Server.Items;

namespace Server.Mobiles
{
	public class ArenaMob : BaseHealer
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
		public ArenaMob()
		{

			Name = null;
			
			Frozen = true;

			SetSkill( SkillName.Forensics, 80.0, 100.0 );
			SetSkill( SkillName.SpiritSpeak, 80.0, 100.0 );
			SetSkill( SkillName.Swords, 80.0, 100.0 );
		}

		public override void InitOutfit()
		{
			if ( Female )
			{
				AddItem( new FancyDress( GetRandomHue() ) );
			}
			else
			{
				AddItem( new FancyShirt( GetRandomHue() ) );
			}

			int lowHue = GetRandomHue();

			AddItem( new ShortPants( lowHue ) );

			if ( Female )
			{
				AddItem( new ThighBoots( lowHue ) );
			}
			else
			{
				AddItem( new Boots( lowHue ) );
			}

			if ( !Female )
			{
				AddItem( new BodySash( lowHue ) );
			}

			AddItem( new Cloak( GetRandomHue() ) );

			switch ( Utility.Random( 4 ) )
			{
				case 0:
					AddItem( new ShortHair( Utility.RandomHairHue() ) );
					break;
				case 1:
					AddItem( new TwoPigTails( Utility.RandomHairHue() ) );
					break;
				case 2:
					AddItem( new ReceedingHair( Utility.RandomHairHue() ) );
					break;
				case 3:
					AddItem( new KrisnaHair( Utility.RandomHairHue() ) );
					break;
			}

			PackGold( 100, 150 );
		}
		public override bool IsActiveVendor { get { return true; } }
		public override bool IsInvulnerable { get { return true; } }

		public ArenaMob( Serial serial ) : base( serial )
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