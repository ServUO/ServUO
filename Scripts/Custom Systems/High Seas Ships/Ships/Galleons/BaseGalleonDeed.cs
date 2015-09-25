using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public abstract class BaseGalleonDeed : Item
	{
		private int _multiIDSouth, _multiIDEast, _multiIDWest, _multiIDNorth;
		private Direction _chosenDirection;	

		[CommandProperty( AccessLevel.GameMaster )]
		public int MultiID{ get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Offset{ get; set; }
		
		public abstract BaseGalleon Boat{ get; }		

		public BaseGalleonDeed( int id, Point3D offset ) : base( 0x14F2 )
		{
			Weight = 1.0;

			if ( !Core.AOS )
			LootType = LootType.Newbied;
      
      		Hue = 2401;
			MultiID = id;
			_multiIDNorth = id;
			_multiIDSouth = id + 2;
			_multiIDEast = id + 1;
			_multiIDWest = id + 3;
			
			Offset = offset;
			_chosenDirection = Direction.North;
		}

		public BaseGalleonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( _multiIDNorth ); //version 1
			writer.Write( _multiIDEast );
			writer.Write( _multiIDSouth );
			writer.Write( _multiIDWest );	

			writer.Write( MultiID );
			writer.Write( Offset );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					_multiIDNorth = reader.ReadInt();
					_multiIDEast = reader.ReadInt();
					_multiIDSouth = reader.ReadInt();
					_multiIDWest = reader.ReadInt();
					
					goto case 0;
				}
				
				case 0:
				{
					MultiID = reader.ReadInt();
					Offset = reader.ReadPoint3D();

					break;
				}
			}

			if ( Weight == 0.0 )
				Weight = 1.0;
		}

		public override void OnDoubleClick( Mobile from )
		{
			from.CloseGump(typeof(GalleonPlacementGump));
			from.SendGump(new GalleonPlacementGump(from, this, null));
		}
		
		public void PlacementDirection( Mobile from, Direction chosenDirection)
		{
			_chosenDirection = chosenDirection;
		
			switch ( chosenDirection )
			{		
					case Direction.West:
					{
						MultiID = _multiIDWest;	

						break;
					}	

					case Direction.South:
					{
						MultiID = _multiIDSouth;	

						break;					
					}

					case Direction.East:
					{
						MultiID = _multiIDEast;
						
						break;
					}	

					case Direction.North:
					{
						MultiID = _multiIDNorth;
						
						break;
					}					
			}
			
			ShipPlacement(from);
		}
		
		public void ShipPlacement( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.AccessLevel < AccessLevel.GameMaster && (from.Map == Map.Ilshenar || from.Map == Map.Malas) )
			{
				from.SendLocalizedMessage( 1010567, null, 0x25 ); // You may not place a boat from this location.
			}
			else
			{
				if ( Core.SE )
					from.SendLocalizedMessage( 502482 ); // Where do you wish to place the ship?
				else
					from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 502482 ); // Where do you wish to place the ship?

				from.Target = new InternalTarget( this );
			}
		}

		public void OnPlacement( Mobile from, Point3D p )
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

				if ( from.Region.IsPartOf( typeof( HouseRegion ) ) /*|| BaseGalleon.FindBoatAt( from, from.Map ) != null */)
				{
					from.SendLocalizedMessage( 1010568, null, 0x25 ); // You may not place a ship while on another ship or inside a house.
					return;
				}

				BaseGalleon boat = Boat;

				if ( boat == null )
					return;

				p = new Point3D( p.X - Offset.X, p.Y - Offset.Y, p.Z - Offset.Z );

				if ( boat.CanFit( p, map, boat.ItemID ) )
				{
					Delete();
				
					boat.Owner = from;

					uint keyValue = boat.CreateKeys( from );

					for (int i = 0; i < boat.Ropes.Count; ++i)
						boat.Ropes[i].KeyValue = keyValue;

					boat.MoveToWorld( p, map );
					
					boat.SetFacing(_chosenDirection);
				}
				else
				{
					boat.Delete();
					from.SendLocalizedMessage( 1043284 ); // A ship can not be created here.
				}
			}
		}

		private class InternalTarget : MultiTarget
		{
			private BaseGalleonDeed _deed;

			public InternalTarget( BaseGalleonDeed deed ) : base( deed.MultiID, deed.Offset )
			{
				_deed = deed;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D ip = o as IPoint3D;

				if ( ip != null )
				{
					if ( ip is Item )
						ip = ((Item)ip).GetWorldTop();

					Point3D p = new Point3D( ip );

					Region region = Region.Find( p, from.Map );

					if ( region.IsPartOf( typeof( DungeonRegion ) ) )
						from.SendLocalizedMessage( 502488 ); // You can not place a ship inside a dungeon.
					else if ( region.IsPartOf( typeof( HouseRegion ) ) || region.IsPartOf( typeof( ChampionSpawnRegion ) ) )
						from.SendLocalizedMessage( 1042549 ); // A boat may not be placed in this area.
					else
						_deed.OnPlacement( from, p );
				}
			}
		}
	}
}