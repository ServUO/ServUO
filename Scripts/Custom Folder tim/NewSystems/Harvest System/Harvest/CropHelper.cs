using System;
using Server;
using System.Collections;
using Server.Network;
using Server.Gumps;
using Server.Items; 

namespace Server.Items.Crops 
{ 
	public class BaseCrop : Item
	{
		// Set defaults here for all Crop types
		// Override in Seed scripts to modify per crop type
		public virtual bool CanGrowFarm{ get{ return true; } }
		public virtual bool CanGrowHouseTiles{ get{ return true; } }
		public virtual bool CanGrowDirt{ get{ return false; } }
		public virtual bool CanGrowGround{ get{ return false; } }
		public virtual bool CanGrowSwamp{ get{ return false; } }
		public virtual bool CanGrowSand{ get{ return false; } }
		 // For Player Deeded Plots
		public virtual bool CanGrowGarden{ get{ return false; } }

		// Default time in which sower must reharvest his crops
		// Use TimeSpan.Zero to disable crop decays
		// May be overridden in individual Crop scripts
		public virtual TimeSpan SowerPickTime{ get{ return TimeSpan.FromDays( 7 ); } }  //changed FromDays ( 14 ) to mins...for testing

		// If true any Player can destroy a crop, overriddable in Crop scripts
		public virtual bool PlayerCanDestroy{ get{ return true; } }

		private bool i_bumpZ = false;
		public bool BumpZ
		{
			get{ return i_bumpZ; }
			set{ i_bumpZ = value; }
		}

		public BaseCrop( int itemID ) : base( itemID )
		{
		}

		public BaseCrop( Serial serial ) : base( serial ) 
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
	
	public class CropHelper
	{ 
		// if true Player cannot plant or pick while mounted
		public static bool CanWorkMounted{ get{ return false; } }

		public static int[] FarmTiles = new int[]
		{
			0x009, 0x00A, 
			0x00C, 0x00E, 
			0x013, 0x015, 
			0x150, 0x155, 
			0x15A, 0x15C 
		};

		public static int[] HouseTiles = new int[]  //ADDED HOUSETILES
		{
			0x31F4, 0x31F5, 
			0x31F6, 0x31F7, 
			0x515, 0x516, 
			0x517, 0x518, 
			0x31F4, 0x31F9, 
			0x31FA, 0x31FB 
		};
		
		public static int[] DirtTiles = new int[]
		{
			0x071, 0x07C, // Roads
			0x165, 0x174, // Roads
			0x1DC, 0x1EF, // Rock Border
			0x306, 0x31F, // Snow Border
			0x08D, 0x0A7, // Steep Slopes
			0x2E5, 0x305, // Steep Slopes
			0x777, 0x791, // Steep Slopes
			0x98C, 0x9BF, // Steep Slopes
		};

		public static int[] GroundTiles = new int[]
		{
			0x003, 0x006, 
			0x033, 0x03E, 
			0x078, 0x08C, 
			0x0AC, 0x0DB, 
			0x108, 0x10B, 
			0x14C, 0x174, 
			0x1A4, 0x1A7, 
			0x1B1, 0x1B2, 
			0x26E, 0x281, 
			0x292, 0x295, 
			0x355, 0x37E, 
			0x3CB, 0x3CE, 
			0x547, 0x5A6, 
			0x5E3, 0x618, 
			0x66B, 0x66E, 
			0x6A1, 0x6C2, 
			0x6DE, 0x6E1, 
			0x73F, 0x742, 
		};

		public static int[] SwampTiles = new int[]
		{
			//0x7DC, 0x808, 
			0x3DC1, 0x3DC2, 
			0x3DD9, 0x3EF0, 
		};

		public static int[] SandTiles = new int[]
		{
			0x016, 0x019, 
			0x033, 0x03E, 
			0x1A8, 0x1AB, 
			0x282, 0x291, 
			0x335, 0x35C, 
			0x3B7, 0x3CA, 
			0x5A7, 0x5BA, 
			0x64B, 0x66A, 
			0x66F, 0x672, 
			0x7D5, 0x7D8, 
		};

		public static bool CheckCanGrow( BaseCrop crop, Map map, int x, int y )
		{
			if ( crop.CanGrowFarm && ValidateFarmLand( map, x, y ) )
				return true;
			if ( crop.CanGrowHouseTiles && ValidateHouseTiles( map, x, y ) ) //HOUSETILES
				return true;
			if ( crop.CanGrowDirt && ValidateDirt( map, x, y ) )
				return true;
			if ( crop.CanGrowGround && ValidateGround( map, x, y ) )
				return true;
			if ( crop.CanGrowSand && ValidateSand( map, x, y ) )
				return true;
			if ( crop.CanGrowSwamp && ValidateSwamp( map, x, y ) )
				return true;
			if ( crop.CanGrowGarden )
			{
				crop.BumpZ = ValidateGardenPlot( map, x, y );
				return crop.BumpZ;
			}
			return false;
		}

		public static bool ValidateGardenPlot( Map map, int x, int y )
		{
			bool ground = false;
			
			// Test for Dynamic Item
			IPooledEnumerable eable = map.GetItemsInBounds( new Rectangle2D( x, y, 1, 1 ) );
			foreach( Item item in eable )
			{
				if( item.ItemID == 0x32C9 ) // dirt; possibly also 0x32CA 
					ground = true;
			}
			eable.Free();

			// Test for Frozen into Map
			if ( !ground )
			{
				StaticTile[] tiles = map.Tiles.GetStaticTiles( x, y );
				for ( int i = 0; i < tiles.Length; ++i )
				{
					if ( ( tiles[i].ID & 0x3FFF ) == 0x32C9 )
						ground = true;
				}
			}

			return ground;
		}

