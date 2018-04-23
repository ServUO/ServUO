using System; 
using System.IO; 
using System.Collections; 
using Server; 
using Server.Mobiles; 
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using System.Collections.Generic; 
namespace Server.Items 
{ 	
	[Furniture]
	[FlipableAttribute( 0xe3c, 0xe3d )]
	public class VoransTownCrate : LockableContainer, ILockpickable
	{
		private int m_Count; //how much items to spawn 
		private TimeSpan m_MinDelay;   //min delay to respawn 
		private TimeSpan m_MaxDelay;   //max delay to respawn 
		private List<string> m_ItemsName;   //list of item names 
		private ArrayList m_Items;      //list of items spawned 
		private DateTime m_End;         //time to next respawn 
		private InternalTimer m_Timer;   //internaltimer 
		private bool m_Running;         //active ? 
		private bool m_Unlocked; // does this lock?
		public override int DefaultGumpID{ get{ return 0x44; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public List<string> ItemsName 
		{ 
			get { return m_ItemsName; } 
			set 
			{
				m_ItemsName = value; 
				// If no itemname, no spawning 
				if ( m_ItemsName.Count < 1 ) 
					Stop(); 

				InvalidateProperties(); 
			} 
		} 
    
		[CommandProperty( AccessLevel.GameMaster )] 
		public int Count 
		{ 
			get { return m_Count; } 
			set { m_Count = value; InvalidateProperties(); } 
		}

		[CommandProperty( AccessLevel.GameMaster )] 
		public bool Unlocked 
		{ 
			get { return m_Unlocked; } 
			set { m_Unlocked = value; InvalidateProperties(); } 
		} 

		[CommandProperty( AccessLevel.GameMaster )] 
		public bool Running 
		{ 
			get { return m_Running; } 
			set 
			{ 
				if ( value ) 
					Start(); 
				else 
					Stop(); 

				InvalidateProperties(); 
			} 
		} 

       
		[CommandProperty( AccessLevel.GameMaster )] 
		public TimeSpan MinDelay 
		{ 
			get { return m_MinDelay; } 
			set { m_MinDelay = value; InvalidateProperties(); } 
		} 
      
		[CommandProperty( AccessLevel.GameMaster )] 
		public TimeSpan MaxDelay 
		{ 
			get { return m_MaxDelay; } 
			set { m_MaxDelay = value; InvalidateProperties(); } 
		}
       
		[CommandProperty( AccessLevel.GameMaster )] 
		public TimeSpan NextSpawn 
		{ 
			get 
			{ 
				if ( m_Running ) 
					return m_End - DateTime.Now; 
				else 
					return TimeSpan.FromSeconds( 0 ); 
			} 
			set 
			{ 
				Start(); 
				DoTimer( value ); 
			} 
		}
 
		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 10, 150, 90 ); }
		}

		[Constructable] 
		public VoransTownCrate( int amount, int minDelay, int maxDelay, string itemName ) : base( 0xE3C ) 
		{ 
			List<string> itemsName = new List<string>(); 
			itemsName.Add( itemName.ToLower() ); 
			Weight = 1.0;
			InitSpawn( amount, TimeSpan.FromMinutes( minDelay ), TimeSpan.FromMinutes( maxDelay ), itemsName ); 
		} 

       
		[Constructable] 
		public VoransTownCrate( string itemName ) : base( 0xE3C ) 
		{ 
			List<string> itemsName = new List<string>(); 
			itemsName.Add( itemName.ToLower() ); 
			Weight = 1.0;
			InitSpawn( 1, TimeSpan.FromMinutes( 20 ), TimeSpan.FromMinutes( 60 ), itemsName ); 
		} 

		[Constructable]
		public VoransTownCrate() : base( 0xE3C )
		{
			Weight = 1.0;
			List<string> itemsName = new List<string>(); 
			InitSpawn( 1, TimeSpan.FromMinutes( 20 ), TimeSpan.FromMinutes( 60 ), itemsName ); 
		}

		public VoransTownCrate( Serial serial ) : base( serial )
		{
		}

