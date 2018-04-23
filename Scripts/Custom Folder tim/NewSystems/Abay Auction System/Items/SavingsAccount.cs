#region AuthorHeader
//
//	Savings Account version 2.0 - Abay version 2.1, by Xanthos
//	based on a concept by Phoo
//
//
#endregion AuthorHeader
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Server;
using Server.Misc;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;
using Arya.Abay;
using Server.Commands;

namespace Arya.Savings
{
	public enum CurrencyType { Gold, Tokens, }

	public class SavingsAccount : Item
	{
		private const int kTwoBillion = 2000000000;
		private long m_Gold;
		private long m_Tokens;
		private static DateTime DateLastPaid;

		public enum SavingsOption
		{
			Both,
			Gold,
			Tokens,
		}

		public static void Initialize()
		{
			// alter AccessLevel to be AccessLevel.Admin if you only want admins to use.
			CommandHandlers.Register( "InstallSavings", AccessLevel.GameMaster, new CommandEventHandler( InstallSavings_OnCommand ) );
			CommandHandlers.Register( "ScaleSavings", AccessLevel.GameMaster, new CommandEventHandler( ScaleSavings_OnCommand ) );

			EventSink.WorldSave += new WorldSaveEventHandler( World_Save );
		}

		private static void World_Save( WorldSaveEventArgs args )
		{
			DateTime now = DateTime.UtcNow;

			if ( AbayConfig.InterestHour <= now.Hour && SavingsAccount.DateLastPaid != now.Date )
			{
				ArrayList accounts = new ArrayList();

				foreach ( Item item in World.Items.Values )
				{
					if ( item is SavingsAccount && item.Parent is BankBox )
						accounts.Add( item );
				}

				foreach ( Item item in accounts )
				{
					SavingsAccount account = item as SavingsAccount;

					account.Gold += (long)( AbayConfig.GoldInterestRate * account.Gold );
					if ( Arya.Savings.SavingsAccount.EnableTokens )
						account.Tokens += (long)( AbayConfig.TokensInterestRate * account.Tokens );

					BankBox box = account.Parent as BankBox;

					if ( null != box && null != box.Owner && box.Owner.Map != Map.Internal )
						box.Owner.SendMessage( Xanthos.Utilities.AbayLabelHue.kRedHue, "Interest has been paid on the {0} in your savings account!", AbayConfig.EnableTokens ? "gold and tokens" : "gold" );
				}
				SavingsAccount.DateLastPaid = now.Date;
			}
		}


		[CommandProperty( AccessLevel.Administrator )]
		public long Gold
		{
			get { return m_Gold; }
			set { m_Gold = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.Administrator )]
		public long Tokens
		{
			get { return m_Tokens; }
			set { m_Tokens = value; InvalidateProperties(); }
		}

		public static bool EnableTokens
		{
			get { return AbayConfig.EnableTokens && AbayConfig.TokenType != null; }
		}

		[Constructable]
		public SavingsAccount() : base( 0xe80 )
		{
			Name = "Savings Account";
			Weight = 1.0;
			Movable = false;
		}

		public SavingsAccount( Serial serial ) : base( serial )
		{
		}

		public override bool DisplayLootType{ get{ return false; }}

		public static int BalanceGold( Mobile mobile )
		{
			SavingsAccount account = GetAccountForMobile( mobile );

			if ( null != account )
				return ( account.Gold > kTwoBillion ? kTwoBillion : (int)(account.Gold) );
			else
				return 0;
		}

		public static long BalanceTokens( Mobile mobile )
		{
			SavingsAccount account = GetAccountForMobile( mobile );

			if ( null != account )
				return account.Tokens;
			else
				return 0;
		}

		public static bool WithdrawGold( Mobile mobile, int amount )
		{
			SavingsAccount account = GetAccountForMobile( mobile );

			if ( null != account )
			{
				bool result = account.Withdraw( CurrencyType.Gold, amount );
				if ( mobile.HasGump( typeof(SavingsGump) ))
				{
					mobile.SendGump( new SavingsGump( mobile ));
				}
				return result;
			}
			else
				return false;
		}

		public static bool DepositGold( Mobile mobile, int amount )
		{
			SavingsAccount account = GetAccountForMobile( mobile );

			if ( null != account )
			{
				bool result = account.Deposit( CurrencyType.Gold, amount );
				if ( mobile.HasGump( typeof(SavingsGump) ) )
				{
					mobile.SendGump( new SavingsGump( mobile ) );
				}
				return result;
			}
			return false;
		}

