using System;
using System.IO;
using UOArchitectInterface;
using Server.Targeting;
using System.Collections;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System.Threading;
using OrbServerSDK;
using Server.Commands;

namespace Server.Engines.UOArchitect
{
	public class ExtractItemsRequest : BaseOrbToolRequest  
	{
		private BoundingBoxPickerEx _picker;
		private Rect2DCol _rects = new Rect2DCol();
		private Map _map = null;
		private DesignItemCol _items = new DesignItemCol();
		private ExtractRequestArgs _args;
		private ArrayList _extractedMultiIds = new ArrayList();

		public static void Initialize()
		{
			OrbRemoteServer.OrbServer.Register("UOAR_ExtractDesign", typeof(ExtractItemsRequest), AccessLevel.GameMaster, true);
		}

		public override void OnRequest(OrbClientInfo client, OrbRequestArgs args)
		{
			FindOnlineMobile(client);

			if(args == null)
				SendResponse(null);
			else if(!(args is ExtractRequestArgs))
				SendResponse(null);
			else if(!this.IsOnline)
				SendResponse(null);

			_args = args as ExtractRequestArgs;

			if(_args.ItemSerials == null)
			{
				_picker = new BoundingBoxPickerEx();
				_picker.OnCancelled += new BoundingBoxExCancelled(OnTargetCancelled);
				_picker.Begin( this.Mobile, new BoundingBoxCallback( BoundingBox_Callback ), null );
			}
			else
			{
				ExtractItems(_args.ItemSerials);
			}
		}

		private void BoundingBox_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
		{
			Utility.FixPoints( ref start, ref end );
			Rectangle2D rect = new Rectangle2D(start, end);
			_map = map;
		
			_rects.Add(new Rect2D(rect.Start.X, rect.Start.Y, rect.Width, rect.Height));

			if(_args.MultipleRects)
			{
				_picker.Begin( this.Mobile, new BoundingBoxCallback( BoundingBox_Callback ), null );
			}
			else
			{
				ExtractItems();
			}
		}

		private void ExtractItems(int[] itemSerials)
		{
			for(int i=0; i < itemSerials.Length; ++i)
			{
				Item item = World.FindItem(itemSerials[i]);

				if(item != null)
				{
					DesignItem designItem = new DesignItem();
					designItem.ItemID = (short)item.ItemID;
					designItem.X = item.X;
					designItem.Y = item.Y;
					designItem.Z = item.Z;
					designItem.Hue = (short)item.Hue;

					_items.Add(designItem);
				}
			}

			ExtractResponse resp = null;

			if(_items.Count > 0)
				resp = new ExtractResponse(_items);
			else
				resp = null;

			SendResponse(resp);
		}

		private void ExtractItems()
		{
			foreach(Rect2D rect in _rects)
			{
				#region MobileSaver
				Rectangle2D realrectangle = new Rectangle2D( rect.TopX, rect.TopY, rect.Width, rect.Height );

				foreach ( Mobile m in _map.GetMobilesInBounds( realrectangle ) )
				{
					if ( m != null && m is BaseCreature )
					{
						int saveflag = MobileSaver.GetSaveFlag( m );

						if ( saveflag > 0 )
						{
							DesignItem designItem = new DesignItem();
							designItem.ItemID = (short)0x1;
							designItem.X = m.X;
							designItem.Y = m.Y;
							designItem.Z = m.Z + saveflag;
							designItem.Hue = (short)m.Hue;
						}
					}
				}
				#endregion

				for ( int x = 0; x <= rect.Width; ++x )
				{
					for ( int y = 0; y <= rect.Height; ++y )
					{
						int tileX = rect.TopX + x;
						int tileY = rect.TopY + y;

						Sector sector = _map.GetSector( tileX, tileY );

						if (_args.NonStatic || _args.Static)
						{
							for ( int i = 0; i < sector.Items.Count; ++i )
							{
								Item item = (Item)sector.Items[i];

								if(!item.Visible)
									continue;
								else if( (!_args.NonStatic) && !(item is Static) )
									continue;
								else if( (!_args.Static) && (item is Static) )
									continue;
								else if( _args.MinZSet && item.Z < _args.MinZ)
									continue;
								else if( _args.MaxZSet && item.Z > _args.MaxZ)
									continue;

								int hue = 0;

								if(_args.ExtractHues)
									hue = item.Hue;

								if ( item.X == tileX && item.Y == tileY && !((item is BaseMulti) || (item is HouseSign)))
								{
									DesignItem designItem = new DesignItem();
									designItem.ItemID = (short)item.ItemID;
									designItem.X = item.X;
									designItem.Y = item.Y;
									designItem.Z = item.Z;
									designItem.Hue = (short)hue;

									_items.Add(designItem);
								}

								// extract multi
								if(item is HouseFoundation)
								{
									HouseFoundation house = (HouseFoundation)item;

									if(_extractedMultiIds.IndexOf(house.Serial.Value) == -1)
										ExtractCustomMulti(house);
								}
							}
						}

					}
				}
			}

			ExtractResponse response = new ExtractResponse(_items);

			if(_args.Frozen)
			{
				response.Rects = _rects;
				response.Map = _map.Name;
			}

			// send response back to the UOAR tool
			SendResponse(response);
		}

