// This Script was Scripted by Espcevan
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class RegenManaTarget : Target
	{
		private RegenManaDeed m_Deed;

		public RegenManaTarget( RegenManaDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is BaseWeapon || target is BaseShield || target is BaseJewel || target is BaseArmor )
			{
				Item item = (Item)target;
				if (item is BaseWeapon)
				{
					if ( ((BaseWeapon)item).Attributes.RegenMana == 1 ) from.SendMessage( "That already has regen mana!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You can not put regen mana on that there!" );
						else
						{
							((BaseWeapon)item).Attributes.RegenMana = 1;
							from.SendMessage( "You magically add regen mana to your item...." );
						}
					}
				}
				if (item is BaseShield)
				{
					if ( ((BaseShield)item).Attributes.RegenMana == 1 ) from.SendMessage( "That already has regen mana!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put regen mana on that there!" );
						else
						{
							((BaseShield)item).Attributes.RegenMana = 1;
							from.SendMessage( "You magically add regen mana to your item...." );
						}
					}
				}
				if (item is BaseArmor)
				{
					if ( ((BaseArmor)item).Attributes.RegenMana == 1 ) from.SendMessage( "That already has regen mana!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put regen mana on that there!" );
						else
						{
							((BaseArmor)item).Attributes.RegenMana = 1;
							from.SendMessage( "You magically add regen mana to your item...." );
						}
					}
				}
				if (item is BaseJewel)
				{
					if ( ((BaseJewel)item).Attributes.RegenMana == 1 ) from.SendMessage( "That already has regen mana!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put regen mana on that there!" );
						else
						{
							((BaseJewel)item).Attributes.RegenMana = 1;
							from.SendMessage( "You magically add regen mana to your item...." );
						}
					}
				}
			}
			else from.SendMessage( "You can not put regen mana on that" );
		}
	}

	public class RegenManaDeed : Item
	{
		[Constructable]
		public RegenManaDeed() : base( 0x14F0 )
		{
			Name = "a Regen Mana deed";
			Hue = 0x492;
		}

		public RegenManaDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to add regen mana to?"  );
				from.Target = new RegenManaTarget( this );
			 }
		}
	}
}