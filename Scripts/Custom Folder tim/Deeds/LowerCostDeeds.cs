// This Script was Scripted by Espcevan
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
//Lower Reg Cost Deed
	public class LowerRegCostTarget : Target
	{
		private LowerRegCostDeed m_Deed;

		public LowerRegCostTarget( LowerRegCostDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is BaseWeapon || target is BaseShield || target is BaseJewel || target is BaseArmor )
			{
				int augment = ( ( Utility.Random( 2 ) ) /* scalar*/ ) + 4;
				int augmentper = ( ( Utility.Random( 4 ) ) /* scalar*/ ) + 10;
				
				Item item = (Item)target;
				if (item is BaseWeapon)
				{
					if ( ((BaseWeapon)item).Attributes.LowerRegCost == 20 ) from.SendMessage( "That already has lower reg cost!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You can not put lower reg cost on that there!" );
						else
						{
							((BaseWeapon)item).Attributes.LowerRegCost += augmentper;
							from.SendMessage( "You magically add lower reg cost to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseShield)
				{
					if ( ((BaseShield)item).Attributes.LowerRegCost == 20 ) from.SendMessage( "That already has lower reg cost!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put lower reg cost on that there!" );
						else
						{
							((BaseShield)item).Attributes.LowerRegCost += augmentper;
							from.SendMessage( "You magically add lower reg cost to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseArmor)
				{
					if ( ((BaseArmor)item).Attributes.LowerRegCost == 20 ) from.SendMessage( "That already has lower reg cost!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put lower reg cost on that there!" );
						else
						{
							((BaseArmor)item).Attributes.LowerRegCost += augmentper;
							from.SendMessage( "You magically add lower reg cost to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseJewel)
				{
					if ( ((BaseJewel)item).Attributes.LowerRegCost == 20 ) from.SendMessage( "That already has lower reg cost!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put lower reg cost on that there!" );
						else
						{
							((BaseJewel)item).Attributes.LowerRegCost += augmentper;
							from.SendMessage( "You magically add lower reg cost to your item...." );
							m_Deed.Delete();
						}
					}
				}
			}
			else from.SendMessage( "You can not put lower reg cost on that" );
		}
	}

	public class LowerRegCostDeed : Item
	{
		[Constructable]
		public LowerRegCostDeed() : base( 0x14F0 )
		{
			Name = "a Lower Reg Cost deed";
			Hue = 0x84A;
		}

		public LowerRegCostDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to add lower reg cost to?"  );
				from.Target = new LowerRegCostTarget( this );
			 }
		}
	}
//Lower Mana Cost Deed
	public class LowerManaCostTarget : Target
	{
		private LowerManaCostDeed m_Deed;

		public LowerManaCostTarget( LowerManaCostDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is BaseWeapon || target is BaseShield || target is BaseJewel || target is BaseArmor )
			{
				int augment = ( ( Utility.Random( 2 ) ) /* scalar*/ ) + 2;
				int augmentper = ( ( Utility.Random( 4 ) ) /* scalar*/ ) + 6;
					
				Item item = (Item)target;
				if (item is BaseWeapon)
				{
					if ( ((BaseWeapon)item).Attributes.LowerManaCost == 8 ) from.SendMessage( "That already has lower mana cost!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You can not put lower mana cost on that there!" );
						else
						{
							((BaseWeapon)item).Attributes.LowerManaCost += augmentper; 
							from.SendMessage( "You magically add lower mana cost to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseShield)
				{
					if ( ((BaseShield)item).Attributes.LowerManaCost == 8 ) from.SendMessage( "That already has lower mana cost!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put lower mana cost on that there!" );
						else
						{
							((BaseShield)item).Attributes.LowerManaCost += augmentper; 
							from.SendMessage( "You magically add lower mana cost to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseArmor)
				{
					if ( ((BaseArmor)item).Attributes.LowerManaCost == 8 ) from.SendMessage( "That already has lower mana cost!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put lower mana cost on that there!" );
						else
						{
							((BaseArmor)item).Attributes.LowerManaCost += augmentper; 
							from.SendMessage( "You magically add lower mana cost to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseJewel)
				{
					if ( ((BaseJewel)item).Attributes.LowerManaCost == 8 ) from.SendMessage( "That already has lower mana cost!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put lower mana cost on that there!" );
						else
						{
							((BaseJewel)item).Attributes.LowerManaCost += augmentper; 
							from.SendMessage( "You magically add lower mana cost to your item...." );
							m_Deed.Delete();
						}
					}
				}
			}
			else from.SendMessage( "You can not put lower mana cost on that" );
		}
	}

	public class LowerManaCostDeed : Item
	{
		[Constructable]
		public LowerManaCostDeed() : base( 0x14F0 )
		{
			Name = "a Lower Mana Cost deed";
			Hue = 0x7D3;
		}

		public LowerManaCostDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to add lower mana cost to?"  );
				from.Target = new LowerManaCostTarget( this );
			 }
		}
	}
}