		public VoransTownCrate( int amount, TimeSpan minDelay, TimeSpan maxDelay, List<string> itemsName ) : base( 0xE3C ) 
		{ 
			InitSpawn( amount, minDelay, maxDelay, itemsName ); 
		} 

		public void InitSpawn( int amount, TimeSpan minDelay, TimeSpan maxDelay, List<string> itemsName ) 
		{        
			Movable = false; 
			m_Running = true; 
			m_Unlocked=false;
			m_MinDelay = minDelay; 
			m_MaxDelay = maxDelay; 
			m_Count = amount; 
			m_ItemsName = itemsName;
			Locked=true;
			RequiredSkill=(Utility.Random(20,50));
			LockLevel=(Utility.Random(20,50));
			m_Items = new ArrayList(); //create new list of creatures 
			DoTimer( TimeSpan.FromSeconds( 1 ) ); //spawn in 1 sec 

			switch( Utility.Random(5) )
			{
				case 0:
				TrapType = TrapType.ExplosionTrap;
				break;

				case 1:
				TrapType = TrapType.DartTrap;
				break;

				case 2:
				TrapType = TrapType.ExplosionTrap;
				break;

				case 3:
				TrapType = TrapType.PoisonTrap;
				break;

				case 4:
				TrapType = TrapType.None;
				break;                   
			}	
		}

		public void Start() 
		{ 
			if ( !m_Running ) 
			{ 
				if ( m_ItemsName.Count > 0 ) 
				{ 
					m_Running = true; 
					DoTimer(); 
				} 
			} 
		} 
       
