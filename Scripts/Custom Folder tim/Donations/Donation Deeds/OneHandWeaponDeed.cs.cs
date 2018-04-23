using System;
using Server;
using Server.Items;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using System.Net;
using System.Net.Sockets;

namespace Server.Items
{
	public class OneHandedWeaponDeed : Item
	{
		[Constructable]
		public OneHandedWeaponDeed() : this( 1 )
		{
		}

		[Constructable]
		public OneHandedWeaponDeed( int amount ) : base( 8792 )
		{
			Weight = 1.0;
			Name = "One Handed Weapon Deed";
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				from.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( OnTarget ) );
				from.SendMessage("What weapon will you make one handed?");
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		public void OnTarget( Mobile from, object obj )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( obj is BaseWeapon )
			{
				Item weapon = (Item)obj;

				if ( weapon.RootParent != from )
				{
					from.SendMessage("The weapon must be in your back.");
				}
				else if(weapon.Layer == Layer.TwoHanded)
				{
					weapon.Layer = Layer.FirstValid;
					weapon.LootType = LootType.Blessed;
					from.SendMessage("Your weapon is now one handed!" );
					this.Delete();
				}
				else if(weapon.Layer == Layer.FirstValid)
				{
					from.SendMessage("That is already one handed");
				}
				else
				{
					from.SendMessage("That is not a weapon!");
				}
			}
			else
			{
				from.SendMessage("The dyes will not work on that.");
			}
		}

		public OneHandedWeaponDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		
		}
	}
}