/*
Script Name: HiLoCards.cs
Author: CEO
Version: 1.0
Donations Accepted! Using this regularly on your shard? Feel free to drop me a few bucks via Paypal!
https://www.paypal.com/xclick/business=ceo%40easyuo%2ecom&item_name=CEO%20BlackJack%20Donation&no_shipping=0&no_note=

Purpose: Casino card game

Description: See ingame help button.
		
Installation:	Hilocards.cs into your custom scripts folder and restart.

			]add HiLoCards
 
Requirements: Also requires carddeck.cs object.
 
Payout Percentage for HiLoCards:

Using a 1000 gp bet and 1 million automated plays.
If card is a < 7 choice is higher, if card is > 7 
choice is lower. If card is 7 the choice is random.

Guess 2: 	81.08% 
Guess 3: 	80.52%
Guess 4: 	79.27%
Guess 5: 	80.34%
Guess 6: 	79.79%
Guess 7: 	81.88%
Guess 8: 	82.17%
Guess 9: 	78.80%
Guess 10:  	81.54%
 
*/
//Use this if do your own profiling if you adjust payout amounts.
//#define PROFILE
// If you're using an SVN that supports List<> methods (remove the //s) to use those instead of arrays or add them for RC1.
#define RC2
// If you have XMLSpawner installed this definition will give you a larger property gump
#define XMLSPAWNER
using System;
using Server.Accounting;
using Server.Network;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;
#if RC2
using System.Collections.Generic;
#endif
using System.Collections;

namespace Server.Items
{
	[DynamicFliping]
	[Flipable(4479, 4480)]
	public class HiLoCards : Item
	{
		private bool m_Active = true;
		private int m_ErrorCode;
		private int m_OrigHue;

		//Stats and Totals
		private int m_Won = 0;
		private bool m_ResetTotals;
		private ulong m_TotalCollected = 0;
		private ulong m_TotalWon = 0;
		private ulong m_TotalPlays = 0;
		private ulong m_TotalWins = 0;

		private int m_OnCredit;
		private int m_CurrentBet = 100;
		private int m_Bet = 0;
		private int[] m_BetTable = new int[] { 100, 250, 500, 1000, 2500, 5000, 10000, 25000 };
		private int[] m_HueTable = new int[] { 84, 85, 86, 87, 88, 89, 90, 91, 92};

		private Mobile m_SecurityCamMobile = null; // Set to a mobile to "watch" people playing
		private VerboseType m_SecurityChatter = VerboseType.Low;
		public enum VerboseType { Low, Medium, High }

		private int m_GuessCount = 4;
		private CardDeck carddeck;
		private int m_Count = 0;
		private int m_Progress = 0;
		private int[] m_Hand = new int[HANDSIZE] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
		private const int HANDSIZE = 11;
		public double[] m_WinMult = new double[9] { 1.5, 2.25, 3.25, 4.75, 6.80, 10.0, 14.5, 20.0, 30.0};

		//Mobile & timeout
		private Mobile m_InUseBy = null;
		private DateTime m_LastPlayed = DateTime.Now;
		private TimeSpan m_TimeOut;
		private TimeSpan m_IdleTimer = TimeSpan.FromMinutes(5); // How long can a person be standing at the machine not playing?

		//Last Win info
		private Mobile m_LastWonBy = null;
		private DateTime m_LastWonByDate = DateTime.Now;
		private int m_LastWonAmount = 0;

		//ATM Stuff
		private int m_CreditCashOut = 750000;
		private int m_CreditATMLimit = 500000;
		private int m_CreditATMIncrements = 10000;

		//Config type stuff
		private bool m_FixedBets = false;
		private bool m_FixedGuesses = false;
		private bool m_HelpGump = false;
		private bool m_TestMode = false; // Make the game essentially free (no payouts!).

