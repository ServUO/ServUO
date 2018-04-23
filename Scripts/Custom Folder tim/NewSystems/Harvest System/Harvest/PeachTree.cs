using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;

namespace Server.Items.Crops
{
	public class PeachSapling : BaseCrop 
	{ 
		public Timer thisTimer;
		public DateTime treeTime;

		[CommandProperty( AccessLevel.GameMaster )]
		public String FullGrown{ get{ return treeTime.ToString( "T" ); } }

		[Constructable] 
		public PeachSapling() : base( Utility.RandomList ( 0xCE9, 0xCEA ) ) 
		{ 
			Movable = false; 
			Name = "peach tree sapling"; 
			
			init( this );
		} 

		public static void init( PeachSapling plant )
		{
			TimeSpan delay = TreeHelper.SaplingTime;
			plant.treeTime = DateTime.Now + delay;

			plant.thisTimer = new TreeHelper.TreeTimer( plant, typeof(PeachTree), delay ); 
			plant.thisTimer.Start(); 
		}

		public PeachSapling( Serial serial ) : base( serial ) 
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

			init( this );
		} 
	} 

	public class PeachTree : BaseTree
	{
		public Item i_trunk;
		private Timer chopTimer;

		private const int max = 12;
		private DateTime lastpicked;
		private int m_yield;

		public Timer regrowTimer;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Yield{ get{ return m_yield; } set{ m_yield = value; } }

		public int Capacity{ get{ return max; } }
		public DateTime LastPick{ get{ return lastpicked; } set{ lastpicked = value; } }
		
		[Constructable] 
		public PeachTree( Point3D pnt, Map map ) : base( Utility.RandomList( 0xD9E, 0xDA2 ) ) // leaves
		{
			Movable = false;
			MoveToWorld( pnt, map );

			int trunkID = ((Item)this).ItemID - 2;

			i_trunk = new TreeTrunk( trunkID, this );
			i_trunk.MoveToWorld( pnt, map );

			init( this, false );
		}
		
		public static void init ( PeachTree plant, bool full )
		{
			plant.LastPick = DateTime.Now;
			plant.regrowTimer = new FruitTimer( plant );

			if ( full )
			{
				plant.Yield = plant.Capacity;
			}
			else
			{
				plant.Yield = 0;
				plant.regrowTimer.Start();
			}
		}
		
		public override void OnAfterDelete()
		{
			if (( i_trunk != null ) && ( !i_trunk.Deleted ))
					i_trunk.Delete();

			base.OnAfterDelete();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.Mounted && !TreeHelper.CanPickMounted )
			{
				from.SendMessage( "You cannot pick fruit while mounted." ); 
				return; 
			}

			if ( DateTime.Now > lastpicked.AddSeconds(3) ) // 3 seconds between picking
			{
				lastpicked = DateTime.Now;
			
				int lumberValue = (int)from.Skills[SkillName.Lumberjacking].Value / 20;
				if ( from.Mounted ) 
					++lumberValue;

				if ( lumberValue < 0 )
				{
					from.SendMessage( "You have no idea how to pick this fruit." ); 
					return;
				}

				if ( from.InRange( this.GetWorldLocation(), 2 ) ) 
				{ 
					if ( m_yield < 1 )
					{
						from.SendMessage( "There is nothing here to harvest." ); 
					}
					else //check skill
					{
						from.Direction = from.GetDirectionTo( this );

						from.Animate( from.Mounted ? 26:17, 7, 1, true, false, 0 ); 

						if ( lumberValue < m_yield ) 
							lumberValue = m_yield + 1;

						int pick = Utility.Random( lumberValue );
						if ( pick == 0 )
						{
							from.SendMessage( "You do not manage to gather any fruit." ); 
							return;
						}
					
						m_yield -= pick;
						from.SendMessage( "You pick {0} peach{1}!", pick, ( pick == 1 ? "" : "es" ) ); 

						//PublicOverheadMessage( MessageType.Regular, 0x3BD, false, string.Format( "{0}", m_yield )); 

						Peach crop = new Peach( pick ); 
						from.AddToBackpack( crop );

						if ( !regrowTimer.Running )
						{
							regrowTimer.Start();
						}
					}
				} 
				else 
				{ 
					from.SendLocalizedMessage( 500446 ); // That is too far away. 
				} 
			}
		}

		private class FruitTimer : Timer
		{
			private PeachTree i_plant;

			public FruitTimer( PeachTree plant ) : base( TimeSpan.FromSeconds( 900 ), TimeSpan.FromSeconds( 30 ) )
			{
				Priority = TimerPriority.OneSecond;
				i_plant = plant;
			}

			protected override void OnTick()
			{
				if ( ( i_plant != null ) && ( !i_plant.Deleted ) )
				{
					int current = i_plant.Yield;

					if ( ++current >= i_plant.Capacity )
					{
						current = i_plant.Capacity;
						Stop();
					}
					else if ( current <= 0 )
						current = 1;

					i_plant.Yield = current;

					//i_plant.PublicOverheadMessage( MessageType.Regular, 0x22, false, string.Format( "{0}", current )); 
				}
				else Stop();
			}
		}
		
		public void Chop( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) ) 
			{ 
				if ( ( chopTimer == null ) || ( !chopTimer.Running ) )
				{
					if ( ( TreeHelper.TreeOrdinance ) && ( from.AccessLevel == AccessLevel.Player ) )
					{
						if ( from.Region is Regions.GuardedRegion )
							from.CriminalAction( true );
					}
					
					chopTimer = new TreeHelper.ChopAction( from );
					
					Point3D pnt = this.Location;
					Map map =  this.Map;

					from.Direction = from.GetDirectionTo( this );
					chopTimer.Start();

					double lumberValue = from.Skills[SkillName.Lumberjacking].Value / 100;
					if ( ( lumberValue > .5 ) && ( Utility.RandomDouble() <= lumberValue ) )
					{
						Peach fruit = new Peach( (int)Utility.Random( 13 ) + m_yield );
						from.AddToBackpack( fruit );

						int cnt = Utility.Random( (int)( lumberValue * 10 ) + 1 );
						Log logs = new Log( cnt ); // Fruitwood Logs ??
						from.AddToBackpack( logs ); 

						FruitTreeStump i_stump = new FruitTreeStump( typeof( PeachTree ) );
						Timer poof = new StumpTimer( this, i_stump, from );
						poof.Start();
					}
					else from.SendLocalizedMessage( 500495 ); // You hack at the tree for a while, but fail to produce any useable wood.
				}
			}
			else from.SendLocalizedMessage( 500446 );  // That is too far away.
		}

		private class StumpTimer : Timer
		{
			private PeachTree i_tree;
			private FruitTreeStump i_stump;
			private Mobile m_chopper;

			public StumpTimer( PeachTree Tree, FruitTreeStump Stump, Mobile from ) : base( TimeSpan.FromMilliseconds( 5500 ) )
			{
				Priority = TimerPriority.TenMS;

				i_tree = Tree;
				i_stump = Stump;
				m_chopper = from;
			}

			protected override void OnTick()
			{
				i_stump.MoveToWorld( i_tree.Location, i_tree.Map );
				i_tree.Delete();
				m_chopper.SendMessage( "You put some logs and fruit into your backpack" );
			}
		}

		public override void OnChop( Mobile from )
		{
			if ( !this.Deleted )
					Chop( from );
		}

		public PeachTree( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); 

			writer.Write( (Item)i_trunk ); 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			Item item = reader.ReadItem();
			if ( item != null )
				i_trunk = item;

			init( this, true );
		}
	}
}