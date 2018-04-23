//Add luck to an item
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class LuckIncreaseTarget : Target
	{			
		private LuckIncreaseDeed m_Deed;

		public LuckIncreaseTarget( LuckIncreaseDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}
		
		protected override void OnTarget( Mobile from, object target )
		{
			int luckAdd = 100; //Amount of luck to be added
			int luckCap = 500; //Limit of luck that an item can have
			
			//Change to false if you don't want it to be used on any of these items
			bool allowWeapon = true;
			bool allowArmor = true;
			bool allowJewel = true;
			bool allowClothing = true;
			bool allowSpellbook = true;
			bool allowTalisman = true;
			bool allowQuiver = true;
			
			if ( target is BaseWeapon && allowWeapon)
			{
				Item item = (Item)target;
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add Luck to that item there!" ); 
					}
					else
					{
						luckAdd = luckToAdd(((BaseWeapon)item).Attributes.Luck, luckAdd, luckCap, from);
						if( luckAdd > 0 )
						{
							((BaseWeapon)item).Attributes.Luck += luckAdd;
							m_Deed.Delete();
						}
					}
					return;
			}
			else if ( target is BaseArmor && allowArmor )
			{
					Item item = (Item)target;
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add Luck to that item there!" ); 
					}
					else
					{
						luckAdd = luckToAdd(((BaseArmor)item).Attributes.Luck, luckAdd, luckCap, from);
						if( luckAdd > 0 )
						{
							((BaseArmor)item).Attributes.Luck += luckAdd;
							m_Deed.Delete();
						}
					}
				
			}
			else if ( target is BaseClothing && allowClothing )
			{
					Item item = (Item)target;
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add Luck to that item there!" ); 
					}
					else
					{
						luckAdd = luckToAdd(((BaseClothing)item).Attributes.Luck, luckAdd, luckCap, from);
						if( luckAdd > 0 )
						{
							((BaseClothing)item).Attributes.Luck += luckAdd;
							m_Deed.Delete();
						}
					}
			}
			else if ( target is BaseTalisman && allowTalisman )
			{
					Item item = (Item)target;
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add Luck to that item there!" ); 
					}
					else
					{
						luckAdd = luckToAdd(((BaseTalisman)item).Attributes.Luck, luckAdd, luckCap, from);
						if( luckAdd > 0 )
						{
							((BaseTalisman)item).Attributes.Luck += luckAdd;
							m_Deed.Delete();
						}
					}
			}
			else if ( target is BaseJewel && allowJewel )
			{
					Item item = (Item)target;
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add Luck to that item there!" ); 
					}
					else
					{
						luckAdd = luckToAdd(((BaseJewel)item).Attributes.Luck, luckAdd, luckCap, from);
						if( luckAdd > 0 )
						{
							((BaseJewel)item).Attributes.Luck += luckAdd;
							m_Deed.Delete();
						}
					}
			}
			else if ( target is Spellbook && allowSpellbook )
			{
					Item item = (Item)target;
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add Luck to that item there!" ); 
					}
					else
					{
						luckAdd = luckToAdd(((Spellbook)item).Attributes.Luck, luckAdd, luckCap, from);
						if( luckAdd > 0 )
						{
							((Spellbook)item).Attributes.Luck += luckAdd;
							m_Deed.Delete();
						}
					}
			}
			else if ( target is BaseQuiver && allowQuiver )
			{
					Item item = (Item)target;
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add Luck to that item there!" ); 
					}
					else
					{
						luckAdd = luckToAdd(((BaseQuiver)item).Attributes.Luck, luckAdd, luckCap, from);
						if( luckAdd > 0 )
						{
							((BaseQuiver)item).Attributes.Luck += luckAdd;
							m_Deed.Delete();
						}
					}
			}
			else
			{
				from.SendMessage( "You cannot use this deed on that!" );
			}
		}
		
		public int luckToAdd(int itemLuck, int luckAdd ,int luckCap, Mobile from)
		{
			int ret = 0;
			if(itemLuck < luckCap)
			{
				if( (itemLuck + luckAdd ) > luckCap )
				{
					ret = luckAdd - ( (itemLuck + luckAdd ) - luckCap );
					from.SendMessage("You increase the Luck on the item and it has now reached it's max. +"+ret+" Luck has been added.");
				}
				else{
					from.SendMessage( "You increase the Luck on the item. +"+luckAdd+" Luck has been added." );
					ret = luckAdd;
				}
			}
			else
			{
				from.SendMessage( "That item has reached the maximum amount of Luck." );
			}
			
			return ret;
		}
	}

	public class LuckIncreaseDeed : Item
	{
		[Constructable]
		public LuckIncreaseDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "+100 Luck Increase Deed";
			LootType = LootType.Blessed;
			Hue = 1174;
		}

		public LuckIncreaseDeed( Serial serial ) : base( serial )
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
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendMessage("Which item would you like to increase luck?"  );
				from.Target = new LuckIncreaseTarget( this );
			}
		}	
	}
}