		public bool HelpGump
		{
			get { return m_HelpGump; }
			set
			{
				m_HelpGump = value;
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public bool TestMode
		{
			get { return m_TestMode; }
			set
			{
				if (m_InUseBy != null)
				{
					this.PublicOverheadMessage(0, (this.Hue == 907 ? 0 : this.Hue), false, "Can not alter Test Mode while in use.");
					return;
				}
				m_TestMode = value;
				if (!m_TestMode)
				{
					m_OnCredit = 0;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active
		{
			get { return m_Active; }
			set
			{
				if (!m_Active && value)
				{
					if (m_OrigHue != -1)
					{
						this.Hue = m_OrigHue;
						m_OrigHue = -1;
					}
					Effects.SendLocationEffect(new Point3D(this.X, this.Y + 1, this.Z), this.Map, 0x373A, 15, this.Hue - 1, 0);
					Effects.SendLocationEffect(new Point3D(this.X + 1, this.Y, this.Z), this.Map, 0x373A, 15, this.Hue - 1, 0);
					Effects.SendLocationEffect(new Point3D(this.X, this.Y, this.Z - 1), this.Map, 0x373A, 15, this.Hue - 1, 0);
					Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, 1481);
					this.PublicOverheadMessage(0, (this.Hue == 907 ? 0 : this.Hue), false, "CEOHiLo online!");
				}
				else if (m_Active && !value)
				{
					m_OrigHue = this.Hue;
					this.Hue = 1001;
					this.PublicOverheadMessage(0, this.Hue, false, "CEOHiLo offline.");
				}
				m_Active = value;
				InvalidateProperties();
			}
		}

		private void HiLoOffline(int error)
		{
			if (m_InUseBy != null)
			{
				m_InUseBy.SendMessage("A critical error has forced this game offline, notify a GameMaster please.");
				m_InUseBy = null;
			}
				string text = String.Format("Critical Error: {0}", error);
			SecurityCamera(0, text);
			m_ErrorCode = error;
			Active = false;
		
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile SecurityCamMob
		{
			get { return m_SecurityCamMobile; }
			set { m_SecurityCamMobile = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public VerboseType SecurityChatter
		{
			get { return m_SecurityChatter; }
			set { m_SecurityChatter = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Won
		{
			get { return m_Won; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ResetStats
		{
			get { return m_ResetTotals; }
			set
			{
				if (value)
				{
					m_TotalWon = 0;
					m_TotalCollected = 0;
					m_TotalPlays = 0;
					m_TotalWins = 0;
					m_LastWonBy = null;
					m_LastWonByDate = DateTime.Now;
					m_LastWonAmount = 0;
					InvalidateProperties();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int CurrentBet
		{
			get { return m_CurrentBet; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ulong TotalPlays
		{
			get { return m_TotalPlays; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ulong TotalWins
		{
			get { return m_TotalWins; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public long TotalNetProfit
		{
			get { return (long)(m_TotalCollected - m_TotalWon); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ulong TotalCollected
		{
			get { return m_TotalCollected; }
#if PROFILE
            set { m_TotalCollected = value; }
#endif
		}

#if PROFILE
		[CommandProperty(AccessLevel.Administrator)]
		public bool ProfileGame
		{
			get { return false; }
			set
			{
				if (value && m_InUseBy != null)
				{
					for (int x = 0; x < 100000; x++)
					{
						m_TotalPlays++;
						m_CurrentBet = m_BetTable[m_Bet];
						m_TotalCollected += (ulong)m_CurrentBet;
						m_Progress = 0;
						carddeck.QuickShuffle();
						m_Hand = new int[HANDSIZE] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
						m_Hand[0] = carddeck.GetOneCard();
						m_Count = 1;
						for (int y = 0; y < m_GuessCount + 1; y++)
						{
							if (m_Progress == 100 || m_Progress == -1)
								break;
							else
							{

								int card = m_Hand[y] % 13 == 0 ? 14 : m_Hand[y] % 13;
								if (card < 7)
									Higher(m_InUseBy);
								else if (card > 7)
									Lower(m_InUseBy);
								else if (Utility.Random(2) == 0)
									Higher(m_InUseBy);
								else
									Lower(m_InUseBy);
							}
						}
					}
					m_OnCredit = 0;
					m_InUseBy.CloseGump(typeof(HiloCardGump));
					m_InUseBy.SendGump(new HiloCardGump(m_InUseBy, this, carddeck.BackDesign, new int[3] { -1, -1, -1 } , -2, false, null));
				}
			}
		}
#endif

		[CommandProperty(AccessLevel.GameMaster)]
		public ulong TotalWon
		{
			get { return m_TotalWon; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public float WinningPercentage
		{
			get
			{
				if (m_TotalWon == 0 || m_TotalCollected == 0)
					return 0;
				if (m_TotalCollected == 0)
					return (float)0;
				return ((float)(m_TotalWon / (float)m_TotalCollected) * 100.00f);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int GuessCount
		{
			get { return m_GuessCount; }
			set
			{
				m_GuessCount = value;
				if (m_GuessCount < 2)
					m_GuessCount = 2;
				if (m_GuessCount > 10)
					m_GuessCount = 10;
				m_Bet = AdjustBetToMax(m_BetTable[m_Bet], m_Bet, m_GuessCount);
				m_CurrentBet = m_BetTable[m_Bet];
#if PROFILE
				m_TotalWon = 0;
				m_TotalCollected = 0;
				m_TotalPlays = 0;
				m_TotalWins = 0;
#endif
				Hue = m_HueTable[m_GuessCount - 2];
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int CreditCashOutAt
		{
			get { return m_CreditCashOut; }
			set { m_CreditCashOut = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int CreditATMLimit
		{
			get { return m_CreditATMLimit; }
			set { m_CreditATMLimit = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int CreditATMIncrements
		{
			get { return m_CreditATMIncrements; }
			set { m_CreditATMIncrements = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile InUseBy
		{
			get { return m_InUseBy; }
			set { m_InUseBy = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ErrorCode
		{
			get { return m_ErrorCode; }
			set { m_ErrorCode = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool FixedBets
		{
			get { return m_FixedBets; }
			set { m_FixedBets = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool FixedGuesses
		{
			get { return m_FixedGuesses; }
			set { m_FixedGuesses = value; }
		}

		[Constructable]
		public HiLoCards()
			: base(4479)
		{
			Movable = false;
			Name = "HiLo Card Machine";
			carddeck = new CardDeck(1, 0);
			Hue = m_OrigHue = m_HueTable[4];
			Active = false;
		}

		public HiLoCards(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version 
			writer.Write(m_Active);
			writer.Write(m_TotalPlays);
			writer.Write(m_TotalCollected);
			writer.Write(m_TotalWon);
			writer.Write(m_ErrorCode);
			writer.Write(m_OrigHue);

			writer.Write(m_LastWonBy);
			writer.Write(m_LastWonByDate);
			writer.Write(m_LastWonAmount);

			writer.Write(m_SecurityCamMobile);
			writer.Write((int)m_SecurityChatter);

			writer.Write(m_GuessCount);
			writer.Write(m_Bet);
			writer.Write(m_FixedBets);
			writer.Write(m_FixedGuesses);
			writer.Write(m_TestMode);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			m_Active = reader.ReadBool();
			m_TotalPlays = reader.ReadULong();
			m_TotalCollected = reader.ReadULong();
			m_TotalWon = reader.ReadULong();
			m_ErrorCode = reader.ReadInt();
			m_OrigHue = reader.ReadInt();

			m_LastWonBy = reader.ReadMobile();
			m_LastWonByDate = reader.ReadDateTime();
			m_LastWonAmount = reader.ReadInt();

			m_SecurityCamMobile = reader.ReadMobile();
			m_SecurityChatter = (VerboseType)reader.ReadInt();

			m_GuessCount = reader.ReadInt();
			m_Bet = reader.ReadInt();
			m_FixedBets = reader.ReadBool();
			m_FixedGuesses = reader.ReadBool();
			m_TestMode = reader.ReadBool();

			carddeck = new CardDeck(1, 0);
			m_CurrentBet = m_BetTable[m_Bet];
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			int numberpos = 1060658;
			if (!m_Active)
			{
				if (m_ErrorCode == 0)
					list.Add(1060658, "Status\tOffline");
				else
					list.Add(1060658, "Status\tMaintenance Required({0})", m_ErrorCode);
				return;
			}
			else if (m_InUseBy == null)
			{
				list.Add(1060658, "Status\tAvailable");
				numberpos = 1060659;
			}
			else
			{
				list.Add(1060658, "Status\tIn Use");
				list.Add(1060659, "Player\t{0}", m_InUseBy.Name);
				numberpos = 1060660;
			}
			if (m_LastWonBy != null)
			{
				list.Add(numberpos, "Last Payout\t{0}", m_LastWonBy.Name);
				numberpos++;
				list.Add(numberpos, "Date\t{0}", m_LastWonByDate);
				numberpos++;
				list.Add(numberpos, "Amount\t{0}", m_LastWonAmount);
			}
		}

		public override bool HandlesOnMovement { get { return (m_InUseBy != null && m_Active); } }// Tell the core that we implement OnMovement

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (m_InUseBy != null)
			{
				if (!m_InUseBy.InRange(GetWorldLocation(), 3) || m_InUseBy.Map == Map.Internal)
				{
                    m_InUseBy.CloseGump(typeof(HiloCardGump));
					if (m_OnCredit != 0)
					{
						m_InUseBy.PlaySound(52);
						m_InUseBy.SendMessage("Hey, you left some cash in the machine! Cashing out.");
						DoCashOut(m_InUseBy); // Give them their winnings
					}
					else
						m_InUseBy.SendMessage("You walk away from this machine.");
					InUseBy = null;
					InvalidateProperties();
				}
			}
		}


		public override void OnDoubleClick(Mobile from)
		{
			if (!from.InRange(GetWorldLocation(), 2) && (from.AccessLevel >= AccessLevel.GameMaster))
			{
#if XMLSPAWNER
                from.SendGump(new XmlPropertiesGump(from, this));
#else
				from.SendGump(new PropertiesGump(from, this));
#endif
				return;
			}

			if (!from.InRange(GetWorldLocation(), 2) || !from.InLOS(this))
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}
			if (!m_Active)
			{
				if (m_InUseBy != null && (from == InUseBy))
				{
					from.CloseGump(typeof(HiloCardGump));
					if (m_OnCredit != 0)
						DoCashOut(from);
				}
				from.SendMessage("Sorry, this machine is currently down for maintenance.");
				return;
			}

			m_TimeOut = DateTime.Now - m_LastPlayed;

			if (m_InUseBy == null || m_InUseBy.Deleted)
				InUseBy = from;
			else
			{
				if (m_IdleTimer < m_TimeOut)
				{
                    string tempName = m_InUseBy != null ? m_InUseBy.Name : "Someone";
					if (m_InUseBy != null && m_InUseBy != from && m_OnCredit != 0)
						DoCashOut(m_InUseBy); // Previous user disconnected or something? Give them their cash before releasing.
                    from.SendMessage("{0} has left this machine idle too long, it is yours to play.", tempName);
					InUseBy = from;
				}
			}

			if (from == m_InUseBy)
			{
				from.CloseGump(typeof(HiloCardGump));
			}
			else
			{
				string text = String.Format("{0} is currently using this machine.", m_InUseBy.Name);
				from.SendMessage(text);
				return;
			}
			if (m_TestMode)
			{
				m_OnCredit = 100000;
			}
			m_CurrentBet = m_BetTable[m_Bet];
			m_Progress = 0;
			carddeck.QuickShuffle();
			m_Hand = new int[HANDSIZE] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 , -1};
			//m_Hand[0] = carddeck.GetOneCard();
			m_Count = 1;
			m_HelpGump = false;
			from.CloseGump(typeof(HiloCardGump));
			from.SendGump(new HiloCardGump(from, this, carddeck.BackDesign, Focus3(m_Hand, m_Count - 1, m_GuessCount), -2, false, null));
		}

		public void PlayHiLo(Mobile from)
		{
			PlayHiLo(from, m_CurrentBet);
		}

		public void PlayHiLo(Mobile from, int cost)
		{
			int amount = 0;
			if (!from.InRange(this.GetWorldLocation(), 10) || !from.InLOS(this))
			{
				from.SendMessage("You are too far away from CEOHiLo to play.");
				RemovePlayer(from);
				return;
			}
			if (m_TestMode)
			{}
			else if (from.Backpack.ConsumeTotal(typeof(CasinoToken), 1))
			{
				m_Bet = 0;
				m_CurrentBet = cost = m_BetTable[m_Bet];
				m_OnCredit += m_CurrentBet;
			}
			else if (from.Backpack.ConsumeTotal(typeof(Gold), cost))
			{
				m_OnCredit += cost;
			}
			else if (OnCredit(from, 0) >= cost)
			{
			}
			else if (CashCheck(from, out amount))
			{
				string message = string.Format("Cashing check for {0} from backpack.", amount);
				from.SendGump(new HiloCardGump(from, this, carddeck.BackDesign, new int[] { -1, -1, -1 }, -2, false, message));
				return;
			}
			else
			{
				from.SendGump(new HiloCardGump(from, this, carddeck.BackDesign, new int[] { -1, -1, -1 }, -2, false, "Insufficient funds to play!"));
				return;
			}
			OnCredit(from, -cost);
			m_Progress = m_Won = 0;
			if (carddeck.Remaining < 26 + Utility.Random(0))
				carddeck.QuickShuffle();
			m_Hand = new int[HANDSIZE] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 , -1};
			m_Hand[0] = carddeck.GetOneCard();
			m_Count = 1;
			m_TotalPlays++;
			
#if !PROFILE
			from.CloseGump(typeof(HiloCardGump));
			from.SendGump(new HiloCardGump(from, this, carddeck.BackDesign, Focus3(m_Hand, m_Count - 1, m_GuessCount), 0, true, null));
#endif
			from.PlaySound(739);
			if (from.Hidden && from.AccessLevel == AccessLevel.Player) // Don't let someone sit at the slots and play hidden
			{
				from.Hidden = false;
				from.SendLocalizedMessage(500816); // You have been revealed!
			}
		}

		public bool CashCheck(Mobile m, out int amount)
		{
			amount = 0;
			if (m == null)
			{
				m.SendMessage("This game needs maintenance.");
				SecurityCamera(0, "This game needs maintenance.");
				Active = false;
				return false;
			}
			if (m == null || m.Backpack == null || m_TestMode)
				return false;
#if RC2
			List<Item> packlist = m.Backpack.Items;
#else
			ArrayList packlist = m.Backpack.Items;
#endif

			for (int i = 0; i < packlist.Count; ++i)
			{
				Item item = (Item)packlist[i];
				if (item != null && !item.Deleted && item is BankCheck)
				{
					amount = ((BankCheck)item).Worth;
					item.Delete();
					if (item.Deleted)
					{
						string text = null;
						Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, 501);
						m_OnCredit += amount;
						text = String.Format("{0}:Check={1}.", m.Name, amount);
						SecurityCamera(amount > 5000 ? 0 : 1, text);
						text = String.Format("OnCredit={1}.", m.Name, m_OnCredit);
						SecurityCamera(m_OnCredit > 10000 ? 1 : 2, text);
					}
					else
					{
						m.SendMessage("There's a problem trying to cash a check in your backpack, this game is offline. Page for help.");
						HiLoOffline(9503);
						return false;
					}
					return true;
				}
			}
			return false;
		}

		public bool RemovePlayer(Mobile from)
		{
			if (from == null )
			{
				from.SendMessage("This game needs maintenance.");
				SecurityCamera(0, "This game needs maintenance.");
				Active = false;
				return false;
			}
			string text = String.Format("Removing: {0}.", from.Name);
			SecurityCamera(0, text);
			if (m_OnCredit != 0)
				DoCashOut(from);
			m_InUseBy = null;
			InvalidateProperties();
			return true;
		}

		public void DoCashOut(Mobile from)
		{
			if (m_TestMode)
			{
				m_OnCredit = 0;
				return;
			}
			if (from == null)
			{
				from.SendMessage("This game needs maintenance.");
				SecurityCamera(0, "This game needs maintenance.");
				Active = false;
				return;
			}
			int credit = OnCredit();
			if (from == null || credit == 0)
				return;
			if (!m_Active && (m_ErrorCode == 9500 || m_ErrorCode == 9501 || m_ErrorCode == 9502)) // Prevent a loop cashing out
				return;
			if (m_OnCredit < 0) // This should never happen but protect against some kind of overflow and a wild payout
			{
				if (from.AccessLevel >= AccessLevel.GameMaster) // Allow a GM to clear out the invalid amount
				{
					from.SendMessage("Invalid gold won amount({0}), reset to 0.", m_OnCredit);
					m_OnCredit = m_Won = 0;
				}
				from.SendMessage("There's a problem with this machine's gold amount, this game is offline. Page for help.");
				HiLoOffline(9502);
				return;
			}
			if (m_OnCredit < 1000)
			{
				try
				{
					from.AddToBackpack(new Gold(m_OnCredit));
					from.SendMessage("{0} gold has been added to your pack.", credit);
				}
				catch
				{
					from.SendMessage("There's a problem returning your gold, this game is offline. Page for help.");
					HiLoOffline(9500);
					return;
				}
			}
			else
			{
				try
				{
					from.AddToBackpack(new BankCheck(m_OnCredit));
					from.SendMessage("A bank check for {0} gold has been placed in your pack.", credit);
				}
				catch
				{
					from.SendMessage("There's a problem returning your gold, this game is offline. Page for help.");
					HiLoOffline(9501);
					return;
				}

			}
			m_OnCredit = m_Won = 0;
			m_InUseBy = null;
			string text = null;
			if (credit >= 10000)
			{
				text = String.Format("{0} is cashing out {1} Gold!", from.Name, credit);
				this.PublicOverheadMessage(0, (this.Hue == 907 ? 0 : this.Hue), false, text);
			}
			text = String.Format("{0} is cashing out {1} Gold!", from.Name, credit);
			SecurityCamera(credit >= 10000 ? 0 : 1, text);
			from.PlaySound(52);
			from.PlaySound(53);
			from.PlaySound(54);
			from.PlaySound(55);
		}

		public void Higher(Mobile from)
		{
			m_Hand[m_Count] = carddeck.GetOneCard();
			if (CardHigher(m_Hand[m_Count - 1], m_Hand[m_Count]))
			{
				m_Progress++;
				m_Count++;
				if (m_Count > m_GuessCount)
				{
					m_TotalWins++;
					m_Progress = 100;
					m_Won = (int)(m_CurrentBet * (m_WinMult[m_GuessCount - 2]));
					OnCredit(from, m_Won);
					m_TotalWon += (ulong)m_Won;
#if !PROFILE
					UpdateLastWonBy(from, m_Won);
					DoWinSound(this, from, m_Won);
					from.SendGump(new HiloCardGump(from, this, carddeck.BackDesign, Focus3(m_Hand, m_Count - 2, m_GuessCount), m_Progress, false, null));
#endif
					return;
				}
#if !PROFILE
				from.SendGump(new HiloCardGump(from, this, carddeck.BackDesign, Focus3(m_Hand, m_Count - 1, m_GuessCount), m_Progress, true, null));
#endif
			}
			else
				YouLose(from);
		}

		public void Lower(Mobile from)
		{
			m_Hand[m_Count] = carddeck.GetOneCard();
			if (CardLower(m_Hand[m_Count - 1], m_Hand[m_Count]))
			{
				m_Progress++;
				m_Count++;
				if (m_Count > m_GuessCount)
				{
					m_TotalWins++;
					m_Progress = 100;
					m_Won = (int)(m_CurrentBet * (m_WinMult[m_GuessCount - 2]));
					OnCredit(from, m_Won);
					m_TotalWon += (ulong)m_Won;
#if !PROFILE
					UpdateLastWonBy(from, m_Won);				
					DoWinSound(this, from, m_Won);
					from.SendGump(new HiloCardGump(from, this, carddeck.BackDesign, Focus3(m_Hand, m_Count - 2, m_GuessCount), m_Progress, false, null));
#endif
					return;
				}
#if !PROFILE
				from.SendGump(new HiloCardGump(from, this, carddeck.BackDesign, Focus3(m_Hand, m_Count - 1, m_GuessCount), m_Progress, true, null));
#endif
			}
			else
				YouLose(from);
		}

		private void UpdateLastWonBy(Mobile m, int winamount)
		{
			if (m_LastWonBy == null || m_LastWonBy.Deleted)
			{
				m_LastWonBy = m;
				m_LastWonByDate = DateTime.Now;
				m_LastWonAmount = winamount;
			}
			else
			{
				TimeSpan timespan = DateTime.Now - m_LastWonByDate;
				if (m_LastWonAmount <= winamount || TimeSpan.FromDays(30) < timespan)
				{
					m_LastWonBy = m;
					m_LastWonByDate = DateTime.Now;
					m_LastWonAmount = winamount;
				}
			}
			InvalidateProperties();
		}

		private static int[] Focus3(int[] m_Hand, int current, int guesscount)
		{
			int[] cards = new int[] { -1, -1, -1 };

			if (current == 0)
			{
				cards[1] = m_Hand[0];
				cards[2] = m_Hand[1];
			}
			else if (current == 1)
			{
				cards[0] = m_Hand[0];
				cards[1] = m_Hand[1];
				cards[2] = m_Hand[2];
			}
			else if (current == guesscount)
			{
				cards[2] = m_Hand[current];
				cards[1] = m_Hand[current - 1];
				cards[0] = m_Hand[current - 2];
			}
			else
			{
				cards[0] = m_Hand[current - 1];
				cards[1] = m_Hand[current];
				cards[2] = m_Hand[current + 1];
			}

			return cards;
		}
		private static bool CardHigher(int card1, int card2)
		{
			int c1 = card1 % 13 == 0 ? 14 : card1 % 13;
			int c2 = card2 % 13 == 0 ? 14 : card2 % 13;
			return c2 > c1;
		}

		private static bool CardLower(int card1, int card2)
		{
			int c1 = card1 % 13 == 0 ? 14 : card1 % 13;
			int c2 = card2 % 13 == 0 ? 14 : card2 % 13;
			return c2 < c1;
		}

		private void YouLose(Mobile from)
		{
#if !PROFILE
			from.SendGump(new HiloCardGump(from, this, carddeck.BackDesign, Focus3(m_Hand, m_Count - 1, m_GuessCount), -1, false, null));
#endif
			m_Count = 0;
			m_Hand = new int[HANDSIZE] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
			carddeck.QuickShuffle();
			m_Hand[m_Count] = carddeck.GetOneCard();
			m_Count++;
		}

		public void IncBet()
		{
			m_Bet++;
			if ( (m_Bet > m_BetTable.Length-1) || AdjustBetToMin(m_BetTable[m_Bet], m_GuessCount))
				m_Bet = 0;
			m_CurrentBet = m_BetTable[m_Bet];
		}

		public void DecBet()
		{
			m_Bet--;
			if (m_Bet < 0)
				m_Bet = m_BetTable.Length - 1;
			m_Bet = AdjustBetToMax(m_BetTable[m_Bet], m_Bet, m_GuessCount);
			m_CurrentBet = m_BetTable[m_Bet];
		}

		private static bool AdjustBetToMin(int bet, int count)
		{
			if (bet > 1000 && count < 5)
				return true;
			if (bet > 5000 && count < 7)
				return true;
			return false;
		}

		private static int AdjustBetToMax(int bet, int mbet, int count)
		{
			if (bet >= 1000 && count < 5)
				return 3;
			if (bet >= 5000 && count < 7)
				return 5;
			return mbet;
		}

		public int OnCredit()
		{
			return m_OnCredit;
		}

		public int OnCredit(Mobile from, int amount)
		{
			if (from == null || from.Deleted)
			{
				from.SendMessage("This game needs maintenance.");
				SecurityCamera(0, "This game needs maintenance.");
				Active = false;
				return m_OnCredit;
			}
			m_OnCredit += amount;
			if (amount < 0)
				m_TotalCollected += (ulong)Math.Abs(amount);
			return m_OnCredit;
		}

		public void SecurityCamera(int chatter, string text)
		{
			if (m_SecurityCamMobile == null || m_SecurityCamMobile.Deleted)
				return;
			if (chatter > (int)m_SecurityChatter)
				return;
			if (m_SecurityCamMobile.Player)
				m_SecurityCamMobile.SendMessage(text);
			else
				m_SecurityCamMobile.PublicOverheadMessage(0, (this.Hue == 907 ? 0 : this.Hue), false, text);
		}

		public static void DoWinSound(Object o, Mobile from, int amount)
		{
			string text = null;
			if (amount > 9999)
			{
				text = String.Format("{0} wins {1}!", from.Name, amount);
				((Item)o).PublicOverheadMessage(0, (((Item)o).Hue == 907 ? 0 : ((Item)o).Hue), false, text);
			}
			if (amount > 499999)
			{
				for (int i = 0; i < 5; i++)
					DoFireworks(from);
				from.PlaySound(from.Female ? 824 : 1098);
			}
			else if (amount > 199999)
			{
				DoFireworks(from);
				from.PlaySound(from.Female ? 823 : 1097);
			}
			else if (amount > 9999)
			{
				from.PlaySound(from.Female ? 783 : 1054);
			}
			else if (amount > 999 && Utility.Random(10) > 5)
				from.PlaySound(from.Female ? 794 : 1066);
			else if (amount > 100 && Utility.Random(10) > 7)
			{
				switch (Utility.Random(7))
				{
					case 0:
						from.PlaySound(from.Female ? 794 : 1066);
						break;
					case 1:
						from.PlaySound(from.Female ? 797 : 1069);
						break;
					case 2:
						from.PlaySound(from.Female ? 783 : 1054);
						break;
					case 3:
						from.PlaySound(from.Female ? 823 : 1097);
						break;
					default:
						break;
				}
			}
		}

		private static void DoFireworks(Mobile m)
		{
			FireworksWand fwand = new FireworksWand();

			if (fwand != null && !fwand.Deleted)
			{
				try
				{
					fwand.Parent = m;
					fwand.BeginLaunch(m, true);
					fwand.Delete();
				}
				catch { }
			}
		}

		public static string LoseString()
		{
			if (Utility.RandomDouble() < .001)
				return "\u2665" + " CEO says, \'Loser...loser...\' " + "\u2665";
			switch (Utility.Random(25))
			{
				case 0:
					return "Better luck next time.";
				case 5:
					return "Sorry, not this time.";
				case 10:
					return "I hope you can afford this!";
				case 15:
					return "What happens here... stays here...";
				case 20:
					return "Simply put, you lost... AGAIN!";
				default:
					return "Sorry, try again.";
			}
		}

		public static string WinString()
		{
			if (Utility.RandomDouble() < .0001)
				return "\u2665" + " CEO says, \'Have a nice day, winner!\' " + "\u2665";
			switch (Utility.Random(25))
			{
				case 0:
					return "Good job!";
				case 5:
					return "Lucky you!";
				case 10:
					return "You did it!";
				case 15:
					return "Nice one!";
				case 20:
					return "Feed me more!";
				default:
					return "You Won!";
			}
		}
	}
}

#region gump
namespace Server.Gumps
{
	public class HiloCardGump : Gump
	{
		private HiLoCards m_HiLoCards;
		private int m_BackDesign;
		private int[] m_Card = new int[3];
		private int m_GuessCount;
		private int m_xSize;
		private int m_Progress;
		private bool m_Playing;
		private int m_Base;
		private bool m_HelpGump;

		public HiloCardGump(Mobile player, HiLoCards hilocards, int backdesign, int[] card, int progress, bool playing, string message)
			: base(20, 20)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
			m_HiLoCards = hilocards;
			m_BackDesign = backdesign;
			m_GuessCount = m_HiLoCards.GuessCount;
			m_Card = card;
			m_Progress = progress;
			m_Playing = playing;
			m_xSize = 465;
			m_Base = Utility.Random(500);
			m_HelpGump = m_HiLoCards.HelpGump;

			AddBackground(0, 0, m_xSize, 225, Utility.RandomList(9260));

			if (m_HelpGump)
				AddBackground(m_xSize, 0, 280, 400, 9260);
			if (m_HiLoCards.TestMode)
				AddLabel(12, 12, 37, "Free Play");

			AddLabel(m_xSize / 2 - 50, 15, m_HiLoCards.Hue, "CE");
			AddLabel(m_xSize / 2 - 35, 16, 136, "\u25C6");
			AddLabel(m_xSize / 2 - 10, 15, m_HiLoCards.Hue, "HiLo Cards" + "\u00A9");
			for (int x = 0; x < 3; x++)
			{
				if (card[x] == -1)
					DrawBlankCard(30 + (x * 85), 40, backdesign);
				else
					DrawCard(30 + (x * 85), 40, card[x]);
			}
			if (m_Playing)
			{
				AddButton(95 + 1 * 85, 35, 10701, 10700, m_Base + 10, GumpButtonType.Reply, 0);
				AddButton(95 + 1 * 85, 85, 10721, 10720, m_Base + 11, GumpButtonType.Reply, 0);
			}
			else
			{
				if ((progress == -1 || progress == 100) && message == null)
				{
					if (progress == -1)
					{
						AddLabel(15, 168, 36, HiLoCards.LoseString());
					}
					else
					{
						AddLabel(15, 168, 50 + Utility.Random(50), HiLoCards.WinString());
					}
				}
				AddButton(15, 190, 4020, 4021, m_Base + 300, GumpButtonType.Reply, 0); //PLAY
				AddLabel(50, 190, 1149, @"Play");
				AddButton(100, 190, 4029, 4030, m_Base + 301, GumpButtonType.Reply, 0); //CASHOUT
				if (m_HiLoCards.TestMode)
					AddLabel(135, 190, 1149, @"Quit");
				else
					AddLabel(135, 190, 1149, @"Cash Out");
				AddButton(200, 180, 4037, 4036, m_Base + 302, GumpButtonType.Reply, 0); //ATM
				AddLabel(235, 190, 1149, @"ATM");
				AddButton(92, 124, 0x983, 0x984, m_Base + 101, GumpButtonType.Reply, 0);
				AddButton(92, 139, 0x985, 0x986, m_Base + 102, GumpButtonType.Reply, 0);
			}
			AddLabel(15, 125, 0, "Current bet:");
			if (m_HiLoCards.FixedBets)
				AddLabel(95, 125, 2213, m_HiLoCards.CurrentBet.ToString());
			else
				AddLabel(115, 125, 2213, m_HiLoCards.CurrentBet.ToString());
			AddLabel(15, 150, 0, "Credits:");
			AddLabel(70, 150, 2213, m_HiLoCards.OnCredit(player, 0).ToString());
			AddLabel(160, 150, 0, "Last Pay:");
			AddLabel(225, 150, 2213, m_HiLoCards.Won.ToString());
			if (player.AccessLevel >= AccessLevel.GameMaster)
			{
				int paybackhue = 66;
				if (m_HiLoCards.WinningPercentage > 95.0)
				{
					paybackhue = 37;
				}
				AddLabel(295, 3, 1152, "Payout Percentage:");
				string text = String.Format("{0:##0.00%}", m_HiLoCards.WinningPercentage / 100);
				AddLabel(408, 3, paybackhue, text);
			}
			DrawPayTable();
			if (message != null)
				AddLabel(15, 168, 1150, message);
			if (Utility.RandomDouble() < .0008)
				CEOCookie(m_HiLoCards.Hue, player);
			AddButton(m_xSize - 45, 20, m_HelpGump ? 4014 : 4005, m_HelpGump ? 4016 : 4007, m_Base + 401, GumpButtonType.Reply, 0); //Help
			if (m_HelpGump)
				DisplayHelpGump();
		}

		private void DisplayHelpGump()
		{
			const int TC = 0x556B2F;
			const int HC = 0x000080;
			const int CR = 0x708090;
			string text = Color("Objective: ", HC, false) +
				Color("Guess the next card in the sequence, the more you choose to guess the higher the pay out!\n", TC, false) +
				"<p>" + Color("Rules: ", HC, false) +
				Color("Aces are high. A tie goes to the house(IE you lose).", TC, false) +
				Color(" Do not sit idle at a machine(over 5 mins) or someone else can take it away from you.", TC, false) +
				"</p><p>" + Color("Controls: ", HC, false) +
				Color("Use the small arrows to change your bet, the blue buttons to select how many cards in a row you want to try and guess.", TC, false) +
				Color(" Press 'Play' to start, then use the large red arrows to select high/low for your guess.", TC, false) +
				Color(" Need more cash? Use the ATM button to withdraw gold direct from your bank.", TC, false) +
				Color(" Be sure to Cash Out when you leave!", TC, false) +
				Color(" Press the 'help' arrow again to close this box.", TC, false) +
				"</p><p>" + Color(Center("Copyright by ceo@easyuo.com"), 0x696969, false) +
				Color("This casino program may not be copied, reproduced, or distributed without", CR, false) +
				Color(" the expressed permission of the author. If you run a shard and would like this", CR, false) +
				Color(" casino game on your shard, paypal $10 to ceo@easyuo.com, be sure", CR, false) +
				Color(" to contact me either via email, easyuo.com, or runuo.com forums FIRST. Thank you!", CR, false) +
				"</p></BASEFONT>";
			AddHtml(m_xSize + 15, 15, 250, 370, text, true, true);
		}

		private void DrawPayTable()
		{
			const int GREEN = 0x33FF00;
			int payx = m_xSize - 125;
			int payy = 20;
			bool FixedGuesses = m_HiLoCards.FixedGuesses;
			int loopcount = FixedGuesses ? m_GuessCount + 1 : 11;
			AddLabel(payx - 50, 35, m_HiLoCards.Hue, "Guess:        Win");
			for (int x = 0; x < loopcount; x++)
			{
				if (x < 2)
				{
					if (x == m_Progress)
						AddLabel(payx, payy + 15 + x * 16, 1149, string.Format("{0}", x));
					else
						AddLabel(payx, payy + 15 + x * 16, 0, string.Format("{0}", x));
				}
				else if (m_Playing)
				{
					if (x == m_GuessCount)
					{
						AddHtml(payx, payy + 15 + x * 16, 85, 15, Color(Right(string.Format("{0}", (int)(m_HiLoCards.m_WinMult[x - 2] * m_HiLoCards.CurrentBet))), GREEN), false, false);
						AddHtml(payx, payy + 15 + x * 16, 85, 15, Color(string.Format("{0}", x), GREEN), false, false);

					}
					else if (x == m_Progress)
						AddLabel(payx, payy + 15 + x * 16, 1149, string.Format("{0}", x));
					else if (x < m_GuessCount)
						AddLabel(payx, payy + 15 + x * 16, 0, string.Format("{0}", x));
				}
				else
				{
					if (x == m_GuessCount)
					{
						if (!FixedGuesses)
							AddButton(payx - 35, payy + 19 + x * 16, 2224, 2224, m_Base + 200 + x, GumpButtonType.Reply, 0);
						AddHtml(payx, payy + 15 + x * 16, 85, 15, Color(string.Format("{0}", x), GREEN), false, false);
						AddHtml(payx, payy + 15 + x * 16, 85, 15, Color(Right(string.Format("{0}", (int)(m_HiLoCards.m_WinMult[x - 2] * m_HiLoCards.CurrentBet))), GREEN), false, false);
					}
					else
					{
						if (!m_HiLoCards.FixedGuesses)
							AddButton(payx - 35, payy + 20 + x * 16, 2103, 2224, m_Base + 200 + x, GumpButtonType.Reply, 0);
						if (x == m_Progress)
						{
							AddHtml(payx, payy + 15 + x * 16, 85, 15, Color(string.Format("{0}", x), GREEN), false, false);
							AddHtml(payx, payy + 15 + x * 16, 85, 15, Color(Right(string.Format("{0}", (int)(m_HiLoCards.m_WinMult[x - 2] * m_HiLoCards.CurrentBet))), GREEN), false, false);
						}
						else
						{
							AddLabel(payx, payy + 15 + x * 16, 0, string.Format("{0}", x));
							if (!FixedGuesses)
								AddHtml(payx, payy + 15 + x * 16, 85, 15, Right(string.Format("{0}", (int)(m_HiLoCards.m_WinMult[x - 2] * m_HiLoCards.CurrentBet))), false, false);
						}
					}
				}
			}
		}

		private string Right(string text)
		{
			return String.Format("<DIV ALIGN=RIGHT>{0}</DIV>", text);
		}

		private string Center(string text)
		{
			return String.Format("<CENTER>{0}</CENTER>", text);
		}

		private string Color(string text, int color)
		{
			return Color(text, color, true);
		}

		private string Color(string text, int color, bool usetag)
		{
			if (usetag)
				return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
			return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</COLOR>", color, text);
		}

		private void CEOCookie(int hue, Mobile m)
		{
			AddImageTiled(m_xSize - 80, 5, 142, 230, 990);
			AddLabel(15, 110, hue, "CEO says, \"Hello! Enjoying my HiLo Cards?\" :)");
			m.PlaySound(Utility.RandomList(1358, 1359, 1360, 1361, 1362, 1363, 1368, 1382));
		}

		private void DrawBlankCard(int x, int y, int backdesign)
		{
			AddImageTiled(x, y, 55, 70, 2624);
			AddImageTiled(x + 2, y + 2, 51, 66, backdesign); // 10306 10155
		}

		private void DrawCard(int x, int y, int card)
		{
			string[] CardFace = new string[] {"A", "2", "3", "4", "5", "6" , "7",
                "8", "9", "10", "J", "Q", "K", "A"};
			string[] suit_t = new string[] { "\u2660", "\u25C6", "\u2663", "\u2665" };
			int[] color_t = new int[] { 0, 36, 0, 36 };
			int suit_i = card / 13;
			int color = color_t[suit_i];
			AddImageTiled(x, y, 55, 70, 2624);
			AddImageTiled(x + 2, y + 2, 51, 66, 0xBBC);
			AddLabel(x + 22, y + 25, color, CardFace[card % 13]);
			AddLabel(x + 5, y + 5, color, suit_t[suit_i]);
			AddLabel(x + 33, y + 47, color, suit_t[suit_i]);
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;
			string message = null;

			if (from == null)
				return;
			if (info.ButtonID == 0) // CashOut
			{
				m_HiLoCards.RemovePlayer(from);
				return;
			}
            else if (info.ButtonID == 1) // Close by machine
            {
                return;
            }
			else if (info.ButtonID == m_Base + 401) // HelpGump
			{
				m_HiLoCards.HelpGump = !m_HiLoCards.HelpGump;
				from.SendGump(new HiloCardGump(from, m_HiLoCards, m_BackDesign, m_Card, m_Progress, m_Playing, message));
				return;
			}

			if (m_Playing)
			{
				if (info.ButtonID == m_Base + 10) // Higher
				{
					m_HiLoCards.Higher(from);
					return;
				}
				else if (info.ButtonID == m_Base + 11) // Lower
				{
					m_HiLoCards.Lower(from);
					return;
				}
			}
			else
			{
				if (info.ButtonID == m_Base + 300) // Play
				{
					m_HiLoCards.PlayHiLo(from);
					return;
				}
				else if (info.ButtonID == m_Base + 301) // CashOut
				{
					m_HiLoCards.RemovePlayer(from);
					return;
				}
				else if (info.ButtonID == m_Base + 302) // ATM
				{
					if (m_HiLoCards.OnCredit() >= m_HiLoCards.CreditATMLimit)
					{
						message = "This machine is at or over its credit limit.";
					}
					else if (m_HiLoCards.TestMode)
					{
						message = "Machine in test mode, doubleclick for credits.";
					}
					else
					{
						int amount = (m_HiLoCards.CreditATMLimit - m_HiLoCards.OnCredit() < m_HiLoCards.CreditATMIncrements) ? m_HiLoCards.CreditATMLimit - m_HiLoCards.OnCredit() : m_HiLoCards.CreditATMIncrements;
						if (from.BankBox.ConsumeTotal(typeof(Gold), amount))
						{
							m_HiLoCards.OnCredit(from, amount);
							message = string.Format("{0} gold withdrawn from bank.", amount);
							Effects.PlaySound(new Point3D(m_HiLoCards.X, m_HiLoCards.Y, m_HiLoCards.Z), m_HiLoCards.Map, 501);
							string text = String.Format("{0}:ATM={1}.", from.Name, amount);
							//m_Keno.SecurityCamera(amount > 5000 ? 0 : 1, text);
							text = String.Format("OnCredit={1}.", from.Name, m_HiLoCards.OnCredit());
							//m_Keno.SecurityCamera(m_Player.OnCredit > 10000 ? 1 : 2, text);
						}
						else
							message = "Insufficient funds for ATM withdrawal.";
					}
					from.SendGump(new HiloCardGump(from, m_HiLoCards, m_BackDesign, m_Card, m_Progress, m_Playing, message));
					return;
				}
				else if ((info.ButtonID == m_Base + 101) && (!m_HiLoCards.FixedBets))
				{
					m_HiLoCards.IncBet();
				}
				else if ((info.ButtonID == m_Base + 102) && (!m_HiLoCards.FixedBets))
				{
					m_HiLoCards.DecBet();
				}
				else if ((info.ButtonID > m_Base + 200 && info.ButtonID < m_Base + 211) && (!m_HiLoCards.FixedGuesses))
				{
					m_HiLoCards.GuessCount = info.ButtonID - 200 - m_Base;
				}
				from.SendGump(new HiloCardGump(from, m_HiLoCards, m_BackDesign, m_Card, -2, false, null));
			}
		}
	}
}
#endregion
