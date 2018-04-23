// This Script was Scripted by Espcevan
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class NightSightTarget : Target
	{
		private NightSightDeed m_Deed;

		public NightSightTarget( NightSightDeed deed ) : base( 1, false, TargetFlags.None )
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
					if ( ((BaseWeapon)item).Attributes.NightSight == 1 ) from.SendMessage( "That already has night sight!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You can not put night sight on that there!" );
						else
						{
							((BaseWeapon)item).Attributes.NightSight = 1;
							from.SendMessage( "You magically add night sight to your item...." );
						}
					}
				}
				if (item is BaseShield)
				{
					if ( ((BaseShield)item).Attributes.NightSight == 1 ) from.SendMessage( "That already has night sight!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put night sight on that there!" );
						else
						{
							((BaseShield)item).Attributes.NightSight = 1;
							from.SendMessage( "You magically add night sight to your item...." );
						}
					}
				}
				if (item is BaseArmor)
				{
					if ( ((BaseArmor)item).Attributes.NightSight == 1 ) from.SendMessage( "That already has night sight!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put night sight on that there!" );
						else
						{
							((BaseArmor)item).Attributes.NightSight = 1;
							from.SendMessage( "You magically add night sight to your item...." );
						}
					}
				}
				if (item is BaseJewel)
				{
					if ( ((BaseJewel)item).Attributes.NightSight == 1 ) from.SendMessage( "That already has night sight!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put night sight on that there!" );
						else
						{
							((BaseJewel)item).Attributes.NightSight = 1;
							from.SendMessage( "You magically add night sight to your item...." );
						}
					}
				}
			}
			else from.SendMessage( "You can not put night sight on that" );
		}
	}

	public class NightSightDeed : Item
	{
		[Constructable]
		public NightSightDeed() : base( 0x14F0 )
		{
			Name = "a Night Sight deed";
			Hue = 0x492;
		}

		public NightSightDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to add night sight to?"  );
				from.Target = new NightSightTarget( this );
			 }
		}
	}
}