		public bool Withdraw( CurrencyType currency, int amount )
		{
			if ( amount <= ( CurrencyType.Gold == currency ? Gold : Tokens ))
			{
				if ( CurrencyType.Gold == currency )
					Gold -= amount;
				else
					Tokens -= amount;

				return true;
			}
			
			return false;
		}

		public bool Deposit( CurrencyType currency, int amount )
		{
			if ( CurrencyType.Gold == currency )
				Gold += amount;
			else
				Tokens += amount;

			return true;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( Parent is BankBox )
				from.SendGump( new SavingsGump( from ) );
			
			else if ( null != GetAccountForMobile( from ) )
				from.SendMessage( "You already have a savings account." );

			else if ( GiveSavingsAccount( from ) && Movable )
				Delete();
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( null == from )
				return false;

			long gold = Gold;
			long tokens = Tokens;
			string str = null;
			bool isContainer = false;

			bool success = DepositItem( dropped, out isContainer );

			gold = Gold - gold;
			tokens = Tokens - tokens;

			if ( gold > 0 && tokens > 0 )
				str = gold.ToString( "#,0" ) + " gold and " + tokens.ToString( "#,0" ) + " tokens";
			else if ( gold > 0 )
				str = gold.ToString( "#,0" ) + " gold";
			else if ( tokens > 0 )
				str = tokens.ToString( "#,0" ) + " tokens";

			if ( null != str )
				from.SendMessage( "You have deposited {0} into savings.", str );

			if ( !success )
			{
				string text = Arya.Savings.SavingsAccount.EnableTokens ? ", tokens " : " ";
				from.SendMessage( "Only gold{0}and checks (single or in containers) are accepted.", text );
			}

			if ( from.HasGump( typeof(SavingsGump) ) )
			{
				from.SendGump( new SavingsGump( from ) );
			}

			if ( isContainer )
				return false;
			else
				return success;
		}

		private bool DepositItem( Item forDeposit, out bool isContainer )
		{
			bool result = true;
			isContainer = false;

			if ( forDeposit is Gold )
			{
				Gold += ( forDeposit as Gold ).Amount;
				forDeposit.Delete();
			}
			else if ( forDeposit is BankCheck )
			{	
				Gold += ( forDeposit as BankCheck ).Worth;
				forDeposit.Delete();
			}
			else if ( EnableTokens && forDeposit.GetType() == AbayConfig.TokenType )
			{
				int deposit = 0;
				
				try
				{
					Type t = forDeposit.GetType();
					PropertyInfo prop = t.GetProperty( "Amount" );
					deposit = (int)( prop.GetValue( forDeposit, null ) );
				}
				catch { }

				Tokens += deposit > 0 ? deposit : 0;
				forDeposit.Delete();
			}
			else if ( EnableTokens && forDeposit.GetType() == AbayConfig.TokenCheckType )
			{
				int deposit = 0;

				try
				{
					Type t = forDeposit.GetType();
					PropertyInfo prop = t.GetProperty( "tokensAmount" );
					deposit = (int)( prop.GetValue( forDeposit, null ) );
				}
				catch { }

				Tokens += deposit > 0 ? deposit : 0;
				forDeposit.Delete();
			}
			else if ( forDeposit is Container )
			{
				Container container = forDeposit as Container;

				if ( null != container.Items )
				{
					List<Server.Item> list = container.Items;

					for ( int i = list.Count - 1; i >= 0; i-- )
					{
						if ( list[i] is Item && !DepositItem( (Item)list[i], out isContainer ))
							result = false;
					}
				}
				isContainer = true;
			}
			else
				result = false;

			return result;
		}

		public static SavingsAccount GetAccountForMobile( Mobile mobile )
		{
			BankBox bank = mobile.BankBox;

			if ( null == bank )
				return null;
			else
				return (SavingsAccount)bank.FindItemByType( typeof( SavingsAccount ));
		}

		private static void InstallSavings_OnCommand( CommandEventArgs e )
		{
			InstallSavingsAccounts( e.Mobile );
		}

