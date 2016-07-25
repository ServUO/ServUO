using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Items
{
	public class GoblinTrapGenerator : Item
	{
		[CommandProperty( AccessLevel.GameMaster )]
		public int Num { get { return m_ActiveTraps.Count; } }

		public class GoblinTrapEntry
		{
			private Map m_Map;
			private Point3D m_Location;

			public Map Map { get { return m_Map; } }
			public Point3D Location { get { return m_Location; } }

			public GoblinTrapEntry( Map map, Point3D location )
			{
				m_Map = map;
				m_Location = location;
			}

			public Item CreateInstance()
			{
				GoblinTrap item = (GoblinTrap) Activator.CreateInstance( typeof( GoblinTrap ) );

				item.MoveToWorld( this.Location, this.Map );

				return item;
			}
		}

		private static GoblinTrapEntry[] m_GoblinTrapEntries = new GoblinTrapEntry[]
			{
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1035, 1086, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1038, 1085, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1041, 1087, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1053, 1082, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1055, 1082, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1058, 1081, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1111, 1065, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1112, 1065, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1112, 1066, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1113, 1066, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1113, 1067, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1114, 1067, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1114, 1068, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1115, 1068, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1115, 1069, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1116, 1069, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1116, 1070, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1117, 1070, -42 ) ),
                new GoblinTrapEntry( Map.TerMur, new Point3D( 1117, 1071, -42 ) )
			};

		private static Dictionary<Point3D, Item> m_ActiveTraps = new Dictionary<Point3D, Item>();

		public static Dictionary<Point3D, Item> ActiveTraps { get { return m_ActiveTraps; } }

		public static GoblinTrapEntry[] GoblinTrapEntries { get { return m_GoblinTrapEntries; } }

		private static GoblinTrapGenerator m_GoblinTrapsInstance;

		public GoblinTrapGenerator()
			: base( 1 )
		{
			Name = "Goblin Trap Generator";

			m_ActiveTraps.Clear();

			foreach ( GoblinTrapEntry e in GoblinTrapEntries )
			{
				Item trap = e.CreateInstance();
				m_ActiveTraps.Add( trap.Location, trap );
			}
		}

		public GoblinTrapGenerator( Serial serial )
			: base( serial )
		{
			m_GoblinTrapsInstance = this;
		}

		public static void Initialize()
		{
			CommandSystem.Register( "GoblinTrapsGenerate", AccessLevel.GameMaster, new CommandEventHandler( GoblinTrapsGenerate_OnCommand ) );
			CommandSystem.Register( "GoblinTrapsDelete", AccessLevel.GameMaster, new CommandEventHandler( GoblinTrapsDelete_OnCommand ) );
		}

		[Usage( "GoblinTrapsGenerate" )]
		[Description( "Generates the Goblin Traps." )]
		private static void GoblinTrapsGenerate_OnCommand( CommandEventArgs args )
		{
			Mobile from = args.Mobile;

			if ( GenerateTraps() )
				from.SendMessage( "Goblin Traps generated." );
			else
				from.SendMessage( "Goblin Traps already present." );
		}

		[Usage( "GoblinTrapsDelete" )]
		[Description( "Removes the Goblin Traps." )]
		private static void GoblinTrapsDelete_OnCommand( CommandEventArgs args )
		{
			Mobile from = args.Mobile;

			if ( RemoveTraps() )
				from.SendMessage( "Goblin Traps removed." );
			else
				from.SendMessage( "Goblin Traps not present." );
		}


		public static bool GenerateTraps()
		{
			if ( m_GoblinTrapsInstance != null && !m_GoblinTrapsInstance.Deleted )
				return false;

			m_GoblinTrapsInstance = new GoblinTrapGenerator();
			return true;
		}

		private static bool RemoveTraps()
		{
			if ( m_GoblinTrapsInstance == null )
				return false;

			m_GoblinTrapsInstance.Delete();
			m_ActiveTraps.Clear();
			m_GoblinTrapsInstance = null;
			return true;
		}

		public override void OnDelete()
		{
			base.OnDelete();

			foreach ( Item i in m_ActiveTraps.Values )
			{
				if ( m_ActiveTraps.ContainsKey( i.Location ) )
					m_ActiveTraps[i.Location].Delete();
			}

			m_GoblinTrapsInstance = null;
		}

		public static void DisableTrap( Mobile from, Item targ )
		{
			ActiveTraps.Remove( targ.Location );
			GoblinTrapEntry gte = GetGobliTrapEntry( targ.Location, targ.Map );
			targ.Delete();

			// If the Goblin Trap Entry is not null, generate a new trap
			if ( gte != null )
			{
				Timer.DelayCall( TimeSpan.FromSeconds( 30.0 ), new TimerStateCallback( CreateNewGoblinTrap ), gte );
			}

			// Si está haciendo la quest, le tiene que dar el component
			if ( HasTinkerQuest( from ) )
			{
				from.AddToBackpack( new FloorTrapComponents() );
				from.SendLocalizedMessage( 1094989 ); // You collect the components of this trap and put them in your backpack.
			}
		}

		public static bool HasTinkerQuest( Mobile from )
		{
			if ( from != null && from is PlayerMobile )
			{
				BaseQuest quest = QuestHelper.GetQuest((PlayerMobile)from, typeof(DoneInTheNameOfTinkering));

				if ( quest != null )
					return !quest.Completed;
			}

			return false;
		}

		public static void CreateNewGoblinTrap( object state )
		{
			GoblinTrapEntry gte = (GoblinTrapEntry) state;
			Item trap = gte.CreateInstance();
			m_ActiveTraps.Add( trap.Location, trap );
		}

		private static GoblinTrapEntry GetGobliTrapEntry( Point3D point3D, Map map )
		{
			GoblinTrapEntry result = null;
			foreach ( GoblinTrapEntry gte in GoblinTrapEntries )
				if ( gte.Location == point3D && gte.Map == map )
					result = gte;

			return result;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 1 ); // version

			writer.WriteEncodedInt( m_ActiveTraps.Count );

			foreach ( Item i in m_ActiveTraps.Values )
			{
				writer.Write( i );
				writer.Write( (Point3D) i.Location );
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_ActiveTraps.Clear();

			int length = reader.ReadEncodedInt();

			for ( int i = 0; i < length; i++ )
			{
				Item it = reader.ReadItem();
				Point3D loc;

				if ( version > 0 )
					loc = reader.ReadPoint3D();
				else
					loc = it.Location;

				if ( m_ActiveTraps.ContainsKey( loc ) )
					it.Delete();
				else
					m_ActiveTraps.Add( loc, it );
			}
		}
	}
}
