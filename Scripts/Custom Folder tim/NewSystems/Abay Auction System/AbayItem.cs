#region AuthorHeader
//
// Abay version 2.1, by Xanthos and Arya
//
// Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Ultima;
using System.Text;
using Server;
using Server.Network;
using Server.Accounting;
using Server.Items;
using Server.Mobiles;
using Xanthos.Utilities;
using Xanthos.Interfaces;

namespace Arya.Abay
{
	/// <summary>
	/// Defines situations for pending situations
	/// </summary>
	public enum AbayPendency : byte
	{
		/// <summary>
		/// The user still has to make a decision
		/// </summary>
		Pending = 0,
		/// <summary>
		/// The user OKs the Abay
		/// </summary>
		Accepted = 1,
		/// <summary>
		/// The user didn't accept the Abay
		/// </summary>
		NotAccepted = 2
	}

	/// <summary>
	/// Defines what happens with the Abay item if the Abay is ended by the staff
	/// </summary>
	public enum ItemFate
	{
		/// <summary>
		/// The item is returned to the owner
		/// </summary>
		ReturnToOwner,
		/// <summary>
		/// The item is taken by the staff
		/// </summary>
		ReturnToStaff,
		/// <summary>
		/// The item is deleted
		/// </summary>
		Delete
	}

	/// <summary>
	/// Specifies the type of message that should be dispatched for the buyer or the owner
	/// </summary>
	public enum AbayMessage : byte
	{
		/// <summary>
		/// No message should be dispatched
		/// </summary>
		None = 0,
		/// <summary>
		/// An information message should be dispatched
		/// </summary>
		Information = 1,
		/// <summary>
		/// A feedback message should be dispatched
		/// </summary>
		Response = 2
	}

	/// <summary>
	/// An Abay entry, holds all the information about a single Abay process
	/// </summary>
	public class AbayItem
	{
		#region ItemInfo class

		public class ItemInfo
		{
			private string m_Name;
			private Item m_Item;
			private string m_Props;

			public string Name
			{
				get
				{
					if ( m_Item != null )
						return m_Name;
					else
						return "N/A";
				}
			}

			public Item Item
			{
				get { return m_Item; }
			}

			public string Properties
			{
				get
				{
					if ( m_Item != null )
						return m_Props;
					else
						return AbaySystem.ST[ 146 ];
				}
			}

			public ItemInfo( Item item )
			{
				m_Item = item;

				if ( item.Name != null )
					m_Name = item.Name;
				// else
					// m_Name = m_StringList.Table[ item.LabelNumber ] as string;

				if ( item.Amount > 1 )
				{
					m_Name = string.Format( "{0} {1}", item.Amount.ToString("#,0" ), m_Name );
				}

				if ( item is MobileStatuette )
				{
					m_Props = GetCreatureProperties( ( item as MobileStatuette ).ShrunkenPet );
				}
				else
				{
					m_Props = AbayItem.GetItemProperties( item );
				}
			}

			private static string GetCreatureProperties( BaseCreature creature )
			{
				StringBuilder sb = new StringBuilder();

				sb.Append( "<basefont color=#FFFFFF>" );

				if ( creature.Name != null )
				{
					sb.AppendFormat( "Name : {0}<br", creature.Name );
				}

				sb.AppendFormat( AbaySystem.ST[ 147 ] , creature.ControlSlots );
				sb.AppendFormat( AbaySystem.ST[ 148 ], creature.IsBondable ? "Yes" : "No" );
				sb.AppendFormat( AbaySystem.ST[ 149 ] , creature.Str );
				sb.AppendFormat( AbaySystem.ST[ 150 ] , creature.Dex );
				sb.AppendFormat( AbaySystem.ST[ 151 ] , creature.Int );

				int index = 0;
				Skill skill = null;

				while ( ( skill = creature.Skills[ index++ ] ) != null )
				{
					if ( skill.Value > 0 )
					{
						sb.AppendFormat( "{0} : {1}<br>", skill.Name, skill.Value );
					}
				}

				return sb.ToString();
			}

			private ItemInfo()
			{
			}

			public void Serialize( GenericWriter writer )
			{
				// Version 1
				// Version 0
				writer.Write( m_Name );
				writer.Write( m_Item );
				writer.Write( m_Props );
			}

			public static ItemInfo Deserialize( GenericReader reader, int version )
			{
				ItemInfo item = new ItemInfo();

				switch ( version )
				{
					case 1:
					case 0:
						item.m_Name = reader.ReadString();
						item.m_Item = reader.ReadItem();
						item.m_Props = reader.ReadString();
						break;
				}

				return item;
			}

			/// <summary>
			/// Verifies if the mobile referenced by this item is still valid
			/// </summary>
			public void VeirfyIntegrity()
			{
				IShrinkItem shrinkItem = m_Item as IShrinkItem;

				if ( null != shrinkItem && null == shrinkItem.ShrunkenPet )
				{
					m_Item.Delete();
					m_Item = null; // This will make this item invalid
				}
			}
		}

		#endregion

		#region Item Properties

		private static StringList m_StringList;

		static AbayItem()
		{
			string clilocFolder = null;

			if ( AbayConfig.ClilocLocation != null )
			{
				clilocFolder = Path.GetDirectoryName( AbayConfig.ClilocLocation );
				//Ultima.Client.Directories.Insert( 0, clilocFolder );
			}

			m_StringList = new StringList( "ENU" );

			if ( clilocFolder != null )
			{
				//Ultima.Client.Directories.RemoveAt( 0 );
			}
		}