		[Usage( "ScaleSavings [-g|-t] <IntegerPercent>" )]
		[Description( "Scale every players savings account to a percentage of its current value." )]
		private static void ScaleSavings_OnCommand( CommandEventArgs e )
		{
			SavingsOption option = SavingsOption.Both;
			int percent = 100;

			if ( 1 <= e.Length )
			{
				string str = e.GetString( 0 ).ToLower();

				if ( str.Equals( "-g" ) )
					option = SavingsOption.Gold;
				else if ( str.Equals( "-t" ) && EnableTokens )
					option = SavingsOption.Tokens;
				try
				{
					percent = Math.Abs( int.Parse( e.GetString( e.Length - 1 ) ) );
				}
				catch ( Exception exc )
				{
					World.Broadcast( 59, true, "Error - {0}", exc.Message );
					percent = -1;
				}
			}

			if ( percent >= 0 )
				ScaleSavingsAccounts( e.Mobile, option, ((double)percent) / 100.00 );
		}

		private static void ScaleSavingsAccounts( Mobile from, SavingsOption option, double percent )
		{
			ArrayList items = new ArrayList( World.Items.Values );
			int accountsScaled = 0;
			string optionText = option == SavingsOption.Both ? "gold & tokens" : option == SavingsOption.Gold ? "gold" : "tokens";

			foreach ( Item i in items )
			{
				Arya.Savings.SavingsAccount sa = i as Arya.Savings.SavingsAccount;

				if ( null != sa )
				{
					accountsScaled++;

					if ( option == SavingsOption.Both || option == SavingsOption.Gold )
						sa.Gold = (int)(((double)sa.Gold) * percent);
					if ( option == SavingsOption.Both || option == SavingsOption.Tokens )
						sa.Tokens = (int)(((double)sa.Tokens) * percent);
				}
			}
			World.Broadcast( 59, true, "Scaling complete, {0} accounts ({1}) scaled by {2}%", accountsScaled, optionText, percent * 100 ); 
		}

		public static void InstallSavingsAccounts( Mobile from )
		{
			ArrayList mobs = new ArrayList( World.Mobiles.Values );
			ArrayList failed = new ArrayList();
			int i = 0;

			foreach ( Mobile m in mobs )
			{
				if ( m is PlayerMobile )
				{
					if ( !GiveSavingsAccount( m ) )
					{
						failed.Add( m.Name );
					}
					else
						i++;
				}
			}

			if ( i > 0 )
				from.SendMessage( "{0} Players received savings accounts.", i );

			if ( failed.Count > 0 )
			{
				from.SendMessage( "The following players {0} could not receive savings accounts:", failed.Count );
				foreach ( String name in failed )
				{
					if ( null != name )
						from.SendMessage( name );
				}
			}
		}
		
		public static bool GiveSavingsAccount( Mobile mobile )
		{
			BankBox box = mobile.BankBox;

			if ( null == box || null != GetAccountForMobile( mobile ))
				return false;

			if ( box.TryDropItem( mobile, new SavingsAccount(), true ))
			{
				mobile.SendMessage( "A savings account has been placed in your bank box." );
				return true;
			}
			else
				return false;
		}
		
		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			
			list.Add( 1060658, "Gold\t{0}", Gold.ToString( "#,0" ) ); // ~1_val~: ~2_val~
			if ( EnableTokens )
				list.Add( 1060659, "Tokens\t{0}", Tokens.ToString( "#,0" ) ); // ~1_val~: ~2_val~
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version

			// version 1
			writer.Write( (DateTime)DateLastPaid );

