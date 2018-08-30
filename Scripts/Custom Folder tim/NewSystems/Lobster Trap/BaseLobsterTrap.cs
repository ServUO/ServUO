using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items.Crops
{
	public class BaseLobsterTrap : Item, IChopable
	{
		public virtual bool CanSetTrap{ get{ return true; } }

		public virtual TimeSpan FisherPickTime{ get{ return TimeSpan.FromDays( 14 ); } }

		public virtual bool PlayerCanDestroy{ get{ return true; } }
		private bool i_bumpZ = false;

		public bool BumpZ { get{ return i_bumpZ; } set{ i_bumpZ = value; } }

		public BaseLobsterTrap( int itemID ) : base( itemID )
        {
        }

		public virtual void OnChop( Mobile from )
        {
        }

		public BaseLobsterTrap( Serial serial ) : base( serial )
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

    public class TrapHelper
	{
		public static bool CanWorkMounted{ get{ return false; } }

        private static int[] WaterTiles = new int[]
        {
            0x00A8, 0x00AB,
            0x0136, 0x0137,
            0x5797, 0x579C,
            0x746E, 0x7485,
            0x7490, 0x74AB,
            0x74B5, 0x75D5
        };

		public static bool CheckCanTrap( BaseLobsterTrap trap, Map map, int x, int y )
		{
            if (trap.CanSetTrap && ValidateWater(map, x, y)) return true;
			return false;
		}

        public static bool ValidateWater(Map map, int x, int y)
		{
			int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
			bool water = false;
            for (int i = 0; !water && i < WaterTiles.Length; i += 2)
                water = (tileID >= WaterTiles[i] && tileID <= WaterTiles[i + 1]);
			return water;
		}

        public class CatchTimer : Timer
        {
            private Item i_empty;
            private Type t_trap;
            private Mobile m_fisher;
            private int cnt;
            private double rnd;
            public CatchTimer(Item empty, Type traptype, Mobile fisher) : base(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10))
            {
                Priority = TimerPriority.OneSecond;
                i_empty = empty;
                t_trap = traptype;
                m_fisher = fisher;
                cnt = 0;
                rnd = Utility.RandomDouble();
            }

            protected override void OnTick()
            {
                if (cnt++ / 5 > rnd)
                {
                    if ((i_empty != null) && (!i_empty.Deleted))
                    {
                        object[] args = { m_fisher };

                        Item newitem = Activator.CreateInstance(t_trap, args) as Item;
                        if (newitem == null )
                        {
                            newitem = new Weeds(m_fisher);
                        }

                        newitem.Location = i_empty.Location;
                        newitem.Map = i_empty.Map;
                        i_empty.Delete();
                    }
                    Stop();
                }
            }
        }

        public static ArrayList CheckTrap(Point3D pnt, Map map, int range)
		{
			ArrayList ltraps = new ArrayList();
			IPooledEnumerable eable = map.GetItemsInRange( pnt, range );
			foreach ( Item ltrap in eable )
            {
                if ((ltrap != null) && (ltrap is BaseLobsterTrap)) ltraps.Add((BaseLobsterTrap)ltrap);
            }
			eable.Free();
            return ltraps;
		}

		public class Weeds : BaseLobsterTrap
		{
			private static DateTime planted;
			[CommandProperty( AccessLevel.GameMaster )]
			public DateTime t_planted{ get{ return planted; } set{ planted = value; } }
			private static Mobile m_sower;
			[CommandProperty( AccessLevel.GameMaster )]
			public Mobile Sower{ get{ return m_sower; } set{ m_sower = value; } }

			[Constructable]
			public Weeds( Mobile sower ) : base( Utility.RandomList( 0xCAD, 0xCAE, 0xCAF ) )
			{
				Movable = false;
				Name = "Weed";
				m_sower = sower;
                planted = DateTime.UtcNow;
			}
			public override void OnDoubleClick(Mobile from)
			{
                if (from.Mounted && !TrapHelper.CanWorkMounted)
				{
					from.SendMessage( "You cannot pull up a weed while mounted." );
					return;
				}
				if ( from.InRange( this.GetWorldLocation(), 1 ) )
				{
                    if ((from == m_sower) || (DateTime.UtcNow >= planted.AddDays(3)))
					{
						from.Direction = from.GetDirectionTo( this );
						from.Animate( from.Mounted ? 29:32, 5, 1, true, false, 0 );
						from.SendMessage("You pull up the weed.");
                        EmptyLobsterTrap trap = new EmptyLobsterTrap();
						from.AddToBackpack( trap );
						this.Delete();
					}
					else from.SendMessage("The weed is still too tough to pull.");
				}
				else
				{
					from.SendMessage( "You are too far away to harvest anything." );
				}
			}

			public Weeds( Serial serial ) : base( serial ) { }

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
				writer.Write( (int) 0 );
				writer.Write( m_sower );
				writer.Write( planted );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
				int version = reader.ReadInt();
				m_sower = reader.ReadMobile();
				planted = reader.ReadDateTime();
			}
		}
	}
}