		/// <summary>
		/// Gets an html formatted string with all the properies for the item
		/// </summary>
		/// <returns>A string object containing the html structure corresponding to the item properties</returns>
		private static string GetItemProperties( Item item )
		{
			if ( item == null || item.PropertyList == null )
			{
				return AbaySystem.ST[ 78 ];
			}

			if ( Core.AOS )
			{
				#region AoS
				ObjectPropertyList plist = new ObjectPropertyList( item );
				item.GetProperties( plist );

				byte[] data = plist.UnderlyingStream.UnderlyingStream.ToArray();
				ArrayList list = new ArrayList();

				int index = 15; // First localization number index

				while ( true )
				{
					uint number = 0;

					if ( index + 4 >= data.Length )
					{
						break;
					}

					number = (uint) ( data[ index++ ] << 24 | data[ index++ ] << 16 | data[ index++ ] << 8 | data[ index++ ] );
					ushort length = 0;

					if ( index + 2 > data.Length )
					{
						break;
					}

					length = (ushort) ( data[ index++ ] << 8 | data[ index++ ] );

					// Read the string
					int end = index + length;

					if ( end >= data.Length )
					{
						end = data.Length - 1;
					}

					System.Text.StringBuilder s = new System.Text.StringBuilder();
					while ( index + 2 <= end + 1 )
					{
						short next = (short) ( data[ index++ ] | data[ index++ ] << 8 );

						if ( next == 0 )
							break;

						s.Append( System.Text.Encoding.Unicode.GetString( BitConverter.GetBytes( next ) ) );
					}

					list.Add( ComputeProperty( (int) number, s.ToString() ) );
				}

				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append( "<basefont color=#FFFFFF><p>" );

				foreach( string prop in list )
				{
					sb.AppendFormat( "{0}<br>", prop );
				}

				return sb.ToString();
				#endregion
			}
			else
			{
				#region Non AoS
				StringBuilder sb = new StringBuilder();
				sb.Append( "<basefont color=#FFFFFF><p>" );

				// Get the item name
				if ( item.Name != null && item.Name.Length > 0 )
				{
					sb.AppendFormat( "{0}<br>", item.Name );
				}
				else
				{
					//sb.AppendFormat( "{0}<br>", Capitalize( m_StringList.Table[ item.LabelNumber ] as string ) );
				}

				// Amount
				if ( item.Amount > 1 )
				{
					sb.AppendFormat( AbaySystem.ST[ 152 ] , item.Amount.ToString("#,0" ) );
				}

				// Loot type
				if ( item.LootType != LootType.Regular )
				{
					sb.AppendFormat( "{0}<br>", item.LootType.ToString() );
				}

				if ( item is IUsesRemaining )
				{
					sb.AppendFormat( AbaySystem.ST[ 153 ] , ( item as IUsesRemaining ).UsesRemaining );
				}

				// Manage item types

				if ( item is BaseWand )
				{
					#region Wands
					BaseWand bw = item as BaseWand;
					sb.AppendFormat( AbaySystem.ST[ 154 ] , bw.Effect.ToString() );
					sb.AppendFormat( AbaySystem.ST[ 155 ] , bw.Charges );
					#endregion
				}
				else if ( item is BaseArmor )
				{
					#region Armor
					BaseArmor ba = item as BaseArmor;

					if ( ba.PlayerConstructed )
					{
						if ( ba.Crafter != null )
						{
							sb.AppendFormat( AbaySystem.ST[ 156 ] , ba.Crafter.Name );
						}
						sb.AppendFormat( AbaySystem.ST[ 157 ] , ba.Resource.ToString() );
					}

					sb.AppendFormat( AbaySystem.ST[ 158 ] , ba.Quality.ToString() );
					sb.AppendFormat( AbaySystem.ST[ 159 ] , ba.HitPoints, ba.MaxHitPoints );

					if ( ba.Durability != ArmorDurabilityLevel.Regular )
					{
						sb.AppendFormat( AbaySystem.ST[ 160 ] , ba.Durability.ToString() );
					}

					if ( ba.ProtectionLevel != ArmorProtectionLevel.Regular )
					{
						sb.AppendFormat( AbaySystem.ST[ 161 ] , ba.ProtectionLevel.ToString() );
					}
					#endregion
				}
				else if ( item is BaseWeapon )
				{
					#region Weapons
					BaseWeapon bw = item as BaseWeapon;

					if ( bw.PlayerConstructed )
					{
						if ( bw.Crafter != null )
						{
							sb.AppendFormat( AbaySystem.ST[ 156 ] , bw.Crafter.Name );
						}
						sb.AppendFormat( AbaySystem.ST[ 157 ] , bw.Resource.ToString() );
					}

					sb.AppendFormat( AbaySystem.ST[ 158 ] , bw.Quality.ToString() );
					sb.AppendFormat( AbaySystem.ST[ 159 ], bw.HitPoints, bw.MaxHitPoints );

					if ( bw.PoisonCharges > 0 )
					{
						sb.AppendFormat( AbaySystem.ST[ 162 ] , bw.PoisonCharges, bw.Poison.ToString() );
					}

					if ( item is BaseRanged )
					{
						sb.AppendFormat( AbaySystem.ST[ 163 ] , bw.MaxRange.ToString() );
					}

					if ( bw.DamageLevel != WeaponDamageLevel.Regular )
					{
						sb.AppendFormat( AbaySystem.ST[ 164 ] , bw.DamageLevel.ToString() );
					}

					if ( bw.DurabilityLevel != WeaponDurabilityLevel.Regular )
					{
						sb.AppendFormat( AbaySystem.ST[ 160 ] , bw.DurabilityLevel.ToString() );
					}

					if ( bw.AccuracyLevel != WeaponAccuracyLevel.Regular )
					{
						if ( bw.AccuracyLevel == WeaponAccuracyLevel.Accurate )
						{
							sb.AppendFormat( AbaySystem.ST[ 165 ] );
						}
						else
						{
							sb.AppendFormat( AbaySystem.ST[ 166 ] , bw.AccuracyLevel.ToString() );
						}
					}

					if ( bw.Slayer != SlayerName.None )
					{
						sb.AppendFormat( AbaySystem.ST[ 167 ] , bw.Slayer.ToString() );
					}
					#endregion
				}
				else if ( item is TreasureMap )
				{
					#region Treasure Map
					TreasureMap tm = item as TreasureMap;
					//sb.AppendFormat( AbaySystem.ST[ 168 ] , tm.ChestMap );
					#endregion
				}
				else if ( item is Spellbook )
				{
					#region Spellbook
					Spellbook sp = item as Spellbook;
					sb.AppendFormat( AbaySystem.ST[ 169 ] , sp.SpellCount );
					#endregion
				}
				else if ( item is PotionKeg )
				{
					#region Potion Keg
					PotionKeg pk = item as PotionKeg;

					int number;

					if ( pk.Held <= 0 )
						number = 502246; // The keg is empty.
					else if ( pk.Held < 5 )
						number = 502248; // The keg is nearly empty.
					else if ( pk.Held < 20 )
						number = 502249; // The keg is not very full.
					else if ( pk.Held < 30 )
						number = 502250; // The keg is about one quarter full.
					else if ( pk.Held < 40 )
						number = 502251; // The keg is about one third full.
					else if ( pk.Held < 47 )
						number = 502252; // The keg is almost half full.
					else if ( pk.Held < 54 )
						number = 502254; // The keg is approximately half full.
					else if ( pk.Held < 70 )
						number = 502253; // The keg is more than half full.
					else if ( pk.Held < 80 )
						number = 502255; // The keg is about three quarters full.
					else if ( pk.Held < 96 )
						number = 502256; // The keg is very full.
					else if ( pk.Held < 100 )
						number = 502257; // The liquid is almost to the top of the keg.
					else
						number = 502258; // The keg is completely full.

					//sb.AppendFormat( Capitalize( m_StringList.Table[ number ] as string ) );
					#endregion
				}

				return sb.ToString();

				#endregion
			}
		}

