using System;
using Server.Gumps;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
	public class AddonHelper : Item
	{
		[Constructable]
		public AddonHelper() : base( 0xF91 )
		{
			Name = "Addon Helper - Put your items on your addons";
			Hue = 80;
			Weight = 1;
		}

		public AddonHelper( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.CloseGump( typeof( DecoAddonGump ) );
			from.SendGump( new DecoAddonGump( from ) );
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
		}
	}
	public class DecoAddonGump : Gump
	{
		private Mobile m_From;

		public DecoAddonGump( Mobile from ) : base( 50, 50 )
		{
			m_From = from;

			m_From.CloseGump( typeof( DecoAddonGump ) );

			AddPage( 0 );

            	AddLabel( 100, 39, 0x486, "Add item to addon");
			AddButton( 63, 35, 9726, 9728, 1, GumpButtonType.Reply, 1 );
            	AddLabel( 63, 70, 0x486, "Secure the item afterwards!");
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 1 )
			{
				BaseHouse house = BaseHouse.FindHouseAt( m_From );
				if ( house == null || !house.IsOwner( m_From ) )
				{
					if ( m_From.AccessLevel >= AccessLevel.GameMaster )
					{
						m_From.SendMessage( "Target an item you wish to place on an addon." );
						m_From.Target = new DecoAddonTarget( m_From );
					}
					else
					{
						m_From.SendMessage( "You are not the full owner of this house." );
					}
				}
				else 
				{
					m_From.SendMessage( "Target an item you wish to place in an addon." );
					m_From.Target = new DecoAddonTarget( m_From );
				}
			}
		}
		private class DecoAddonTarget : Target
		{
			Item its = null;
			Item id = null;

			public DecoAddonTarget( Mobile from ) : base( 10, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					if ( targeted is AddonComponent)
					{
						from.SendMessage( "Please target an item, not an addon." );
						from.SendGump( new DecoAddonGump( from ) );
					}
					else
					{
						id = ( Item )targeted;
						if ( id.Movable )
						{
							its = ( Item )targeted;
							from.SendMessage( "Please target the addon tile you wish this item placed at.");
							from.Target = new DecoAddonTarget2( its );
						}
						else
						{
							from.SendMessage( "That item is not moveable." );
							from.SendGump( new DecoAddonGump( from ) );
						}
					}
				}
				else
				{
					from.SendMessage( "That is not an item." );
					from.SendGump( new DecoAddonGump( from ) );
				}
			}
		}
		private class DecoAddonTarget2 : Target
		{
			private Item m_Item;
			Item ae = null;

			public DecoAddonTarget2( Item item ) : base( 10, false, TargetFlags.None )
			{
				m_Item = item;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is AddonComponent )
				{	
					ae = ( AddonComponent )targeted;
					int height = ae.ItemData.Height;

					m_Item.MoveToWorld( ae.Location, ae.Map );					
					m_Item.Location = new Point3D( m_Item.Location, m_Item.Z + height + 1 );
					from.SendMessage( "You have placed this item in the addon tile." );
					from.SendMessage( "LOCK IT DOWN and move it up or down with the interior decorator!" );
					from.SendGump( new DecoAddonGump( from ) );
				}
				else
				{
					from.SendMessage( "That is not an addon tile." );
					from.SendGump( new DecoAddonGump( from ) );
				}
			}
		}
	}
}