		public void Stop() 
		{ 
			if ( m_Running ) 
			{ 
				m_Timer.Stop(); 
				m_Running = false; 
			} 
		} 
            public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		//public override void GetContextMenuEntries( Mobile from, ArrayList list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.AccessLevel>=AccessLevel.Seer )
			{
				list.Add( new ChestSpawnEntry( this, from ) ); 
			}
		}
			    
		public void Defrag() 
		{ 
			bool removed = false; 

			for ( int i = 0; i < m_Items.Count; ++i ) 
			{ 
				object o = m_Items[i]; 

				if ( o is Item ) 
				{               
					Item item = (Item)o; 
                
					//if not in the original container or deleted -> delete from list 
					if(item.Deleted) 
					{ 
						m_Items.RemoveAt( i ); 
						--i; 
						removed = true; 
					} 
					else 
					{ 
						if (item.Parent !=this) 
						{                    
							m_Items.RemoveAt( i ); 
							--i; 
							removed = true; 
						}                                      
					}
				}
				else 
				{ 
					//should not be something else 
					m_Items.RemoveAt( i ); 
					--i; 
					removed = true; 
				} 
			} 

			if ( removed ) 
				InvalidateProperties(); 
		} 
    
		public void OnTick() 
		{ 
			DoTimer(); 
			Spawn(); 
			if(!Unlocked)
			{
				Locked=true;
				RequiredSkill=(Utility.Random(20,50));
				LockLevel=(Utility.Random(20,50));

				switch( Utility.Random(5) )
				{
					case 0:
					TrapType = TrapType.ExplosionTrap;
					break;

					case 1:
					TrapType = TrapType.DartTrap;
					break;

					case 2:
					TrapType = TrapType.ExplosionTrap;
					break;

					case 3:
					TrapType = TrapType.PoisonTrap;
					break;

					case 4:
					TrapType = TrapType.None;
					break;                  
				}
			}
		} 
       
		public void Respawn() 
		{ 
			RemoveItems(); 
			for ( int i = 0; i < m_Count; i++ ) 
				Spawn();

			if(!Unlocked)
			{
				Locked=true;
				RequiredSkill=(Utility.Random(20,50));
				LockLevel=(Utility.Random(20,50));

				switch( Utility.Random(5) )
				{
					case 0:
					TrapType = TrapType.ExplosionTrap;
					break;

					case 1:
					TrapType = TrapType.DartTrap;
					break;

					case 2:
					TrapType = TrapType.ExplosionTrap;
					break;

					case 3:
					TrapType = TrapType.PoisonTrap;
					break;

					case 4:
					TrapType = TrapType.None;
					break;
				}
			}
		} 
       
		public void Spawn() 
		{ 
			//if there are item to spawn in list 
			if ( m_ItemsName.Count > 0 ) 
				Spawn( Utility.Random( m_ItemsName.Count ) ); //spawn on of them index 
		} 

		public void Spawn( string itemName ) 
		{ 
			for ( int i = 0; i < m_ItemsName.Count; i++ ) 
			{ 
				if ( (string)m_ItemsName[i] == itemName ) 
				{ 
					Spawn( i ); 
					break; 
				} 
			} 
		} 
              
		public void Spawn( int index ) 
		{ 
			if ( m_ItemsName.Count == 0 || index >= m_ItemsName.Count ) 
				return; 

			Defrag(); 

			//limit already at 
			if ( m_Items.Count >= m_Count ) 
				return; 

			Type type = SpawnerType.GetType( (string)m_ItemsName[index] ); 

			if ( type != null ) 
			{ 
				try 
				{ 
					object o = Activator.CreateInstance( type ); 

					if ( o is Item ) 
					{               
						Item item = (Item)o; 
                   
						//add it to the list 
						m_Items.Add( item ); 
						InvalidateProperties(); 
						//spawn it in the container 
						item.Movable=false;
						this.DropItem(item);                  
					} 
				} 
				catch 
				{ 
				} 
			}
		}
       
		public void DoTimer() 
		{ 
			if ( !m_Running ) 
				return; 

			int minSeconds = (int)m_MinDelay.TotalSeconds; 
			int maxSeconds = (int)m_MaxDelay.TotalSeconds; 

			TimeSpan delay = TimeSpan.FromSeconds( Utility.RandomMinMax( minSeconds, maxSeconds ) ); 
			DoTimer( delay ); 
		} 
   
		public void DoTimer( TimeSpan delay ) 
		{ 
			if ( !m_Running ) 
				return; 

			m_End = DateTime.Now + delay; 

			if ( m_Timer != null ) 
				m_Timer.Stop(); 

			m_Timer = new InternalTimer( this, delay ); 
			m_Timer.Start(); 
		} 
    
		private class InternalTimer : Timer 
		{ 
			private VoransTownCrate m_Spawner; 

			public InternalTimer( VoransTownCrate spawner, TimeSpan delay ) : base( delay ) 
			{ 
				Priority = TimerPriority.OneSecond; 
				m_Spawner = spawner; 
			} 

			protected override void OnTick() 
			{ 
				if ( m_Spawner != null ) 
					if ( !m_Spawner.Deleted ) 
						m_Spawner.OnTick(); 
			} 
		} 
       
		public int CountItems( string itemName ) 
		{ 
			Defrag(); 

			int count = 0; 

			for ( int i = 0; i < m_Items.Count; ++i ) 
			if ( Insensitive.Equals( itemName, m_Items[i].GetType().Name ) ) 
				++count; 

			return count; 
		} 
     
		public void RemoveItems( string itemName ) 
		{ 
			Console.WriteLine( "defrag from removeitems" ); 
			Defrag(); 

			itemName = itemName.ToLower(); 

			for ( int i = 0; i < m_Items.Count; ++i ) 
			{ 
				object o = m_Items[i]; 

				if ( Insensitive.Equals( itemName, o.GetType().Name ) ) 
				{ 
					if ( o is Item ) 
						((Item)o).Delete();               
				} 
			} 

			InvalidateProperties(); 
		} 
              
		public void RemoveItems() 
		{ 
			Defrag(); 

			for ( int i = 0; i < m_Items.Count; ++i ) 
			{ 
				object o = m_Items[i]; 

				if ( o is Item ) 
					((Item)o).Delete(); 
             
			} 

			InvalidateProperties(); 
		} 
              
		public override void OnDelete() 
		{ 
			base.OnDelete(); 

			RemoveItems(); 
			if ( m_Timer != null ) 
				m_Timer.Stop(); 
		}
      
		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 1 ); // version 
			writer.Write(m_Unlocked);
			writer.Write( m_MinDelay ); 
			writer.Write( m_MaxDelay ); 
			writer.Write( m_Count ); 
			writer.Write( m_Running ); 
          
			if ( m_Running ) 
				writer.Write( m_End - DateTime.Now ); 

			writer.Write( m_ItemsName.Count ); 

			for ( int i = 0; i < m_ItemsName.Count; ++i ) 
				writer.Write( (string)m_ItemsName[i] ); 

			writer.Write( m_Items.Count ); 

			for ( int i = 0; i < m_Items.Count; ++i ) 
			{ 
				object o = m_Items[i]; 

				if ( o is Item ) 
					writer.Write( (Item)o ); 
				else 
					writer.Write( Serial.MinusOne ); 
			} 
		} 

		private static WarnTimer m_WarnTimer; 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 
			switch (version)
			{
				case 1:
				{
					m_Unlocked=reader.ReadBool();
					goto case 0;
				}
				case 0:
				{
					m_MinDelay = reader.ReadTimeSpan(); 
					m_MaxDelay = reader.ReadTimeSpan(); 
					m_Count = reader.ReadInt(); 
					m_Running = reader.ReadBool(); 
					break;
				}
			}
			if ( m_Running ) 
			{ 
				TimeSpan delay = reader.ReadTimeSpan(); 
				DoTimer( delay ); 
			} 
			if(m_Unlocked)
			{
				Locked=false;
				TrapType=TrapType.None;
			}
			int size = reader.ReadInt(); 
    
			m_ItemsName = new List<string>( size ); 

			for ( int i = 0; i < size; ++i ) 
			{ 
				string typeName = reader.ReadString(); 

				m_ItemsName.Add( typeName ); 

				if ( ItemSpawnerType.GetType( typeName ) == null ) 
				{ 
					if ( m_WarnTimer == null ) 
						m_WarnTimer = new WarnTimer(); 

					m_WarnTimer.Add( Location, Map, typeName ); 
				} 
			} 

			int count = reader.ReadInt(); 

			m_Items = new ArrayList( count ); 

			for ( int i = 0; i < count; ++i ) 
			{ 
				IEntity e = World.FindEntity( reader.ReadInt() ); 

				if ( e != null ) 
					m_Items.Add( e ); 
			} 
		} 

		private class WarnTimer : Timer 
		{ 
			private ArrayList m_List; 

			private class WarnEntry 
			{ 
				public Point3D m_Point; 
				public Map m_Map; 
				public string m_Name; 

				public WarnEntry( Point3D p, Map map, string name ) 
				{ 
					m_Point = p; 
					m_Map = map; 
					m_Name = name; 
				} 
			} 

			public WarnTimer() : base( TimeSpan.FromSeconds( 1.0 ) ) 
			{ 
				m_List = new ArrayList(); 
				Start(); 
			} 

			public void Add( Point3D p, Map map, string name ) 
			{ 
				m_List.Add( new WarnEntry( p, map, name ) ); 
			} 

			protected override void OnTick() 
			{ 
				try 
				{ 
					Console.WriteLine( "Warning: {0} bad spawns detected, logged: 'badspawn.log'", m_List.Count ); 

					using ( StreamWriter op = new StreamWriter( "badspawn.log", true ) ) 
					{ 
						op.WriteLine( "# Bad spawns : {0}", DateTime.Now ); 
						op.WriteLine( "# Format: X Y Z F Name" ); 
						op.WriteLine(); 

						foreach ( WarnEntry e in m_List ) 
						op.WriteLine( "{0}\t{1}\t{2}\t{3}\t{4}", e.m_Point.X, e.m_Point.Y, e.m_Point.Z, e.m_Map, e.m_Name ); 

						op.WriteLine(); 
						op.WriteLine(); 
					} 
				} 
				catch 
				{ 
				} 
			} 
		} 
	} 
} 
