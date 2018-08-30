using System;
using Server;

namespace Server.Items
{
	public class HagCouldron : Item
	{
		private InternalItem m_Item;
		private Point3D m_Destination;
		private Point3D m_Location;
		private Moongate m_Moongate;
		private Timer m_Timer;

		public Moongate Gate
		{
			get{ return m_Moongate; } set{ m_Moongate = value; }
		}

		public Timer Timer
		{
			get{ return m_Timer; } set{ m_Timer = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Destination
		{
			get{ return m_Destination; } set{ m_Destination = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Loc
		{
			get{ return m_Location; } set{ m_Location = value; }
		}

		[Constructable]
		public HagCouldron() : base( 0x974 )
		{
			Movable = false;

			m_Item = new InternalItem( this );
		}

		public HagCouldron( Serial serial ) : base( serial )
		{
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get{ return base.Hue; }
			set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (m_Timer != null || m_Moongate != null)
				return;
			m_Moongate = new Moongate();
			m_Moongate.Hue = 1175;
			m_Moongate.MoveToWorld( m_Location, Map );
			m_Moongate.Target = m_Destination;
			m_Moongate.TargetMap = Map;
			m_Timer = new InternalTimer( this, m_Moongate );
			m_Timer.Start();
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			if ( m_Item != null )
				m_Item.Location = new Point3D( X, Y, Z + 8 );
		}

		public override void OnMapChange()
		{
			if ( m_Item != null )
				m_Item.Map = Map;
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Item != null )
				m_Item.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Item );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Item = reader.ReadItem() as InternalItem;
		}

		private class InternalItem : Item
		{
			private HagCouldron m_Item;

			public InternalItem( HagCouldron item ) : base( 0x970 )
			{
				Movable = false;

				m_Item = item;
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void OnLocationChange( Point3D oldLocation )
			{
				if ( m_Item != null )
					m_Item.Location = new Point3D( X, Y, Z - 8 );
			}

			public override void OnMapChange()
			{
				if ( m_Item != null )
					m_Item.Map = Map;
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Item != null )
					m_Item.Delete();
			}

			[Hue, CommandProperty( AccessLevel.GameMaster )]
			public override int Hue
			{
				get{ return base.Hue; }
				set{ base.Hue = value; if ( m_Item.Hue != value ) m_Item.Hue = value; }
			}

			public override void OnDoubleClick( Mobile from )
			{
				m_Item.OnDoubleClick( from );
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 ); // version

				writer.Write( m_Item );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				m_Item = reader.ReadItem() as HagCouldron;
			}
		}

		private class InternalTimer : Timer
		{
			private Moongate m_Moongate;
			private HagCouldron m_HagCouldron;

			public InternalTimer( HagCouldron couldron, Moongate gate ) : base( TimeSpan.FromSeconds( 3.0 ) )
			{
				m_HagCouldron = couldron;
				m_Moongate = gate;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Moongate.Delete();
				m_HagCouldron.Gate = null;
				m_HagCouldron.Timer = null;
				Stop();
			}
		}
	}
}