			// version 0
			writer.Write( (long)Gold );
			writer.Write( (long)Tokens );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
					Gold = reader.ReadLong();
					Tokens = reader.ReadLong();
					break;
				case 1:
					DateLastPaid = reader.ReadDateTime();
					goto case 0;
			}
		}
	}


	public class SavingsGump : Gump
	{
		private SavingsAccount m_Account;
		private BankBox m_Bank;
		private Container m_Pack;
		private int m_AmountID = 0;
		private string m_MaximumCheckValue;

		private static Type[] s_ArgTypes = new Type[]
		{
			typeof( Int32 ),
		};

		public SavingsGump( Mobile from ) : base( 400, 300 )
		{
			from.CloseGump( typeof( SavingsGump ));

			m_Bank = from.BankBox;
			m_Pack = from.Backpack;
			m_Account = (SavingsAccount)m_Bank.FindItemByType( typeof( SavingsAccount ));

			String gold = m_Account.Gold.ToString( "#,0" ) + " Gold";
			String tokens = m_Account.Tokens.ToString( "#,0" ) + " Tokens";

			m_MaximumCheckValue = AbayConfig.MaximumCheckValue.ToString( "#,0" );

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			AddPage( 0 );
			AddBackground( 0, 0, 240, 268, 9200 );
			AddAlphaRegion( 10, 9, 220, 100 );
			AddAlphaRegion( 10, 115, 220, 145 );

			AddLabel( 50, 15, 4, @"Savings Account Ledger" );
			AddLabel( 50, 35, 4, @"-----------------------" );
			AddLabel( 25, 55, 53, gold );
			if ( Arya.Savings.SavingsAccount.EnableTokens )
				AddLabel( 25, 80, 18, tokens );

			AddLabel( 40, 130, 4, @"Withdraw 1 to " + m_MaximumCheckValue );
			AddBackground( 75, 155, 95, 25, 0x2486 );
			AddTextEntry( 85, 155, 90, 20, 0, m_AmountID, "" );

			AddButton( 25, 180, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddLabel( 65, 180, 53, @"Make this gold check." );

			if ( Arya.Savings.SavingsAccount.EnableTokens )
			{
				AddButton( 25, 205, 4005, 4007, 2, GumpButtonType.Reply, 0 );
				AddLabel( 65, 205, 18, @"Make this token check." );
			}	
			AddButton( 25, 230, 4017, 4019, 0, GumpButtonType.Reply, 0 );
			AddLabel( 65, 230, 4, @"Done." );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			TextRelay relay = info.GetTextEntry( m_AmountID );
			string amountText = ( relay == null ? null : relay.Text.Trim() );
			int checkAmount;

			switch ( info.ButtonID )
			{
				case 1: //Gold
					if (( checkAmount = GetCheckAmount( from, CurrencyType.Gold, amountText )) > 0 )
					{
						IssueCheck( from, CurrencyType.Gold, checkAmount );
					}
					from.SendGump( new SavingsGump( from ));
					break;
				case 2: //Tokens
					if (( checkAmount = GetCheckAmount( from, CurrencyType.Tokens, amountText )) > 0 )
					{
						IssueCheck( from, CurrencyType.Tokens, checkAmount );
					}
					from.SendGump( new SavingsGump( from ) );
					break;
				default:
					break;
			}
		}

		private int GetCheckAmount( Mobile from, CurrencyType currency, string amountText )
		{
			if( amountText == null || amountText.Length == 0 )
				from.SendMessage( "Enter an amount between 1 and " + m_MaximumCheckValue + " for a bank check." );
			else
			{
				try
				{
					int amount = Int32.Parse( amountText, NumberStyles.AllowThousands | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite );

					if ( amount >= 1 && amount <= AbayConfig.MaximumCheckValue )
						return amount;
					else
						from.SendMessage( "Enter an amount between 1 and " + m_MaximumCheckValue + " for a bank check." );
				}
				catch
				{
					from.SendMessage( "Enter an amount between 1 and " + m_MaximumCheckValue + " for a bank check." );
				}
			}
			return 0;
		}

		private void IssueCheck( Mobile from, CurrencyType currency, int amount )
		{
			string currencyString;
			Item check = null;

			if ( currency == CurrencyType.Gold )
			{
				currencyString = "gold";
				check = (Item)new BankCheck( amount );
			}
			else
			{
				currencyString = "tokens";
				try
				{
					ConstructorInfo ci = AbayConfig.TokenCheckType.GetConstructor( s_ArgTypes );

					if ( ci != null )
					{
						Object[] array = new Object[] { amount };
						Object obj = ci.Invoke( array );
						check = obj as Item;
					}
				}
				catch ( Exception exc )
				{
					from.SendMessage( "There was a problem creating the token check: {0}", exc.Message );
				}
			}

			if ( null != check )
			{
				if ( m_Account.Withdraw( currency, amount ) )
				{
					if ( m_Pack == null || !m_Pack.TryDropItem( from, check, false ) )
					{
						if ( m_Bank == null || !m_Bank.TryDropItem( from, check, false ) )
						{
							from.SendMessage( "There's not enough room in your bankbox or backpack for the check!" );
							check.Delete();
							m_Account.Deposit( currency, amount );
						}
						else
						{
							from.SendMessage( "You have withdrawn " + amount.ToString( "#,0" ) + " {0} from savings.", currencyString );
						}
					}
					else
					{
						from.SendMessage( "You have withdrawn " + amount.ToString( "#,0" ) + " {0} from savings.", currencyString );
					}
				}
				else
					from.SendMessage( "You have insufficient funds to withdraw " + amount.ToString( "#,0" ) + " {0} from savings!", currencyString );
			}
		}
	}
}