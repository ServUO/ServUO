using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Mobiles;

namespace Server.Items.Crops
{
	public class CoconutPalm : Item
	{
		private const int max = 5;
		private DateTime lastpicked;
		private int m_yield;

		public Timer regrowTimer;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Yield{ get{ return m_yield; } set{ m_yield = value; } }

		public int Capacity{ get{ return max; } }
		public DateTime LastPick{ get{ return lastpicked; } set{ lastpicked = value; } }
		
		[Constructable] 
		public CoconutPalm() : base( 0xC95 ) 
		{
			Movable = false;
			init( this, false );
		}
		
		public static void init ( CoconutPalm plant, bool full )
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
						from.SendMessage( "You pick {0} coconut{1}!", pick, ( pick == 1 ? "" : "s" ) ); 

						//PublicOverheadMessage( MessageType.Regular, 0x3BD, false, string.Format( "{0}", m_yield )); 

						Coconut crop = new Coconut( pick ); 
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
			private CoconutPalm i_plant;

			public FruitTimer( CoconutPalm plant ) : base( TimeSpan.FromSeconds( 1200 ), TimeSpan.FromSeconds( 45 ) )
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
		
		public CoconutPalm( Serial serial ) : base( serial )
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

			init( this, true );
		}
	}
}