using System;
using System.Collections;
using Server;
using Server.Network;

namespace Knives.TownHouses
{
	public class Errors
	{
		private static ArrayList s_ErrorLog = new ArrayList();
		private static ArrayList s_Checked = new ArrayList();

		public static ArrayList ErrorLog{ get{ return s_ErrorLog; } }
		public static ArrayList Checked{ get{ return s_Checked; } }

		public static void Initialize()
		{
            RUOVersion.AddCommand("TownHouseErrors", AccessLevel.Counselor, new TownHouseCommandHandler(OnErrors));
            RUOVersion.AddCommand("the", AccessLevel.Counselor, new TownHouseCommandHandler(OnErrors));

            EventSink.Login += new LoginEventHandler( OnLogin );
		}

		private static void OnErrors( CommandInfo e )
		{
			if ( e.ArgString == null || e.ArgString == "" )
				new ErrorsGump( e.Mobile );
			else
				Report( e.ArgString + " - " + e.Mobile.Name );
		}

		private static void OnLogin( LoginEventArgs e )
		{
			if ( e.Mobile.AccessLevel != AccessLevel.Player
			&& s_ErrorLog.Count != 0
			&& !s_Checked.Contains( e.Mobile ) )
				new ErrorsNotifyGump( e.Mobile );
		}

		public static void Report( string error )
		{
			s_ErrorLog.Add( String.Format( "<B>{0}</B><BR>{1}<BR>", DateTime.Now, error ) );

			s_Checked.Clear();

			Notify();
		}

		private static void Notify()
		{
			foreach( NetState state in NetState.Instances )
			{
				if ( state.Mobile == null )
					continue;

				if ( state.Mobile.AccessLevel != AccessLevel.Player )
					Notify( state.Mobile );
			}
		}

		public static void Notify( Mobile m )
		{
			if ( m.HasGump( typeof( ErrorsGump ) ) )
				new ErrorsGump( m );
			else
				new ErrorsNotifyGump( m );
		}
	}
}