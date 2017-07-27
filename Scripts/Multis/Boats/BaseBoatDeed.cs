using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Gumps;
using Server.Items;

namespace Server.Multis
{
	public abstract class BaseBoatDeed : Item
	{
		private int m_MultiID;
		private Point3D m_Offset;
        private Direction m_Direction;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MultiID{ get{ return m_MultiID; } set{ m_MultiID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Offset{ get{ return m_Offset; } set{ m_Offset = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction BoatDirection { get { return m_Direction; } set { m_Direction = value; } }

		public BaseBoatDeed( int id, Point3D offset ) : base( 0x14F2 )
		{
			Weight = 1.0;

			if ( !Core.AOS )
				LootType = LootType.Newbied;

			m_MultiID = id;
			m_Offset = offset;
            m_Direction = Direction.North;
		}

		public BaseBoatDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_MultiID );
			writer.Write( m_Offset );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_MultiID = reader.ReadInt();
					m_Offset = reader.ReadPoint3D();

					break;
				}
			}

			if ( Weight == 0.0 )
				Weight = 1.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.AccessLevel < AccessLevel.GameMaster && (from.Map == Map.Ilshenar || from.Map == Map.Malas) )
			{
				from.SendLocalizedMessage( 1010567, null, 0x25 ); // You may not place a boat from this location.
			}
            else if (IsGalleon() && BaseGalleon.HasGalleon(from) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendLocalizedMessage(1116758); //You already have a ship deployed!
            }
            else if(!from.HasGump(typeof(BoatPlacementGump)))
            {
                if (Core.SE)
                    from.SendLocalizedMessage(502482); // Where do you wish to place the ship?
                else
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502482); // Where do you wish to place the ship?

                from.SendGump(new BoatPlacementGump(this, from));
            }
		}

        public bool IsGalleon()
        {
            return this is BritannianShipDeed || this is GargishGalleonDeed || this is TokunoGalleonDeed || this is OrcishGalleonDeed;
        }

		public abstract BaseBoat Boat{ get; }

		public void OnPlacement( Mobile from, Point3D p, int itemID, Direction d )
		{
			if ( Deleted )
			{
				return;
			}
			else if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				Map map = from.Map;

				if ( map == null )
					return;

				if ( from.AccessLevel < AccessLevel.GameMaster && (map == Map.Ilshenar || map == Map.Malas) )
				{
					from.SendLocalizedMessage( 1043284 ); // A ship can not be created here.
					return;
				}

				if ( from.Region.IsPartOf( typeof( HouseRegion ) ) || (Server.Multis.BaseBoat.FindBoatAt( from, from.Map ) != null && !(this is RowBoatDeed)) )
				{
					from.SendLocalizedMessage( 1010568, null, 0x25 ); // You may not place a ship while on another ship or inside a house.
					return;
				}

                m_Direction = d;
				BaseBoat boat = Boat;

				if ( boat == null )
					return;

				p = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );

				if ( BaseBoat.IsValidLocation( p, map ) && boat.CanFit( p, map, itemID ) )
				{
					Delete();

					boat.Owner = from;
                    boat.ItemID = itemID;

                    if (boat is BaseGalleon)
                    {
                        ((BaseGalleon)boat).SecurityEntry = new SecurityEntry((BaseGalleon)boat);
                        ((BaseGalleon)boat).BaseBoatHue = RandomBasePaintHue();
                    }

                    if (boat.IsClassicBoat)
                    {
                        uint keyValue = boat.CreateKeys(from);

                        if (boat.PPlank != null)
                            boat.PPlank.KeyValue = keyValue;

                        if (boat.SPlank != null)
                            boat.SPlank.KeyValue = keyValue;
                    }

					boat.MoveToWorld( p, map );

                    var addon = LighthouseAddon.GetLighthouse(from);

                    if (addon != null)
                    {
                        if (boat.CanLinkToLighthouse)
                            from.SendLocalizedMessage(1154592); // You have linked your boat lighthouse.
                        else
                            from.SendLocalizedMessage(1154597); // Failed to link to lighthouse.
                    }
				}
				else
				{
					boat.Delete();
					from.SendLocalizedMessage( 1043284 ); // A ship can not be created here.
				}
			}
		}

        private int RandomBasePaintHue()
        {
            if (0.6 > Utility.RandomDouble())
            {
                return Utility.RandomMinMax(1701, 1754);
            }

            return Utility.RandomMinMax(1801, 1908);
        }
	}
}