/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
~~~~~~~    Ultimate Hue Room Generation System
~~~~~~~~~~          designed by MightyHythloth
~~~~~~~~~~~~~~~~
                with credits to the following:
~~~~~~~~~~~~~~~~~~~~
               Lord_GreyWolf - Equation Coding
          tangentzero - Precursor for dye tubs
Cottonballs (Revision of [hue, author unknown)
~~~~~~~~~~~~~~~~~~~~~~~~
Feel free to modify for your use... but please 
leave all credits if you distribute it further
~~~~~~~~~~~~~~~~~~~~~~~~~~~~
~~~  UltimateDyeTub.cs and HueRoomGen.cs  ~~~~
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

using System;
using System.Collections;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class UltimateTubTarget : Target
	{
		private Item m_Item;

		public UltimateTubTarget( Item item ) : base( 12, false, TargetFlags.None )
		{
			m_Item = item;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			Gold m_Gold = (Gold)from.Backpack.FindItemByType( typeof( Gold ) );
			Gold b_Gold = (Gold)from.BankBox.FindItemByType( typeof( Gold ) );
			int m_Amount = from.Backpack.GetAmount( typeof( Gold ) );
			int b_Amount = from.BankBox.GetAmount( typeof( Gold ) );

			if (target is BaseJewel || target is BaseArmor || target is BaseClothing || target is BaseShield || target is BaseWeapon || target is EtherealMount || target is BaseSuit || target is BaseContainer || target is Item )
			{
				Item item = (Item)target;

				if (item.RootParent == from) // Make sure its in their pack or they are wearing it
					item.Hue = m_Item.Hue;
				else
					from.SendMessage("You can only dye objects that are in your backpack or on your person!");
			}

			// Begin Enable Pets
			else if (target is BaseCreature)
			{
				if (m_Amount >= 25000)
				{
					BaseCreature c = target as BaseCreature;
					
					if (c.Controlled && c.ControlMaster == from)
					{
						c.Hue = m_Item.Hue;
						from.Backpack.ConsumeTotal( typeof( Gold ), 25000 ); // Delete the gold
						from.SendMessage( "Removed 10k from backpack and hued your pet." );
					}
					else
						from.SendMessage("You can only dye animals whom you control!");
				}

				else if (b_Amount >= 25000)
				{
					BaseCreature c = target as BaseCreature;
					
					if (c.Controlled && c.ControlMaster == from)
					{
						c.Hue = m_Item.Hue;
						from.Backpack.ConsumeTotal( typeof( Gold ), 750000 ); // Delete the gold
						from.SendMessage( "Removed 25k from your bank and hued your pet." );
					}
					else
						from.SendMessage("You can only dye animals whom you control!");
				}
			} // End enable pets.
			else
				from.SendMessage("Invalid target.");
		}
	}
	
	public class UltimateDyeTub : Item
	{
		private bool m_Redyable;

		[Constructable]
		public UltimateDyeTub() : base( 0xFAB )
		{
			Weight = 0.0;
			Hue = 0;
			Name = "Ultimate Dye Tub";
			m_Redyable = false;
			Movable = false;
		}

		public UltimateDyeTub( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			{
				from.Target = new UltimateTubTarget( this );
				from.SendMessage( "You may now hue your backpack and worn equipment" );
				from.SendMessage( "& You may also hue your pets for 25,000 gp each." );
			}
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
			if ( Name == "Ultimate Dye Tub" )
			{
				int intNumber = this.Hue;
				string strNumber = intNumber.ToString("#");
				Name = strNumber;
			}
		}
	}
}
