// This Script was Scripted by Espcevan
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class RegenHitsTarget : Target
	{
		private RegenHitsDeed m_Deed;

		public RegenHitsTarget( RegenHitsDeed deed ) : base( 1, false, TargetFlags.None )
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
					if ( ((BaseWeapon)item).Attributes.RegenHits == 1 ) from.SendMessage( "That already has regen hits!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You can not put regen hits on that there!" );
						else
						{
							((BaseWeapon)item).Attributes.RegenHits = 1;
							from.SendMessage( "You magically add regen hits to your item...." );
						}
					}
				}
				if (item is BaseShield)
				{
					if ( ((BaseShield)item).Attributes.RegenHits == 1 ) from.SendMessage( "That already has regen hits!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put regen hits on that there!" );
						else
						{
							((BaseShield)item).Attributes.RegenHits = 1;
							from.SendMessage( "You magically add regen hits to your item...." );
						}
					}
				}
				if (item is BaseArmor)
				{
					if ( ((BaseArmor)item).Attributes.RegenHits == 1 ) from.SendMessage( "That already has regen hits!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put regen hits on that there!" );
						else
						{
							((BaseArmor)item).Attributes.RegenHits = 1;
							from.SendMessage( "You magically add regen hits to your item...." );
						}
					}
				}
				if (item is BaseJewel)
				{
					if ( ((BaseJewel)item).Attributes.RegenHits == 1 ) from.SendMessage( "That already has regen hits!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put regen hits on that there!" );
						else
						{
							((BaseJewel)item).Attributes.RegenHits = 1;
							from.SendMessage( "You magically add regen hits to your item...." );
						}
					}
				}
			}
			else from.SendMessage( "You can not put regen hits on that" );
		}
	}

	public class RegenHitsDeed : Item
	{
		[Constructable]
		public RegenHitsDeed() : base( 0x14F0 )
		{
			Name = "a Regen Hits deed";
			Hue = 0x492;
		}

		public RegenHitsDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to add regen hits to?"  );
				from.Target = new RegenHitsTarget( this );
			 }
		}
	}
}