		public static bool ValidateFarmLand( Map map, int x, int y )
		{
			int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
			bool ground = false;

			for ( int i = 0; !ground && i < FarmTiles.Length; i += 2 )
				ground = ( tileID >= FarmTiles[i] && tileID <= FarmTiles[i + 1] );

			return ground;
		}

		public static bool ValidateHouseTiles( Map map, int x, int y )	//HOUSETILES
		{
			int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
			bool ground = false;

			for ( int i = 0; !ground && i < HouseTiles.Length; i += 2 )
				ground = ( tileID >= HouseTiles[i] && tileID <= HouseTiles[i + 1] );

			return ground;
		}

		public static bool ValidateDirt( Map map, int x, int y )
		{
			int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
			bool ground = false;

			for ( int i = 0; !ground && i < DirtTiles.Length; i += 2 )
				ground = ( tileID >= DirtTiles[i] && tileID <= DirtTiles[i + 1] );

			return ground;
		}
		
		public static bool ValidateGround( Map map, int x, int y )
		{
			int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
			bool ground = false;

			for ( int i = 0; !ground && i < GroundTiles.Length; i += 2 )
				ground = ( tileID >= GroundTiles[i] && tileID <= GroundTiles[i + 1] );

			return ground;
		}
		
		public static bool ValidateSwamp( Map map, int x, int y )
		{
			int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
			bool ground = false;

			for ( int i = 0; !ground && i < SwampTiles.Length; i += 2 )
				ground = ( tileID >= SwampTiles[i] && tileID <= SwampTiles[i + 1] );

			return ground;
		}
		
		public static bool ValidateSand( Map map, int x, int y )
		{
			int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
			bool ground = false;

			for ( int i = 0; !ground && i < SandTiles.Length; i += 2 )
				ground = ( tileID >= SandTiles[i] && tileID <= SandTiles[i + 1] );

			return ground;
		}
		
		public class GrowTimer : Timer 
		{ 
			private Item i_seedling; 
			private Type t_crop; 
			private Mobile m_sower;
			private int cnt;
			private double cookValue;
			private double rnd;

			public GrowTimer( Item seedling, Type croptype, Mobile sower ) : base( TimeSpan.FromMinutes( 30 ), TimeSpan.FromMinutes( 5 ) ) 
			{ 
				Priority = TimerPriority.OneSecond; 
				i_seedling = seedling; 
				t_crop = croptype;
				m_sower = sower;
				cnt = 0;
				
				rnd = Utility.RandomDouble();
				cookValue = sower.Skills[SkillName.Cooking].Value / 50;
			} 

			protected override void OnTick() 
			{ 
				if ( cnt++ / 20 > rnd ) // between 10 and 30 minutes changed from 100 to 500
				{
					if (( i_seedling != null ) && ( !i_seedling.Deleted )) 
					{ 
						object[] args = {m_sower};
						Item newitem = Activator.CreateInstance( t_crop, args ) as Item;

                        if ( newitem == null/* || Utility.RandomDouble() > cookValue*/ )
						{
							newitem = new Weeds( m_sower );
						}

						newitem.Location = i_seedling.Location; 
						newitem.Map = i_seedling.Map; 
						i_seedling.Delete(); 
					} 
					Stop();
				}
			} 
		} 

		public static ArrayList CheckCrop( Point3D pnt, Map map, int range )
		{
			ArrayList crops = new ArrayList();

			IPooledEnumerable eable = map.GetItemsInRange( pnt, range );
			foreach ( Item crop in eable ) 
			{ 
				if ( ( crop != null ) && ( crop is BaseCrop ) )
					crops.Add( (BaseCrop)crop ); 
			} 
			eable.Free();

			return crops;
		}

		public class Weeds : BaseCrop 
		{ 
			private static DateTime planted;
			private static Mobile m_sower;

			[CommandProperty( AccessLevel.GameMaster )]
			public Mobile Sower{ get{ return m_sower; } set{ m_sower = value; } }
		
			[Constructable] 
			public Weeds( Mobile sower ) : base( Utility.RandomList( 0xCAD, 0xCAE, 0xCAF ) ) 
			{ 
				Movable = false; 
				Name = "a weed"; 

				m_sower = sower;
				planted = DateTime.Now;
			} 

			public override void OnDoubleClick(Mobile from) 
			{ 
				if ( from.Mounted && !CropHelper.CanWorkMounted )
				{
					from.SendMessage( "You cannot pull up a weed while mounted." ); 
					return; 
				}

				if ( from.InRange( this.GetWorldLocation(), 1 ) ) 
				{ 
					if (( from == m_sower ) || ( DateTime.Now >= planted.AddDays(3) ))
					{
						from.Direction = from.GetDirectionTo( this );

						from.Animate( from.Mounted ? 29:32, 5, 1, true, false, 0 ); 

						from.SendMessage("You pull up the weed."); 
						this.Delete(); 
					}
					else from.SendMessage("The weed is still too tough to pull."); 
				} 
				else 
				{ 
					from.SendMessage( "You are too far away to harvest anything." ); 
				} 
			} 

			public Weeds( Serial serial ) : base( serial ) 
			{ 
			} 

			public override void Serialize( GenericWriter writer ) 
			{ 
				base.Serialize( writer ); 
				writer.Write( (int) 0 ); 
				writer.Write( m_sower );
				writer.Write((string) planted.ToString());
			} 

			public override void Deserialize( GenericReader reader ) 
			{ 
				base.Deserialize( reader ); 
				int version = reader.ReadInt(); 
				m_sower = reader.ReadMobile();
				planted = DateTime.Parse(reader.ReadString());
			} 
		} 
	}
}


