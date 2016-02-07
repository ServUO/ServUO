using System;
using Server;

namespace Server.Items
{
	public class SimpleMap : MapItem
	{
		private int m_PinIndex;

		[CommandProperty( AccessLevel.GameMaster )]
		public int CurrentPin
		{
			// get/set the index (one-based) of the pin that will be referred to by PinLocation
			get{ return m_PinIndex; }
			set{ m_PinIndex = value;}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int NPins
		{
			get 
			{ 
				if(Pins != null)
				{
					return Pins.Count; 
				}
				else
				{
					return 0;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point2D PinLocation
		{
			set 
			{ 
				// change the coordinates of the current pin
				if(Pins != null && CurrentPin > 0 && CurrentPin <=Pins.Count)
				{
					int mapx, mapy;
					ConvertToMap(value.X, value.Y, out mapx, out mapy);
					Pins[CurrentPin -1] = new Point2D(mapx, mapy);
				} 
			}
			get
			{
				// get the coordinates of the current pin
				if(Pins != null && CurrentPin > 0 && CurrentPin <=Pins.Count)
				{
					int mapx, mapy;
					ConvertToWorld(((Point2D)Pins[CurrentPin -1]).X, ((Point2D)Pins[CurrentPin -1]).Y, out mapx, out mapy);
					return new Point2D(mapx, mapy);
				} 
				else
				{
					return Point2D.Zero;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point2D NewPin
		{
			set 
			{ 
				// add a new pin at the specified world coordinate
				AddWorldPin(value.X, value.Y);
				CurrentPin = NPins;
			}
			get
			{
				// return the last pin added to the Pins arraylist
				if(Pins != null && NPins > 0)
				{
					int mapx, mapy;
					ConvertToWorld(((Point2D)Pins[NPins -1]).X, ((Point2D)Pins[NPins -1]).Y, out mapx, out mapy);
					return new Point2D(mapx, mapy);
				} 
				else
				{
					return Point2D.Zero;
				}
			}
		}


		[CommandProperty( AccessLevel.GameMaster )]
		public bool ClearAllPins
		{
			get { return false; }
			set { if(value == true) ClearPins(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PinRemove
		{
			set { RemovePin(value); }
			get { return 0; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile ShowTo
		{
			set 
			{ 
				if(value != null)
				{
					//DisplayTo(value);
					OnDoubleClick(value);
				}
			}
			get { return null; }
		}


		[Constructable]
		public SimpleMap()
		{
			SetDisplay( 0, 0, 5119, 4095, 400, 400 );
		}

		public override int LabelNumber{ get{ return 1025355; } } // map

		public SimpleMap( Serial serial ) : base( serial )
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

			int version = reader.ReadInt();
		}
	}
}