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
using Xanthos.Utilities;

namespace Arya.Abay
{
	// This file is for configuration of the Abay System.  It is advised
	// that you DO NOT edit this file, instead place AbayConfig.xml in the 
	// RunUO/Data directory and modify the values there to configure the system
	// without changing code.  This allows you to take updates to the system
	// without losing your specific configuration settings.

	public class AbayConfig
	{
		/// <summary>
		/// The hue used for messages in the system
		/// </summary>
		public static int MessageHue = 0x40;

		/// <summary>
		/// List of the types that can not be sold through the Abay system
		/// </summary>
		public static Type[] ForbiddenTypes =
		{
			typeof(Server.Items.Gold),
			typeof(Server.Items.BankCheck),
			typeof(Server.Items.DeathRobe),
			typeof(AbayGoldCheck),
			typeof(AbayItemCheck)
		};

		/// <summary>
		/// This is the number of days the system will wait for the buyer or seller to decide on an ambiguous situation.
		/// This can occur whether the highest bid didn't match the Abay reserve. The owner will have then X days to
		/// accept or refuse the Abay. Another case is when one or more items is deleted due to a wipe or serialization error.
		/// The buyer will have to decide in this case.
		/// </summary>
		public static int DaysForConfirmation = 5;

		/// <summary>
		/// This value specifies how higher the reserve can be with respect to the starting bid. This factor should limit
		/// any possible abuse of the reserve and prevent players from using very high values to be absolutely sure they will have
		/// to sell only if they're happy with the outcome.
		/// </summary>
		public static double MaxReserveMultiplier = 3.0d;

		/// <summary>
		/// This is the hue used to simulate the black hue because hue #1 isn't displayed
		/// correctly on gumps. If your shard is using custom hues, you might want to
		/// double check this value and verify it corresponds to a pure black hue.
		/// </summary>
		public static int BlackHue = 2000;

		/// <summary>
		/// This variable controls whether the system will sell pets as well
		/// </summary>
		public static bool AllowPetsAbay = true;

		/// <summary>
		/// This is the Access Level required to admin an Abay through its
		/// view gump. This will allow to see the props and to delete it.
		/// </summary>
		public static AccessLevel AbayAdminAcessLevel = AccessLevel.Administrator;

		/// <summary>
		/// If you don't have a valid UO installation on the server, or have trouble with the system
		/// specify the location of the cliloc.enu file here:
		/// 
		/// Example - Make sure you use the @ before the string:
		/// 
		/// public static string ClilocLocation = @"C:\RunUO\Misc\cliloc.enu";
		/// </summary>
		public static string ClilocLocation = @"C:\Program Files (x86)\Electronic Arts\Ultima Online Classic\cliloc.enu\cliloc.enu";

		/// <summary>
		/// Set this to false if you don't want to the system to produce a log file in \Logs\Abay.txt
		/// </summary>
		public static bool EnableLogging = true;

		/// <summary>
		/// When a bid is placed within 5 minutes from the Abay's ending, the Abay duration will be
		/// extended by this value.
		/// </summary>
		public static TimeSpan LateBidExtention = TimeSpan.FromMinutes( 0.0 );

		/// <summary>
		/// This value specifies how much a player will have to pay to Abay an item:
		/// - 0.0 means that Abaying is free
		/// - A value less or equal than 1.0 represents a percentage from 0 to 100%. This percentage will be applied on
		/// the max value between the starting bid and the reserve.
		/// - A value higher than 1.0 represents a fixed cost for the service (rounded).
		/// </summary>
		public static double CostOfAbay = 0.0;

		/// <summary>
		/// Savings Account configuration for daily interest paid
		/// </summary>
		public static double GoldInterestRate = .04;		// Percentage paid each day for gold
		public static double TokensInterestRate = .04;		// Percentage paid each day for tokens

		public static bool EnableTokens = false;			// Enable/disable them

		public static Type TokenType = null;
		public static Type TokenCheckType = null;

		public static int InterestHour = 20;
		public static double MaximumCheckValue = 25000000;


		//--------------------

		private const string kConfigFile = @"Data/AbayConfig.xml";
		private const string kConfigName = "AbaySystem";

		public static void Initialize()
		{
			Element element = ConfigParser.GetConfig( kConfigFile, kConfigName );

			if ( null == element || element.ChildElements.Count <= 0 )
				return;

			AccessLevel tempAccessLevel;
			Type[] tempTypeArray;
			double tempDouble;
			bool tempBool;
			int tempInt;

			try
			{
				TokenType = Type.GetType( "Server.Items.Daat99Tokens" );
				TokenCheckType = Type.GetType( "Server.Items.TokenCheck" );
			}
			catch ( Exception exc )
			{
				Console.WriteLine( "Error attempting to load token classes {0}...", exc.Message );
			}

			foreach( Element child in element.ChildElements) 
			{
				if ( child.TagName == "MessageHue" && child.GetIntValue( out tempInt ))
					MessageHue = tempInt;
				
				else if ( child.TagName == "DaysForConfirmation" && child.GetIntValue( out tempInt ))
					DaysForConfirmation = tempInt;

				else if ( child.TagName == "MaxReserveMultiplier" && child.GetDoubleValue( out tempDouble ))
					MaxReserveMultiplier = tempDouble;

				else if ( child.TagName == "BlackHue" && child.GetIntValue( out tempInt ))
					BlackHue = tempInt;

				else if ( child.TagName == "AllowPetsAbay" && child.GetBoolValue( out tempBool ))
					AllowPetsAbay = tempBool;

				else if ( child.TagName == "AbayAdminAcessLevel" && child.GetAccessLevelValue( out tempAccessLevel ))
					AbayAdminAcessLevel = tempAccessLevel;

				else if ( child.TagName == "ClilocLocation" && null != child.Text )
					ClilocLocation = child.Text;

				else if ( child.TagName == "EnableLogging" && child.GetBoolValue( out tempBool ))
					EnableLogging = tempBool;
				
				else if ( child.TagName == "LateBidExtention" && child.GetDoubleValue( out tempDouble ))
					LateBidExtention = TimeSpan.FromMinutes( tempDouble );
				
				else if ( child.TagName == "CostOfAbay" && child.GetDoubleValue( out tempDouble ))
					CostOfAbay = tempDouble;

				else if ( child.TagName == "ForbiddenTypes" && child.GetArray( out tempTypeArray ) )
					ForbiddenTypes = tempTypeArray;

				else if ( child.TagName == "InterestHour" && child.GetIntValue( out tempInt ) )
					InterestHour = tempInt;

				else if ( child.TagName == "GoldInterestRate" && child.GetDoubleValue( out tempDouble ) )
					GoldInterestRate = tempDouble;

				else if ( child.TagName == "TokensInterestRate" && child.GetDoubleValue( out tempDouble ) )
					TokensInterestRate = tempDouble;

				else if ( child.TagName == "EnableTokens" && child.GetBoolValue( out tempBool ) )
					EnableTokens = tempBool;
			}
		}
	}
}