		private void ExtractCustomMulti(HouseFoundation house)
		{
			_extractedMultiIds.Add(house.Serial.Value);

			for(int x=0; x < house.Components.Width; ++x)
			{
				for(int y=0; y < house.Components.Height; ++y)
				{
					StaticTile[] tiles = house.Components.Tiles[x][y];

					for ( int i = 0; i < tiles.Length; ++i )
					{
						DesignItem designItem = new DesignItem();
						designItem.ItemID = (short)(tiles[i].ID ^ 0x4000);
	
						designItem.X = x + house.Sign.Location.X;
						designItem.Y = (y + house.Sign.Location.Y) - (house.Components.Height - 1);
						designItem.Z = house.Location.Z + tiles[i].Z;
						
						_items.Add(designItem);
					}

				}
			}

			DesignItem sign = new DesignItem();
			sign.ItemID = (short)(house.Sign.ItemID);
	
			sign.X = house.Sign.Location.X;
			sign.Y = house.Sign.Location.Y;
			sign.Z = house.Sign.Location.Z;
						
			_items.Add(sign);
		}

		private void OnTargetCancelled()
		{
			if(_rects.Count > 0)
				ExtractItems();
			else
				SendResponse(null);
		}

//		private void GetFrozenItems( Map map, IPoint2D start, IPoint2D end, ref ArrayList designItems)
//		{
//			start = map.Bound( new Point2D(start) );
//			end = map.Bound( new Point2D(end) );
//
//			int xStartBlock = start.X >> 3;
//			int yStartBlock = start.Y >> 3;
//			int xEndBlock = end.X >> 3;
//			int yEndBlock = end.Y >> 3;
//
//			int xTileStart = start.X, yTileStart = start.Y;
//			int xTileWidth = end.X - start.X + 1, yTileHeight = end.Y - start.Y + 1;
//
//			TileMatrix matrix = map.Tiles;
//
//			using ( FileStream idxStream = OpenFile(matrix.IndexStream) )
//			{
//				using ( FileStream mulStream = OpenFile(matrix.DataStream) )
//				{
//					if ( idxStream == null || mulStream == null )
//					{
//						return;
//					}
//
//					BinaryReader idxReader = new BinaryReader( idxStream );
//
//					//for ( int x = xStartBlock; x <= xEndBlock; ++x )
//					for ( int x = xTileStart; x <= xTileWidth; ++x )
//					{
//						//for ( int y = yStartBlock; y <= yEndBlock; ++y )
//						for ( int y = yTileStart; y <= yTileHeight; ++y )
//						{
//							int tileCount;
//							StaticTile[] staticTiles = ReadStaticBlock( idxReader, mulStream, x, y, matrix.BlockWidth, matrix.BlockHeight, out tileCount );
//
//							if ( tileCount < 0 )
//								continue;
//							
//							for(int i = 0; i < tileCount; ++i)
//							{
//								StaticTile tile = staticTiles[i];
//								DesignItem item = new DesignItem();
//
//								item.X = x;
//								item.Y = y;
//								item.Z = tile.m_Z;
//								item.Hue = _args.ExtractHues ? (short)tile.m_Hue : (short)0;
//
//								designItems.Add(item);
//							}
//						}
//					}
//				}
//			}
//		}

//		private byte[] m_Buffer;
//		private StaticTile[] m_TileBuffer = new StaticTile[128];
//
//		private StaticTile[] ReadStaticBlock( BinaryReader idxReader, FileStream mulStream, int x, int y, int width, int height, out int count )
//		{
//			try
//			{
//				if ( x < 0 || x >= width || y < 0 || y >= height )
//				{
//					count = -1;
//					return m_TileBuffer;
//				}
//
//				idxReader.BaseStream.Seek( ((x * height) + y) * 12, SeekOrigin.Begin );
//
//				int lookup = idxReader.ReadInt32();
//				int length = idxReader.ReadInt32();
//
//				if ( lookup < 0 || length <= 0 )
//				{
//					count = 0;
//				}
//				else
//				{
//					count = length / 7;
//
//					mulStream.Seek( lookup, SeekOrigin.Begin );
//
//					if ( m_TileBuffer.Length < count )
//						m_TileBuffer = new StaticTile[count];
//
//					StaticTile[] staTiles = m_TileBuffer;
//
//					if ( m_Buffer == null || length > m_Buffer.Length )
//						m_Buffer = new byte[length];
//
//					mulStream.Read( m_Buffer, 0, length );
//
//					int index = 0;
//
//					for ( int i = 0; i < count; ++i )
//					{
//						staTiles[i].m_ID = (short)(m_Buffer[index++] | (m_Buffer[index++] << 8));
//						staTiles[i].m_X = m_Buffer[index++];
//						staTiles[i].m_Y = m_Buffer[index++];
//						staTiles[i].m_Z = (sbyte)m_Buffer[index++];
//						staTiles[i].m_Hue = (short)(m_Buffer[index++] | (m_Buffer[index++] << 8));
//					}
//				}
//			}
//			catch
//			{
//				count = -1;
//			}
//
//			return m_TileBuffer;
//		}
//
//		private FileStream OpenFile( FileStream orig )
//		{
//			if ( orig == null )
//				return null;
//
//			try{ return new FileStream( orig.Name, FileMode.Open, FileAccess.Read, FileShare.Read ); }
//			catch{ return null; }
//		}

	}

}
