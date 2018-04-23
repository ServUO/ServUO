using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
    public class RobeOfFire : BaseOuterTorso
	{
             public override int ArtifactRarity{ get{ return 13; } }
		[Constructable]
		public RobeOfFire() : base( 0x2684 )	// Hooded Shroud
		{	
			Hue = 33;	//  red
			Name = "That Which Burns";
			Attributes.BonusStr = 5;
            Attributes.BonusDex = 20;
		}

		public RobeOfFire( Serial serial ) : base( serial )
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

			from.SendMessage( "You feel the power of burning life taking over your body!" );
			from.SolidHueOverride = 33;

			if( !from.Mounted )	// Only transform if not mounted..
			{
			 	from.BodyMod = 15;
				from.FixedParticles( 0x375A, 10, 15, 5010, EffectLayer.Waist );
				from.FixedParticles( 0x376A, 1, 14, 0x13B5, EffectLayer.Waist );
				from.PlaySound( from.Body.IsFemale ? 0x338 : 0x44A );
			}

			else
			{
				from.SendMessage( 1154, "The transformation was incomplete! This form is too hot to ride mounts!!" );
			}
			return true;
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = ( Mobile ) parent;
				
				from.SolidHueOverride = -1;	// Remove the Black Hue

				from.BodyMod = 0;	// Remove the Drgon Body.
				
				from.FixedParticles( 0x375A, 10, 15, 5010, EffectLayer.Waist );
				from.SendMessage( "You feel yourself cooling off." );
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