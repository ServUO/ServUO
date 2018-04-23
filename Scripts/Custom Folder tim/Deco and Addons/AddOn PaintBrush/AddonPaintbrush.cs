
//Created by Ashlar, beloved of Morrigan
//If and only if this header remains intact,permission is granted to
//do anything you wish with this script.  Sharing is encouraged :)
//I would expect DarkSaints shard not to use this however.

using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Multis;
using System.Collections;
using Server.ContextMenus;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class AddonPaintbrush : Item, IRewardItem
	{
//Begin region changable in-game
//		Basicly upon item creation the allowed region is null.  
//		A staff member can context menu mark the region to the current one.
//		Once the region is set the AddonPaintbrush can be [dupe with the region already set.

		private string m_HomeRegion;

		[CommandProperty( AccessLevel.GameMaster )]
		public string HomeRegion{ get{ return m_HomeRegion; } set{ m_HomeRegion = value; }}

		private class SetRegionEntry : ContextMenuEntry
		{
			private AddonPaintbrush m_Item;
			private Mobile m_Mobile;

			public SetRegionEntry( Mobile from, Item item ) : base( 2055 ) // uses "Mark" entry
			{
				m_Item = (AddonPaintbrush)item;
				m_Mobile = from;
			}
			public override void OnClick()
			{
				// set region location
				m_Item.HomeRegion = m_Mobile.Region.Name;
				m_Mobile.SendMessage( "The usable region on your addon paintbrush has been set to your current region." );
			}
		}

		/*public static void GetContextMenuEntries( Mobile from, Item item, ArrayList list )
		{
			list.Add( new SetRegionEntry( from, item ) );
		}

		public override void GetContextMenuEntries(Mobile from, ArrayList list)
		{
			if ( from.AccessLevel >= AccessLevel.GameMaster )
			{
				base.GetContextMenuEntries( from, list );
				AddonPaintbrush.GetContextMenuEntries( from, this, list );
			}
			else
			{
				from.SendMessage( "Only staff can change the usable region." );
				return;
			}
		}*/
//End region changable in-game

        private bool m_IsRewardItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

		[Constructable]
		public AddonPaintbrush() : base( 0x3EE8 )
		{
			Name = "an Addon Paintbrush";
			Weight = 1.0;
		}

		public AddonPaintbrush( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return /*false*/;
            }

			BaseHouse house = BaseHouse.FindHouseAt( from );
			bool InRegion = ( from.Region.Name == HomeRegion );

			if ( !InRegion )
			{
				if ( house == null || !house.IsOwner( from ) )
				{
					from.SendMessage( "You cannot use this here." );
				}
				else
				{
					from.SendMessage( "Target an item, the paintbrush or a paintbrush item." );
					from.Target = new AddonPaintbrushTarget( this );
				}
			}
			else
			{
				from.SendMessage( "Target an item, the paintbrush or a paintbrush item." );
				from.Target = new AddonPaintbrushTarget( this );
			}
 		}
		private class AddonPaintbrushTarget : Target
		{
			private Item m_Item;
			Item id = null;

			public AddonPaintbrushTarget( Item item ) : base( 3, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Item.Deleted )
					return;

				else if ( targeted is AddonComponent )
				{	
					id = ( AddonComponent )targeted;
					{
						bool onSelf = ( m_Item.ItemID == id.ItemID );
						if ( onSelf )
						{
							m_Item.Location = new Point3D( id.Location, id.Z + 1 );
							from.SendMessage( "Raised." );
						}
						else
						{
							id.ItemID = m_Item.ItemID;
							id.Name = m_Item.Name;
							id.Hue = m_Item.Hue;
							from.SendMessage( "You have changed the addon.");
						}
					}
				}
				else if ( targeted is Item )
				{
					id = ( Item )targeted;

					bool onSelf = ( m_Item == id );
					if ( onSelf )
					{
						PaintbrushItemAddon PbI = new PaintbrushItemAddon();
						PbI.MoveToWorld( from.Location, from.Map );
						from.SendMessage( "You created a new addon piece, now decorate it!"); 
					}
					else
					{
						m_Item.ItemID = id.ItemID;
						m_Item.Name = id.Name;
						m_Item.Hue = id.Hue;
						from.SendMessage( "You have changed the addon paintbrush.");
					}
				}
				else
				{
					from.SendMessage( "That is not even possible...for now." );
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( m_HomeRegion );
            writer.Write((bool)m_IsRewardItem);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_HomeRegion = reader.ReadString();
            m_IsRewardItem = reader.ReadBool();
		}
	}
}