		/// <summary>
		/// Capitalizes each word in a string
		/// </summary>
		/// <param name="property">The input string</param>
		/// <returns>The output string </returns>
		private static string Capitalize( string property )
		{

			string[] parts = property.Split( ' ' );
			StringBuilder sb = new StringBuilder();

			for ( int i = 0; i < parts.Length; i++ )
			{
				string part = parts[ i ];

				if ( part.Length == 0 )
				{
					continue;
				}

				char c = char.ToUpper( part[ 0 ] );

				part = part.Substring( 1 );
				sb.AppendFormat( "{0}{1}", string.Concat( c, part ), i < parts.Length - 1 ? " " : "" );
			}

			return sb.ToString();
		}

		/// <summary>
		/// Converts a localization number and its arguments to a valid string
		/// </summary>
		/// <param name="number">The localization number of the label</param>
		/// <param name="arguments">The arguments for the label</param>
		/// <returns>The translated string</returns>
		private static string ComputeProperty( int number, string arguments )
		{
			string prop = ""; //m_StringList.Table[ number ] as string;

			if ( prop == null )
			{
				return AbaySystem.ST[ 170 ] ;
			}

			if ( arguments == null || arguments.Length == 0 )
			{
				return Capitalize( prop );
			}

			string[] args = arguments.Split( '\t' );
			Regex reg = new Regex( @"~\d+\w*~", RegexOptions.None );
			MatchCollection matches = reg.Matches( prop, 0 );

			if ( matches.Count == args.Length )
			{
				// Valid
				for ( int i = 0; i < matches.Count; i++ )
				{
					if ( args[ i ].StartsWith( "#" ) )
					{
						int loc = -1;

						try
						{
							loc = int.Parse( args[ i ].Substring( 1 ) );
						}
						catch {}

						if ( loc != -1 )
						{
						//	args[ i ] = m_StringList.Table[ loc ] as string;
						}
					}

					Match m = matches [ i ];

					prop = prop.Replace( m.Value, args[ i ] );
				}

				return Capitalize( prop );
			}
			else
			{
				return AbaySystem.ST[ 171 ] ;
			}
		}

		#endregion

		#region Variables

		private Item m_Item;
		private Mobile m_Owner;
		private DateTime m_StartTime;
		private DateTime m_EndTime;
		private TimeSpan m_Duration = TimeSpan.FromDays( 7 );
		private int m_MinBid = 1000;
		private int m_Reserve = 2000;
		private string m_Description = "";
		private ArrayList m_Bids;
		private string m_WebLink = "";
		private string m_ItemName;
		private bool m_Pending;
		private ItemInfo[] m_Items;
		private Guid m_ID;
		private AbayPendency m_OwnerPendency = AbayPendency.Pending;
		private AbayPendency m_BuyerPendency = AbayPendency.Pending;
		private AbayMessage m_OwnerMessage = AbayMessage.None;
		private AbayMessage m_BuyerMessage = AbayMessage.None;
		private DateTime m_PendingEnd;
		private int m_BuyNow = 0;

