using System;
using Server;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Items
{
	public class TillerMan : Item
	{
		private BaseBoat m_Boat;
        public BaseBoat Boat { get { return m_Boat; } }

		public TillerMan( BaseBoat boat ) : base( 0x3E4E )
		{
			m_Boat = boat;
			Movable = false;
		}

		public TillerMan( Serial serial ) : base(serial)
		{
		}

		public virtual void SetFacing( Direction dir )
		{
			switch ( dir )
			{
				case Direction.South: ItemID = 0x3E4B; break;
				case Direction.North: ItemID = 0x3E4E; break;
				case Direction.West:  ItemID = 0x3E50; break;
				case Direction.East:  ItemID = 0x3E55; break; //Issue 99. Was Using Incorrect Graphic.
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( m_Boat.Status );
		}

		public virtual void Say( int number )
		{
			PublicOverheadMessage( MessageType.Regular, 0x3B2, number );
		}

		public virtual void Say( int number, string args )
		{
			PublicOverheadMessage( MessageType.Regular, 0x3B2, number, args );
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( m_Boat != null && m_Boat.ShipName != null )
				list.Add( 1042884, m_Boat.ShipName ); // the tiller man of the ~1_SHIP_NAME~
			else
				base.AddNameProperty( list );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_Boat != null && m_Boat.ShipName != null )
				LabelTo( from, 1042884, m_Boat.ShipName ); // the tiller man of the ~1_SHIP_NAME~
			else
				base.OnSingleClick( from );
		}

        public Mobile Pilot { get { return m_Boat != null ? m_Boat.Pilot : null; } }

		public override void OnDoubleClick( Mobile from )
		{
            BaseBoat boat = BaseBoat.FindBoatAt(from, from.Map);
            Item mount = from.FindItemOnLayer(Layer.Mount);

            if (!from.InRange(this.Location, 3))
                from.SendLocalizedMessage(500295); //You are too far away to do that.
            else if (boat == null || m_Boat != boat || m_Boat == null)
                from.SendLocalizedMessage(1116724); //You cannot pilot a ship unless you are aboard it!
            else if (boat.Owner != from)
                from.SendLocalizedMessage(1116726); //This is not your ship!
            else if (from.Flying)
                from.SendLocalizedMessage(1116615); // You cannot pilot a ship while flying!
            else if (from.Mounted && !(mount is BoatMountItem))
                from.SendLocalizedMessage(1010097); //You cannot use this while mounted or flying. 
            else if (from != Pilot && Pilot != null && Pilot == m_Boat.Owner)
                from.SendMessage("Someone is already piloting this vessle!");
            else if (Pilot != null)
                boat.RemovePilot(from);
            else 
                boat.LockPilot(from);
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is MapItem && m_Boat != null && m_Boat.CanCommand( from ) && m_Boat.Contains( from ) )
			{
				m_Boat.AssociateMap( (MapItem) dropped );
			}

			return false;
		}

		public override void OnAfterDelete()
		{
			if ( m_Boat != null )
				m_Boat.Delete();
		}

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (m_Boat != null && (m_Boat.Owner == from || from.AccessLevel > AccessLevel.Player))
            {
                if(m_Boat.Contains(from))
                    list.Add(new RenameShipEntry(this, from));
                else
                    list.Add(new DryDockEntry(m_Boat, from));
            }
        }


        private class RenameShipEntry : ContextMenuEntry
        {
            private TillerMan m_TillerMan;
            private Mobile m_From;

            public RenameShipEntry(TillerMan tillerman, Mobile from)
                : base(1111680, 3)
            {
                m_TillerMan = tillerman;
                m_From = from;
            }

            public override void OnClick()
            {
                if (m_TillerMan != null && m_TillerMan.Boat != null)
                    m_TillerMan.Boat.BeginRename(m_From);
            }
        }

        private class DryDockEntry : ContextMenuEntry
        {
            private Mobile m_From;
            private BaseBoat m_Boat;

            public DryDockEntry(BaseBoat boat, Mobile from)
                : base(1116520, 12)
            {
                m_From = from;
                m_Boat = boat;
            }

            public override void OnClick()
            {
                if (m_Boat != null && !m_Boat.Contains(m_From))
                    m_Boat.BeginDryDock(m_From);
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );//version

			writer.Write( m_Boat );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Boat = reader.ReadItem() as BaseBoat;

					if ( m_Boat == null )
						Delete();

					break;
				}
			}
		}
	}
}