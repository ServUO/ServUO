using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using Server.ContextMenus; 

namespace Server.Items
{
	public class SlayerRemovalTarget : Target
	{
		private SlayerRemovalDeed m_Deed;

		public SlayerRemovalTarget( SlayerRemovalDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is BaseWeapon )
			{
				Item item = (Item)target;

				if ( !m_Deed.SlayerOne && !m_Deed.SlayerTwo )
				{
					from.SendMessage( "Your deed is not set to remove any slayers!" );
				}
				else if ( ((BaseWeapon)item).Slayer == SlayerName.None && ((BaseWeapon)item).Slayer2 == SlayerName.None )
				{
					from.SendMessage( "That has no slayers to remove!");
				}
				else
				{
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot remove any slayers there!" );
					}
					else
					{
						if( m_Deed.SlayerOne && m_Deed.SlayerTwo && ((BaseWeapon)item).Slayer != SlayerName.None && ((BaseWeapon)item).Slayer2 != SlayerName.None )
						{
							((BaseWeapon)item).Slayer = SlayerName.None;
							((BaseWeapon)item).Slayer2 = SlayerName.None;
							from.SendMessage( "You remove both slayers from the weapon." );
							m_Deed.Delete();
						}						
						else if( m_Deed.SlayerOne && ((BaseWeapon)item).Slayer != SlayerName.None )
						{
							((BaseWeapon)item).Slayer = SlayerName.None;
							from.SendMessage( "You remove the slayer from the weapon." );
							m_Deed.Delete();
						}
						else if( m_Deed.SlayerTwo && ((BaseWeapon)item).Slayer2 != SlayerName.None )
						{
							((BaseWeapon)item).Slayer2 = SlayerName.None;
							from.SendMessage( "You remove the slayer from the weapon." );
							m_Deed.Delete();
						}
					}
				}
			}
			else if ( target is Spellbook )
			{
				Item item = (Item)target;

				if ( !m_Deed.SlayerOne && !m_Deed.SlayerTwo )
				{
					from.SendMessage( "Your deed is not set to remove any slayers!" );
				}
				else if ( ((Spellbook)item).Slayer == SlayerName.None && ((Spellbook)item).Slayer2 == SlayerName.None )
				{
					from.SendMessage( "That has no slayers to remove!");
				}
				else
				{
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot remove any slayers there!" );
					}
					else
					{
						if( m_Deed.SlayerOne && m_Deed.SlayerTwo && ((Spellbook)item).Slayer != SlayerName.None && ((Spellbook)item).Slayer2 != SlayerName.None )
						{
							((Spellbook)item).Slayer = SlayerName.None;
							((Spellbook)item).Slayer2 = SlayerName.None;
							from.SendMessage( "You remove both slayers from the spellbook." );
							m_Deed.Delete();
						}						
						else if( m_Deed.SlayerOne && ((Spellbook)item).Slayer != SlayerName.None )
						{
							((Spellbook)item).Slayer = SlayerName.None;
							from.SendMessage( "You remove the slayer from the spellbook." );
							m_Deed.Delete();
						}
						else if( m_Deed.SlayerTwo && ((Spellbook)item).Slayer2 != SlayerName.None )
						{
							((Spellbook)item).Slayer2 = SlayerName.None;
							from.SendMessage( "You remove the slayer from the spellbook." );
							m_Deed.Delete();
						}
					}
				}
			}
			else
			{
				from.SendMessage( "The deed cannot be used on this." );
			}
		}
	}


	public class SlayerRemovalDeed : Item
	{
		public bool SlayerOne;
		public bool SlayerTwo;

		[Constructable]
		public SlayerRemovalDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "Slayer Removal Deed";
			LootType = LootType.Blessed;
			Hue = 1175;

			SlayerOne = true;
			SlayerTwo = false;
			
		}

		public SlayerRemovalDeed( Serial serial ) : base( serial )
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
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				 from.SendLocalizedMessage( 1042001 );
			}
			else
			{
				from.SendMessage("What item would you like to remove slayer from?");
				from.Target = new SlayerRemovalTarget( this );
			 }
		}

        	public override void GetProperties( ObjectPropertyList list )
        	{
            		base.GetProperties( list );

			if( this.SlayerOne && this.SlayerTwo )
		    		list.Add(String.Format("Removing: Top and Bottom Slayer"));
			else if( this.SlayerOne && !this.SlayerTwo )
		    		list.Add(String.Format("Removing: Top Slayer"));
			else if( !this.SlayerOne && this.SlayerTwo )
		    		list.Add(String.Format("Removing: Bottom Slayer"));
			else if( this.SlayerOne && this.SlayerTwo )
		    		list.Add(String.Format("Removing: none"));
			
        	}


	#region ContextMenu
        private class PropsEntry : ContextMenuEntry
        {
            private SlayerRemovalDeed m_Item;
            private Mobile m_Mobile;
          
            public PropsEntry( Mobile from, Item item )
                : base( 5109 ) // "Switch"
            {
                m_Item = ( SlayerRemovalDeed )item; 
                m_Mobile = from;         
            }

            public override void OnClick()
            {   
		if ( m_Item.IsChildOf( m_Mobile.Backpack ) )
		{                                                                   		//When this menu entry is clicked by the player.. Do the following:

			if ( m_Item.SlayerOne && m_Item.SlayerTwo )
			{
				m_Item.SlayerOne = true;
				m_Item.SlayerTwo = false;
				m_Mobile.SendMessage( "The deed is set to remove only the top slayer.");
				m_Item.InvalidateProperties();
			}
			else if ( m_Item.SlayerOne && !m_Item.SlayerTwo )
			{
				m_Item.SlayerOne = false;
				m_Item.SlayerTwo = true;
				m_Mobile.SendMessage( "The deed is set to remove only the bottom slayer.");
				m_Item.InvalidateProperties();
			}
			else if ( !m_Item.SlayerOne && m_Item.SlayerTwo )
			{
				m_Item.SlayerOne = true;
				m_Item.SlayerTwo = true;
				m_Mobile.SendMessage( "The deed is set to remove both slayers.");
				m_Item.InvalidateProperties();
			}
		}
		else m_Mobile.SendLocalizedMessage( 1042001 ); 					// That must be in your pack for you to use it.
            }
        }


        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {                                                                           		//We are overriding GetContextMenuEntries because we want to do something to it.
            base.GetContextMenuEntries( from, list );                               		//Items and Mobiles may already have context menus on them.. 
            SlayerRemovalDeed.GetContextMenuEntries( from, this, list );                 	//We want to add another menu entry to what already exists, so call our function that makes the addition
        }

        public static void GetContextMenuEntries( Mobile from, Item item, List<ContextMenuEntry> list )
        {
            list.Add( new PropsEntry( from, item ) );                               		//Add the context menu we just created to the list of context menus that go with this item
        }
        #endregion

	}
}