		#region Props

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// States whether this Abay allows the buy now feature
			/// </summary>
		public bool AllowBuyNow
		{
			get { return m_BuyNow > 0; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the buy now value
			/// </summary>
		public int BuyNow
		{
			get { return m_BuyNow; }
			set { m_BuyNow = value; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the date and time corrsponding to the moment when the pending situation will automatically end
			/// </summary>
		public DateTime PendingEnd
		{
			get { return m_PendingEnd; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the item being sold at the Abay
			/// </summary>
		public Item Item
		{
			get { return m_Item; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the owner of the item
			/// </summary>
		public Mobile Owner
		{
			get { return m_Owner; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the starting time for this Abay
			/// </summary>
		public DateTime StartTime
		{
			get { return m_StartTime; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the end time for this Abay
			/// </summary>
		public DateTime EndTime
		{
			get { return m_EndTime; }
		}

		/// <summary>
		/// Gets the running length of the Abay for this item
		/// </summary>
		public TimeSpan Duration
		{
			get { return m_Duration; }
			set
			{
				try
				{
					m_Duration = value;
				}
				catch
				{
					m_Duration = TimeSpan.Zero;
				}
			}
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the time to live left for this Abay
			/// </summary>
		public TimeSpan TimeLeft
		{
			get
			{
				return m_EndTime - DateTime.UtcNow;
			}
		}

		/// <summary>
		/// Gets or sets the minimum bid allowed for this item
		/// </summary>
		public int MinBid
		{
			get { return m_MinBid; }
			set { m_MinBid = value; }
		}

		/// <summary>
		/// Gets or sets the reserve price for the item
		/// </summary>
		public int Reserve
		{
			get { return m_Reserve; }
			set { m_Reserve = value; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets or sets the description for this item
			/// </summary>
		public string Description
		{
			get { return m_Description; }
			set { m_Description = value; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// A web link associated with this Abay item
			/// </summary>
		public string WebLink
		{
			get { return m_WebLink; }
			set
			{
				if ( value != null && value.Length > 0 )
				{
					if ( value.ToLower().StartsWith( "http://" ) && value.Length > 7 )
					{
						value = value.Substring( 7 );
					}
				}

				m_WebLink = value;
			}
		}

		/// <summary>
		/// Gets or sets the list of existing bids
		/// </summary>
		public ArrayList Bids
		{
			get { return m_Bids; }
			set { m_Bids = value; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the account that's selling this item
			/// </summary>
		public Account Account
		{
			get
			{
				if ( m_Owner != null && m_Owner.Account != null )
				{
					return m_Owner.Account as Account;
				}
				else
				{
					return null;
				}
			}
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets or sets the name of the item being sold
			/// </summary>
		public string ItemName
		{
			get { return m_ItemName; }
			set { m_ItemName = value; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// True if the Abay is over but the reserve hasn't been met and the owner still haven't decided
			/// if to sell the item or not. This value makes no sense before the Abay is over.
			/// </summary>
		public bool Pending
		{
			get { return m_Pending; }
		}

		/// <summary>
		/// Gets the definitions of the items sold
		/// </summary>
		public ItemInfo[] Items
		{
			get { return m_Items; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the number of items sold
			/// </summary>
		public int ItemCount
		{
			get { return m_Items.Length; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the unique ID of this Abay
			/// </summary>
		public Guid ID
		{
			get { return m_ID; }
		}

		#endregion

		#endregion

		#region Serialization

		/// <summary>
		/// Saves the Abay item into the world file
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize( GenericWriter writer )
		{
			// Version 1
			writer.Write( m_BuyNow );

			// Version 0
			writer.Write( m_Owner );
			writer.Write( m_StartTime );
			writer.Write( m_Duration );
			writer.Write( m_MinBid );
			writer.Write( m_Reserve );
			writer.Write( m_Duration );
			writer.Write( m_Description );
			writer.Write( m_WebLink );
			writer.Write( m_Pending );
			writer.Write( m_ItemName );
			writer.Write( m_Item );
			writer.Write( m_ID.ToString() );
			writer.WriteDeltaTime( m_EndTime );
			writer.Write( (byte) m_OwnerPendency );
			writer.Write( (byte) m_BuyerPendency );
			writer.Write( (byte) m_OwnerMessage );
			writer.Write( (byte) m_BuyerMessage );
			writer.WriteDeltaTime( m_PendingEnd );

			writer.Write( m_Items.Length );
			// Items
			foreach( ItemInfo ii in m_Items )
			{
				ii.Serialize( writer );
			}

			// Bids
			writer.Write( m_Bids.Count );
			foreach( Bid bid in m_Bids )
			{
				bid.Serialize( writer );
			}
		}

		/// <summary>
		/// Loads an AbayItem
		/// </summary>
		/// <returns>An AbayItem</returns>
		public static AbayItem Deserialize( GenericReader reader, int version )
		{
			AbayItem Abay = new AbayItem();

			switch ( version )
			{
				case 1:
					Abay.m_BuyNow = reader.ReadInt();
					goto case 0;

				case 0:
					Abay.m_Owner = reader.ReadMobile();
					Abay.m_StartTime = reader.ReadDateTime();
					Abay.m_Duration = reader.ReadTimeSpan();
					Abay.m_MinBid = reader.ReadInt();
					Abay.m_Reserve = reader.ReadInt();
					Abay.m_Duration = reader.ReadTimeSpan();
					Abay.m_Description = reader.ReadString();
					Abay.m_WebLink = reader.ReadString();
					Abay.m_Pending = reader.ReadBool();
					Abay.m_ItemName = reader.ReadString();
					Abay.m_Item = reader.ReadItem();
					Abay.m_ID = new Guid( reader.ReadString() );
					Abay.m_EndTime = reader.ReadDeltaTime();
					Abay.m_OwnerPendency = (AbayPendency) reader.ReadByte();
					Abay.m_BuyerPendency = (AbayPendency) reader.ReadByte();
					Abay.m_OwnerMessage = (AbayMessage) reader.ReadByte();
					Abay.m_BuyerMessage = (AbayMessage) reader.ReadByte();
					Abay.m_PendingEnd = reader.ReadDeltaTime();

					int count = reader.ReadInt();
					Abay.m_Items = new ItemInfo[ count ];

					for ( int i = 0; i < count; i++ )
					{
						Abay.m_Items[ i ] = ItemInfo.Deserialize( reader, version );
					}

					count = reader.ReadInt();

					for ( int i = 0; i < count; i++ )
					{
						Abay.Bids.Add( Bid.Deserialize( reader, version ) );
					}
					break;
			}

			return Abay;
		}

		#endregion

		#region Properties

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the minimum increment required for the Abay
			/// </summary>
		public int BidIncrement
		{
			get
			{
				if ( m_MinBid <= 100 )
					return 10;

				if ( m_MinBid <= 500 )
					return 20;

				if ( m_MinBid <= 1000 )
					return 50;

				if ( m_MinBid <= 5000 )
					return 100;

				if ( m_MinBid <= 10000 )
					return 200;

				if ( m_MinBid <= 20000 )
					return 250;

				if ( m_MinBid <= 50000 )
					return 500;

				return 1000;
			}
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// States whether an item has at least one bid
			/// </summary>
		public bool HasBids
		{
			get { return m_Bids.Count > 0; }
		}

		/// <summary>
		/// Gets the highest bid for this item
		/// </summary>
		public Bid HighestBid
		{
			get
			{
				if ( m_Bids.Count > 0 )
					return m_Bids[ 0 ] as Bid;
				else
					return null;
			}
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
		public Mobile HighestBidder
		{
			get
			{
				if ( m_Bids.Count > 0 )
					return ( m_Bids[ 0 ] as Bid ).Mobile;
				else
					return null;
			}
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
		public int HighestBidValue
		{
			get
			{
				if ( m_Bids.Count > 0 )
					return ( m_Bids[ 0 ] as Bid ).Amount;
				else return 0;
			}
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// States whether the reserve has been met for this item
			/// </summary>
		public bool ReserveMet
		{
			get { return HighestBid != null && HighestBid.Amount >= m_Reserve; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// States whether this Abay has expired
			/// </summary>
		public bool Expired
		{
			get { return DateTime.UtcNow > m_EndTime; }
		}

		/// <summary>
		/// Gets the minimum bid that a player can place
		/// </summary>
		public int MinNewBid
		{
			get
			{
				if ( HighestBid != null )
					return HighestBid.Amount;
				else
					return m_MinBid;
			}
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the next deadline required by this Abay
			/// </summary>
		public DateTime Deadline
		{
			get
			{
				if ( ! Expired )
				{
					return m_EndTime;
				}
				else if ( m_Pending )
				{
					return m_PendingEnd;
				}
				else
				{
					return DateTime.MaxValue;
				}
			}
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Specifies if the pending period has timed out
			/// </summary>
		public bool PendingExpired
		{
			get
			{
				return DateTime.UtcNow >= m_PendingEnd;
			}
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the time left before the pending period expired
			/// </summary>
		public TimeSpan PendingTimeLeft
		{
			get { return m_PendingEnd - DateTime.UtcNow; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// States whether this Abay is selling a pet
			/// </summary>
		public bool Creature
		{
			get { return m_Item is MobileStatuette; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
			/// <summary>
			/// Gets the BaseCreature sold through this Abay. This will be null when selling an item.
			/// </summary>
		public BaseCreature Pet
		{
			get
			{
				if ( Creature )
				{
					return ((MobileStatuette)m_Item).ShrunkenPet;
				}

				return null;
			}
		}

		#endregion

		/// <summary>
		/// Creates a new AbayItem
		/// </summary>
		/// <param name="item">The item being sold</param>
		/// <param name="owner">The owner of the item</param>
		public AbayItem( Item item, Mobile owner )
		{
			m_ID = Guid.NewGuid();
			m_Item = item;
			m_Owner = owner;
			m_Bids = new ArrayList();

			if ( ! Creature )
			{
				m_Item.Visible = false;
				m_Owner.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 172 ] );
			}

			// Item name
			if ( m_Item.Name != null && m_Item.Name.Length > 0 )
			{
				m_ItemName = m_Item.Name;
			}
			else
			{
//				m_ItemName = m_StringList.Table[ m_Item.LabelNumber ] as string;
			}

			if ( m_Item.Amount > 1 )
			{
				m_ItemName = string.Format( "{0} {1}", m_Item.Amount.ToString("#,0" ), m_ItemName );
			}
		}

		/// <summary>
		/// Creates an AbayItem - for use in deserialization
		/// </summary>
		private AbayItem()
		{
			m_Bids = new ArrayList();
		}

		[ System.Runtime.CompilerServices.IndexerName( "SoldItem" ) ]
			/// <summary>
			/// Gets the item info corresponding to the index value
			/// </summary>
		public ItemInfo this[int index]
		{
			get
			{
				if ( index > -1 && index < m_Items.Length )
				{
					return m_Items[ index ];
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Confirms the Abay item and adds it into the system
		/// </summary>
		public void Confirm()
		{
			m_StartTime = DateTime.UtcNow;
			m_EndTime = m_StartTime + m_Duration;

			if ( Creature && Pet != null )
			{
				Pet.ControlTarget = null;
				Pet.ControlOrder = OrderType.Stay;
				Pet.Internalize();

				Pet.SetControlMaster( null );
				Pet.SummonMaster = null;
			}

			// Calculate all the ItemInfo
			if ( m_Item is Container && m_Item.Items.Count > 0 )
			{
				// Container with items
				m_Items = new ItemInfo[ m_Item.Items.Count ];

				for ( int i = 0; i < m_Items.Length; i++ )
				{
					m_Items[ i ] = new ItemInfo( m_Item.Items[ i ] as Item );
				}
			}
			else
			{
				m_Items = new ItemInfo[ 1 ];

				m_Items[ 0 ] = new ItemInfo( m_Item );
			}

			AbaySystem.Add( this );
			AbayScheduler.UpdateDeadline( m_EndTime );

			AbayLog.WriteNewAbay( this );
		}

		/// <summary>
		/// Cancels the new Abay and returns the item to the owner
		/// </summary>
		public void Cancel()
		{
			if ( !Creature )
			{
				m_Item.Visible = true;
				m_Owner.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 173 ] );
			}
			else
			{
				( m_Item as MobileStatuette ).GiveCreatureTo( m_Owner );
				m_Owner.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 174 ] );
			}
		}

		/// <summary>
		/// Sends the associated web link to a mobile
		/// </summary>
		/// <param name="m">The mobile that should receive the web link</param>
		public void SendLinkTo( Mobile m )
		{
			if ( m != null && m.NetState != null )
			{
				if ( m_WebLink != null && m_WebLink.Length > 0 )
				{
					m.LaunchBrowser( string.Format( "http://{0}", m_WebLink ) );
				}
			}
		}

		/// <summary>
		/// Verifies if a mobile can place a bid on this item
		/// </summary>
		/// <param name="m">The Mobile trying to bid</param>
		/// <returns>True if the mobile is allowed to bid</returns>
		public bool CanBid( Mobile m )
		{
			if ( m.AccessLevel > AccessLevel.Player )
				return false; // Staff shoudln't bid. This will also give the bids view to staff members.

			if ( this.Account == ( m.Account as Account ) ) // Same account as Abayeer
				return false;

			if ( Creature )
			{
				return ( Pet != null && Pet.CanBeControlledBy( m ) );
			}

			return true;
		}

		/// <summary>
		/// Verifies if a mobile is the owner of this Abay (checks accounts)
		/// </summary>
		/// <param name="m">The mobile being checked</param>
		/// <returns>True if the mobile is the owner of the Abay</returns>
		public bool IsOwner( Mobile m )
		{
			return ( this.Account == ( m.Account as Account ) );
		}

		/// <summary>
		/// Places a new bid
		/// </summary>
		/// <param name="from">The Mobile bidding</param>
		/// <param name="amount">The bid amount</param>
		/// <returns>True if the bid has been added and accepted</returns>
		public bool PlaceBid( Mobile from, int amount )
		{
			if ( ! CanBid( from ) )
				return false;

			if ( HighestBid != null )
			{
				if ( amount <= HighestBid.Amount )
				{
					from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 176 ] );
					return false;
				}
			}
			else if ( amount <= m_MinBid )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 177 ] );
				return false;
			}

			int delta = 0;

			if ( HighestBid != null )
				delta = amount - HighestBid.Amount;
			else
				delta = amount - m_MinBid;

			if ( BidIncrement > delta )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 204 ], BidIncrement );
				return false;
			}

			// Ok, do bid
			Bid bid = Bid.CreateBid( from, amount );

			if ( bid != null )
			{
				if ( HighestBid != null )
				{
					HighestBid.Outbid( this ); // Return money to previous highest bidder
				}

				m_Bids.Insert( 0, bid );
				AbayLog.WriteBid( this );

				// Check for Abay extension
				if ( AbayConfig.LateBidExtention > TimeSpan.Zero )
				{
					TimeSpan timeLeft = m_EndTime - DateTime.UtcNow;

					if ( timeLeft < TimeSpan.FromMinutes( 5.0 ) )
					{
						m_EndTime += AbayConfig.LateBidExtention;
						bid.Mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 230 ] );
					}
				}
			}

			return bid != null;
		}

		/// <summary>
		/// Forces the end of the Abay when the item or creature has been deleted
		/// </summary>
		public void EndInvalid()
		{
			AbaySystem.Abays.Remove( this );

			if ( HighestBid != null )
			{
				AbayGoldCheck gold = new AbayGoldCheck( this, AbayResult.ItemDeleted );
				GiveItemTo( HighestBid.Mobile, gold );
			}

			// The item has been deleted, no need to return it to the owner.
			// If it's a statuette, delete it
			if ( Creature && m_Item != null )
			{
				m_Item.Delete();
			}

			AbayLog.WriteEnd( this, AbayResult.ItemDeleted, null, null );

			// Over.
		}

		/// <summary>
		/// Forces the end of the Abay and removes it from the system
		/// </summary>
		/// <param name="m">The staff member deleting the Abay</param>
		/// <param name="itemfate">Specifies what should occur with the item</param>
		public void StaffDelete( Mobile m, ItemFate itemfate )
		{
			if ( AbaySystem.Abays.Contains( this ) )
				AbaySystem.Abays.Remove( this );
			else if ( AbaySystem.Pending.Contains( this ) )
				AbaySystem.Pending.Remove( this );

			if ( HighestBid != null )
			{
				AbayGoldCheck gold = new AbayGoldCheck( this, AbayResult.StaffRemoved );
				GiveItemTo( HighestBid.Mobile, gold );
			}

			AbayItemCheck check = new AbayItemCheck( this, AbayResult.StaffRemoved );
			string comments = null;

			switch( itemfate )
			{
				case ItemFate.Delete :

					check.ForceDelete();
					comments = "The item has been deleted";
					break;

				case ItemFate.ReturnToOwner:

					GiveItemTo( m_Owner, check );
					comments = "The item has been returned to the owner";
					break;

				case ItemFate.ReturnToStaff:

					GiveItemTo( m, check );
					comments = "The item has been claimed by the staff";
					break;
			}

			AbayLog.WriteEnd( this, AbayResult.StaffRemoved, m, comments );

			// OVer.
		}

		/// <summary>
		/// Ends the Abay.
		/// This function is called by the Abay system during its natural flow
		/// </summary>
		/// <param name="m">The Mobile eventually forcing the ending</param>
		public void End( Mobile m )
		{
			AbaySystem.Abays.Remove( this );

			if ( HighestBid == null )
			{
				// No bids, simply return the item
				AbayCheck item = new AbayItemCheck( this, AbayResult.NoBids );
				GiveItemTo( m_Owner, item );

				// Over, this Abay no longer exists
				AbayLog.WriteEnd( this, AbayResult.NoBids, m, null );
			}
			else
			{
				// Verify that all items still exist too, otherwise make it pending
				if ( IsValid() && ReserveMet )
				{
					// Abay has been succesful
					AbayCheck item = new AbayItemCheck( this, AbayResult.Succesful );
					GiveItemTo( HighestBid.Mobile, item );

					AbayCheck gold = new AbayGoldCheck( this, AbayResult.Succesful );
					GiveItemTo( m_Owner, gold );

					// Over, this Abay no longer exists
					AbayLog.WriteEnd( this, AbayResult.Succesful, m, null );
				}
				else
				{
					// Reserve hasn't been met or Abay isn't valid, this Abay is pending
					m_Pending = true;
					m_PendingEnd = DateTime.UtcNow + TimeSpan.FromDays( AbayConfig.DaysForConfirmation );
					AbaySystem.Pending.Add( this );

					DoOwnerMessage();
					DoBuyerMessage();

					Mobile owner = GetOnlineMobile( m_Owner );
					Mobile buyer = GetOnlineMobile( HighestBid.Mobile );

					SendMessage( owner );
					SendMessage( buyer );

					AbayScheduler.UpdateDeadline( m_PendingEnd );

					AbayLog.WritePending( this, ReserveMet ? "Item deleted" : "Reserve not met" );
				}
			}
		}

		/// <summary>
		/// Gets the online mobile belonging to a mobile's account
		/// </summary>
		private Mobile GetOnlineMobile( Mobile m )
		{
			if ( m == null || m.Account == null )
				return null;

			if ( m.NetState != null )
				return m;

			Account acc = m.Account as Account;

			for ( int i = 0; i < 5; i++ )
			{
				Mobile mob = acc[ i ];

				if ( mob != null && mob.NetState != null )
				{
					return mob;
				}
			}

			return null;
		}

		/// <summary>
		/// Ends the Abay.
		/// This function is called when the system is being disbanded and all Abays must be forced close
		/// The item will be returned to the original owner, and the highest bidder will receive the money back
		/// </summary>
		public void ForceEnd()
		{
			AbaySystem.Abays.Remove( this );

			// Turn the item into a deed and give it to the Abay owner
			AbayCheck item = new AbayItemCheck( this, AbayResult.SystemStopped );

			if ( item != null )
				GiveItemTo( m_Owner, item ); // This in case the item has been wiped or whatever

			if ( HighestBid != null )
			{
				HighestBid.AbayCanceled( this );
			}

			AbayLog.WriteEnd( this, AbayResult.SystemStopped, null, null );
		}

		/// <summary>
		/// This function will put an item in a player's backpack, and if full put it inside their bank.
		/// If the mobile is null, this will delete the item.
		/// </summary>
		/// <param name="m">The mobile receiving the item</param>
		/// <param name="item">The item being given</param>
		private static void GiveItemTo( Mobile m, Item item )
		{
			if ( m == null || item == null )
			{
				if ( item != null )
					item.Delete();

				return;
			}

			if ( m.Backpack == null || !m.Backpack.TryDropItem( m, item, false ) )
			{
				if ( m.BankBox != null )
				{
					m.BankBox.AddItem( item );
				}
				else
				{
					item.Delete(); // Sucks to be you
				}
			}
		}

		/// <summary>
		/// Verifies if all the items being sold through this Abay still exist
		/// </summary>
		/// <returns>True if all the items still exist</returns>
		public bool IsValid()
		{
			bool valid = true;

			foreach( ItemInfo info in m_Items )
			{
				if ( info.Item == null )
					valid = false;
			}

			return valid;
		}

		/// <summary>
		/// Defines what kind of message the Abay owner should receive. Doesn't send any messages.
		/// </summary>
		public void DoOwnerMessage()
		{
			if ( m_Owner == null || m_Owner.Account == null )
			{
				// If owner deleted the character, accept the Abay by default
				m_OwnerPendency = AbayPendency.Accepted;
			}
			else if ( !IsValid() && ReserveMet )
			{
				// Assume the owner will sell even if invalid when reserve is met
				m_OwnerPendency = AbayPendency.Accepted;
			}
			else if ( !ReserveMet )
			{
				m_OwnerPendency = AbayPendency.Pending;
				m_OwnerMessage = AbayMessage.Response; // This is always reserve not met for the owner
			}
			else if ( !IsValid() )
			{
				m_OwnerPendency = AbayPendency.Accepted;
				m_OwnerMessage = AbayMessage.Information; // This is always about validty for the owner
			}
		}

		/// <summary>
		/// Defines what kind of message the buyer should receive. Doesn't send any messages.
		/// </summary>
		public void DoBuyerMessage()
		{
			if ( HighestBid.Mobile == null || HighestBid.Mobile.Account == null )
			{
				// Buyer deleted the character, accept the Abay by default
				m_BuyerPendency = AbayPendency.Accepted;
			}
			else if ( ! IsValid() )
			{
				// Send the buyer a message about missing items in the Abay
				m_BuyerMessage = AbayMessage.Response;
				m_BuyerPendency = AbayPendency.Pending;
			}
			else if ( !ReserveMet )
			{
				// Assume the buyer will buy even if the reserve hasn't been met
				m_BuyerPendency = AbayPendency.Accepted;
				// Send the buyer a message to inform them of the reserve issue
				m_BuyerMessage = AbayMessage.Information;
			}
		}

		/// <summary>
		/// Validates the pending status of the Abay. This method should be called whenever a pendency
		/// value is changed. If the Abay has been validated, it will finalize items and remove the Abay from the system.
		/// This is the only method that should be used to finalize a pending Abay.
		/// </summary>
		public void Validate()
		{
			if ( ! AbaySystem.Pending.Contains( this ) )
				return;

			if ( m_OwnerPendency == AbayPendency.Accepted && m_BuyerPendency == AbayPendency.Accepted )
			{
				// Both parts confirmed the Abay
				m_Pending = false;
				AbaySystem.Pending.Remove( this );

				AbayCheck item = new AbayItemCheck( this, AbayResult.PendingAccepted );
				AbayCheck gold = new AbayGoldCheck( this, AbayResult.PendingAccepted );

				if ( item != null )
				{
					GiveItemTo( HighestBid.Mobile, item ); // Item to buyer
				}

				GiveItemTo( m_Owner, gold ); // Gold to owner

				// Over, this Abay no longer exists
				AbayLog.WriteEnd( this, AbayResult.PendingAccepted, null, null );
			}
			else if ( m_OwnerPendency == AbayPendency.NotAccepted || m_BuyerPendency == AbayPendency.NotAccepted )
			{
				// At least one part refused
				m_Pending = false;
				AbaySystem.Pending.Remove( this );

				AbayCheck item = new AbayItemCheck( this, AbayResult.PendingRefused );
				AbayCheck gold = new AbayGoldCheck( this, AbayResult.PendingRefused );

				if ( item != null )
				{
					GiveItemTo( m_Owner, item ); // Give item back to owner
				}

				GiveItemTo( HighestBid.Mobile, gold ); // Give gold to highest bidder

				// Over, this Abay no longer exists
				AbayLog.WriteEnd( this, AbayResult.PendingRefused, null, null );
			}
		}

		/// <summary>
		/// Sends any message this Abay might have in store for a given mobile
		/// </summary>
		/// <param name="to">The Mobile logging into the server</param>
		public void SendMessage( Mobile to )
		{
			if ( ! m_Pending || to == null )
				return;

			if ( to == m_Owner || ( m_Owner != null && to.Account.Equals( m_Owner.Account ) ) )
			{
				// This is the owner loggin in
				if ( this.m_OwnerMessage != AbayMessage.None )
				{
					// Owner needs a message
					if ( m_OwnerMessage == AbayMessage.Information )
					{
						// Send information message about validity condition
						AbayMessaging.SendInvalidMessageToOwner( this );
					}
					else if ( m_OwnerMessage == AbayMessage.Response )
					{
						// Send reserve not met confirmation request
						AbayMessaging.SendReserveMessageToOwner( this );
					}
				}
			}
			else if ( to == HighestBid.Mobile || ( HighestBid.Mobile != null && to.Account.Equals( HighestBid.Mobile.Account ) ) )
			{
				// This is the buyer logging in
				if ( m_BuyerMessage != AbayMessage.None )
				{
					// Buyer should receive a message
					if ( m_BuyerMessage == AbayMessage.Information )
					{
						// Send message about reserve not met condition
						AbayMessaging.SendReserveMessageToBuyer( this );
					}
					else if ( m_BuyerMessage == AbayMessage.Response )
					{
						// Send request to confirm invalid items Abay
						AbayMessaging.SendInvalidMessageToBuyer( this );
					}
				}
			}
		}

		/// <summary>
		/// Confirms an information message
		/// </summary>
		/// <param name="owner">True if the message was sent to the owner, false if to the buyer</param>
		public void ConfirmInformationMessage( bool owner )
		{
			if ( owner )
			{
				// Owner
				m_OwnerMessage = AbayMessage.None; // Don't resent
			}
			else
			{
				// Buyer
				m_BuyerMessage = AbayMessage.None;
			}
		}

		/// <summary>
		/// Gives a response to a message
		/// </summary>
		/// <param name="owner">True if the message was sent to the owner, false if to the buyer</param>
		/// <param name="ok">The response to the message</param>
		public void ConfirmResponseMessage( bool owner, bool ok )
		{
			if ( owner )
			{
				if ( ok )
				{
					m_OwnerPendency = AbayPendency.Accepted;
				}
				else
				{
					m_OwnerPendency = AbayPendency.NotAccepted;
				}
			}
			else
			{
				if ( ok )
				{
					m_BuyerPendency = AbayPendency.Accepted;
				}
				else
				{
					m_BuyerPendency = AbayPendency.NotAccepted;
				}
			}

			Validate();
		}

		/// <summary>
		/// The pending period has timed out and the Abay must end unsuccesfully
		/// </summary>
		public void PendingTimeOut()
		{
			AbaySystem.Pending.Remove( this );

			m_OwnerPendency = AbayPendency.NotAccepted;
			m_BuyerPendency = AbayPendency.NotAccepted;
			m_OwnerMessage = AbayMessage.None;
			m_BuyerMessage = AbayMessage.None;

			AbayCheck item = new AbayItemCheck( this, AbayResult.PendingTimedOut );
			AbayCheck gold = new AbayGoldCheck( this, AbayResult.PendingTimedOut );

			if ( item != null )
				GiveItemTo( m_Owner, item );
			GiveItemTo( HighestBid.Mobile, gold );

			// Over, this Abay no longer exists
			AbayLog.WriteEnd( this, AbayResult.PendingTimedOut, null, null );
		}

		/// <summary>
		/// Verifies is a mobile has bid on this Abay
		/// </summary>
		public bool MobileHasBids( Mobile m )
		{
			ArrayList bids = new ArrayList( m_Bids );

			foreach( Bid bid in bids )
			{
				if ( bid.Mobile == m )
					return true;
			}

			return false;
		}

		/// <summary>
		/// Outputs relevant information about this Abay
		/// </summary>
		public void Profile( StreamWriter writer )
		{
			writer.WriteLine( "ID : {0}", m_ID );
			writer.WriteLine( "Name : {0}", m_ItemName );

			if ( m_Owner != null && m_Owner.Account != null )
				writer.WriteLine( "Owner : {0} [ Account {1} - Serial {2} ]",
					m_Owner.Name,
					( m_Owner.Account as Account ).Username,
					m_Owner.Serial );
			else
				writer.WriteLine( "Owner : no longer existing" );

			writer.WriteLine( "Starting bid: {0}", m_MinBid );
			writer.WriteLine( "Reserve : {0}", m_Reserve );

			writer.WriteLine( "Created on {0} at {1}", m_StartTime.ToShortDateString(), m_StartTime.ToShortTimeString() );
			writer.WriteLine( "Duration: {0}", m_Duration.ToString() );
			writer.WriteLine( "End Time: {0} at {1}", m_EndTime.ToShortDateString(), m_EndTime.ToShortTimeString() );

			writer.WriteLine( "Expired : {0}", Expired.ToString() );
			writer.WriteLine( "Pending : {0}", Pending.ToString() );
			writer.WriteLine( "Next Deadline : {0} at {1}", Deadline.ToShortDateString(), Deadline.ToShortTimeString() );

			writer.WriteLine();

			if ( Creature )
			{
				writer.WriteLine( "** This Abay is selling a pet" );

				// Pet
				if ( m_Item != null && Pet != null )
				{
					writer.WriteLine( "Creature: {0}", Pet.Serial );
					writer.WriteLine( "Statuette : {0}", m_Item.Serial );
					writer.WriteLine( "Type : {0}", m_Item.Name );
				}
				else
				{
					writer.WriteLine( "Pet deleted, this Abay is invalid" );
				}
			}
			else
			{
				// Items
				writer.WriteLine( "{0} Items", m_Items.Length );

				foreach( ItemInfo item in m_Items )
				{
					writer.Write( "- {0}", item.Name );

					if ( item.Item != null )
						writer.WriteLine( " [{0}]", item.Item.Serial );
					else
						writer.WriteLine( " [Deleted]" );
				}
			}

			writer.WriteLine();
			writer.WriteLine( "{0} Bids", m_Bids.Count );

			foreach ( Bid bid in m_Bids )
			{
				bid.Profile( writer );
			}

			writer.WriteLine();
		}

		/// <summary>
		/// Attempts to buy now
		/// </summary>
		/// <param name="m">The user trying to purchase</param>
		/// <returns>True if the item has been sold</returns>
		public bool DoBuyNow( Mobile m )
		{
			if ( !Banker.Withdraw( m, m_BuyNow ) )
			{
				m.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 211 ] );
				return false;
			}

			AbaySystem.Abays.Remove( this );

			if ( HighestBid != null )
			{
				HighestBid.Outbid( this );
			}

			// Simulate bid
			Bid bid = new Bid( m, BuyNow );
			m_Bids.Insert( 0, bid );

			AbayGoldCheck gold = new AbayGoldCheck( this, AbayResult.BuyNow );
			AbayItemCheck item = new AbayItemCheck( this, AbayResult.BuyNow );

			GiveItemTo( m, item );
			GiveItemTo( m_Owner, gold );

			// Over.
			AbayLog.WriteEnd( this, AbayResult.BuyNow, m, null );

			return true;
		}

		/// <summary>
		/// Verifies if the eventual pets in this Abay are gone
		/// </summary>
		public void VeirfyIntergrity()
		{
			foreach( ItemInfo ii in this.m_Items )
				ii.VeirfyIntegrity();
		}
	}
}