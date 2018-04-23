#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Reflection;
using System.IO;

using Server;
using Server.Items;
using Server.Accounting;
using Server.Mobiles;

namespace Arya.Abay
{
	/// <summary>
	/// The main Abay system process
	/// </summary>
	public class AbaySystem
	{

		#region Variables

		/// <summary>
		/// The Abay control stone
		/// </summary>
		private static AbayControl m_ControlStone;

		/// <summary>
		/// Text provider for the Abay system
		/// </summary>
		private static StringTable m_StringTable;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the String Table for the Abay system
		/// </summary>
		public static StringTable ST
		{
			get
			{
				if ( m_StringTable == null )
					m_StringTable = new StringTable();
				return m_StringTable;
			}
		}

		/// <summary>
		/// Gets or sets the Abay control stone
		/// </summary>
		public static AbayControl ControlStone
		{
			get { return m_ControlStone; }
			set { m_ControlStone = value; }
		}

		/// <summary>
		/// Gets the listing of the current Abays
		/// </summary>
		public static ArrayList Abays
		{
			get
			{
				if ( m_ControlStone != null )
				{
					return m_ControlStone.Abays;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the listing of pending Abays (ended but reserve not met)
		/// </summary>
		public static ArrayList Pending
		{
			get
			{
				if ( m_ControlStone != null )
				{
					return m_ControlStone.Pending;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Get the max number of Abays for a single account
		/// </summary>
		private static int MaxAbays
		{
			get
			{
				if ( m_ControlStone == null )
					return 0;
				else
					return m_ControlStone.MaxAbaysParAccount;
			}
		}

		/// <summary>
		/// Gets the min number of days an Abay can last
		/// </summary>
		public static int MinAbayDays
		{
			get { return m_ControlStone.MinAbayDays; }
		}

		/// <summary>
		/// Gets the max number of days an Abay can last
		/// </summary>
		public static int MaxAbayDays
		{
			get { return m_ControlStone.MaxAbayDays; }
		}

		/// <summary>
		/// States whether the Abay system is functional or not
		/// </summary>
		public static bool Running
		{
			get { return m_ControlStone != null; }
		}

		#endregion

		#region Abay Managment

		/// <summary>
		/// Adds an Abay into the system
		/// </summary>
		/// <param name="Abay">The new Abay entry</param>
		public static void Add( AbayItem abay )
		{
			// Put the item into the control stone
			abay.Item.Internalize();
			m_ControlStone.AddItem( abay.Item );
			abay.Item.Parent = m_ControlStone;
			abay.Item.Visible = true;

			Abays.Add( abay );

			m_ControlStone.InvalidateProperties();
		}

		/// <summary>
		/// Requests the start of a new Abay
		/// </summary>
		/// <param name="m">The mobile requesting the Abay</param>
		public static void AbayRequest( Mobile mobile )
		{
			if ( CanAbay( mobile ) )
			{
				mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 191 ] );
				mobile.CloseAllGumps();
				mobile.Target = new AbayTarget( new AbayTargetCallback( OnNewAbayTarget ), -1, false );
			}
			else
			{
				mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 192 ], AbaySystem.MaxAbays );
				mobile.SendGump( new AbayGump( mobile ) );
			}
		}

		private static void OnCreatureAbay( Mobile from, BaseCreature creature )
		{
			MobileStatuette ms = MobileStatuette.Create( from, creature );

			if ( ms == null )
			{
				from.Target = new AbayTarget( new AbayTargetCallback( OnNewAbayTarget ), -1, false );
			}

			/*
			 * Pets are Abayed within an item (MobileStatuette)
			 * 
			 * The item's name is the type of the pet, the hue corresponds to the pet
			 * and the item id is retrieved from the shrink table.
			 * 
			 */

			AbayItem abay = new AbayItem( ms, from );
			from.SendGump( new NewAbayGump( from, abay ) );
		}

		private static void OnNewAbayTarget( Mobile from, object targeted )
		{
			Item item = targeted as Item;
			BaseCreature bc = targeted as BaseCreature;

			if ( item == null && ! AbayConfig.AllowPetsAbay )
			{
				// Can't Abay pets and target it invalid
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 193 ] );
				from.Target = new AbayTarget( new AbayTargetCallback( OnNewAbayTarget ), -1, false );
				return;
			}

			if ( bc != null )
			{
				// Abaying a pet
				OnCreatureAbay( from, bc );
				return;
			}

			if ( !CheckItem( item ) )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 194 ] );
				from.Target = new AbayTarget( new AbayTargetCallback( OnNewAbayTarget ), -1, false );
				return;
			}

			if ( !CheckIdentified( item ) )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 195 ] );
				from.Target = new AbayTarget( new AbayTargetCallback( OnNewAbayTarget ), -1, false );
				return;
			}

			if ( !item.Movable )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 205 ] );
				from.Target = new AbayTarget( new AbayTargetCallback( OnNewAbayTarget ), -1, false );
				return;
			}

			bool ok = true;

			if ( item is Container )
			{
				foreach( Item sub in item.Items )
				{
					if ( !CheckItem( sub ) )
					{
						from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 196 ] );
						ok = false;
						break;
					}

					if ( ! sub.Movable )
					{
						from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 205 ] );
						ok = false;
						break;
					}

					if ( sub is Container && sub.Items.Count > 0 )
					{
						ok = false;
						from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 197 ] );
						break;
					}
				}
			}

			if ( ! ( item.IsChildOf( from.Backpack ) || item.IsChildOf( from.BankBox ) ) )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 198 ] );
				ok = false;
			}

			if ( ! ok )
			{
				from.Target = new AbayTarget( new AbayTargetCallback( OnNewAbayTarget ), -1, false );
			}
			else
			{
				// Item ok, start Abay creation
				AbayItem abay = new AbayItem( item, from );

				from.SendGump( new NewAbayGump( from, abay ) );
			}
		}

		/// <summary>
		/// Verifies if an item can be sold through the Abay
		/// </summary>
		/// <param name="item">The item being sold</param>
		/// <returns>True if the item is allowed</returns>
		private static bool CheckItem( Item item )
		{
			foreach( Type t in AbayConfig.ForbiddenTypes )
			{
				if ( t == item.GetType() )
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// This check is intended for non-AOS only. It verifies whether the item has an Identified
		/// property and in that case it it's set to true.
		/// </summary>
		/// <param name="item">The item being evaluated</param>
		/// <remarks>This method always returns true if Core.AOS is set to true.</remarks>
		/// <returns>True if the item can be Abayed, false otherwise</returns>
		private static bool CheckIdentified( Item item )
		{
			if ( Core.AOS )
				return true;

			PropertyInfo prop = item.GetType().GetProperty( "Identified" ); // Do not translate this!

			if ( prop == null )
				return true;

			bool identified = true;

			try
			{
				identified = (bool) prop.GetValue( item, null );
			}
			catch {} // Possibly there's an Identified property whose value is not bool - allow Abay of this

			if ( identified && item.Items.Count > 0 )
			{
				foreach( Item child in item.Items )
				{
					if ( ! CheckIdentified( child ) )
					{
						identified = false;
						break;
					}
				}
			}

			return identified;
		}

		/// <summary>
		/// Removes the Abay system from the server. All Abays will end unsuccesfully.
		/// </summary>
		/// <param name="m">The mobile terminating the system</param>
		public static void ForceDelete( Mobile m )
		{
			Console.WriteLine( "Abay system terminated on {0} at {1} by {2} ({3}, Account: {4})", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), m.Name, m.Serial, ( m.Account as Account ).Username );

			while ( Abays.Count > 0 || Pending.Count > 0 )
			{
				while ( Abays.Count > 0 )
				{
					( Abays[ 0 ] as AbayItem ).ForceEnd();
				}

				while ( Pending.Count > 0 )
				{
					( Pending[ 0 ] as AbayItem ).ForceEnd();
				}
			}

			ControlStone.ForceDelete();
			ControlStone = null;
		}

		/// <summary>
		/// Finds an Abay through its id
		/// </summary>
		/// <param name="id">The GUID identifying the Abay</param>
		/// <returns>An AbayItem object if the speicifies Abay is still in the system</returns>
		public static AbayItem Find( Guid id )
		{
			if ( !Running )
				return null;

			foreach( AbayItem item in Pending )
			{
				if ( item.ID == id )
					return item;
			}

			foreach( AbayItem item in Abays )
			{
				if ( item.ID == id )
					return item;
			}

			return null;
		}

		/// <summary>
		/// Gets the Abays created by a player
		/// </summary>
		/// <param name="m">The player requesting the Abays</param>
		public static ArrayList GetAbays( Mobile m )
		{
			ArrayList abays = new ArrayList();

			try
			{
				foreach( AbayItem abay in Abays )
				{
					if ( abay.Owner == m || ( abay.Owner != null && m.Account.Equals( abay.Owner.Account ) ) )
					{
						abays.Add( abay );
					}
				}
			}
			catch{}

			return abays;
		}

		/// <summary>
		/// Gets the list of Abays a mobile has bids on
		/// </summary>
		public static ArrayList GetBids( Mobile m )
		{
			ArrayList bids = new ArrayList();
         
			try
			{
				foreach( AbayItem abay in Abays )
				{
					if ( abay.MobileHasBids( m ) )
					{
						bids.Add( abay );
					}
				}
			}
			catch {}		

			return bids;
		}

		/// <summary>
		/// Gets the list of pendencies for a mobile
		/// </summary>
		public static ArrayList GetPendencies( Mobile m )
		{
			ArrayList list = new ArrayList();

			try
			{
				foreach( AbayItem abay in Pending )
				{
					if ( abay.Owner == m || ( abay.Owner != null && m.Account.Equals( abay.Owner.Account ) ) )
					{
						list.Add( abay );
					}
					else if ( abay.HighestBid.Mobile == m || ( abay.HighestBid.Mobile != null && m.Account.Equals( abay.HighestBid.Mobile.Account ) ) )
					{
						list.Add( abay );
					}
				}
			}
			catch {}

			return list;
		}

		#endregion

		#region Checks

		/// <summary>
		/// Verifies if a mobile can create a new Abay
		/// </summary>
		/// <param name="m">The mobile trying to create an Abay</param>
		/// <returns>True if allowed</returns>
		public static bool CanAbay( Mobile m )
		{
			if ( m.AccessLevel >= AccessLevel.GameMaster ) // Staff can always Abay
				return true;

			int count = 0;

			foreach( AbayItem abay in Abays )
			{
				if ( abay.Account == ( m.Account as Account ) )
				{
					count++;
				}
			}

			return count < MaxAbays;
		}

		#endregion

		#region Scheduling

		public static void Initialize()
		{
			try
			{
				if ( Running )
				{
					VerifyIntegrity();
					VerifyAbays();
					VerifyPendencies();
				}
			}
			catch ( Exception err )
			{
				m_ControlStone = null;

				Console.WriteLine( "An error occurred when initializing the Abay System. The system has been temporarily disabled." );
				Console.WriteLine( "Error details: {0}", err.ToString() );
			}
		}

		public static void OnDeadlineReached()
		{
			if ( ! Running )
				return;

			VerifyAbays();
			VerifyPendencies();
		}

		/// <summary>
		/// Verifies whether any pets in current Abays have been deleted
		/// </summary>
		private static void VerifyIntegrity()
		{
			foreach( AbayItem abay in Abays )
				abay.VeirfyIntergrity();
		}

		/// <summary>
		/// Verifies current Abays ending the ones that expired
		/// </summary>
		public static void VerifyAbays()
		{
			lock ( World.Items )
			{
				lock ( World.Mobiles )
				{
					if ( ! Running )
						return;

					ArrayList list = new ArrayList();
					ArrayList invalid = new ArrayList();

					foreach( AbayItem abay in Abays )
					{
						if ( abay.Item == null || ( abay.Creature && abay.Pet == null ) )
							invalid.Add( abay );
						else if ( abay.Expired )
							list.Add( abay );
					}

					foreach( AbayItem inv in invalid )
					{
						inv.EndInvalid();
					}

					foreach( AbayItem expired in list )
					{
						expired.End( null );
					}
				}
			}
		}

		/// <summary>
		/// Verifies pending Abays ending the ones that expired
		/// </summary>
		public static void VerifyPendencies()
		{
			lock ( World.Items )
			{
				lock ( World.Mobiles )
				{
					if ( !Running )
						return;

					ArrayList list = new ArrayList();

					foreach( AbayItem abay in Pending )
					{
						if ( abay.PendingExpired )
							list.Add( abay );
					}

					foreach( AbayItem expired in list )
					{
						expired.PendingTimeOut();
					}
				}
			}
		}

		/// <summary>
		/// Disables the system until the next reboot
		/// </summary>
		public static void Disable()
		{
			m_ControlStone = null;
			AbayScheduler.Stop();
		}

		#endregion

		/// <summary>
		/// Outputs all relevant Abay data to a text file
		/// </summary>
		public static void ProfileAbays()
		{
			string file = Path.Combine( Core.BaseDirectory, "AbayProfile.txt" );

			try
			{
				StreamWriter sw = new StreamWriter( file, false );

				sw.WriteLine( "Abay System Profile" );
				sw.WriteLine( "{0}", DateTime.Now.ToLongDateString() );
				sw.WriteLine( "{0}", DateTime.Now.ToShortTimeString() );
				sw.WriteLine( "{0} Running Abays", Abays.Count );
				sw.WriteLine( "{0} Pending Abays", Pending.Count );
				sw.WriteLine( "Next Deadline : {0} at {1}", AbayScheduler.Deadline.ToShortDateString(), AbayScheduler.Deadline.ToShortTimeString() );

				sw.WriteLine();
				sw.WriteLine( "Abays List" );
				sw.WriteLine();

				ArrayList abays = new ArrayList( Abays );

				foreach( AbayItem a in abays )
				{
					a.Profile( sw );
				}

				sw.WriteLine( "Pending Abays List" );
				sw.WriteLine();

				ArrayList pending = new ArrayList( Pending );

				foreach( AbayItem p in pending )
				{
					p.Profile( sw );
				}

				sw.WriteLine( "End of profile" );
				sw.Close();
			}
			catch ( Exception err )
			{
				Console.WriteLine( "Couldn't output abay profile. Error: {0}", err.ToString() );
			}
		}
	}
}