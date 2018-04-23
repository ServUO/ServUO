using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class ThiefsGlasses : Glasses, ITokunoDyable
	{
		[Constructable]
		public ThiefsGlasses() : base()
		{	
			Hue = 0;
			Name = "Thiefs Glasses";
			SkillBonuses.SetValues( 0, SkillName.Stealing, 10.0 );
			SkillBonuses.SetValues( 1, SkillName.Stealth, 10.0 );
			SkillBonuses.SetValues( 2, SkillName.Begging, 10.0 );
		}

		public ThiefsGlasses( Serial serial ) : base( serial )
		{
		}
		
		public override bool OnEquip( Mobile from )
		{
			if ( !from.CanBeginAction( typeof( Server.Spells.Seventh.PolymorphSpell ) ) )
			{
				from.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
				return false;
			}
			else if ( TransformationSpellHelper.UnderTransformation( from ) )
			{
				from.SendMessage( "You cannot equip that in your current form." );
				return false;
			}
			else if ( DisguiseTimers.IsDisguised( from ) )
			{
				from.SendLocalizedMessage( 1061631 ); // You can't do that while disguised.
				return false;
			}

			from.SolidHueOverride = 0x5555;

			if( !from.Mounted )
			{
			 	from.BodyMod = 0;
				from.FixedParticles( 0x375A, 10, 15, 5010, EffectLayer.Waist );
				from.FixedParticles( 0x376A, 1, 14, 0x13B5, EffectLayer.Waist );
				from.PlaySound( from.Body.IsFemale ? 0x338 : 0x44A );
			}

			else
			{
			}
			return true;
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = ( Mobile ) parent;
				
				from.SolidHueOverride = -1;

				from.BodyMod = 0;
				
				from.FixedParticles( 0x375A, 10, 15, 5010, EffectLayer.Waist );
			}

			base.OnRemoved( parent );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( ( int ) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}