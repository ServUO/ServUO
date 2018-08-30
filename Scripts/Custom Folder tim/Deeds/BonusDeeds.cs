// This Script was Scripted by Espcevan
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class BonusDexTarget : Target
	{
		private BonusDexDeed m_Deed;

		public BonusDexTarget( BonusDexDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is BaseWeapon || target is BaseShield || target is BaseJewel || target is BaseArmor )
			{
				int augment = ( ( Utility.Random( 2 ) ) /* scalar*/ ) + 1;
				int augmentper = ( ( Utility.Random( 4 ) ) /* scalar*/ ) + 8;
					
				Item item = (Item)target;
				if (item is BaseWeapon)
				{
					if ( ((BaseWeapon)item).Attributes.BonusDex == 15 ) from.SendMessage( "That already has bonus dex!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You can not put bonus dex on that there!" );
						else
						{
							((BaseWeapon)item).Attributes.BonusDex += augmentper; 
							from.SendMessage( "You magically add bonus dex to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseShield)
				{
					if ( ((BaseShield)item).Attributes.BonusDex == 15 ) from.SendMessage( "That already has bonus dex!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put bonus dex on that there!" );
						else
						{
							((BaseShield)item).Attributes.BonusDex += augmentper; 
							from.SendMessage( "You magically add bonus dex to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseArmor)
				{
					if ( ((BaseArmor)item).Attributes.BonusDex == 15 ) from.SendMessage( "That already has bonus dex!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put bonus dex on that there!" );
						else
						{
							((BaseArmor)item).Attributes.BonusDex += augmentper; 
							from.SendMessage( "You magically add bonus dex to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseJewel)
				{
					if ( ((BaseJewel)item).Attributes.BonusDex == 15 ) from.SendMessage( "That already has bonus dex!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put bonus dex on that there!" );
						else
						{
							((BaseJewel)item).Attributes.BonusDex += augmentper; 
							from.SendMessage( "You magically add bonus dex to your item...." );
							m_Deed.Delete();
						}
					}
				}
			}
			else from.SendMessage( "You can not put bonus dex on that" );
		}
	}

	public class BonusDexDeed : Item
	{
		[Constructable]
		public BonusDexDeed() : base( 0x14F0 )
		{
			Name = "a Bonus Dex deed";
			Hue = 0x492;
		}

		public BonusDexDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to add bonus dex to?"  );
				from.Target = new BonusDexTarget( this );
			 }
		}
	}
	public class BonusIntTarget : Target
	{
		private BonusIntDeed m_Deed;

		public BonusIntTarget( BonusIntDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is BaseWeapon || target is BaseShield || target is BaseJewel || target is BaseArmor )
			{
				int augment = ( ( Utility.Random( 2 ) ) /* scalar*/ ) + 1;
				int augmentper = ( ( Utility.Random( 4 ) ) /* scalar*/ ) + 8;
					
				Item item = (Item)target;
				if (item is BaseWeapon)
				{
					if ( ((BaseWeapon)item).Attributes.BonusInt == 15 ) from.SendMessage( "That already has bonus int!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You can not put bonus int on that there!" );
						else
						{
							((BaseWeapon)item).Attributes.BonusInt += augmentper; 
							from.SendMessage( "You magically add bonus int to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseShield)
				{
					if ( ((BaseShield)item).Attributes.BonusInt == 15 ) from.SendMessage( "That already has bonus int!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put bonus int on that there!" );
						else
						{
							((BaseShield)item).Attributes.BonusInt += augmentper; 
							from.SendMessage( "You magically add bonus int to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseArmor)
				{
					if ( ((BaseArmor)item).Attributes.BonusInt == 15 ) from.SendMessage( "That already has bonus int!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put bonus int on that there!" );
						else
						{
							((BaseArmor)item).Attributes.BonusInt += augmentper; 
							from.SendMessage( "You magically add bonus int to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseJewel)
				{
					if ( ((BaseJewel)item).Attributes.BonusInt == 15 ) from.SendMessage( "That already has bonus int!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put bonus int on that there!" );
						else
						{
							((BaseJewel)item).Attributes.BonusInt += augmentper; 
							from.SendMessage( "You magically add bonus int to your item...." );
							m_Deed.Delete();
						}
					}
				}
			}
			else from.SendMessage( "You can not put bonus int on that" );
		}
	}

	public class BonusIntDeed : Item
	{
		[Constructable]
		public BonusIntDeed() : base( 0x14F0 )
		{
			Name = "a Bonus Int deed";
			Hue = 0x492;
		}

		public BonusIntDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to add bonus int to?"  );
				from.Target = new BonusIntTarget( this );
			 }
		}
	}
	public class BonusHitsTarget : Target
	{
		private BonusHitsDeed m_Deed;

		public BonusHitsTarget( BonusHitsDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is BaseWeapon || target is BaseShield || target is BaseJewel || target is BaseArmor )
			{
				int augment = ( ( Utility.Random( 2 ) ) /* scalar*/ ) + 1;
				int augmentper = ( ( Utility.Random( 4 ) ) /* scalar*/ ) + 8;
					
				Item item = (Item)target;
				if (item is BaseWeapon)
				{
					if ( ((BaseWeapon)item).Attributes.BonusHits == 15 ) from.SendMessage( "That already has bonus hits!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You can not put bonus hits on that there!" );
						else
						{
							((BaseWeapon)item).Attributes.BonusHits += augmentper; 
							from.SendMessage( "You magically add bonus hits to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseShield)
				{
					if ( ((BaseShield)item).Attributes.BonusHits == 15 ) from.SendMessage( "That already has bonus hits!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put bonus hits on that there!" );
						else
						{
							((BaseShield)item).Attributes.BonusHits += augmentper; 
							from.SendMessage( "You magically add bonus hits to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseArmor)
				{
					if ( ((BaseArmor)item).Attributes.BonusHits == 15 ) from.SendMessage( "That already has bonus hits!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put bonus hits on that there!" );
						else
						{
							((BaseArmor)item).Attributes.BonusHits += augmentper; 
							from.SendMessage( "You magically add bonus hits to your item...." );
							m_Deed.Delete();
						}
					}
				}
				if (item is BaseJewel)
				{
					if ( ((BaseJewel)item).Attributes.BonusHits == 15 ) from.SendMessage( "That already has bonus hits!");
					else
					{
						if( item.RootParent != from ) from.SendMessage( "You cannot put bonus hits on that there!" );
						else
						{
							((BaseJewel)item).Attributes.BonusHits += augmentper; 
							from.SendMessage( "You magically add bonus hits to your item...." );
							m_Deed.Delete();
						}
					}
				}
			}
			else from.SendMessage( "You can not put bonus hits on that" );
		}
	}

	public class BonusHitsDeed : Item
	{
		[Constructable]
		public BonusHitsDeed() : base( 0x14F0 )
		{
			Name = "a Bonus Hits deed";
			Hue = 0x492;
		}

		public BonusHitsDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to add bonus hits to?"  );
				from.Target = new BonusHitsTarget( this );
			 }
		}
	}
}