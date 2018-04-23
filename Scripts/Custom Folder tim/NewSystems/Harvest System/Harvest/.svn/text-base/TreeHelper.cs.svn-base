using System;
using Server;
using System.Collections;
using Server.Network;
using Server.Gumps;
using Server.Items; 

namespace Server.Items.Crops 
{ 
	public enum TreeType
	{
		AppleTree,
		PearTree,
		PeachTree
	}

	public class TreeHelper
	{ 
		public static bool CanPickMounted{ get{ return true; } } // If true Player can pick fruit while mounted
		public static bool TreeOrdinance{ get{ return false; } } // Criminal to Chop fruit trees in town.
		
		public static TimeSpan SaplingTime = TimeSpan.FromMinutes( 5 ); // Time spent as a Sapling 
		public static TimeSpan StumpTime = TimeSpan.FromMinutes( 30 ); // Time spent as a Stump --changed from-- TimeSpan.FromHours (1)

		public class ChopAction : Timer
		{
			private Mobile m_chopper;
			private int cnt;

			public ChopAction( Mobile from ) : base( TimeSpan.FromMilliseconds( 900 ), TimeSpan.FromMilliseconds( 900 ) )
			{
				Priority = TimerPriority.TenMS;
				m_chopper = from;
				from.CantWalk = true;
				cnt = 1;
			}
			
			protected override void OnTick()
			{
				switch( cnt++ )
				{
					case 1: case 3: case 5:
					{
						m_chopper.Animate( 13, 7, 1, true, false, 0 ); // Chop
						break;
					}
					case 2: case 4:
					{
						Effects.PlaySound( m_chopper.Location, m_chopper.Map, 0x13E );
						break;
					}
					case 6:
					{
						Effects.PlaySound( m_chopper.Location, m_chopper.Map, 0x13E );
						m_chopper.CantWalk = false;
						this.Stop();
						break;
					}
				}
			}
		}

		public class TreeTimer : Timer 
		{ 
			private Item i_sapling; 
			private Type t_crop; 

			public TreeTimer( Item sapling, Type croptype, TimeSpan delay ) : base( delay ) 
			{ 
				Priority = TimerPriority.OneMinute; 

				i_sapling = sapling; 
				t_crop = croptype;
			} 

			protected override void OnTick() 
			{ 
				if (( i_sapling != null ) && ( !i_sapling.Deleted )) 
				{ 
					object[] args = { i_sapling.Location, i_sapling.Map };
					Item newitem = Activator.CreateInstance( t_crop, args ) as Item;

					i_sapling.Delete(); 
				} 
			} 
		} 

		public class GrowTimer : Timer 
		{ 
			private Item i_stump; 
			private Type t_tree; 
			private DateTime d_timerstart;
			private Item i_newtree;

			public GrowTimer( Item stump, Type treetype, TimeSpan delay ) : base( delay ) 
			{ 
				Priority = TimerPriority.OneMinute; 

				i_stump = stump; 
				t_tree = treetype;

				d_timerstart = DateTime.Now;
			} 

			protected override void OnTick() 
			{ 
				Point3D pnt = i_stump.Location;
				Map map = i_stump.Map;

				if ( t_tree == typeof(PeachTree) )
					i_newtree = new PeachSapling();
				else if ( t_tree == typeof(PearTree) )
					i_newtree = new PearSapling();
				else 
					i_newtree = new AppleSapling();

				i_stump.Delete();
				i_newtree.MoveToWorld( pnt, map );
			} 
		} 
	}

	public class BaseTree : Item, IChopable
	{
		public BaseTree( int itemID ) : base( itemID )
		{
		}

		public BaseTree( Serial serial ) : base( serial ) 
		{ 
		} 

		public virtual void OnChop( Mobile from )
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
	
	public class TreeTrunk : Item, IChopable
	{
		private Item i_leaves;

		public Item Leaves{ get{ return i_leaves; } }

		public TreeTrunk( int itemID, Item TreeLeaves ) : base( itemID )
		{
			Movable = false;
			i_leaves = TreeLeaves;
		}

		public TreeTrunk( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void OnAfterDelete()
		{
			if (( i_leaves != null ) && ( !i_leaves.Deleted ))
				i_leaves.Delete();

			base.OnAfterDelete();
		}

		public void OnChop( Mobile from )
		{
			int testID = ((Item)i_leaves).ItemID;

			switch (testID)
			{
				case 0xD96: 
				case 0xD9A: 
				{ 
					AppleTree thistree = i_leaves as AppleTree;
					if ( thistree != null )
						thistree.Chop( from );
					break;
				}
				case 0xDAA: 
				case 0xDA6: 
				{ 
					PearTree thistree = i_leaves as PearTree;
					if ( thistree != null )
						thistree.Chop( from );
					break;
				}
				case 0xD9E: 
				case 0xDA2: 
				{ 
					PeachTree thistree = i_leaves as PeachTree;
					if ( thistree != null )
						thistree.Chop( from );
					break;
				}
			}
		}
			
		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 

			writer.Write( (Item)i_leaves ); 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 

			Item item = reader.ReadItem();
			if ( item != null )
				i_leaves = item;
		} 
	}
	
	public class FruitTreeStump : Item
	{
		private Type t_treeType;
		private int e_tree;
		public Timer thisTimer;
		public DateTime treeTime;

		[CommandProperty( AccessLevel.GameMaster )]
		public String Sapling{ get{ return treeTime.ToString( "T" ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public String Type
		{ get
			{
				switch( e_tree )
				{
					case (int)TreeType.AppleTree:		return "Apple Tree";	
					case (int)TreeType.PearTree:		return "Pear Tree";		
					case (int)TreeType.PeachTree:		return "Peach Tree";
					default:	return "Error Bad Treetype";
				}
			} 
		}

		[Constructable] 
		public FruitTreeStump( Type FruitTree ) : base( 0xDAC )
		{
			Movable = false;
			Hue = 0x74E;
			Name = "fruit tree stump";

			t_treeType = FruitTree;

			if ( FruitTree == typeof( PearTree ) )
				e_tree = (int)TreeType.PearTree;
			else if ( FruitTree == typeof( PeachTree ) )
				e_tree = (int)TreeType.PeachTree;
			else 
				e_tree = (int)TreeType.AppleTree;

			init( this );
		}

		public static void init( FruitTreeStump plant )
		{
			TimeSpan delay = TreeHelper.StumpTime;
			plant.treeTime = DateTime.Now + delay;

			plant.thisTimer = new TreeHelper.GrowTimer( plant, plant.t_treeType, delay ); 
			plant.thisTimer.Start(); 
		}

		public FruitTreeStump( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 

			writer.Write( e_tree ); 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 

			int e_tree = reader.ReadInt();
			switch( e_tree )
			{
				case (int)TreeType.AppleTree:		t_treeType = typeof(AppleTree);	break;
				case (int)TreeType.PearTree:		t_treeType = typeof(PearTree);	break;
				case (int)TreeType.PeachTree:		t_treeType = typeof(PeachTree);	break;
			}

			init( this );
		} 
	}
}


