// Undefine for a "live" shard. See profiling notes below.
// Only use PROFILE options on a test shard!
//#define PROFILE
// Undefine if you are not using RunUo 2.0
#define RUNUO2RC1
 /* Using Arte Gordon's XMLSPAWNER? If so leave "#define XMLSPAWNER" below as is. 
 * If you do not have XMLSPAWNER installed comment out the line. All this does is
 * give you a larger property gump to view the slot's properties. 
 */
#define XMLSPAWNER
//
// This section allows you turn of themes and/or options that may not work on your shard
//#define MINIHOUSES
#define MINIHOUSES_CEORANDOM
/*
Package Name: CEOTurboSlots
Author: CEO
Version: 1.0e
Public Release: 12/22/05
Purpose:  Real Casino style slots. 14 different themes and progressive jackpots.

Description:  This package allows you to add casino style slots to your shard. You can choose
 * from 14 different themes that all give different symbols, pay outs, and unique options. You can
 * also link slots together for progressive jackpots. Some slots can be configured to give item 
 * rewards. Special "Casino Club" cards are given out to big jackpot winners allowing you to configure 
 * some slots just for those players. A top ten board is included that will show recent jackpot 
 * winners as well as give overall shardwide stats for GMs and above.
 * 
 * Themes:

 *          Classic, ClassicII, ClassicIII: Modeled after real Las Vegas slots!
 *          FarmerFaceoff: "Nickle" slots with a farm theme.
 *          GruesomeGambling: Creepy. Bleeds player now and then and throws out bones. Top prize
 *                              Hooded Shroud of Shadows. More deadly in Fel.
 *          Holiday1: Christmas themed slot.
 *          LadyLuck: Gives out luck items, sashe, cloak, or robe.
 *          MinerMadness: The original minermadness with exploit removed, streamlined bonus round gump,
 *                          and is NOT a gold farm slot!
 *          OffToTheRaces: Inexpensive slot with a racing theme.
 *          Pirates: Arr mateys. 10gp slot with a priate ship model as top prize.
 *          PowerScrolls: Very colorful slot, that changes symbols (and top prize) every 4 hours.
 *          StatScrolls: The harrower! Watchout this one bites, more deadly in Fel.
 *          TailorTreats: Barbed kits as top prize.
 *          TrophyHunter: Uses the animal trophy mounts as symbols.
 * 
 * Slots that give rewards have values assigned to the rewards. I spent many hours profiling and simulating
 * these slots, the text file, allodds.txt, shows the statistical odds and simulated odds over many spins. 
 * Over alot of spins, they *will* remove gold from the economy. Some may seem to payout more
 * often, but overall players will have fun while losing their shirts. :)

 * In general the different odds are as such:
 * 
 *      Loose:              96%-98%
 *      Normal:             93%-96%
 *      Tight:              89%-92%
 *      Extremely Tight:    85%-88%
 *      Casino Cheats:      79%-84% (Not really cheating. The preferred setting for progressive groups.)
 *      Random:             The machine will randomly bounce around from Loose-CasioCheats every 15 minutes.
 *                          This setting can make a slot appear "hot", but then go "cold" after awhile. 
 * 
 * The odds tables are pretty accurate for each machine. They use odds distribution tables like real
 * casinos with each reel having it's own symbol distribution/odds table.
 * 
 * Note: A player's luck can also affect odds, though it'll be random and reevaluted every 10-15 minutes
 * simular to the Random setting. The player will never know whether their luck helped or not. If the "roll" favors 
 * the player, the machine's odds will be bumped up by one favorable level, maxing out of course at Loose.
 * 
 * The Percentage is basically the payback. For instance, a slot with a 95% payback odds means that
 * for every 100gp played 95gp will be returned to the player. 
 * So over time and alot of spins/plays the casino's take will be 5%.
 * 
 * Progressive Jackpots:
 * 
 * With progressive jackpots you can link groups of machines together. As people play these slots
 * a percentage of the gold played is added to what's called a progressive jackpot. This jackpot
 * continues to grow until someone hits the "big one" on a linked machine. Progressive jackpots can
 * get to be over several million gp and cause a playing "frenzy" as they grow. Because the odds of 
 * a machine can be dramtically affected, you should only link machines together that cost the same amount
 * to play. IE. do not link a 5gp machine to a 100gp progresssive pool as the 5gp machine WILL become
 * a gold farm. It's also recommended that progressive machine's odds are setup to be either Extremely
 * Tight or Casino Cheats as over time the statistical payout will increase, and may even go over 100%.
 * With Normal and Loose odds the machines have the potentional to be gold farms. Even in Extremely/CasinoCheats
 * modes odds may go over 100%, odds like that are normal though, and only the machine that "hits" will 
 * be temporarily in the red and appear to be a gold farm.
 * 
 * To setup a progressive pool, first set one slot machine's properties to ProgIsMaster = true. Then
 * point other machine's property of ProgSlotMaster toward the "Master" slot machine. The more
 * you link together the faster the jackpot will rise (as people play those machines).
 * 
 * Other Options:
 * 
 * Active(false):   Determins whether a slot is available for play. A new slot will default to the
 *                  Classic theme and Active=false allowing you to configure it before putting it
 *                  online.
 * 
 * AnnounceJackpots(True): Does a shardwide annoucement when someone hits a big jackpot.
 * CardClubOnly(False): Only players holding a Casino Membership card may play this slot.
 * CreditATMIncrements(10000):Amount to add to credit from bank gold in ATM transfer.
 * CreditATMLimits:Varies with theme. The maximum amount that can be withdrawn from bank to machines credit.
 * CreditCashOutAt:The maximum amount allowed on a machine before a bank check is put in player's backpack.
 * 
 * ErrorCode: For determining the cause of a slot machine failure.
 * 
 * JackpotRewards: Values are None (no item rewards), RewardOnly(jackpot gives items only), or
 *                 RewardAndCash(jackpot gives rewards AND cash). This value only applies to those
 *                 themes that give item rewards.
 * 
 * MembershipCard(True):Issues a Casino membership card to jackpot winners over 500k. 
 *                      See CardClubOnly above. Also makes for a useful "bragging" trophy.
 * 
 * PlayerSounds:Usually defaults to true. These slots are noisy! Designed to attract attention like
 *              real slots.
 * 
 * ProgPercent(5%): The amount of gp played to be put into a progressive jackpot. See above for configuring
 *              progressive jackpot pools.
 * 
 * Random Max/Min: The amount of time a Random machine stays on one odds table. Used also for when a
 *                  player's luck has affected the odds table.
 * 
 * ResetTotals: Used to reset the slots totals. Usually not recommended as you'll lose your stats.
 * 
 * ShowPayback(false):Not recommended, but will show the slot's current payback amount to players. Note: GMs
 *                      or higher will always be able to see this figure on the slot.
 * 
 * SlotTheme: The current slot's theme. See Above.
 * 
 * TotalCollected: Total amount collected by this slot.
 * TotalNetProfit: The Casino's "Take".
 * TotalSpins: The number of times the slot has been played.
 * TotalWon: The amount of gp payed out to players.
 * 
 * w_Percentage: Statistical odds of the machine.
 * WinningPercentage: Real "current" odds of the machine.
 * 
 * zJackpotStats0-S2: The # of times a particular jackpot has been hit. Jackpot 0 is the
 *                      top line, S1 and S2 are scatter counters.
 * zReelOne,ReelTwo,ReelThree: The current symbol line.
 * 
 * 
 * Profiling/Creating your own machine.
 * 
 * While there are already 14 themes to choose from, you might find yourself bored one day
 * and decide to create your own unique machine. This can be as simple as changing the symbols
 * and odds table to as complex as creating new rewards. To start you will need to remove the comments on
 * the line "#define PROFILE". This option will present you with additional properties and let you
 * put the machine into "test" mode. This IS NOT recommended for live shards as it has the potentional
 * to be abused and turn your slots into a gold farmers dream or admin's nightmare!
 * 
 * Building a new theme can also be quite complicated the first time, but after a few times you 
 * can usually create an all new theme in under an hour. I won't go over all the details on doing 
 * this, you're on your own except for any posts/info I may give in the script's thread. You should 
 * have a good grasp of how the slots work and especially what payback odds mean as well as generating 
 * profiling data to verify you're not creating a gold farm. 
 * 
 * -------------------------------------------------------------------
 * Top Ten Board.
 * 
 * I've also included a nice litte message board that players can click on to see a list of
 * players that have recently won jackpots and encourage play. Put them at your banks and/or casino.
 * GMs and above will also have an extra icon that takes you to a shard-wide information gump that 
 * will display all slots and totals/etc of your casino. Very helpful for an overall view of the 
 * slots and how much gold they have removed from your economy. It also allows you to get properties of 
 * a slot machine and zip quickly to a slot machine. You can also set a slot's Active (offline/online)
 * property from this gump.
 * 
 * Usage: ]add TurboSlotStats
 * -------------------------------------------------------------------
 * Casino Tokens.
 * 
 * This item allows a player to play the slots for free up until uses remaining hits 0, which it
 * then self deletes. A nice way to get people playing your slots is to drop one of these in their
 * backpacks while online, or include it your new player startup pack. Defaults to 10 uses, but can
 * be setup to do more then that.
 * 
 * Usage: ]add CasinoToken {uses} {hue}
 * -------------------------------------------------------------------
 * 
Acknowledgements: Thanks to RoninGT for the original miner madness slot.

Installation:	Unzip CEOTurboSlots.zip into your custom script folder and restart.

Usage:			]add TurboSlot
*/


using System; 
using Server;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using System.Xml;

namespace Server.Items
{
	public class TurboSlot : Item
	{
		public enum BonusRoundType { None, MinerMadness }
		public enum PaybackType { Loose, Normal, Tight, ExtremelyTight, CasinoCheats, Random }
		public enum SlotThemeType { Classic, ClassicII, ClassicIII, FarmerFaceoff, GruesomeGambling, Holiday1, LadyLuck, MinerMadness, OffToTheRaces, Pirates, PowerScrolls, StatScrolls, TailorTreats, TrophyHunter 
#if MINIHOUSES
			, MiniHouses
#endif 
		}
		public enum ScatterType { None, Any, LeftOnly };
		public enum JackpotRewardType { None, RewardOnly, RewardAndCash };
		private int m_ReelOne = 0;
		private int m_ReelTwo = 0;
		private int m_ReelThree = 0;

		// Machine Data
		private bool m_Active = false;
		public PaybackType m_PaybackType = PaybackType.Normal;
		public SlotThemeType m_SlotTheme = SlotThemeType.MinerMadness;
		public BonusRoundType m_BonusRound = BonusRoundType.MinerMadness;
		private ScatterType m_ScatterPay = ScatterType.Any;
		private bool m_AnyBars = true;
		private bool m_PlayerSounds = true;
		private int m_TotalSymbols;
		private JackpotRewardType m_Rewards = JackpotRewardType.None;

		//Stats and Totals
		private int m_Cost = 100;
		private int m_Won = 0;
		private int m_LastPay;
		private int m_TotalCollected = 0;
		private int m_TotalWon = 0;
		private int m_TotalSpins = 0;

		//Mobile & timeout
		private Mobile m_InUseBy = null;
		private DateTime m_LastPlayed = DateTime.Now;
		private TimeSpan m_TimeOut;
		private TimeSpan m_IdleTimer = TimeSpan.FromMinutes(5); // How long can a person be standing at the machine not playing?
		private Mobile m_LastWonBy = null;
		private DateTime m_LastWonByDate = DateTime.Now;
		private int m_LastWonAmount = 0;

		// For Progressive Slots
		private int m_DefaultStartProgressive = 10000;
		private bool m_isProgMaster = false;
		private int m_ProgressivePercent = 5;
		private Item m_ProgressiveMaster = null;
		private int m_ProgressiveJackpot = 10000;
		ArrayList m_SlotSlaves = new ArrayList();

		// Misc

		private bool m_ShowPayback = false;
		private PaybackType m_CurrentPaybackType = PaybackType.Normal;
		private bool m_AnnounceJackpot = true;
		private bool m_FreeSpin = false;
		private int m_OrigHue = -1;
		private int m_CreditCashOut = 250000;
		private int m_CreditATMLimit = 100000;
		private int m_CreditATMIncrements = 10000;
		private int m_ErrorCode = 0;
		private bool m_GiveReward = false;

		//Profiling stuff
#if PROFILE 
		private int m_TotalNetProfit = 0;
		private bool m_ProfGetCurrent = false;
		private float m_ProfPercentagehigh;
		private float m_ProfPercentagelow;
		private bool m_TestMode = false;
		private int m_TestSpin = 0;
		private bool m_ProfileAll = false;
#endif
		private bool m_ResetTotals = false;
		private string m_ProfSymbols = null;
		private string[] m_ProfReel = { null, null, null };
		private string m_ProfPayTable = null;
		private int[] m_ProfDist = { 0, 0, 0, 0, 0, 0, 0, 0 };
		private bool m_Profile = false;
		private float m_ProfPercentage;
		private int[,] m_CurrentDist = new int[3, 8];

		// Odds Table and Jackpot events
		private int m_MaxRoll = 100;
		private int[,] m_ReelTable = new int[3, 100];
		private string[] m_JackpotText = new string[9];
		private int[] m_JackpotEffect = new int[9];
		private int[] m_Symbols = new int[21];
		private int[] m_Sounds = new int[9];
		private int[] m_FemaleSounds = new int[9];
		private int[] m_MaleSounds = new int[9];
		private int[] m_jackpotStats = new int[11];
		private int[] m_jackpotmultiplier = new int[11];
		private int[] m_Bars = new int[4];

		//Timer Stuff
		private TimeSpan m_RandomMin = TimeSpan.FromMinutes(10);
		private TimeSpan m_RandomMax = TimeSpan.FromMinutes(15);
		private bool m_RandomActivated = false;
		private DateTime m_RandomTimerEnd;
		private InternalTimer1 m_RandomTimer;
		private DateTime m_RandomSymbolsTimer = DateTime.Now;
		private TimeSpan m_RandomSymbolsTimerEnd = TimeSpan.FromHours(4);

		private int m_BlinkCount = 0;
		private int m_BlinkHue;
		private DateTime m_BlinkTimerEnd;
		private InternalTimer2 m_BlinkTimer;

		//Issue a special "card" for jackpot winners that could be used to get into special places
		private bool m_MembershipCard = true;
		private bool m_CardClubOnly = false; //Only club members can play this slot

		#region CommandProperties
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active
		{
			get { return m_Active; }
			set
			{
				if (!m_Active && value)
				{
					if (m_BlinkTimer != null)
						m_BlinkTimer.Stop();
					if (m_OrigHue != -1)
					{
						this.Hue = m_OrigHue;
						m_OrigHue = -1;
					}
					Sparkle(-3);
					Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, 1481);
					this.PublicOverheadMessage(0, this.Hue, false, "CEOTurboSlot online!");
				}
				else if (m_Active && !value)
				{
					m_OrigHue = this.Hue;
					this.Hue = 1001;
					this.PublicOverheadMessage(0, this.Hue, false, "CEOTurboSlot offline.");
				}
				m_Active = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ErrorCode
		{
			get { return m_ErrorCode; }
			set { m_ErrorCode = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool FreeSpin
		{
			get { return m_FreeSpin; }
			set { m_FreeSpin = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public SlotThemeType SlotTheme
		{
			get { return m_SlotTheme; }
			set
			{
				bool currentstate = m_Active;
				m_ErrorCode = 0;
				if (value != m_SlotTheme)
				{
					if (m_BlinkTimer != null)
						m_BlinkTimer.Stop();
					m_Active = false;
				}
				m_SlotTheme = value;
				SetupTheme(m_SlotTheme, true);
				m_OrigHue = -1;
				if (currentstate && m_ErrorCode == 0)
					Active = true;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ScatterType ScatterPay
		{
			get { return m_ScatterPay; }
#if PROFILE
			set { m_ScatterPay = value; m_ProfPercentage = CalcOdds(m_CurrentDist); }
#endif
		}

#if PROFILE

		[CommandProperty(AccessLevel.Administrator)]
		public bool TestMode
		{
			get { return m_TestMode; }
			set { m_TestMode = value; }
		}

		[CommandProperty(AccessLevel.Administrator)]
		public bool w_GetCurTables
		{
			get { return m_ProfGetCurrent; }
			set
			{
				if (value)
				{
					w_Symbols = CreateProfileStrings(m_Symbols);
					m_ProfPayTable = CreateProfileStrings(m_jackpotmultiplier);
					int[] temp = new int[m_TotalSymbols];
					for (int h = 0; h < 3; h++)
					{
						for (int i = 0; i < m_TotalSymbols; i++)
							temp[i] = m_CurrentDist[h, i];
						m_ProfReel[h] = CreateProfileStrings(temp);
					}
					InvalidateProperties();
				}
				else
				{
					w_Symbols = null;
					m_ProfPayTable = null;
					for (int h = 0; h < 3; h++)
						m_ProfReel[h] = null;
				}
				m_ProfGetCurrent = false;
			}
		}
		[CommandProperty(AccessLevel.Administrator)]
		public string w_DistReel1
		{
			get { return m_ProfReel[0]; }
			set
			{
				if (value == null)
				{
					m_ProfReel[0] = value;
					return;
				}

				string[] sargs = value.Split(new Char[] { ',' }, m_TotalSymbols);
				for (int i = 0; i < sargs.Length; i++)
				{
					m_ProfDist[i] = GetInt(sargs[i]);
				}
				CreateOddsTable(0, m_ProfDist);
				m_ProfPercentage = CalcOdds(m_CurrentDist);
				m_ProfReel[0] = value;
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public string w_DistReel2
		{
			get { return m_ProfReel[1]; }
			set
			{
				if (value == null)
				{
					m_ProfReel[1] = value;
					return;
				}

				string[] sargs = value.Split(new Char[] { ',' }, m_TotalSymbols);
				for (int i = 0; i < sargs.Length; i++)
				{
					m_ProfDist[i] = GetInt(sargs[i]);
				}
				CreateOddsTable(1, m_ProfDist);
				m_ProfPercentage = CalcOdds(m_CurrentDist);
				m_ProfReel[1] = value;
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public string w_DistReel3
		{
			get { return m_ProfReel[2]; }
			set
			{
				if (value == null)
				{
					m_ProfReel[2] = value;
					return;
				}
				string[] sargs = value.Split(new Char[] { ',' }, m_TotalSymbols);
				for (int i = 0; i < sargs.Length; i++)
				{
					m_ProfDist[i] = GetInt(sargs[i]);
				}
				CreateOddsTable(2, m_ProfDist);
				m_ProfPercentage = CalcOdds(m_CurrentDist);
				m_ProfReel[2] = value;
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public string w_PayTable
		{
			get { return m_ProfPayTable; }
			set
			{
				if (value == null)
				{
					m_ProfPayTable = value;
					return;
				}

				string[] sargs = value.Split(new Char[] { ',' }, 11);
				for (int i = 0; i < sargs.Length; i++)
					m_jackpotmultiplier[i] = GetInt(sargs[i]);
				m_ProfPercentage = CalcOdds(m_CurrentDist);
				m_ProfPayTable = value;
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public string w_Symbols
		{
			get { return m_ProfSymbols; }
			set
			{
				if (value == null)
				{
					m_ProfSymbols = value;
					return;
				}
				value.Trim();

				string[] sargs = value.Split(new Char[] { ',' }, 21);

				for (int i = 0; i < sargs.Length; i++)
					m_Symbols[i] = GetInt(sargs[i]);
				m_ProfPercentage = CalcOdds(m_CurrentDist);
				m_ProfSymbols = value;
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public bool w_ProfileAll
		{
			get { return false; }
			set
			{
				if (value)
				{
					ProfileAll();
				}
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public bool w_ProfileProgressive
		{
			get { return false; }
			set
			{
				if (value)
				{
					m_Profile = true;
					Profile(false, 5);
					m_Profile = false;
				}
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public bool w_Profile
		{
			get { return m_Profile; }
			set
			{
				if (!value)
					return;
				m_Profile = true;
				if (m_ProfPayTable == null && m_ProfSymbols == null && m_ProfReel[0] == null && m_ProfReel[1] == null && m_ProfReel[2] == null)
				{
					//m_ProfPercentage = CalcOdds(m_CurrentDist);
					Profile(true, 1);
				}
				else
				{
					if (m_ProfPayTable != null)
					{
						string[] sargs = m_ProfPayTable.Split(new Char[] { ',' }, 11);
						for (int i = 0; i < sargs.Length; i++)
							m_jackpotmultiplier[i] = GetInt(sargs[i]);
					}
					if (m_ProfSymbols != null)
					{
						string[] sargs = m_ProfSymbols.Split(new Char[] { ',' }, 21);
						for (int i = 0; i < sargs.Length; i++)
							m_Symbols[i] = GetInt(sargs[i]);
					}
					m_ProfPercentage = CalcOdds(m_CurrentDist);
					Profile(false, 1);
				}
				m_Profile = false;
			}
		}
#endif

		[CommandProperty(AccessLevel.GameMaster)]
		public float w_Percentage
		{
			get { return m_ProfPercentage; }
			//set { m_ProfPercentage = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AnyBars
		{
			get { return m_AnyBars; }
#if PROFILE
			set { m_AnyBars = value; m_ProfPercentage = CalcOdds(m_CurrentDist); }
#endif
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public JackpotRewardType JackpotRewards
		{
			get { return m_Rewards; }
			set
			{
				m_Rewards = value;
				SetupTheme(m_SlotTheme, false);
				m_ProfPercentage = CalcOdds(m_CurrentDist);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool MembershipCard
		{
			get { return m_MembershipCard; }
			set { m_MembershipCard = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CardClubOnly
		{
			get { return m_CardClubOnly; }
			set { m_CardClubOnly = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool PlayerSounds
		{
			get { return m_PlayerSounds; }
			set { m_PlayerSounds = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public BonusRoundType BonusRound
		{
			get { return m_BonusRound; }
#if PROFILE
			set { m_BonusRound = value; }
#endif
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
		public bool AnnounceJackpots
		{
			get { return m_AnnounceJackpot; }
			set { m_AnnounceJackpot = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ShowPayback
		{
			get { return m_ShowPayback; }
			set { m_ShowPayback = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public PaybackType SlotPaybackOdds
		{
			get { return m_PaybackType; }
			set
			{
				m_PaybackType = value;
				SetupOddsTable(m_PaybackType, true);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public PaybackType CurrentPayback
		{
			get { return m_CurrentPaybackType; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ProgJackpot
		{
			get { return m_ProgressiveJackpot; }
			set { m_ProgressiveJackpot = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ProgJackpotStart
		{
			get { return m_DefaultStartProgressive; }
			set { m_DefaultStartProgressive = value; }
		}


		[CommandProperty(AccessLevel.GameMaster)]
		public bool ResetTotals
		{
			get { return m_ResetTotals; }
			set
			{
				if (value)
				{
					m_TotalCollected = 0;
					m_TotalWon = 0;
					m_TotalSpins = 0;
					for (int i = 0; i < 11; i++)
						m_jackpotStats[i] = 0;
				}
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ProgPercent
		{
			get { return m_ProgressivePercent; }
			set
			{
				int newvalue = value;
				if (value < 0)
					newvalue = 0;
				if (value > 35)
					newvalue = 35;
				m_ProgressivePercent = newvalue;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ProgIsMaster
		{
			get { return m_isProgMaster; }
			set
			{
				if (m_ProgressiveMaster != null)
				{
					this.PublicOverheadMessage(0, this.Hue, false, "This machine can not be a Slot Master while linked to another Slot Master.");
					return;
				}
				m_isProgMaster = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Item ProgSlotMaster
		{
			get { return m_ProgressiveMaster; }
			set
			{
				if (m_isProgMaster)
					return;
				if (value != null && value is TurboSlot && this != value && !value.Deleted)
				{
					if (((TurboSlot)value).ProgIsMaster)
					{
						m_ProgressiveMaster = value;
						((TurboSlot)m_ProgressiveMaster).AddSlave(this);
					}
					else
						this.PublicOverheadMessage(0, this.Hue, false, "The machine you selected is not a Slot Master.");
				}
				else if (value == null)
				{
					if (m_ProgressiveMaster != null && !m_ProgressiveMaster.Deleted)
						((TurboSlot)m_ProgressiveMaster).RemoveSlave(this);
					m_ProgressiveMaster = null;
				}
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan RandomMin
		{
			get { return m_RandomMin; }
			set { m_RandomMin = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan RandomMax
		{
			get { return m_RandomMax; }
			set { m_RandomMax = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan RandomOver
		{
			get
			{
				if (m_RandomActivated)
					return m_RandomTimerEnd - DateTime.Now;
				else
					return TimeSpan.FromSeconds(0);
			}
			set
			{
				DoTimer1(value);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Cost
		{
			get { return m_Cost; }

			set
			{
				if (value < 5)
					m_Cost = 5;
				else if (value > 100)
					m_Cost = 100;
				else
					m_Cost = value;
			}
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
			//set{ m_Won = value; }
		}

#if PROFILE
		[CommandProperty(AccessLevel.Administrator)]
#else 
        [CommandProperty(AccessLevel.GameMaster)]
#endif
		public int Won
		{
			get { return m_Won; }
#if PROFILE
			set { m_Won = (value >= 0) ? value : 0; }
#endif

		}

		public int SlotWon
		{
			get { return m_Won; }
			set { m_Won = (value >= 0) ? value : 0; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalNetProfit
		{
			get { return m_TotalCollected - m_TotalWon; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalCollected
		{
			get { return m_TotalCollected; }
#if PROFILE
			set { m_TotalCollected = value; }
#endif
		}
		public int SlotTotalCollected
		{
			get { return m_TotalCollected; }
			set { m_TotalCollected = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalWon
		{
			get { return m_TotalWon; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int TotalSpins
		{
			get { return m_TotalSpins; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastPlayed
		{
			get { return m_LastPlayed; }
			set { m_LastPlayed = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile InUseBy
		{
			get { return m_InUseBy; }
			set { m_InUseBy = value; InvalidateProperties(); }

		}

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile LastWonBy
		{
			get { return m_LastWonBy; }
			set
			{
				m_LastWonBy = value;
				if (m_LastWonBy == null)
					m_LastWonAmount = 0;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastWonByDate
		{
			get { return m_LastWonByDate; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int LastWonAmount
		{
			get { return m_LastWonAmount; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int LastPay
		{
			get { return m_LastPay; }
			set { m_LastPay = value; }
		}
		#endregion
		#region Jackpot Statistics
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStats0
		{
			get { return m_jackpotStats[0]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStats1
		{
			get { return m_jackpotStats[1]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStats2
		{
			get { return m_jackpotStats[2]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStats3
		{
			get { return m_jackpotStats[3]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStats4
		{
			get { return m_jackpotStats[4]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStats5
		{
			get { return m_jackpotStats[5]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStats6
		{
			get { return m_jackpotStats[6]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStats7
		{
			get { return m_jackpotStats[7]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStats8
		{
			get { return m_jackpotStats[8]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStatsS1
		{
			get { return m_jackpotStats[9]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zJackpotStatsS2
		{
			get { return m_jackpotStats[10]; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zReelOne
		{
			get { return m_ReelOne; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zReelTwo
		{
			get { return m_ReelTwo; }
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public int zReelThree
		{
			get { return m_ReelThree; }
		}
		#endregion

		public int ReelOne
		{
			get { return m_ReelOne; }
			set { m_ReelOne = value; }
		}

		public int ReelTwo
		{
			get { return m_ReelTwo; }
			set { m_ReelTwo = value; }
		}

		public int ReelThree
		{
			get { return m_ReelThree; }
			set { m_ReelThree = value; }
		}

		private int GetInt(string intstr)
		{
			int newint = 0;
			try
			{
				newint = int.Parse(intstr);
			}
			catch { newint = 0; };
			return newint;
		}

		[Constructable]
		public TurboSlot()
			: base(3804)
		{
			Movable = false;
			m_SlotTheme = SlotThemeType.Classic;
			SetupTheme(m_SlotTheme, true);
			m_Active = false;
		}

		private bool CheckInRange(Point3D loc, int range)
		{
			return Utility.InRange(GetWorldLocation(), loc, range);
		}


		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (m_InUseBy != null)
			{
				if (!m_InUseBy.InRange(GetWorldLocation(), 3) || m_InUseBy.Map == Map.Internal)
				{
					m_InUseBy.CloseGump(typeof(TurboSlotGump));
#if !RUNUO2RC1
					m_InUseBy.CloseGump(typeof(NewMinerBonusGump));
					m_InUseBy.CloseGump(typeof(TurboSlotPayTableGump));
					m_InUseBy.SendMessage("You have walked away from the slot machine, others may now use it.");
					if (m_Won != 0)
					{
						m_InUseBy.PlaySound(52);
						m_InUseBy.SendMessage("Hey, you left some cash in the machine! Cashing out.");
						DoCashOut(m_InUseBy); // Give them their winnings
					} 
#endif
					InUseBy = null;
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
                from.SendGump( new PropertiesGump( from, this ) );
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
				if (m_InUseBy != null && (from.Serial == InUseBy.Serial))
				{
					from.CloseGump(typeof(TurboSlotGump));
					from.CloseGump(typeof(NewMinerBonusGump));
					from.CloseGump(typeof(TurboSlotPayTableGump));
					if (m_Won != 0)
						DoCashOut(from);
				}
				from.SendMessage("Sorry, this machine is currently down for maintenance.");
				return;
			}

			m_TimeOut = DateTime.Now - m_LastPlayed;

			if (m_CardClubOnly && !CarryingClubCard(from))
			{
				from.SendMessage("You must be carrying your Captain's Cabin Membership Card to play this slot.");
				return;
			}
			if (m_InUseBy == null || m_InUseBy.Deleted)
				InUseBy = from;
			else
			{
				if (m_IdleTimer < m_TimeOut)
				{
					if (m_InUseBy != null && m_InUseBy != from && m_Won != 0)
						DoCashOut(m_InUseBy); // Previous user disconnected or something? Give them their cash before releasing.
					if (from != m_InUseBy)
						from.SendMessage("{0} has left this machine idle too long, it is yours to play.", m_InUseBy.Name);
					InUseBy = from;
				}
			}

			if (from == m_InUseBy)
			{
				from.CloseGump(typeof(TurboSlotGump));
				from.CloseGump(typeof(NewMinerBonusGump));
				if (m_SlotTheme == SlotThemeType.PowerScrolls)
				{
					TimeSpan symtimer = DateTime.Now - m_RandomSymbolsTimer;
					if (m_RandomSymbolsTimerEnd < symtimer)
					{
						RandomScrollSymbols();
						m_RandomSymbolsTimer = DateTime.Now;
						m_ReelOne = m_ReelTwo = m_ReelThree = m_Symbols[0];
					}
				}
				//if (m_PaybackType == PaybackType.Random)
				SetupOddsTable(m_PaybackType, false);
				m_LastPlayed = DateTime.Now;
				from.SendGump(new TurboSlotGump(this, m_Symbols));
			}
			else
			{
				string text = String.Format("{0} is currently using this machine.", m_InUseBy.Name);
				from.SendMessage(text);
			}
		}

		private bool CarryingClubCard(Mobile m)
		{
			if (m.Backpack == null)
				return false;
#if RUNUO2RC1
			List<Item> packlist = m.Backpack.Items;
#else
			ArrayList packlist = m.Backpack.Items;
#endif
			for (int i = 0; i < packlist.Count; ++i)
			{
				Item item = (Item)packlist[i];
				if (item != null && !item.Deleted && item is CasinoMembershipCard)
				{
					if (((CasinoMembershipCard)item).ClubMember == m)
						return true;
				}
			}
			return false;
		}

		public bool CashCheck(Mobile m, out int amount)
		{
			amount = 0;
			if (m.Backpack == null)
				return false;
#if RUNUO2RC1
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
						Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, 501);
						m_Won += amount;
					}
					else
					{
						m.SendMessage("There's a problem trying to cash a check in your backpack, this slot machine is offline. Page for help.");
						SlotOffline(9503);
						return false;
					}
					return true;
				}
			}
			return false;
		}

		public string[] GetJackpotPayoutStr(int index, out decimal payout)
		{
			String[] RString = { "Unknown", null };
			decimal rewardamount = 0;
			payout = 0;

			if (m_jackpotmultiplier[index] >= 0 && (index <= 10 && index > -1))
			{
				if (index == 0)
				{
					int p = m_Cost * m_jackpotmultiplier[0];
					if ((m_ProgressiveMaster != null && !m_ProgressiveMaster.Deleted && ((TurboSlot)m_ProgressiveMaster).ProgIsMaster) || m_isProgMaster)
					{
						if (m_isProgMaster)
							p += m_ProgressiveJackpot;
						else
							p += ((TurboSlot)m_ProgressiveMaster).ProgJackpot;
					}
					payout = p;
					RString[0] = p.ToString();
					if (m_Rewards != JackpotRewardType.None)
					{
						RString = GetRewardStr(RString, index, (payout == 0) ? 0 : 1, out rewardamount);
						payout += rewardamount;
					}
					payout = payout / m_Cost;
					return RString;
				}
				else
				{
					if (index == 9 || index == 10)
					{
						payout = ((int)(m_Cost * ((float)m_jackpotmultiplier[index] / 100.00)));
						RString[0] = payout.ToString();
						payout = payout * 100;
						return RString;
					}
					else
					{
						payout = (m_Cost * m_jackpotmultiplier[index]);
						RString[0] = payout.ToString();
						if (m_Rewards != JackpotRewardType.None)
							RString = GetRewardStr(RString, index, (payout == 0) ? 0 : 1, out rewardamount);
						payout += rewardamount;
						payout = payout / m_Cost;
						return RString;
					}
				}
			}
			if (m_jackpotmultiplier[index] == -1)
			{
				string[] sreturn ={ "Free", "Spin!", null };
				payout = m_Cost;
				return sreturn;
			}
			if (m_jackpotmultiplier[index] == -2)
			{
				string[] sreturn ={ "0", null, null };
				payout = 0;
				return sreturn;
			}
			return RString;
		}

		private string[] GetRewardStr(string[] RString, int index, int sindex, out decimal rewardamount)
		{
			rewardamount = 0;
			switch (m_SlotTheme)
			{
				case SlotThemeType.GruesomeGambling:
					switch (index)
					{
						case 0:
							RString[sindex] = "Hooded Shroud";
							if (sindex == 0)
								RString[1] = "Of Shadows";
							rewardamount = 500000;
							return RString;

						default:
							break;
					}
					break;
#if MINIHOUSES
				case SlotThemeType.MiniHouses:
					if (index == 0)
					{
						if (sindex == 0)
						{
							RString[0] = "Random Pirate";
							RString[1] = "Ship Model";
						}
						else
							if (m_Rewards == JackpotRewardType.RewardAndCash)
							{
								RString[0] = RString[0] + " +";
								RString[sindex] = "25%/Pirate Ship";
							}
							else
								RString[sindex] = "Pirate Ship";
						if (m_Rewards == JackpotRewardType.RewardOnly)
							rewardamount = 40000;
						else
							rewardamount = 5000;
						return RString;
					}
					break;
#endif
				case SlotThemeType.LadyLuck:
					switch (index)
					{
						case 0:
							if (sindex == 0)
							{
								RString[0] = "Luck Robe";
								RString[1] = "or Necklace";
							}
							else
								RString[sindex] = "Luck Item";
							rewardamount = 1000000;
							return RString;

						case 1:
							RString[sindex] = "Luck Cloak";
							rewardamount = 500000;
							return RString;

						case 2:
							RString[sindex] = "Luck Sash";
							rewardamount = 250000;
							return RString;

						default:
							break;
					}
					break;

				case SlotThemeType.Pirates:
					if (index == 0)
					{
						if (sindex == 0)
						{
							RString[0] = "Random Pirate";
							RString[1] = "Ship Model";
						}
						else
							if (m_Rewards == JackpotRewardType.RewardAndCash)
							{
								RString[0] = RString[0] + " +";
								RString[sindex] = "25%/Pirate Ship";
							}
							else
								RString[sindex] = "Pirate Ship";
						if (m_Rewards == JackpotRewardType.RewardOnly)
							rewardamount = 40000;
						else
							rewardamount = 5000;
						return RString;
					}
					break;

				case SlotThemeType.PowerScrolls:
					switch (index)
					{
						case 0:
							RString[sindex] = "+20 Scroll";
							rewardamount = 2000000;
							return RString;

						case 1:
							RString[sindex] = "+15 Scroll";
							rewardamount = 1500000;
							return RString;

						case 2:
							RString[sindex] = "+10 Scroll";
							rewardamount = 750000;
							return RString;

						case 8:
							if (sindex == 0)
							{
								RString[0] = "+5 Scroll";
								RString[1] = "(1% Chance)";
							}
							else
							{
								RString[0] = RString[0] + " & +5";
								RString[sindex] = "(1% Chance)";
							}
							rewardamount = 12500;
							return RString;

						default:
							break;
					}
					break;

				case SlotThemeType.StatScrolls:
					if (index == 0)
					{
						if (sindex == 0)
						{
							RString[0] = "Random (5-25)";
							RString[1] = "Stat Scroll";
						}
						else
							RString[sindex] = "+ Stat Scroll";
						rewardamount = 2000000;
						return RString;
					}
					break;

				case SlotThemeType.TailorTreats:
					switch (index)
					{
						case 0:
							if (sindex == 0)
							{
								RString[0] = "Barbed Runic";
								RString[1] = "Sewing Kit";
							}
							else
							{
								if (m_Rewards == JackpotRewardType.RewardAndCash)
								{
									RString[0] = RString[0] + " +";
									RString[sindex] = "15% Kit Chance";
								}
								else
								{
									RString[sindex] = "+ Barbed Kit";
								}
							}
							if (m_Rewards == JackpotRewardType.RewardAndCash && m_Profile) // Artificially lower value due to % chance
								rewardamount = 300000;
							else
								rewardamount = 2000000;
							return RString;

						case 1:
							if (sindex == 0)
							{
								RString[0] = "Horned Runic";
								RString[1] = "Sewing Kit";
							}
							else
							{
								RString[0] = RString[0] + " + 30%";
								RString[sindex] = "Kit Chance";
							}
							if (m_Rewards == JackpotRewardType.RewardAndCash && m_Profile)
								rewardamount = 75000;
							else
								rewardamount = 250000;
							return RString;

						case 2:
							if (sindex == 0)
							{
								RString[0] = "Spined Runic";
								RString[1] = "Sewing Kit";
							}
							else
							{
								RString[0] = RString[0] + " + 50%";
								RString[sindex] = "Kit Chance";
							}
							if (m_Rewards == JackpotRewardType.RewardAndCash && m_Profile)
								rewardamount = 62500;
							else
								rewardamount = 125000;
							return RString;

						case 6:
							RString[0] = RString[0] + " + 3%";
							RString[1] = "Cloth Chance";
							return RString;

						default:
							break;
					}
					break;

				default:
					break;
			}
			return RString;
		}

		public override void OnDelete()
		{
			base.OnDelete();
			if (m_isProgMaster && m_SlotSlaves != null)
			{
				foreach (TurboSlot s in m_SlotSlaves)
				{
					if (!s.Deleted)
						s.RemoveMaster(this);
				}
			}
			if (m_ProgressiveMaster != null && !m_ProgressiveMaster.Deleted)
				((TurboSlot)m_ProgressiveMaster).RemoveSlave(this);
			if (this.m_InUseBy != null && m_Won > 0)
				DoCashOut(m_InUseBy);
		}

		#region Progressive Jackpot Region
		public void AddToJackpot(int amount)
		{
			if (!m_isProgMaster)
				return;
			m_ProgressiveJackpot += amount;
			//InvalidateProperties();
			return;
		}

		public void JackpotHit(Mobile from, int jackpot, TurboSlot slot)
		{
			if (!m_isProgMaster)
				return;
			m_ProgressiveJackpot = m_DefaultStartProgressive;
#if PROFILE
			if (m_Profile)
				return;
#endif
			Point3D loc = Point3D.Zero;
			ArrayList announcedSlots = new ArrayList();
			announcedSlots.Add(this);
			//InvalidateProperties();		
			Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, 1035);
			from.FixedParticles(0x375A, 9, 20, 5027, EffectLayer.Waist);
			Effects.SendLocationEffect(new Point3D(this.X, this.Y, this.Z), this.Map, 0x375A, 15, this.Hue, 0);
			string text = String.Format("{0} has won a Progressive Jackpot valued at {1} gold!!!", from.Name, jackpot);
			if (m_AnnounceJackpot && jackpot > 500000)  // Don't report the small jackpots
				AnnounceJackpot(from, text);
			this.PublicOverheadMessage(0, this.Hue, false, text);
			if (m_SlotSlaves != null)
			{

				foreach (TurboSlot s in m_SlotSlaves)   // Announce the jackpot, but not to every machine to avoid a messy screen
				{
					bool announce = true;
					s.ActivateBlinkTimer(this.Hue);
					foreach (TurboSlot a in announcedSlots)
					{
						if (Utility.InRange(s.Location, a.Location, 4))
						{
							announce = false;
							break;
						}
					}
					if (announce)
					{
						s.PublicOverheadMessage(0, this.Hue, false, text);
						Effects.PlaySound(new Point3D(s.X, s.Y, s.Z), s.Map, 1035);
						announcedSlots.Add(s);
					}
					Effects.SendLocationEffect(new Point3D(s.X, s.Y, s.Z), s.Map, 0x375A, 15, this.Hue, 0);
					loc = s.Location;
				}
			}
		}

		public void AddSlave(TurboSlot s)
		{
			if (m_SlotSlaves != null && m_SlotSlaves.Contains(s))
				return;
			m_SlotSlaves.Add(s);
		}

		public void RemoveSlave(TurboSlot s)
		{
			if (m_SlotSlaves != null && m_SlotSlaves.Contains(s))
				m_SlotSlaves.Remove(s);
		}

		public void RemoveMaster(TurboSlot s)
		{
			if (m_ProgressiveMaster != null && m_ProgressiveMaster == s)
				m_ProgressiveMaster = null;
		}
		private void AnnounceJackpot(Mobile from, string text)
		{
#if PROFILE
			if (m_Profile)
				return;
#endif
			foreach (Server.Network.NetState state in Server.Network.NetState.Instances)
			{
				Mobile m = state.Mobile;
				if (m != null && m != from)
				{
					m.PlaySound(1460);
					m.SendMessage(text);
				}
			}
		}
		#endregion

		public void DoSpin(Mobile from)
		{
			int r1index = m_ReelTable[0, Utility.Random(m_MaxRoll)];
			m_ReelOne = m_Symbols[r1index];
			m_ReelTwo = m_Symbols[m_ReelTable[1, Utility.Random(m_MaxRoll)]];
			m_ReelThree = m_Symbols[m_ReelTable[2, Utility.Random(m_MaxRoll)]];
			m_TotalSpins += 1;
#if PROFILE
			if (m_TotalSpins % 1000000 == 0 && m_Profile)
			{
				if (WinningPercentage > m_ProfPercentagehigh)
					m_ProfPercentagehigh = WinningPercentage;
				if (WinningPercentage < m_ProfPercentagelow || m_ProfPercentagelow == 0f)
					m_ProfPercentagelow = WinningPercentage;
			}
			if (m_TestMode)
			{
				if (m_ErrorCode < 0 && m_ErrorCode > -11)  // Use m_ErrorCode to force a specific jackpot (-10 for 0).
				{
					if (m_ErrorCode == -10)
						m_TestSpin = 0;
					else if (m_ErrorCode > -9)
						m_TestSpin = Math.Abs(m_ErrorCode);
				}
				if (m_TestSpin < 8)
				{
					r1index = m_TestSpin;
					m_ReelOne = m_ReelTwo = m_ReelThree = m_Symbols[m_TestSpin];
					m_TestSpin++;
				}
				else if (m_TestSpin == 8)
				{
					r1index = 0;
					m_ReelOne = m_Symbols[0];
					m_ReelTwo = m_Symbols[1];
					m_ReelThree = m_Symbols[Utility.Random(3)];
					m_TestSpin++;
				}
				else
					if (m_TestSpin >= m_TotalSymbols)
						m_TestSpin = 0;
			}

			if (!m_Profile)
#endif
				from.PlaySound(739);
			if (from.Hidden && from.AccessLevel == AccessLevel.Player) // Don't let someone sit at the slots and play hidden
			{
				from.Hidden = false;
				from.SendMessage("Playing the slot machine reveals you!");
			}
			m_FreeSpin = false;
			if ((m_ProgressiveMaster != null && !m_ProgressiveMaster.Deleted && ((TurboSlot)m_ProgressiveMaster).ProgIsMaster) || m_isProgMaster)
			{
				if (m_ProgressivePercent != 0)
				{
					float amount = ((float)m_Cost * (float)(m_ProgressivePercent / 100.00));
					if (amount < 1)
						amount = 1;

					if (m_isProgMaster)
						AddToJackpot((int)amount);
					else

						((TurboSlot)m_ProgressiveMaster).AddToJackpot((int)amount);
				}
			}

			if (m_ReelOne == m_ReelTwo && m_ReelOne == m_ReelThree) // Jackpot!
			{
				if (r1index == m_Symbols[8] && m_BonusRound != BonusRoundType.None && !m_Profile)
				{
					switch (m_BonusRound)
					{
						case BonusRoundType.MinerMadness:
							{
								m_jackpotStats[5]++;
								this.PublicOverheadMessage(0, this.Hue, false, "-Bonus Round!-");
								from.SendMessage(38, "You have hit the bonus round.");
								from.PlaySound(1035);
								from.SendGump(new NewMinerBonusGump(this, m_Symbols, true, new bool[] { false, false, false, false, false, false, false, false, false }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0));
								if (m_Won > m_CreditCashOut && m_InUseBy.AccessLevel == AccessLevel.Player)
									DoCashOut(from);
								return;
							}

						default:
							{
								from.SendMessage(38, "Unknown Bonus Round. Slot machine needs maintenance.");
								SlotOffline(8004);
								return;
							}
					}
				}
				else
				{
					m_GiveReward = DetermineReward(r1index);
					switch (r1index)
					{
						case 0:
							{
								int p = m_Cost * m_jackpotmultiplier[0];
								decimal payout = p;
								if (m_GiveReward)
								{
									GetJackpotPayoutStr(r1index, out payout);
									payout = payout * m_Cost;
								}
								if ((m_ProgressiveMaster != null && !m_ProgressiveMaster.Deleted && ((TurboSlot)m_ProgressiveMaster).ProgIsMaster) || m_isProgMaster)
								{
									if (m_isProgMaster)
									{
										p += m_ProgressiveJackpot;
										if (!m_GiveReward)
											payout += m_ProgressiveJackpot;
										DoSpinWin(from, r1index, p, m_InUseBy.Female ? m_FemaleSounds[r1index] : m_MaleSounds[r1index], m_Sounds[r1index], m_JackpotText[r1index], m_JackpotEffect[r1index]);
										ActivateBlinkTimer(0);
										JackpotHit(from, (int)payout, this);
									}
									else
									{
										p += ((TurboSlot)m_ProgressiveMaster).ProgJackpot;
										if (!m_GiveReward)
											payout += ((TurboSlot)m_ProgressiveMaster).ProgJackpot;
										DoSpinWin(from, r1index, p, m_InUseBy.Female ? m_FemaleSounds[r1index] : m_MaleSounds[r1index], m_Sounds[r1index], m_JackpotText[r1index], m_JackpotEffect[r1index]);
										ActivateBlinkTimer(0);
										((TurboSlot)m_ProgressiveMaster).JackpotHit(from, (int)payout, this);
									}
									if (m_MembershipCard && payout > 499999 && !m_Profile)
										IssueMembershipCard(from, (int)payout);
									break;
								}
								else if (m_AnnounceJackpot && payout > 500000) // Don't report the small jackpots
								{
									string text = String.Format("{0} has won a Jackpot valued at {1} gold!!!", from.Name, payout);
									AnnounceJackpot(from, text);
								}
								DoSpinWin(from, r1index, p, m_InUseBy.Female ? m_FemaleSounds[r1index] : m_MaleSounds[r1index], m_Sounds[r1index], m_JackpotText[r1index], m_JackpotEffect[r1index]);
								ActivateBlinkTimer(0);
								if (m_MembershipCard && payout > 499999 && !m_Profile)
									IssueMembershipCard(from, (int)payout);
								break;
							}

						case 1:
							DoSpinWin(from, r1index);
							ActivateBlinkTimer(0);
							break;

						default:
							{
								DoSpinWin(from, r1index);
								break;
							}
					}
				}
			}
			else
			{
				if (m_AnyBars && AnyBarJackpot())
				{
					DoSpinWin(from, 8, m_Cost * m_jackpotmultiplier[8], m_InUseBy.Female ? m_FemaleSounds[8] : m_MaleSounds[8], m_Sounds[8], m_JackpotText[8], 0);
				}
				else
				{

					switch (CountScatter(m_Symbols[m_Symbols[9]], m_ReelOne, m_ReelTwo, m_ReelThree))
					{
						case 1:
							{
								if (m_jackpotmultiplier[9] == -1)
									DoSpinWin(from, 9);
								else
									DoSpinWin(from, 9, (int)(m_Cost * ((float)m_jackpotmultiplier[9] / 100.00)));
								break;
							}

						case 2:
							{
								if (m_jackpotmultiplier[10] == -1)
									DoSpinWin(from, 10);
								else
									DoSpinWin(from, 10, (int)(m_Cost * ((float)m_jackpotmultiplier[10] / 100.00)));
								break;
							}

						default:
							{
								if (!m_Profile)
								{
									from.SendMessage("Sorry you didnt win, Try Again!");
									if (Utility.Random(1000) == 45 && m_PlayerSounds)
									{
										if (m_InUseBy.Female)
											from.PlaySound(Utility.RandomList(1372, 1373, 816, 796, 782));
										else
											from.PlaySound(Utility.RandomList(1372, 1373, 1090, 1068, 1053));
									}
									if (m_SlotTheme == SlotThemeType.GruesomeGambling)
									{
										if (Utility.Random(100) == 45)
											ThrowOutBlood(from, 2, 2);
										if (Utility.Random(2500) == 45 || from.Map == Map.Felucca && Utility.Random(100) < 2)
										{
											from.SendMessage("You anger the demon in this machine!");
											BeginBleed(from);
										}
									}
									else if (m_SlotTheme == SlotThemeType.StatScrolls)
									{
										if (Utility.Random(500) == 45 || from.Map == Map.Felucca && 0.03 > Utility.RandomDouble())
										{
											from.SendMessage("Tentacles attack!");
											DoDrain(from);
										}
									}
									else if (m_SlotTheme == SlotThemeType.LadyLuck && Utility.Random(10000) == 45)
										ThrowOutGold(from, 5, 10);
								}
								m_LastPay = 0;
								break;
							}
					}
				}
			}
			if ((m_Won > m_CreditCashOut || !m_Active) && m_InUseBy.AccessLevel < AccessLevel.Counselor)
				DoCashOut(from);
			if (!m_Profile && m_Active)
				from.SendGump(new TurboSlotGump(this, m_Symbols));
		}

		private bool AnyBarJackpot()
		{
			foreach (int reel1 in m_Bars)
			{
				if (m_ReelOne == reel1)
				{
					foreach (int reel2 in m_Bars)
					{
						if (m_ReelTwo == reel2)
						{
							foreach (int reel3 in m_Bars)
							{
								if (m_ReelThree == reel3)
									return true;
							}
						}
					}
				}
			}
			return false;
		}

		private void DoSpinWin(Mobile from, int r1index)
		{
			if (m_jackpotmultiplier[r1index] >= 0)
				DoSpinWin(from, r1index, m_Cost * m_jackpotmultiplier[r1index], m_InUseBy.Female ? m_FemaleSounds[r1index] : m_MaleSounds[r1index], m_Sounds[r1index], m_JackpotText[r1index], m_JackpotEffect[r1index]);
			else
			{
				if (m_jackpotmultiplier[r1index] == -1)
				{
					if (!m_Profile)
						from.SendMessage(38, "Free Spin!");
					m_jackpotStats[r1index]++;
					m_FreeSpin = true;
					return;
				}
				if (m_jackpotmultiplier[r1index] == -2)
				{
					if (!m_Profile)
						from.SendMessage("Sorry you didnt win, Try Again!");
					return;
				}
				if (!m_Profile)
					from.SendMessage(38, "Unknown payout multiplier. Slot machine needs maintenance.");
				SlotOffline(8005);
			}
		}

		private void DoSpinWin(Mobile from, int r1index, int winnings)
		{
			decimal payout = winnings;
			m_jackpotStats[r1index]++;
			m_Won += winnings;
			if (m_Rewards != JackpotRewardType.None && r1index < 9)
			{
				GetJackpotPayoutStr(r1index, out payout);
				payout = payout * m_Cost;
			}
			m_TotalWon += (int)payout;
			m_LastPay = winnings;
			if (r1index < 9)
				UpdateLastWonBy(from, r1index, (int)payout);
			if (m_Profile || winnings == 0)
				return;
			from.SendMessage(38, "You win {0} Gold!", winnings);
		}

		private void DoSpinWin(Mobile from, int r1index, int winnings, int mobileSound, int jackpotSound, string slotText, int JackpotEffect)
		{

			DoSpinWin(from, r1index, winnings);
#if PROFILE
			if (m_Profile)
				return;
#endif
			if (mobileSound != -1 && m_PlayerSounds)
				from.PlaySound(mobileSound);
			if (jackpotSound != -1)
				Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, jackpotSound);
			if (slotText != null)
				this.PublicOverheadMessage(0, this.Hue, false, slotText);
			if (JackpotEffect != 0)
				DoJackpotEffect(from, JackpotEffect, r1index);
			if (m_Rewards != JackpotRewardType.None)
				GiveRewards(from, r1index);
		}

		private void UpdateLastWonBy(Mobile m, int r1index, int jackpotamount)
		{
			if (m_LastWonBy == null || m_LastWonBy.Deleted)
			{
				m_LastWonBy = m;
				m_LastWonByDate = DateTime.Now;
				m_LastWonAmount = jackpotamount;
			}
			else
			{
				TimeSpan timespan = DateTime.Now - m_LastWonByDate;
				if (m_LastWonAmount <= jackpotamount || r1index == 0 || TimeSpan.FromDays(30) < timespan)
				{
					m_LastWonBy = m;
					m_LastWonByDate = DateTime.Now;
					m_LastWonAmount = jackpotamount;
				}
			}
		}

		private int CountScatter(int ScatterReel, int m_ReelOne, int m_ReelTwo, int m_ReelThree)
		{
			if (m_ScatterPay == ScatterType.None)
				return 0;
			if (m_ScatterPay == ScatterType.LeftOnly)
			{
				if (ScatterReel == m_ReelOne && ScatterReel == m_ReelTwo)
					return 2;
				if (ScatterReel == m_ReelOne)
					return 1;
				return 0;
			}
			int count = 0;
			if (ScatterReel == m_ReelOne)
				count++;
			if (ScatterReel == m_ReelTwo)
				count++;
			if (ScatterReel == m_ReelThree)
				count++;
			return count;
		}

		private void GiveRewards(Mobile from, int jackpotindex)
		{
			Item item = null;
			switch (m_SlotTheme)
			{
				case SlotThemeType.GruesomeGambling:
					{
						switch (jackpotindex)
						{
							case 0:             //Hooded Shroud of Shadows
								{
									Type t = typeof(HoodedShroudOfShadows);
									item = CreateItem(from, t, 10001);
									if (item == null)
										return;
									item.Name = "Hooded Shroud Of Shadows";
									GiveItem(from, item);
									break;
								}

							default:
								break;
						}
						break;
					}

				case SlotThemeType.LadyLuck:
					{

						switch (jackpotindex)
						{
							case 0:             //Lucky Necklace or Robe (50/50 chance for either)
								{
									if (Utility.Random(2) == 0)
									{
										Type t = typeof(GoldNecklace);
										item = CreateItem(from, t, 10002);
										if (item == null)
											return;
										item.Hue = 1150;
										item.LootType = LootType.Blessed;
										((BaseJewel)item).Attributes.Luck = 200;
										item.Name = "Lucky Necklace";
										GiveItem(from, item);
									}
									else
									{
										Type t = typeof(LadyLuckRobe);
										item = CreateItem(from, t, 10003);
										if (item == null)
											return;
										GiveItem(from, item);
									}
									break;
								}

							case 1:             //Lucky Cloak
								{
									Type t = typeof(LadyLuckCloak);
									item = CreateItem(from, t, 10004);
									if (item == null)
										return;
									GiveItem(from, item);
									break;
								}

							case 2:             //Lucky Sash
								{
									Type t = typeof(LadyLuckSash);
									item = CreateItem(from, t, 10005);
									if (item == null)
										return;
									GiveItem(from, item);
									break;
								}
							default:
								break;
						}
						break;
					}
#if MINIHOUSES
				case SlotThemeType.MiniHouses:
					if (jackpotindex == 0)
					{
						if (!m_GiveReward)
						{
							from.SendMessage("Sorry, no ship model this time matey!");
							break;
						}
						Type t = typeof(ShipModelOfTheHMSCape);
						item = CreateItem(from, t, 10009);
						if (item == null)
							return;
						NameAndHueModel(item);
						item.LootType = LootType.Blessed;
						GiveItem(from, item);
					}
					break;
#endif
				case SlotThemeType.Pirates:
					if (jackpotindex == 0)
					{
						if (!m_GiveReward)
						{
							from.SendMessage("Sorry, no ship model this time matey!");
							break;
						}
						Type t = typeof(ShipModelOfTheHMSCape);
						item = CreateItem(from, t, 10009);
						if (item == null)
							return;
						NameAndHueModel(item);
						item.LootType = LootType.Blessed;
						GiveItem(from, item);
					}
					break;

				case SlotThemeType.PowerScrolls:
					if (jackpotindex >= 0 && jackpotindex < 3)
					{
						item = CreatePowerScroll(m_Symbols[jackpotindex], 20 - (jackpotindex * 5));
						if (item != null)
							GiveItem(from, item);
					}
					else if (jackpotindex == 8)
					{
						if (Utility.Random(100) == 45)
						{
							item = CreatePowerScroll(m_ReelThree, 5);
							if (item != null)
							{
								item.LootType = LootType.Blessed;
								GiveItem(from, item);
							}
						}
					}
					break;

				case SlotThemeType.StatScrolls:
					if (jackpotindex == 0)
					{
						int level;
						double random = Utility.RandomDouble();
						if (0.1 >= random)
							level = 25;
						else if (0.25 >= random)
							level = 20;
						else if (0.45 >= random)
							level = 15;
						else if (0.70 >= random)
							level = 10;
						else
							level = 5;
						item = new StatCapScroll(225 + level);
						if (item != null)
							GiveItem(from, item);
						else
							SlotOffline(10010);
					}
					break;

				case SlotThemeType.TailorTreats:
					{
						if (jackpotindex >= 0 && jackpotindex <= 2)
						{
							if (!m_GiveReward)
							{
								from.SendMessage("Sorry, no Runic Kit on this Jackpot.");
								m_GiveReward = true;
								break;
							}
							try { item = CreateRunicKit(3 - jackpotindex); }
							catch
							{
								from.SendMessage("This slot machine has a problem!");
								from.SendMessage("An error code has been recorded, page a GM for help.");
								if (jackpotindex == 0)
									SlotOffline(10006); // Set an ErrorCode
								else if (jackpotindex == 1)
									SlotOffline(10007);
								else
									SlotOffline(10008);
								return;
							}
							if (jackpotindex == 0)
								item.Name = "Barbed Leather Runic Sewing Kit";
							else if (jackpotindex == 1)
								item.Name = "Horned Leather Runic Sewing Kit";
							else
								item.Name = "Spined Leather Runic Sewing Kit";
							GiveItem(from, item);
						}
						else if (jackpotindex == 6)
						{
							if (Utility.Random(100) == 45 || Utility.Random(100) == 13 || Utility.Random(100) == 6)
							{
								Type t = typeof(UncutCloth);
								item = CreateItem(from, t, 10005);
								if (item == null)
									return;
								item.Hue = Utility.RandomList(1150, 1151, 1153, 1154, 1155, 1156, 1157, 1158, 1159, 1160, 1161, 1162, 1163, 1164,
								1165, 1166, 1167, 1168, 1169, 1170, 1171, 1172, 1173);
								item.Name = "Special Cloth";
								item.Amount = 10;
								from.PlaySound(1481);
								GiveItem(from, item);
							}
						}
						break;
					}

				default:
					break;
			}
		}

		private bool DetermineReward(int jackpotindex)
		{
			switch (m_SlotTheme)
			{
#if MINIHOUSES
				case SlotThemeType.MiniHouses:
					if (jackpotindex == 0 && m_Rewards == JackpotRewardType.RewardAndCash && 0.25 < Utility.RandomDouble())
					{
						return false;
					}
					break;
#endif
				case SlotThemeType.Pirates:
					if (jackpotindex == 0 && m_Rewards == JackpotRewardType.RewardAndCash && 0.25 < Utility.RandomDouble())
					{
						return false;
					}
					break;
				case SlotThemeType.TailorTreats:
					{
						int[] chance = new int[] { 14, 29, 49 };
						if (jackpotindex >= 0 && jackpotindex <= 2 && m_Rewards == JackpotRewardType.RewardAndCash && Utility.Random(100) > chance[jackpotindex])
						{
							return false;
						}
					}
					break;
			}
			return true;
		}

		private Item CreatePowerScroll(int type, int level)
		{
			PowerScroll ps = null;
			SkillName[] skillNames = new SkillName[]
			{ SkillName.Bushido, SkillName.Ninjitsu, SkillName.Necromancy, SkillName.Chivalry, SkillName.Focus,
                SkillName.Magery, SkillName.Swords, SkillName.Fencing, SkillName.Macing, SkillName.Archery,
                SkillName.Blacksmith, SkillName.Musicianship, SkillName.AnimalTaming};
			//10923,10924,10925,10926,10927,10928,10929,10930,10931,10932,10933,10934,10935
			try
			{
				if (type == 10927)
					ps = PowerScroll.CreateRandomNoCraft(level, level);
				else
				{
					SkillName skillName = skillNames[type - 10923];
					ps = new PowerScroll(skillName, 100 + level);
				}
			}
			catch { return null; }
			return (Item)ps;
		}

		private Item CreateItem(Mobile from, Type t, int errorcode)
		{
			Item i = null;
			try { i = Loot.Construct(t); }
			catch
			{
				from.SendMessage("This slot machine has a problem creating your item!");
				from.SendMessage("An error code has been recorded, page a GM for help.");
				SlotOffline(errorcode);
			}
			return i;
		}

		private static Item CreateRunicKit(int type)
		{
			return new RunicSewingKit(CraftResource.RegularLeather + type, 60 - (type * 15));
		}

		private void NameAndHueModel(Item item)
		{
			string[] shipnames = new string[] { "The Horrid Pearl of the West" , "The Cursed Raider of the Sargasso Sea",
                                                "The Anger of Tortuga", "The Bloody Plague of the Caribbean",
                                                "The Poison Star of the East", "The Damned Pearl of the South",
                                                "The Black Saber", "The Red Grail of the North", "The Vile Rage",
                                                "The Dark Horror", "Hades' Greed", "The Horrid Murderer", "The Horrid Cry",
                                                "Davy Jones' Insanity", "Neptune's Cruelty", "Posideon's Howl" };
			if (0.03 > Utility.RandomDouble())
			{
				string[] rareshipnames = new string[] { "CEO's Domination of the West", "Warlocke's Fable of Atlantis", "Roadkill's Virus of the Sea" };
				item.Name = rareshipnames[Utility.Random(rareshipnames.Length)];
				item.Hue = 1150;
				return;
			}

			item.Name = shipnames[Utility.Random(shipnames.Length)];
			item.Hue = Utility.RandomList(43, 47, 694, 908, 1718);

		}

		private void IssueMembershipCard(Mobile to, int jackpot)
		{
			Item item = null;
			item = new CasinoMembershipCard();
			if (item != null)
			{
				((CasinoMembershipCard)item).ClubMember = to;
				((CasinoMembershipCard)item).Game = this.Name;
				((CasinoMembershipCard)item).Jackpot = jackpot;
				GiveItem(to, item);
			}
			else
				SlotOffline(10011);
		}

		private void GiveItem(Mobile to, Item i)
		{
			if (to == null || i == null)
				return;

			Container pack = to.Backpack;
			string text = null;
			if (pack != null)
			{
				if (pack.TryDropItem(to, i, false))
				{
					text = String.Format("{0} {1} has been placed in your backpack!", i.Amount > 1 ? "Some" : "A", i.Name);
				}
				else
				{
					to.BankBox.DropItem(i);
					text = String.Format("{0} {1} has been placed in your bankbox!", i.Amount > 1 ? "Some" : "A", i.Name);
				}
				if (i is PowerScroll || i is StatCapScroll)
				{
					DoPowerScrollEffect(to);
					to.SendLocalizedMessage(1049524); // You have received a scroll of power!
				}
				else if (i is ShipModelOfTheHMSCape)
					to.SendMessage(this.Hue, "You have received a model pirate ship!");// You have received a scroll of power!
				else
					to.SendMessage(this.Hue, text);
			}
		}

		private void DoJackpotEffect(Mobile m, int JackpotEffect, int index)
		{
			switch (m_SlotTheme)
			{
				case SlotThemeType.GruesomeGambling:
					if (JackpotEffect == -1)
					{
						FlameStrike();
						if (this.Map == Map.Felucca)
							ThrowOutBones(m, 1);
					}
					else if (JackpotEffect == -2 || JackpotEffect == -3)
					{
						Sparkle(JackpotEffect);
						if (this.Map == Map.Felucca)
							ThrowOutBones(m, Math.Abs(JackpotEffect) - 1);
					}
					else
						ThrowOutBones(m, JackpotEffect);
					if (index < 4)
					{
						switch (Utility.Random(3))
						{
							case 0:
								Sparkle(-2);
								break;
							case 1:
								Sparkle(-3);
								break;
							default:
								FlameStrike();
								break;
						}
						ThrowOutBlood(m, 3 + Utility.Random(3), 3);
						if (index == 0)
							ThrowOutBlood(m, 5, 4);
					}
					break;

				case SlotThemeType.LadyLuck:
					if (index == 0)
						ThrowOutGold(m, 15, 30);
					break;

				default:
					for (int i = 0; i < JackpotEffect; i++)
						DoFireworks(m);
					break;
			}
		}

		private void ThrowOutBones(Mobile from, int count)
		{
			ThrowOutItem(from, count, 0, 3, 1);
		}

		private void ThrowOutBlood(Mobile from, int count, int distance)
		{
			ThrowOutItem(from, count, 1, distance, 1);
		}

		private void ThrowOutGold(Mobile from, int min, int max)
		{
			this.PublicOverheadMessage(0, this.Hue, false, "Share the wealth!");
			int totalstacks = min + Utility.Random(max);
			for (int i = 0; i < totalstacks; ++i)
			{
				int amount = 6 + Utility.Random(94);
				m_TotalWon += amount;
				ThrowOutItem(from, 1, 2, 6, amount);
			}
		}

		private void ThrowOutItem(Mobile from, int count, int itemtype, int distance, int amount)
		{
			Map map = from.Map;

			for (int i = 0; i < count; ++i)
			{
				int x = from.X + Utility.RandomMinMax(-distance, distance);
				int y = from.Y + Utility.RandomMinMax(-distance, distance);
				int z = from.Z;
				if (!map.CanFit(x, y, z, 16, false, true))
				{
					z = map.GetAverageZ(x, y);

					if (z == from.Z || !map.CanFit(x, y, z, 16, false, true))
						continue;
				}
				Object o = null;
				try
				{
					if (itemtype == 0)
					{
						if (this.Map == Map.Felucca)
							o = (Object)new UnholyBone();
						else
							o = (Object)Loot.Construct(typeof(Bone));
					}
					else if (itemtype == 1)
						o = new GruesomeBlood();
					else if (itemtype == 2)
						o = new Gold();
				}
				catch { }
				if (o != null)
				{
					((Item)o).Hue = 0;
					((Item)o).Amount = amount;
					if (itemtype == 0)
					{
						if (this.Map == Map.Felucca)
							((Item)o).Name = "unholy bones";
						else
							((Item)o).Name = "bones";
						((Item)o).ItemID = Utility.Random(0xECA, 9);
					}
					((Item)o).MoveToWorld(new Point3D(x, y, z), map);
					if (itemtype == 2)
						GoldEffect((Item)o);
				}
			}
		}

		private void GoldEffect(Item g)
		{
			if (0.5 >= Utility.RandomDouble())
			{
				switch (Utility.Random(3))
				{
					case 0: // Fire column
						{
							Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
							Effects.PlaySound(g, g.Map, 0x208);
							break;
						}
					case 1: // Explosion
						{
							Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
							Effects.PlaySound(g, g.Map, 0x307);
							break;
						}
					case 2: // Ball of fire
						{
							Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);
							break;
						}
				}
			}
		}

        private void BeginBleed(Mobile m)
        {
            if (m == null) return;
            m.SendLocalizedMessage(1060160); // You are bleeding!
            m.PlaySound(0x133);
            m.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);
            BleedAttack.BeginBleed(m, m, false);
        }


		private void DoDrain(Mobile m)
		{
			if (m == null) return;
			m.FixedParticles(0x374A, 10, 15, 5013, 0x455, 0, EffectLayer.Waist);
			m.PlaySound(0x1F1);
			int drain = Utility.RandomMinMax(14, 36);
			if (this.Map == Map.Felucca)
				drain = Utility.RandomMinMax(23, 46);
			m.Damage(drain, m);
		}

		private void FlameStrike()
		{
			try
			{
				Effects.SendLocationEffect(new Point3D(this.X, this.Y, this.Z + 1), this.Map, 0x3709, 15, this.Hue - 1, 0);
				Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, 0x208);
			}
			catch { }
		}

		private void Sparkle(int JackpotEffect)
		{
			try
			{
				if (JackpotEffect == -2)
				{
					Effects.SendLocationEffect(new Point3D(this.X, this.Y, this.Z + 1), this.Map, 0x375A, 15, this.Hue - 1, 0);
					Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, 0x213);
				}
				else
				{
					Effects.SendLocationEffect(new Point3D(this.X, this.Y + 1, this.Z), this.Map, 0x373A, 15, this.Hue - 1, 0);
					Effects.SendLocationEffect(new Point3D(this.X + 1, this.Y, this.Z), this.Map, 0x373A, 15, this.Hue - 1, 0);
					Effects.SendLocationEffect(new Point3D(this.X, this.Y, this.Z - 1), this.Map, 0x373A, 15, this.Hue - 1, 0);
				}
			}
			catch { }

		}

		private void DoPowerScrollEffect(Mobile from)
		{
			Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
			Effects.PlaySound(from.Location, from.Map, 0x243);

			Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
			Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
			Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

			Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

		}

		private void DoFireworks(Mobile m)
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

		public void DoCashOut(Mobile from)
		{
			int credit = m_Won;
			if (from == null || m_Won == 0)
				return;
			if (!m_Active && (m_ErrorCode == 9500 || m_ErrorCode == 9501 || m_ErrorCode == 9502)) // Prevent a loop cashing out
				return;
			if (from.Serial != m_InUseBy.Serial)
			{
				from.SendMessage("You are no longer playing this machine!");
				return;
			}
			if (m_Won < 0) // This should never happen but protect against some kind of overflow and a wild payout
			{
				if (from.AccessLevel >= AccessLevel.GameMaster) // Allow a GM to clear out the invalid amount
				{
					from.SendMessage("Invalid gold won amount({0}), reset to 0.", m_Won);
					m_Won = 0;
				}
				from.SendMessage("There's a problem with this machine's gold amount, this slot machine is offline. Page for help.");
				SlotOffline(9502);
				return;
			}
			if (m_Won < 1000)
			{
				try
				{
					from.AddToBackpack(new Gold(m_Won));
					from.SendMessage("{0} gold has been added to your pack.", credit);
				}
				catch
				{
					from.SendMessage("There's a problem returning your gold, this slot machine is offline. Page for help.");
					SlotOffline(9500);
					return;
				}
			}
			else
			{
				try
				{
					from.AddToBackpack(new BankCheck(m_Won));
					from.SendMessage("A bank check for {0} gold has been placed in your pack.", credit);
				}
				catch
				{
					from.SendMessage("There's a problem returning your gold, this slot machine is offline. Page for help.");
					SlotOffline(9501);
					return;
				}

			}
			m_Won = 0;
			if (credit >= 10000)
			{
				string text = String.Format("{0} is cashing out {1} Gold!", m_InUseBy.Name, credit);
				this.PublicOverheadMessage(0, this.Hue, false, text);
			}

			from.PlaySound(52);
			from.PlaySound(53);
			from.PlaySound(54);
			from.PlaySound(55);
		}

		private void SlotOffline(int error)
		{
			m_ErrorCode = error;
			Active = false;
			if (this.m_InUseBy != null && this.m_InUseBy.Map != Map.Internal)
			{
				try
				{
					this.m_InUseBy.CloseGump(typeof(NewMinerBonusGump));
					this.m_InUseBy.CloseGump(typeof(TurboSlotPayTableGump));
					Effects.PlaySound(new Point3D(this.X, this.Y, this.Z), this.Map, 484);
				}
				catch { }
			}
		}

		#region Odds Table Region
		private void SetupOddsTable(PaybackType p, bool force)
		{
			if (m_RandomActivated && !force)
				return;
			int luck = 0;
			if (m_InUseBy != null)
				luck = m_InUseBy.Luck;
			m_CurrentPaybackType = p;
			if (m_PaybackType == PaybackType.Random)
				m_CurrentPaybackType = (PaybackType)Utility.Random((int)PaybackType.Loose, (int)PaybackType.Random);
			if (luck > 800 && (luck > Utility.Random(2000)) && m_CurrentPaybackType != PaybackType.Loose)
				m_CurrentPaybackType--;
			if (luck > 800 || m_PaybackType == PaybackType.Random && m_InUseBy != null)
				ActivateRandomTimer();
			switch (m_CurrentPaybackType)
			{
				case PaybackType.Loose:
					LooseSlots();
					break;

				case PaybackType.Normal:
					NormalSlots();
					break;

				case PaybackType.Tight:
					TightSlots();
					break;

				case PaybackType.ExtremelyTight:
					ExtremelyTightSlots();
					break;

				case PaybackType.CasinoCheats:
					CasinoCheats();
					break;

				default:
					NormalSlots();
					break;
			}
			m_ProfPercentage = CalcOdds(m_CurrentDist);
			InvalidateProperties();
		}

		private void LooseSlots()
		{
			switch (m_SlotTheme)
			{
				case SlotThemeType.MinerMadness:
					{
						int[] reel1 = { 3, 3, 6, 9, 15, 17, 21, 26 };
						int[] reel2 = { 2, 2, 6, 8, 15, 18, 23, 26 };
						int[] reel3 = { 1, 2, 5, 8, 12, 20, 24, 28 };
						// int[] reel1 = { 2, 3, 4, 5, 8, 11, 15, 18 }; * 3 The original MinerMadness table of 165% payout.
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.GruesomeGambling:
					{
						int[] reel1 = { 5, 6, 6, 6, 12, 13, 19, 33 };
						int[] reel2 = { 2, 3, 5, 7, 10, 14, 24, 35 };
						int[] reel3 = { 2, 4, 5, 7, 10, 13, 24, 35 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;

							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 4, 4, 4, 7, 9, 15, 21, 36 };
								reel2 = new int[] { 2, 3, 3, 8, 11, 12, 24, 37 };
								reel3 = new int[] { 2, 2, 3, 8, 10, 12, 25, 38 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;

							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 4, 4, 4, 7, 9, 15, 21, 36 };
								reel2 = new int[] { 2, 3, 3, 8, 11, 12, 24, 37 };
								reel3 = new int[] { 2, 2, 3, 8, 10, 12, 25, 38 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.FarmerFaceoff:
					{
						int[] reel1 = { 4, 5, 6, 6, 9, 15, 24, 31 };
						int[] reel2 = { 3, 4, 5, 7, 8, 16, 24, 33 };
						int[] reel3 = { 2, 3, 7, 7, 9, 15, 24, 33 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.OffToTheRaces:
					{
						int[] reel1 = { 3, 3, 4, 4, 9, 16, 27, 34 };
						int[] reel2 = { 3, 4, 5, 7, 8, 16, 24, 33 };
						int[] reel3 = { 2, 3, 7, 7, 9, 16, 24, 32 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.LadyLuck:
					{
						int[] reel1 = { 5, 5, 5, 6, 14, 16, 22, 27 };
						int[] reel2 = { 2, 3, 4, 7, 12, 14, 25, 33 };
						int[] reel3 = { 2, 2, 2, 7, 12, 14, 26, 35 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 4, 4, 4, 7, 9, 14, 21, 37 };
								reel2 = new int[] { 3, 3, 3, 8, 11, 12, 24, 36 };
								reel3 = new int[] { 1, 2, 4, 9, 11, 12, 25, 36 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 5, 5, 4, 6, 12, 14, 22, 32 };
								reel2 = new int[] { 2, 3, 3, 7, 11, 13, 24, 37 };
								reel3 = new int[] { 1, 2, 2, 7, 10, 12, 26, 40 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.Classic:
					{
						int[] reel1 = { 4, 5, 6, 6, 7, 9, 27 };
						int[] reel2 = { 3, 4, 4, 5, 6, 7, 35 };
						int[] reel3 = { 1, 2, 3, 5, 6, 7, 40 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicII:
					{
						int[] reel1 = { 3, 5, 8, 17, 19, 19, 19, 10 };
						int[] reel2 = { 2, 3, 6, 17, 17, 17, 20, 18 };
						int[] reel3 = { 2, 2, 6, 13, 15, 16, 21, 25 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicIII:
					{
						int[] reel1 = { 3, 4, 5, 5, 8, 18, 21, 36 };
						int[] reel2 = { 3, 6, 8, 8, 10, 12, 19, 34 };
						int[] reel3 = { 2, 3, 4, 6, 9, 11, 24, 41 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.Holiday1:
					{
						int[] reel1 = { 3, 4, 5, 9, 11, 18, 23, 27 };
						int[] reel2 = { 2, 3, 6, 11, 10, 13, 21, 34 };
						int[] reel3 = { 2, 2, 2, 11, 11, 12, 23, 37 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.PowerScrolls:
					{
						int[] reel1 = { 2, 5, 3, 16, 14, 18, 18, 24 };
						int[] reel2 = { 2, 1, 2, 16, 17, 15, 20, 27 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 2, 3, 4, 14, 16, 17, 18, 26 };
								reel2 = new int[] { 1, 1, 1, 16, 17, 13, 21, 30 };
								reel3 = new int[] { 1, 1, 1, 15, 13, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 1, 2, 3, 16, 15, 17, 18, 28 };
								reel2 = new int[] { 1, 1, 1, 16, 17, 13, 21, 30 };
								reel3 = new int[] { 1, 1, 1, 15, 13, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.TailorTreats:
					{
						int[] reel1 = { 2, 5, 3, 16, 14, 18, 18, 24 };
						int[] reel2 = { 2, 1, 2, 16, 17, 15, 20, 27 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 1, 5, 3, 16, 14, 18, 18, 25 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 2, 5, 3, 16, 16, 17, 17, 24 };
								reel2 = new int[] { 1, 1, 2, 15, 17, 16, 20, 28 };
								reel3 = new int[] { 1, 1, 1, 15, 13, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}
				case SlotThemeType.StatScrolls:
					goto case SlotThemeType.TailorTreats;

				case SlotThemeType.TrophyHunter:
					{
						int[] reel1 = { 3, 4, 7, 12, 12, 14, 22, 26 };
						int[] reel2 = { 3, 3, 5, 10, 11, 18, 23, 27 };
						int[] reel3 = { 2, 2, 4, 9, 10, 19, 24, 30 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}
				case SlotThemeType.Pirates:
					goto case SlotThemeType.TrophyHunter;

#if MINIHOUSES
				case SlotThemeType.MiniHouses:
					goto case SlotThemeType.TrophyHunter;
#endif
				default:
					string text = String.Format("Unknown Theme ({0})! Slot Machine Offline.", m_SlotTheme);
					this.PublicOverheadMessage(0, this.Hue, false, text);
					SlotOffline(8006);
					break;
			}
		}

		private void NormalSlots()
		{
			switch (m_SlotTheme)
			{
				case SlotThemeType.MinerMadness:
					{
						int[] reel1 = { 2, 3, 6, 8, 13, 17, 23, 28 };
						int[] reel2 = { 2, 2, 6, 8, 14, 18, 23, 27 };
						int[] reel3 = { 1, 1, 7, 8, 11, 22, 24, 26 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.GruesomeGambling:
					{
						int[] reel1 = { 5, 6, 6, 6, 10, 13, 20, 34 };
						int[] reel2 = { 3, 4, 4, 7, 10, 12, 23, 37 };
						int[] reel3 = { 1, 4, 4, 6, 9, 12, 24, 40 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;

							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 2, 4, 5, 8, 11, 15, 23, 32 };
								reel2 = new int[] { 2, 2, 3, 8, 12, 13, 24, 36 };
								reel3 = new int[] { 2, 2, 2, 7, 11, 12, 25, 39 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 2, 4, 5, 8, 11, 15, 23, 32 };
								reel2 = new int[] { 2, 2, 3, 8, 12, 13, 24, 36 };
								reel3 = new int[] { 2, 2, 2, 7, 11, 12, 25, 39 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.FarmerFaceoff:
					{
						int[] reel1 = { 4, 5, 5, 6, 9, 16, 24, 31 };
						int[] reel2 = { 3, 4, 5, 7, 8, 16, 24, 33 };
						int[] reel3 = { 2, 3, 7, 7, 9, 16, 24, 32 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.OffToTheRaces:
					{
						int[] reel1 = { 3, 3, 4, 4, 9, 16, 27, 34 };
						int[] reel2 = { 3, 3, 5, 8, 8, 16, 25, 32 };
						int[] reel3 = { 2, 3, 7, 6, 10, 16, 24, 32 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.LadyLuck:
					{
						int[] reel1 = { 4, 4, 6, 10, 10, 14, 21, 31 };
						int[] reel2 = { 2, 3, 3, 9, 11, 12, 24, 36 };
						int[] reel3 = { 2, 2, 3, 7, 11, 12, 25, 38 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 3, 4, 4, 7, 10, 15, 21, 36 };
								reel2 = new int[] { 3, 3, 3, 8, 10, 13, 24, 36 };
								reel3 = new int[] { 1, 2, 4, 9, 10, 11, 26, 37 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 2, 3, 4, 10, 11, 14, 22, 34 };
								reel2 = new int[] { 2, 2, 2, 9, 11, 13, 25, 36 };
								reel3 = new int[] { 2, 2, 2, 8, 10, 12, 24, 40 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.Classic:
					{
						int[] reel1 = { 4, 5, 6, 6, 7, 8, 28 };
						int[] reel2 = { 3, 4, 4, 5, 5, 6, 37 };
						int[] reel3 = { 1, 2, 3, 4, 6, 6, 42 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicII:
					{
						int[] reel1 = { 3, 5, 8, 17, 19, 19, 19, 10 };
						int[] reel2 = { 2, 3, 6, 17, 17, 17, 20, 18 };
						int[] reel3 = { 2, 2, 6, 13, 13, 15, 22, 27 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicIII:
					{
						int[] reel1 = { 3, 4, 5, 5, 8, 18, 21, 36 };
						int[] reel2 = { 3, 6, 7, 8, 10, 13, 19, 34 };
						int[] reel3 = { 2, 3, 4, 6, 9, 11, 24, 41 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.Holiday1:
					{
						int[] reel1 = { 3, 4, 6, 10, 11, 19, 22, 25 };
						int[] reel2 = { 2, 3, 5, 11, 12, 14, 21, 32 };
						int[] reel3 = { 1, 1, 2, 11, 12, 14, 24, 35 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.PowerScrolls:
					{
						int[] reel1 = { 2, 5, 3, 15, 15, 17, 18, 25 };
						int[] reel2 = { 2, 1, 2, 16, 17, 13, 20, 29 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 2, 3, 4, 13, 17, 17, 18, 26 };
								reel2 = new int[] { 1, 1, 1, 16, 18, 13, 20, 30 };
								reel3 = new int[] { 1, 1, 1, 15, 13, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 1, 2, 3, 15, 14, 17, 18, 30 };
								reel2 = new int[] { 1, 1, 1, 16, 18, 13, 20, 30 };
								reel3 = new int[] { 1, 1, 1, 15, 13, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.TailorTreats:
					{
						int[] reel1 = { 2, 5, 3, 15, 15, 17, 18, 25 };
						int[] reel2 = { 2, 1, 2, 16, 17, 13, 20, 29 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel2 = new int[] { 1, 1, 2, 16, 18, 13, 20, 29 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 2, 5, 3, 15, 16, 17, 18, 24 };
								reel2 = new int[] { 1, 1, 2, 15, 16, 14, 20, 31 };
								reel3 = new int[] { 1, 1, 1, 16, 13, 17, 24, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}
				case SlotThemeType.StatScrolls:
					goto case SlotThemeType.TailorTreats;

				case SlotThemeType.TrophyHunter:
					{
						int[] reel1 = { 3, 4, 6, 10, 14, 15, 22, 26 };
						int[] reel2 = { 3, 3, 5, 10, 10, 18, 23, 28 };
						int[] reel3 = { 2, 2, 4, 10, 12, 17, 24, 29 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}
				case SlotThemeType.Pirates:
					goto case SlotThemeType.TrophyHunter;
#if MINIHOUSES
				case SlotThemeType.MiniHouses:
					goto case SlotThemeType.TrophyHunter;
#endif
				default:
					string text = String.Format("Unknown Theme ({0})! Slot Machine Offline.", m_SlotTheme);
					this.PublicOverheadMessage(0, this.Hue, false, text);
					SlotOffline(8007);
					break;
			}
		}

		private void TightSlots()
		{
			switch (m_SlotTheme)
			{
				case SlotThemeType.MinerMadness:
					{
						int[] reel1 = { 2, 2, 7, 10, 14, 16, 23, 26 };
						int[] reel2 = { 1, 2, 7, 7, 14, 21, 23, 25 };
						int[] reel3 = { 1, 1, 5, 7, 14, 23, 23, 26 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.GruesomeGambling:
					{
						int[] reel1 = { 2, 3, 5, 9, 15, 19, 23, 24 };
						int[] reel2 = { 2, 3, 5, 8, 15, 19, 23, 25 };
						int[] reel3 = { 2, 3, 5, 8, 15, 19, 23, 25 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;

							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 2, 3, 4, 9, 12, 15, 24, 31 };
								reel2 = new int[] { 2, 3, 2, 9, 12, 13, 24, 35 };
								reel3 = new int[] { 1, 1, 2, 6, 11, 14, 25, 40 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;

							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 2, 3, 4, 9, 12, 15, 24, 31 };
								reel2 = new int[] { 2, 3, 2, 9, 12, 13, 24, 35 };
								reel3 = new int[] { 1, 1, 2, 6, 11, 14, 25, 40 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.FarmerFaceoff:
					{
						int[] reel1 = { 4, 5, 4, 5, 9, 16, 24, 33 };
						int[] reel2 = { 3, 4, 5, 7, 8, 16, 24, 33 };
						int[] reel3 = { 2, 3, 7, 7, 9, 16, 24, 32 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.OffToTheRaces:
					{
						int[] reel1 = { 2, 3, 4, 5, 9, 16, 27, 34 };
						int[] reel2 = { 3, 3, 5, 8, 8, 16, 25, 32 };
						int[] reel3 = { 2, 3, 5, 7, 10, 16, 24, 33 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.LadyLuck:
					{
						int[] reel1 = { 4, 4, 6, 8, 10, 13, 22, 33 };
						int[] reel2 = { 2, 3, 3, 9, 11, 12, 24, 36 };
						int[] reel3 = { 2, 2, 3, 8, 10, 12, 25, 38 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 3, 5, 5, 10, 11, 15, 20, 31 };
								reel2 = new int[] { 2, 3, 2, 9, 12, 13, 24, 35 };
								reel3 = new int[] { 1, 1, 2, 7, 11, 12, 25, 41 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 2, 3, 4, 8, 12, 15, 21, 35 };
								reel2 = new int[] { 2, 2, 2, 9, 11, 13, 25, 36 };
								reel3 = new int[] { 2, 2, 2, 8, 10, 12, 24, 40 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.Classic:
					{
						int[] reel1 = { 4, 5, 6, 6, 7, 8, 28 };
						int[] reel2 = { 3, 4, 4, 4, 4, 4, 41 };
						int[] reel3 = { 1, 2, 3, 4, 5, 6, 43 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicII:
					{
						int[] reel1 = { 3, 5, 8, 17, 19, 19, 19, 10 };
						int[] reel2 = { 2, 3, 6, 17, 17, 17, 20, 18 };
						int[] reel3 = { 1, 2, 5, 14, 14, 15, 22, 27 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicIII:
					{
						int[] reel1 = { 3, 4, 5, 5, 8, 18, 21, 36 };
						int[] reel2 = { 3, 6, 7, 7, 7, 15, 19, 36 };
						int[] reel3 = { 2, 3, 4, 6, 8, 12, 24, 41 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.Holiday1:
					{
						int[] reel1 = { 2, 4, 5, 11, 12, 19, 22, 25 };
						int[] reel2 = { 2, 3, 4, 11, 10, 13, 21, 36 };
						int[] reel3 = { 1, 1, 2, 11, 10, 13, 25, 37 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.PowerScrolls:
					{
						int[] reel1 = { 2, 4, 3, 14, 14, 20, 21, 22 };
						int[] reel2 = { 2, 1, 2, 16, 17, 15, 21, 26 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 1, 2, 3, 15, 15, 17, 18, 29 };
								reel2 = new int[] { 1, 1, 1, 15, 17, 13, 21, 31 };
								reel3 = new int[] { 1, 1, 1, 14, 14, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 1, 2, 3, 15, 15, 17, 18, 29 };
								reel2 = new int[] { 1, 1, 1, 15, 16, 13, 21, 32 };
								reel3 = new int[] { 1, 1, 1, 14, 14, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.TailorTreats:
					{
						int[] reel1 = { 2, 4, 3, 14, 14, 20, 21, 22 };
						int[] reel2 = { 2, 1, 2, 16, 17, 15, 21, 26 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel2 = new int[] { 1, 1, 2, 16, 18, 15, 21, 26 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel2 = new int[] { 1, 1, 2, 16, 17, 15, 21, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}
				case SlotThemeType.StatScrolls:
					goto case SlotThemeType.TailorTreats;

				case SlotThemeType.TrophyHunter:
					{
						int[] reel1 = { 3, 5, 5, 10, 14, 14, 23, 26 };
						int[] reel2 = { 2, 3, 5, 11, 11, 15, 24, 29 };
						int[] reel3 = { 1, 3, 3, 10, 9, 19, 24, 31 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}
				case SlotThemeType.Pirates:
					goto case SlotThemeType.TrophyHunter;
#if MINIHOUSES
				case SlotThemeType.MiniHouses:
					goto case SlotThemeType.TrophyHunter;
#endif
				default:
					string text = String.Format("Unknown Theme ({0})! Slot Machine Offline.", m_SlotTheme);
					this.PublicOverheadMessage(0, this.Hue, false, text);
					SlotOffline(8008);
					break;
			}
		}

		private void ExtremelyTightSlots()
		{
			switch (m_SlotTheme)
			{
				case SlotThemeType.MinerMadness:
					{
						int[] reel1 = new int[] { 1, 2, 7, 9, 17, 17, 22, 25 };
						int[] reel2 = new int[] { 1, 2, 6, 8, 14, 21, 23, 25 };
						int[] reel3 = new int[] { 1, 1, 4, 8, 13, 22, 24, 27 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.GruesomeGambling:
					{
						int[] reel1 = new int[] { 2, 3, 3, 10, 12, 15, 25, 30 };
						int[] reel2 = new int[] { 1, 2, 2, 8, 13, 15, 25, 34 };
						int[] reel3 = new int[] { 1, 1, 2, 5, 11, 12, 27, 41 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								reel1 = new int[] { 2, 3, 5, 8, 13, 18, 22, 29 };
								CreateOddsTable(reel1);
								break;

							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 2, 3, 3, 10, 12, 15, 25, 30 };
								reel2 = new int[] { 1, 2, 2, 8, 13, 15, 25, 34 };
								reel3 = new int[] { 1, 1, 2, 5, 11, 12, 27, 41 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;

							case JackpotRewardType.RewardAndCash:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.FarmerFaceoff:
					{
						int[] reel1 = { 3, 4, 4, 5, 11, 16, 24, 33 };
						int[] reel2 = { 3, 4, 5, 6, 8, 16, 24, 34 };
						int[] reel3 = { 2, 3, 6, 7, 9, 16, 24, 33 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.OffToTheRaces:
					{
						int[] reel1 = { 2, 3, 4, 5, 9, 16, 27, 34 };
						int[] reel2 = { 3, 3, 5, 8, 8, 16, 25, 32 };
						int[] reel3 = { 2, 3, 5, 6, 10, 16, 24, 34 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.LadyLuck:
					{
						int[] reel1 = { 5, 3, 6, 9, 10, 13, 22, 32 };
						int[] reel2 = { 2, 3, 3, 9, 11, 12, 24, 36 };
						int[] reel3 = { 1, 2, 3, 8, 10, 12, 25, 39 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 4, 3, 3, 10, 11, 15, 25, 29 };
								reel2 = new int[] { 1, 3, 2, 10, 12, 13, 24, 35 };
								reel3 = new int[] { 1, 1, 2, 7, 11, 12, 25, 41 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 2, 3, 4, 8, 14, 15, 24, 30 };
								reel2 = new int[] { 2, 2, 2, 9, 11, 13, 25, 36 };
								reel3 = new int[] { 1, 1, 1, 11, 10, 12, 24, 40 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.Classic:
					{
						int[] reel1 = { 4, 4, 5, 6, 6, 8, 31 };
						int[] reel2 = { 3, 4, 4, 6, 6, 7, 34 };
						int[] reel3 = { 1, 2, 3, 4, 5, 7, 42 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicII:
					{
						int[] reel1 = { 3, 5, 8, 17, 19, 19, 19, 10 };
						int[] reel2 = { 2, 3, 6, 16, 17, 17, 21, 18 };
						int[] reel3 = { 1, 2, 5, 13, 13, 14, 24, 28 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicIII:
					{
						int[] reel1 = { 3, 4, 5, 5, 8, 18, 21, 36 };
						int[] reel2 = { 3, 6, 6, 7, 8, 15, 19, 36 };
						int[] reel3 = { 2, 3, 4, 5, 7, 12, 24, 43 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.Holiday1:
					{
						int[] reel1 = { 2, 3, 4, 11, 14, 19, 22, 25 };
						int[] reel2 = { 1, 2, 3, 12, 12, 13, 21, 36 };
						int[] reel3 = { 1, 1, 2, 9, 11, 14, 25, 37 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.PowerScrolls:
					{
						int[] reel1 = { 2, 3, 4, 15, 14, 19, 21, 22 };
						int[] reel2 = { 1, 1, 1, 15, 17, 15, 21, 29 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 1, 2, 3, 14, 15, 17, 19, 29 };
								reel2 = new int[] { 1, 1, 1, 15, 15, 13, 22, 32 };
								reel3 = new int[] { 1, 1, 1, 14, 14, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 1, 2, 3, 14, 14, 17, 19, 30 };
								reel2 = new int[] { 1, 1, 1, 15, 14, 14, 22, 32 };
								reel3 = new int[] { 1, 1, 1, 14, 14, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.TailorTreats:
					{
						int[] reel1 = { 2, 3, 4, 15, 14, 19, 21, 22 };
						int[] reel2 = { 1, 1, 1, 15, 17, 15, 21, 29 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 2, 3, 4, 14, 15, 19, 21, 22 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 2, 3, 4, 14, 14, 19, 22, 22 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}
				case SlotThemeType.StatScrolls:
					goto case SlotThemeType.TailorTreats;

				case SlotThemeType.TrophyHunter:
					{
						int[] reel1 = { 3, 4, 5, 10, 14, 14, 23, 27 };
						int[] reel2 = { 1, 3, 4, 11, 11, 15, 25, 30 };
						int[] reel3 = { 1, 2, 3, 10, 10, 19, 24, 31 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.Pirates:
					goto case SlotThemeType.TrophyHunter;
#if MINIHOUSES
				case SlotThemeType.MiniHouses:
					goto case SlotThemeType.TrophyHunter;
#endif
				default:
					string text = String.Format("Unknown Theme ({0})! Slot Machine Offline.", m_SlotTheme);
					this.PublicOverheadMessage(0, this.Hue, false, text);
					SlotOffline(8009);
					break;
			}
		}

		private void CasinoCheats()
		{
			switch (m_SlotTheme)
			{
				case SlotThemeType.MinerMadness:
					{
						int[] reel1 = { 1, 2, 5, 10, 12, 21, 23, 26 };
						int[] reel2 = { 1, 2, 6, 8, 14, 21, 23, 25 };
						int[] reel3 = { 1, 1, 5, 7, 11, 23, 24, 28 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.GruesomeGambling:
					{
						int[] reel1 = new int[] { 1, 3, 3, 8, 10, 16, 24, 35 };
						int[] reel2 = new int[] { 1, 1, 2, 9, 11, 16, 25, 35 };
						int[] reel3 = new int[] { 1, 1, 1, 6, 9, 17, 27, 38 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								reel1 = new int[] { 2, 3, 5, 7, 11, 14, 24, 34 };
								CreateOddsTable(reel1);
								break;

							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 1, 3, 3, 8, 10, 16, 24, 35 };
								reel2 = new int[] { 1, 1, 2, 9, 11, 16, 25, 35 };
								reel3 = new int[] { 1, 1, 1, 6, 9, 17, 27, 38 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;

							case JackpotRewardType.RewardAndCash:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.FarmerFaceoff:
					{
						int[] reel1 = { 3, 4, 4, 5, 11, 16, 24, 33 };
						int[] reel2 = { 3, 4, 5, 6, 8, 16, 24, 34 };
						int[] reel3 = { 1, 2, 7, 7, 9, 16, 24, 34 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.OffToTheRaces:
					{
						int[] reel1 = { 2, 3, 4, 5, 9, 16, 27, 34 };
						int[] reel2 = { 3, 3, 5, 8, 8, 16, 25, 32 };
						int[] reel3 = { 2, 2, 4, 6, 10, 16, 24, 36 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.LadyLuck:
					{
						int[] reel1 = { 3, 3, 4, 9, 11, 14, 21, 35 };
						int[] reel2 = { 2, 3, 3, 9, 11, 12, 24, 36 };
						int[] reel3 = { 1, 1, 1, 9, 10, 12, 25, 41 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 2, 3, 3, 8, 10, 16, 23, 35 };
								reel2 = new int[] { 2, 2, 3, 9, 11, 14, 24, 35 };
								reel3 = new int[] { 1, 1, 1, 8, 10, 14, 27, 38 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 1, 3, 3, 7, 13, 13, 24, 36 };
								reel2 = new int[] { 1, 1, 2, 10, 12, 15, 24, 35 };
								reel3 = new int[] { 1, 1, 1, 10, 11, 13, 25, 38 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.Classic:
					{
						int[] reel1 = { 4, 4, 5, 6, 6, 8, 31 };
						int[] reel2 = { 3, 3, 4, 6, 6, 7, 35 };
						int[] reel3 = { 1, 2, 3, 4, 5, 7, 42 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicII:
					{
						int[] reel1 = { 2, 4, 8, 17, 20, 20, 19, 10 };
						int[] reel2 = { 2, 3, 6, 16, 17, 17, 21, 18 };
						int[] reel3 = { 1, 2, 4, 12, 13, 14, 24, 30 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.ClassicIII:
					{
						int[] reel1 = { 3, 4, 5, 5, 8, 18, 21, 36 };
						int[] reel2 = { 3, 6, 6, 7, 8, 15, 19, 36 };
						int[] reel3 = { 2, 3, 3, 4, 7, 12, 24, 45 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.Holiday1:
					{
						int[] reel1 = { 2, 3, 3, 12, 14, 19, 22, 25 };
						int[] reel2 = { 1, 1, 2, 10, 13, 16, 22, 35 };
						int[] reel3 = { 1, 1, 1, 8, 11, 15, 26, 37 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}

				case SlotThemeType.PowerScrolls:
					{
						int[] reel1 = { 1, 2, 3, 14, 14, 21, 21, 24 };
						int[] reel2 = { 1, 1, 1, 14, 17, 15, 21, 30 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 1, 2, 3, 13, 15, 17, 19, 30 };
								reel2 = new int[] { 1, 1, 1, 13, 15, 13, 23, 33 };
								reel3 = new int[] { 1, 1, 1, 14, 14, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 1, 2, 3, 13, 14, 17, 19, 31 };
								reel2 = new int[] { 1, 1, 1, 13, 15, 13, 23, 33 };
								reel3 = new int[] { 1, 1, 1, 14, 14, 17, 25, 27 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}

				case SlotThemeType.TailorTreats:
					{
						int[] reel1 = { 1, 2, 3, 14, 14, 21, 21, 24 };
						int[] reel2 = { 1, 1, 1, 14, 17, 15, 21, 30 };
						int[] reel3 = { 1, 1, 1, 15, 13, 17, 25, 27 };
						switch (m_Rewards)
						{
							case JackpotRewardType.None:
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardOnly:
								reel1 = new int[] { 1, 2, 3, 14, 13, 22, 21, 24 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
							case JackpotRewardType.RewardAndCash:
								reel1 = new int[] { 1, 2, 3, 14, 12, 22, 22, 24 };
								CreateOddsTable(0, reel1);
								CreateOddsTable(1, reel2);
								CreateOddsTable(2, reel3);
								break;
						}
						break;
					}
				case SlotThemeType.StatScrolls:
					goto case SlotThemeType.TailorTreats;

				case SlotThemeType.TrophyHunter:
					{
						int[] reel1 = { 3, 3, 5, 9, 14, 15, 24, 27 };
						int[] reel2 = { 1, 3, 4, 10, 11, 16, 25, 30 };
						int[] reel3 = { 1, 2, 3, 10, 12, 19, 23, 30 };
						CreateOddsTable(0, reel1);
						CreateOddsTable(1, reel2);
						CreateOddsTable(2, reel3);
						break;
					}
				case SlotThemeType.Pirates:
					goto case SlotThemeType.TrophyHunter;
#if MINIHOUSES
				case SlotThemeType.MiniHouses:
					goto case SlotThemeType.TrophyHunter;
#endif
				default:
					string text = String.Format("Unknown Theme ({0})! Slot Machine Offline.", m_SlotTheme);
					this.PublicOverheadMessage(0, this.Hue, false, text);
					SlotOffline(8010);
					break;
			}
		}

		private void CreateOddsTable(int[] distTable)
		{
			for (int i = 0; i < 3; i++)
				CreateOddsTable(i, distTable);
		}

		private void CreateOddsTable(int table, int[] distTable)
		{
			int r = m_TotalSymbols - 1;
			int[] oddsTable = {
					r, r, r, r, r, r, r, r, r, r,  
					r, r, r, r, r, r, r, r, r, r,  
					r, r, r, r, r, r, r, r, r, r,  
					r, r, r, r, r, r, r, r, r, r,  
					r, r, r, r, r, r, r, r, r, r,  
					r, r, r, r, r, r, r, r, r, r,  
					r, r, r, r, r, r, r, r, r, r,  
					r, r, r, r, r, r, r, r, r, r,  
					r, r, r, r, r, r, r, r, r, r,  
					r, r, r, r, r, r, r, r, r, r};
			int index;
			bool searching = true;
			if (distTable.Length < m_TotalSymbols)
			{
				string text = String.Format("Your distribution table needs to have {0}(It has {1}) values! Slot Machine Offline.", m_TotalSymbols, distTable.Length);
				this.PublicOverheadMessage(0, this.Hue, false, text);
				SlotOffline(8001);
				return;
			}
			int maxRoll = 0;
			for (int i = 0; i < m_TotalSymbols; i++)
				maxRoll += distTable[i];
			if (maxRoll != 100 && maxRoll != 64) // && maxRoll != 66) for profile MinerMadness only... All machines should have 100 or 64 tables.
			{
				string text = String.Format("{0}. Distribution table {1} must total 100 or 64(It totals {2})! Slot Machine Offline.", m_PaybackType, table, maxRoll);
				this.PublicOverheadMessage(0, this.Hue, false, text);
				SlotOffline(8002);
				return;
			}
			m_MaxRoll = maxRoll;
			for (int i = 0; i < m_TotalSymbols; i++)
				m_CurrentDist[table, i] = distTable[i];

			for (int i = 0; i < m_MaxRoll; i++)
				m_ReelTable[table, i] = r;

			for (int i = 0; i < r; i++)
			{
				for (int h = 0; h < distTable[i]; h++)
				{
					searching = true;
					while (searching)
					{
						index = Utility.Random(m_MaxRoll);
						if (m_ReelTable[table, index] == r)
						{
							m_ReelTable[table, index] = i;
							searching = false;
						}

					}
				}
			}

		}

		private float CalcOdds(int[,] distTable)
		{
			decimal odds = 0m;
			decimal percentage = 0m;
			decimal payoutpercentage = 0m;
			decimal rolls = m_MaxRoll * m_MaxRoll * m_MaxRoll;
			decimal[] ptable = { 0, 0, 0, 0, 0, 0, 0, 0 };
			decimal anybarminus = 0;
			decimal scatterjackpotpercent = 0;
			decimal payout;
#if PROFILE
			if (m_Profile)
				Console.WriteLine("/*\nProfiling: Theme:{0}  Payout Table:{1}  Cost:{2}", m_SlotTheme, m_PaybackType, m_Cost);
			if (m_Profile && !m_ProfileAll)
			{
				Console.WriteLine("\nDistribution Tables:\n");
				for (int h = 0; h < 3; h++)
				{
					for (int i = 0; i < m_TotalSymbols - 1; i++)
					{
						Console.Write("{0},", distTable[h, i]);
					}
					Console.WriteLine("{0}", distTable[h, m_TotalSymbols - 1]);
				}
				Console.WriteLine("\nIndividual jackpot odds:");
			}
#endif
			for (int i = 0; i < m_TotalSymbols; i++)
			{
				percentage = (distTable[0, i] * distTable[1, i] * distTable[2, i]) / rolls;
				if (m_ScatterPay == ScatterType.LeftOnly && m_Symbols[9] == i)
					scatterjackpotpercent = percentage;
#if PROFILE
				if (m_Profile && !m_ProfileAll)
					Console.Write("{4}. {0:00}*{1:00}*{2:00}:{6:00}/{3}={5:0.00000}", distTable[0, i], distTable[1, i], distTable[2, i], rolls, i, percentage, distTable[0, i] * distTable[1, i] * distTable[2, i]);
#endif
				if (m_Rewards != JackpotRewardType.None)
					GetJackpotPayoutStr(i, out payout);
				else
					payout = (m_jackpotmultiplier[i] == -1) ? 1 : m_jackpotmultiplier[i];
				if (payout == -2)
					payout = 0;
				payoutpercentage = percentage * payout;
				ptable[i] = percentage;
				odds = odds + payoutpercentage;
#if PROFILE
				if (m_Profile && !m_ProfileAll)
					Console.WriteLine("*{0:0.00000}={1:0.00000} \tcumulative odds:{2:0.00000}", payout, payoutpercentage, odds);
#endif
			}

			//Any Bars
			if (m_AnyBars)
			{
				int[] tbars = new int[] { 0, 0, 0 };

				for (int h = 0; h < 4; h++)
				{
					for (int i = 0; i < m_TotalSymbols; i++)
					{
						if (m_Symbols[i] == m_Bars[h])
						{
							anybarminus += ptable[i];
							break;
						}

					}
				}
				for (int h = 0; h < 3; h++)
				{
					for (int i = 0; i < m_TotalSymbols; i++)
					{
						foreach (int reel in m_Bars)
						{
							if (m_Symbols[i] == reel)
							{
								tbars[h] = tbars[h] + distTable[h, i];
								break;
							}
						}

					}
				}
				percentage = (decimal)(((tbars[0] * tbars[1] * tbars[2]) / rolls));
				if (m_Rewards != JackpotRewardType.None)
					GetJackpotPayoutStr(8, out payout);
				else
					payout = (m_jackpotmultiplier[8] == -1) ? 1 : m_jackpotmultiplier[8];
				if (payout == -2)
					payout = 0;
				payoutpercentage = (decimal)((percentage - anybarminus) * payout);
				odds = odds + payoutpercentage;
#if PROFILE
				if (m_Profile & !m_ProfileAll)
					Console.WriteLine("\nAny 3: ({0}*{1}*{2}/{3})={4:#.000##}-{5:#.000##}={6:#.000##}*{7}={8:#.000##}", tbars[0], tbars[1], tbars[2], rolls, percentage, anybarminus, percentage - anybarminus, payout, payoutpercentage);
#endif
			}
			//Scatter 
			if (m_ScatterPay != ScatterType.None)
			{
				int[] tscatters = { 0, 0, 0 };
				int[] scattercount = { 0, 0, 0 };
				int sindex = m_Symbols[9];
				decimal scat2percent = 0;
				for (int h = 0; h < 3; h++)
					tscatters[h] = distTable[h, sindex];
				if (m_ScatterPay == ScatterType.Any)
				{
					scattercount[0] = tscatters[0] * tscatters[1] * (m_MaxRoll - tscatters[2]);
					scattercount[1] = tscatters[0] * (m_MaxRoll - tscatters[1]) * tscatters[2];
					scattercount[2] = (m_MaxRoll - tscatters[0]) * tscatters[1] * tscatters[2];
					percentage = ((scattercount[0] + scattercount[1] + scattercount[2]) / rolls);
				}
				else
				{
					scattercount[0] = tscatters[0] * tscatters[1] * m_MaxRoll;
					scattercount[1] = 0;// (m_MaxRoll - tscatters[0]) * tscatters[1] * (m_MaxRoll - tscatters[2]);
					scattercount[2] = 0;// (m_MaxRoll - tscatters[0]) * (m_MaxRoll - tscatters[1]) * tscatters[2];
					percentage = ((scattercount[0] + scattercount[1] + scattercount[2]) / rolls);
					scat2percent = percentage - scatterjackpotpercent;
				}
				payout = (m_jackpotmultiplier[10] == -1) ? 100 : m_jackpotmultiplier[10];
				payout = payout / 100;
				payoutpercentage = (decimal)((percentage - scatterjackpotpercent) * (payout));
				odds = odds + payoutpercentage;
#if PROFILE
				if (m_Profile && !m_ProfileAll)
				{
					Console.Write("\nScatter2: (");
					if (m_ScatterPay == ScatterType.Any)
					{
						Console.Write("{0}*{1}*{2}+", tscatters[0], tscatters[1], (m_MaxRoll - tscatters[2]));
						Console.Write("{0}*{1}*{2}+", tscatters[0], (m_MaxRoll - tscatters[1]), tscatters[2]);
						Console.Write("{0}*{1}*{2})/{3}", (m_MaxRoll - tscatters[0]), tscatters[1], tscatters[2], rolls);
					}
					else
					{
						Console.Write("{0}*{1}*{2}", tscatters[0], tscatters[1], m_MaxRoll);
						Console.Write("/{0})-{1:0.000##}={2:0.000##}", rolls, scatterjackpotpercent, percentage - scatterjackpotpercent);
					}
					Console.WriteLine("*{0:0.000##}={1:0.000##}", payout, payoutpercentage);
				}
#endif
				if (m_ScatterPay == ScatterType.Any)
				{
					scattercount[0] = tscatters[0] * (m_MaxRoll - tscatters[1]) * (m_MaxRoll - tscatters[2]);
					scattercount[1] = (m_MaxRoll - tscatters[0]) * tscatters[1] * (m_MaxRoll - tscatters[2]);
					scattercount[2] = (m_MaxRoll - tscatters[0]) * (m_MaxRoll - tscatters[1]) * tscatters[2];
				}
				else
				{
					scattercount[0] = tscatters[0] * m_MaxRoll * m_MaxRoll;
					scattercount[1] = 0;// (m_MaxRoll - tscatters[0]) * tscatters[1] * (m_MaxRoll - tscatters[2]);
					scattercount[2] = 0;// (m_MaxRoll - tscatters[0]) * (m_MaxRoll - tscatters[1]) * tscatters[2];
				}
				percentage = ((scattercount[0] + scattercount[1] + scattercount[2]) / rolls);
				payout = (m_jackpotmultiplier[9] == -1) ? 100 : m_jackpotmultiplier[9];
				payout = payout / 100;
				payoutpercentage = (decimal)((percentage - (scatterjackpotpercent + scat2percent)) * payout);
				odds = odds + payoutpercentage;
#if PROFILE
				if (m_Profile && !m_ProfileAll)
				{
					Console.Write("Scatter1: (");
					if (m_ScatterPay == ScatterType.Any)
					{
						Console.Write("{0}*{1}*{2}+", tscatters[0], (m_MaxRoll - tscatters[1]), (m_MaxRoll - tscatters[2]));
						Console.Write("{0}*{1}*{2}+", (m_MaxRoll - tscatters[0]), tscatters[1], (m_MaxRoll - tscatters[2]));
						Console.Write("{0}*{1}*{2}", (m_MaxRoll - tscatters[0]), (m_MaxRoll - tscatters[1]), tscatters[2]);
						Console.Write(")/{0}={1:0.000##}", rolls, percentage);

					}
					else
					{
						Console.Write("{0}*{1}*{2}/{3})-{4:0.000##}={5:0.000##}", tscatters[0], m_MaxRoll, m_MaxRoll, rolls, (scatterjackpotpercent + scat2percent), (percentage - scat2percent));
					}
					Console.WriteLine("*{0:0.000##}={1:0.000##}", payout, payoutpercentage);
				}
#endif
			}
#if PROFILE
			if (m_Profile)
				Console.WriteLine("\nStatistical payout odds for this slot machine is : {0:0.000##}%\n", odds * 100m);
#endif
			return (float)odds * 100;
		}
		#endregion



#if PROFILE
		private void ProfileAll()
		{
			//m_ProfileAll = true;
			for (int i = 0; i < (int)SlotThemeType.TrophyHunter; i++)
			{
				m_isProgMaster = false;
				m_ProgressiveJackpot = 0;
				m_ProgressivePercent = 0;
				m_SlotTheme = (SlotThemeType)i;
				SetupTheme(m_SlotTheme, true);
				for (int h = 0; h < (int)PaybackType.Random; h++)
				{
					m_PaybackType = (PaybackType)h;
					m_Profile = true;
					SetupOddsTable(m_PaybackType, true);
					Profile(false, 5);
					m_Profile = false;
				}
			}
			m_ProfileAll = false;
		}

		private void Profile(bool setuptheme, int count)
		{
			if (m_InUseBy == null || (m_InUseBy != null && m_InUseBy.AccessLevel == AccessLevel.Player))
			{
				this.PublicOverheadMessage(0, this.Hue, false, "The slot machine must be in use by a GM or higher in order to profile it.");
				return;
			}
			if (setuptheme)
			{
				m_isProgMaster = false;
				m_ProgressiveJackpot = 0;
				m_ProgressivePercent = 0;
				SetupTheme(m_SlotTheme, false);
			}
			if (!m_ProfileAll)
			{
				Console.Write("Symbol Table:");
				for (int i = 0; i < m_Symbols.Length; i++)
					Console.Write("{0},", m_Symbols[i]);
				if (m_AnyBars)
				{
					Console.Write("\nAny bar symbols:");
					for (int i = 0; i < 4; i++)
						Console.Write("{0} ", m_Bars[i]);
				}
				Console.Write("\nPay Table:");
				for (int i = 0; i < 11; i++)
					Console.Write("{0},", m_jackpotmultiplier[i]);
				Console.WriteLine("\n");
			}
			int spins = 20000000;
			int[] progressivelist = new int[] { 0, 5, 10, 15, 20 };
			if (m_ProfPercentage > 98)
				progressivelist = new int[] { 0, 1, 2, 3, 5 };
			else if (m_ProfPercentage > 95)
				progressivelist = new int[] { 0, 1, 3, 5, 7 };
			else if (m_ProfPercentage > 85)
				progressivelist = new int[] { 0, 3, 5, 7, 10 };
			if (m_ProfPercentage > 120)
				spins = 10000000;
			else if (m_ProfPercentage > 105)
				spins = 15000000;
			bool progmaster = m_isProgMaster;
			int progpercent = m_ProgressivePercent;
			for (int h = 0; h < count; h++)
			{
				m_ProgressiveJackpot = m_DefaultStartProgressive;
				if (progressivelist[h] == 0)
				{
					m_isProgMaster = false;
					m_ProgressiveJackpot = 0;
					m_ProgressivePercent = 0;
				}
				else
				{
					m_isProgMaster = true;
					m_ProgressivePercent = progressivelist[h];
				}
				m_Won = 0;
				m_TotalCollected = 0;
				m_TotalWon = 0;
				m_TotalNetProfit = 0;
				m_TotalSpins = 0;
				m_ProfPercentagelow = 0;
				m_ProfPercentagehigh = 0;

				for (int i = 0; i < 11; i++)
					m_jackpotStats[i] = 0;
				for (int i = 0; i < spins; i++)
				{
					m_TotalCollected += m_Cost;
					DoSpin(m_InUseBy);
					if (m_FreeSpin)
						m_TotalWon += m_Cost;
				}
				if (progressivelist[h] == 0)
					Console.WriteLine("@ {3} million spins:\t Low: {0:##0.00}% High: {1:##0.00}%  Actual: {2:##0.00}%\n", m_ProfPercentagelow, m_ProfPercentagehigh, WinningPercentage, spins / 1000000);
				else
					Console.WriteLine("Progressive @ {3}%:\t Low: {0:##0.00}% High: {1:##0.00}%  Actual: {2:##0.00}%", m_ProfPercentagelow, m_ProfPercentagehigh, WinningPercentage, progressivelist[h]);
			}
			Console.WriteLine("*/");
			m_isProgMaster = progmaster;
			m_ProgressivePercent = progpercent;
			Mobile from = m_InUseBy;
			from.CloseGump(typeof(TurboSlotGump));
			from.SendGump(new TurboSlotGump(this, m_Symbols));
			m_InUseBy = from;
		}
#endif
		#region Theme Setup
		private void SetupTheme(SlotThemeType theme, bool initialize)
		{
			switch (theme)
			{
				case SlotThemeType.MinerMadness:
					MinerMadness(initialize);
					break;

				case SlotThemeType.GruesomeGambling:
					GruesomeGambling(initialize);
					break;

				case SlotThemeType.FarmerFaceoff:
					FarmerFaceoff(initialize);
					break;

				case SlotThemeType.OffToTheRaces:
					OffToTheRaces(initialize);
					break;

				case SlotThemeType.LadyLuck:
					LadyLuck(initialize);
					break;

				case SlotThemeType.Classic:
					Classic(initialize);
					break;

				case SlotThemeType.ClassicII:
					ClassicII(initialize);
					break;

				case SlotThemeType.ClassicIII:
					ClassicIII(initialize);
					break;

				case SlotThemeType.Holiday1:
					Holiday1(initialize);
					break;
#if MINIHOUSES
				case SlotThemeType.MiniHouses:
					MiniHouses(initialize);
					break;
#endif
				case SlotThemeType.Pirates:
					Pirates(initialize);
					break;

				case SlotThemeType.PowerScrolls:
					PowerScrolls(initialize);
					break;

				case SlotThemeType.StatScrolls:
					StatScrolls(initialize);
					break;

				case SlotThemeType.TailorTreats:
					TailorTreats(initialize);
					break;

				case SlotThemeType.TrophyHunter:
					TrophyHunter(initialize);
					break;

				default:
					string text = String.Format("Unknown Theme ({0})! Slot Machine Offline.", m_SlotTheme);
					this.PublicOverheadMessage(0, this.Hue, false, text);
					SlotOffline(8003);
					break;
			}
			SetupOddsTable(m_PaybackType, true);
			m_ReelOne = m_Symbols[0];
			m_ReelTwo = m_Symbols[0];
			m_ReelThree = m_Symbols[0];
		}

		private void MinerMadness(bool initialize)
		{
			m_JackpotText = new string[] { "====> JACKPOT!!! <====", "***Big Winner!***", "**Winner**", "*Winner*", "-Winner-", null, null, null, "Winner" };
			m_JackpotEffect = new int[] { 5, 3, 1, 0, 0, 0, 0, 0, 0 };
			m_FemaleSounds = new int[] { 824, 823, 783, 794, 794, 797, 783, 823, 823 };
			m_MaleSounds = new int[] { 1098, 1097, 1054, 1066, 1066, 1069, 1054, 1097, 1097 };
			m_Sounds = new int[] { 61, 61, 61, 1460, -1, -1, -1, -1, 1460 };
			m_Symbols = new int[] { 7147, 7159, 7141, 7153, 3717, 4020, 5091, 6262, 4, 4, 3, 8, 0, 0, -1, 0xFFFFFF, 6, 1149, 1160, 1160, 4017 };
			m_jackpotmultiplier = new int[] { 10000, 5000, 1000, 500, 10, 5, 3, 2, 25, 33, 50 };
			m_Bars = new int[] { 7147, 7159, 7141, 7153 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Tight;
				m_BonusRound = BonusRoundType.MinerMadness;
				m_ScatterPay = ScatterType.Any;
				m_AnyBars = true;
				m_PlayerSounds = true;
				Name = "Miner Madness";
				Hue = 2425; // Utility.RandomList(2219, 2425, 2419, 2207, 2413, 2213, 2418, 2406); // All the ingot colors
				m_Cost = 100;
				m_Rewards = JackpotRewardType.None;
				m_CreditCashOut = 300000;
				m_CreditATMLimit = 200000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 10000;
			}
		}

		private void GruesomeGambling(bool initialize)
		{
			m_JackpotText = new string[] { "====> Rest in Peace! <====", "***Get a Shovel***", "**The Horror!**", "Help me!", "ooO OoOO?", null, null, null, "Ommpf" };
			if (this.Map == Map.Felucca)
				m_JackpotEffect = new int[] { 6, 4, 4, 3, -3, -1, -2, 0, 0 };
			else
				m_JackpotEffect = new int[] { 6, 3, 5, 3, -3, -1, -2, 0, 0 };
			m_FemaleSounds = new int[] { 336, 790, 814, 793, 796, 787, -1, -1, -1 };
			m_MaleSounds = new int[] { 346, 1061, 1088, 1065, 1068, 1058, -1, -1, -1 };
			m_Sounds = new int[] { 1385, 1157, 586, 769, 743, 481, -1, -1, -1 };
			m_Symbols = new int[] { 4457, 3808, 6927, 7960, 7392, 3619, 3618, 4650, 7, 7, 0, 8, 10, 15, 0, 0x8B0000, 6, 905, 36, 1156, 4678 };
			m_jackpotmultiplier = new int[] { 5000, 2500, 1000, 250, 50, 10, 5, 1, 0, 33, 66 };
			m_Bars = new int[] { 9999, 9999, 9999, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_TotalSymbols = 8;
				m_PaybackType = PaybackType.Tight;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.Any;
				m_AnyBars = false;
				m_PlayerSounds = true;
				Name = "Gruesome Gambling";
				Hue = 958; // Utility.RandomList(37, 337, 437, 737, 837, 937, 1157);
				m_Cost = 100;
				m_Rewards = JackpotRewardType.RewardAndCash;
				m_CreditCashOut = 250000;
				m_CreditATMLimit = 200000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 10000;
			}
			if (m_Rewards == JackpotRewardType.RewardOnly)
				m_jackpotmultiplier = new int[] { 0, 3000, 1500, 500, 100, 10, 5, 1, 0, 33, 66 };
			else if (m_Rewards == JackpotRewardType.RewardAndCash)
				m_jackpotmultiplier = new int[] { 1000, 2000, 1500, 500, 100, 10, 5, 1, 0, 33, 66 };
		}

		private void FarmerFaceoff(bool initialize)
		{
			m_Symbols = new int[] { 8451, 8449, 8401, 9608, 3894, 3190, 5378, 5910, 1, 7, 2, 8, 10, 15, -1, 16777011, 1, 642, 52, 62, 7732 };
			m_jackpotmultiplier = new int[] { 4000, 2000, 1000, 200, 100, 20, 10, 2, 100, 33, -1 };
			m_JackpotText = new string[] { "====>Cattle to Market!<====", "***Best in Show!***", "**Sell Your Chickens**", "*Best of Breed*", "Crop Harvested!", null, null, null, null };
			m_FemaleSounds = new int[] { 823, 823, 783, 794, 794, 797, 783, 823, 823 };
			m_MaleSounds = new int[] { 1098, 1097, 1054, 1066, 1066, 1069, 1054, 1097, 1097 };
			m_Sounds = new int[] { 120, 196, 110, 134, 1336, -1, -1, -1, -1 };
			m_JackpotEffect = new int[] { 5, 3, 1, 0, 0, 0, 0, 0, 0 };
			m_Bars = new int[] { 8451, 8449, 8401, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Normal;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.LeftOnly;
				m_AnyBars = false;
				m_PlayerSounds = false;
				Name = "Farmer Faceoff";
				Hue = 643; // Utility.RandomList(643, 53, 63);
				m_Cost = 100;
				m_Rewards = JackpotRewardType.None;
				m_CreditCashOut = 250000;
				m_CreditATMLimit = 200000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 25000;
			}
		}

		private void OffToTheRaces(bool initialize)
		{
			m_Symbols = new int[] { 9678, 9629, 9628, 8479, 8480, 8481, 4980, 4022, 0, 6, 3, 8, 15, 20, -1, 16444375, 6, 546, 50, 1149, 3896 };
			m_jackpotmultiplier = new int[] { 2500, 1000, 500, 200, 25, 10, 5, -1, 100, 40, 125 };
			m_JackpotText = new string[] { "====>Triple Crown!<====", "***Kentucky Derby***", "**Preakness Stakes**", "*Belmont Stakes*", "Your horse wins!", null, null, null, "Collect Stud Fees" };
			m_FemaleSounds = new int[] { 824, 823, 783, -1, 823, 797, -1, -1, -1 };
			m_MaleSounds = new int[] { 1098, 1097, 1054, -1, 1097, 1069, -1, -1, -1 };
			m_Sounds = new int[] { 914, 168, 169, 170, 171, -1, -1, -1, 171 };
			m_JackpotEffect = new int[] { 5, 3, 2, 1, 0, 0, 0, 0, 0 };
			m_Bars = new int[] { 9678, 9629, 9628, 8479 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Random;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.LeftOnly;
				m_AnyBars = true;
				m_PlayerSounds = true;
				Name = "Off to the Races";
				Hue = 51; // Utility.RandomList(547, 51);
				m_Cost = 10;
				m_Rewards = JackpotRewardType.None;
				m_CreditCashOut = 100000;
				m_CreditATMLimit = 75000;
				m_CreditATMIncrements = 5000;
				m_DefaultStartProgressive = 1000;
			}
		}

		private void LadyLuck(bool initialize)
		{
			m_JackpotText = new string[] { "===>Lady Luck Strikes!<===", "***Winner***", "**Nice!**", "Winner!", null, null, null, null, null };
			m_JackpotEffect = new int[] { 5, 3, 1, 0, 0, 0, 0, 0, 0 };
			m_FemaleSounds = new int[] { 783, 823, 783, 797, 797, -1, -1, -1, -1 };
			m_MaleSounds = new int[] { 1054, 1097, 1054, 1069, 1069, -1, -1, -1, -1 };
			m_Sounds = new int[] { 61, 61, 1460, 1048, -1, -1, -1, -1, -1 };
			m_Symbols = new int[] { 90100, 90101, 90102, 90103, 90104, 3083, 3712, 3823, 7, 3, 0, 8, 23, 15, -1, 16763955, 6, 2213, 600, 2212, 90110 };
			m_jackpotmultiplier = new int[] { 10000, 2500, 1000, 250, 100, 50, 5, 3, 0, -1, 200 };
			m_Bars = new int[] { 9999, 9999, 9999, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_TotalSymbols = 8;
				m_PaybackType = PaybackType.ExtremelyTight;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.LeftOnly;
				m_AnyBars = false;
				m_PlayerSounds = true;
				Name = "Lady Luck";
				m_Cost = 100;
				m_Rewards = JackpotRewardType.RewardOnly;
				Hue = 2213;
				m_CreditCashOut = 500000;
				m_CreditATMLimit = 400000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 10000;
			}
			if (m_Rewards == JackpotRewardType.RewardOnly)
			{
				m_jackpotmultiplier[0] = 0;
				m_jackpotmultiplier[1] = 0;
				m_jackpotmultiplier[2] = 0;
			}
			else if (m_Rewards == JackpotRewardType.RewardAndCash)
			{
				m_jackpotmultiplier[0] = 2500;
				m_jackpotmultiplier[1] = 1250;
				m_jackpotmultiplier[2] = 500;
			}
		}

		private void Classic(bool initialize)
		{
			m_Symbols = new int[] { 7146, 2451, 5921, 3164, 2513, 2512, 0, 0, 5, 1, 0, 6, 0, 0, -1, 16777215, 2, 187, 77, 55, 7150 };
			m_jackpotmultiplier = new int[] { 5000, 1000, 200, 100, 50, 25, -2, -2, -2, 200, 1000 };
			m_JackpotText = new string[] { "====>Jackpot!<====", "***Winner***", "*Winner*", "Winner", null, null, null, null, null };
			m_FemaleSounds = new int[] { 824, 823, 794, -1, -1, -1, -1, -1, -1 };
			m_MaleSounds = new int[] { 1098, 1097, 1066, -1, -1, -1, -1, -1, -1 };
			m_Sounds = new int[] { 61, 61, 1460, -1, -1, -1, -1, -1, -1 };
			m_JackpotEffect = new int[] { 3, 1, 0, 0, 0, 0, 0, 0, 0 };
			m_Bars = new int[] { 9999, 9999, 9999, 9999 };
			m_TotalSymbols = 7;
			if (initialize)
			{
				m_PaybackType = PaybackType.Random;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.Any;
				m_AnyBars = false;
				m_PlayerSounds = true;
				Name = "Classic Slots";
				Hue = 188; // Utility.RandomList(188, 78, 56);
				m_Cost = 5;
				m_Rewards = JackpotRewardType.None;
				m_CreditCashOut = 50000;
				m_CreditATMLimit = 40000;
				m_CreditATMIncrements = 1000;
				m_DefaultStartProgressive = 500;
			}
		}

		private void ClassicII(bool initialize)
		{
			m_Symbols = new int[] { 7147, 7146, 7145, 7186, 3164, 5921, 2512, 0, 5, 3, 2, 7, 0, 0, -1, 16777215, 6, 187, 377, 90, 5384 };
			m_jackpotmultiplier = new int[] { 5000, 1000, 300, 50, 30, 10, 5, -2, 100, 50, 100 };
			m_JackpotText = new string[] { "====>Jackpot!<====", "***Winner***", "*Winner*", "Winner", null, null, null, null, "winner" };
			m_FemaleSounds = new int[] { 783, 823, 783, 794, 794, -1, -1, -1, -1 };
			m_MaleSounds = new int[] { 1054, 1097, 1054, 1066, 1066, -1, -1, -1, -1 };
			m_Sounds = new int[] { 61, 61, 1460, 1048, -1, -1, -1, -1, -1 };
			m_JackpotEffect = new int[] { 3, 1, 0, 0, 0, 0, 0, 0, 0 };
			m_Bars = new int[] { 7147, 7146, 7145, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Random;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.Any;
				m_AnyBars = true;
				m_PlayerSounds = true;
				Name = "Classic Slots II";
				Hue = 78; // Utility.RandomList(188, 378, 91);
				m_Cost = 100;
				m_Rewards = JackpotRewardType.None;
				m_CreditCashOut = 250000;
				m_CreditATMLimit = 200000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 10000;
			}
		}

		private void ClassicIII(bool initialize)
		{
			m_Symbols = new int[] { 90001, 90002, 90003, 90004, 7186, 3164, 5921, 2512, 5, 6, 3, 8, 0, 0, -1, 16777215, 6, 165, 82, 45, 5402 };
			m_jackpotmultiplier = new int[] { 10000, 2500, 750, 100, 20, 5, 3, 1, 50, 25, 75 };
			m_JackpotText = new string[] { "====>Jackpot!<====", "***Winner***", "*Winner*", "Winner", null, null, null, null, "winner" };
			m_FemaleSounds = new int[] { 783, 823, 783, 794, 794, -1, -1, -1, -1 };
			m_MaleSounds = new int[] { 1054, 1097, 1054, 1066, 1066, -1, -1, -1, -1 };
			m_Sounds = new int[] { 61, 61, 1460, 1048, -1, -1, -1, -1, -1 };
			m_JackpotEffect = new int[] { 3, 1, 0, 0, 0, 0, 0, 0, 0 };
			m_Bars = new int[] { 90001, 90002, 90003, 90004 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Random;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.LeftOnly;
				m_AnyBars = true;
				m_PlayerSounds = true;
				Name = "Classic Slots III";
				Hue = 54; // Utility.RandomList(166, 83, 45);
				m_Cost = 100;
				m_Rewards = JackpotRewardType.None;
				m_CreditCashOut = 500000;
				m_CreditATMLimit = 400000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 25000;
			}
		}

		private void Holiday1(bool initialize)
		{
			m_Symbols = new int[] { 9000, 9078, 9002, 9006, 9008, 9009, 9070, 9077, 5, 2, 3, 8, 15, 35, 3381504, 10027008, 6, 167, 137, 92, 9079 };
			m_jackpotmultiplier = new int[] { 7500, 2500, 500, 250, 50, 20, 10, 5, 50, 100, 200 };
			m_JackpotText = new string[] { "====>Jackpot!<====", "***Winner***", "*Winner*", "Winner", null, null, null, null, null };
			m_FemaleSounds = new int[] { 824, 823, 783, 794, 794, -1, -1, -1, -1 };
			m_MaleSounds = new int[] { 1098, 1097, 1054, 1066, 1066, -1, -1, -1, -1 };
			m_Sounds = new int[] { 61, 914, 915, 915, -1, -1, -1, -1, -1 };
			m_JackpotEffect = new int[] { 3, 1, 0, 0, 0, 0, 0, 0, 0 };
			m_Bars = new int[] { 9999, 9999, 9999, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Normal;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.Any;
				m_AnyBars = false;
				m_PlayerSounds = true;
				Name = "Christmas Cash";
				Hue = Utility.RandomList(3, 403, 68, 468, 38, 338);
				m_Cost = 100;
				m_Rewards = JackpotRewardType.None;
				m_CreditCashOut = 400000;
				m_CreditATMLimit = 300000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 25000;
			}
		}
#if MINIHOUSES
		private void MiniHouses(bool initialize)
		{
			m_Symbols = new int[] { 5363, 2997, 6998, 90250, 5184, 4167, 5365, 5443, 7, 3, 2, 8, 20, 25, 16777215, 1, 6, 1149, 642, 143, 15943 };
			m_jackpotmultiplier = new int[] { 4000, 2000, 1000, 200, 20, 10, 5, 3, 0, -1, 200 };
			m_JackpotText = new string[] { "Arh, there be booty here!", "Walk the plank matey!", "Man the cannons!", "Raise the Jolly Roger!", "Swab the decks!", "yo ho..yo ho...", "yar", null, null };
			m_FemaleSounds = new int[] { 783, 823, 783, 794, 794, -1, -1, -1, -1 };
			m_MaleSounds = new int[] { 1054, 1097, 1054, 1066, 1066, -1, -1, -1, -1 };
			m_Sounds = new int[] { 519, 61, 1460, 1048, -1, -1, -1, -1, -1 };
			m_JackpotEffect = new int[] { 3, 1, 0, 0, 0, 0, 0, 0, 0 };
			m_Bars = new int[] { 9999, 9999, 9999, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Tight;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.Any;
				m_AnyBars = false;
				m_PlayerSounds = true;
				m_Rewards = JackpotRewardType.RewardOnly;
				m_CreditCashOut = 125000;
				m_CreditATMLimit = 75000;
				m_CreditATMIncrements = 5000;
				m_DefaultStartProgressive = 5000;
				m_Cost = 10;
				Name = "Mini House Deeds";
				Hue = 694;
			}
			if (m_Rewards == JackpotRewardType.RewardOnly)
				m_jackpotmultiplier[0] = 0;
		}
#endif
		private void Pirates(bool initialize)
		{
			m_Symbols = new int[] { 5363, 2997, 6998, 90250, 5184, 4167, 5365, 5443, 7, 3, 2, 8, 20, 25, 16777215, 1, 6, 1149, 642, 143, 15943 };
			m_jackpotmultiplier = new int[] { 4000, 2000, 1000, 200, 20, 10, 5, 3, 0, -1, 200 };
			m_JackpotText = new string[] { "Arh, there be booty here!", "Walk the plank matey!", "Man the cannons!", "Raise the Jolly Roger!", "Swab the decks!", "yo ho..yo ho...", "yar", null, null };
			m_FemaleSounds = new int[] { 783, 823, 783, 794, 794, -1, -1, -1, -1 };
			m_MaleSounds = new int[] { 1054, 1097, 1054, 1066, 1066, -1, -1, -1, -1 };
			m_Sounds = new int[] { 519, 61, 1460, 1048, -1, -1, -1, -1, -1 };
			m_JackpotEffect = new int[] { 3, 1, 0, 0, 0, 0, 0, 0, 0 };
			m_Bars = new int[] { 9999, 9999, 9999, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Tight;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.Any;
				m_AnyBars = false;
				m_PlayerSounds = true;
				m_Rewards = JackpotRewardType.RewardOnly;
				m_CreditCashOut = 125000;
				m_CreditATMLimit = 75000;
				m_CreditATMIncrements = 5000;
				m_DefaultStartProgressive = 5000;
				m_Cost = 10;
				Name = "Pirate's Quest";
				Hue = 694;
			}
			if (m_Rewards == JackpotRewardType.RewardOnly)
				m_jackpotmultiplier[0] = 0;
		}

		private void PowerScrolls(bool initialize)
		{
			m_Symbols = new int[] { 10923, 10924, 10925, 10926, 10928, 10929, 10930, 10931, 7, 3, 2, 8, 50, 30, -1, 64250, 6, 1153, 87, 91, 10974 };
			RandomScrollSymbols();
			m_jackpotmultiplier = new int[] { 10000, 2500, 1250, 100, 50, 10, 5, 3, 400, 100, 200 };
			m_JackpotText = new string[] { "====>Legendary!<====", "***?Elder?***", "*Elder*", "Master", "Adept", "Expert", "Journeyman", "Apprentice", "Grandmaster" };
			m_FemaleSounds = new int[] { 783, 823, 783, 797, 797, -1, -1, -1, -1 };
			m_MaleSounds = new int[] { 1054, 1097, 1054, 1069, 1069, -1, -1, -1, -1 };
			m_Sounds = new int[] { 1481, 1481, 1481, 1318, 1318, -1, -1, -1, 1486 };
			m_JackpotEffect = new int[] { 5, 3, 1, 0, 0, 0, 0, 0, 1 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Tight;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.LeftOnly;
				m_AnyBars = true;
				m_PlayerSounds = true;
				Name = "Power Up";
				Hue = 1150;
				m_Cost = 100;
				m_Rewards = JackpotRewardType.RewardOnly;
				m_CreditCashOut = 400000;
				m_CreditATMLimit = 300000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 5000;
			}
			if (m_Rewards == JackpotRewardType.RewardOnly)
			{
				m_jackpotmultiplier[0] = 0;
				m_jackpotmultiplier[1] = 0;
				m_jackpotmultiplier[2] = 0;
			}
			else if (m_Rewards == JackpotRewardType.RewardAndCash)
			{
				m_jackpotmultiplier[0] = 1000;
				m_jackpotmultiplier[1] = 750;
				m_jackpotmultiplier[2] = 500;
			}
		}

		private void RandomScrollSymbols()
		{
			int[] symbolpool = new int[] { 10923, 10924, 10925, 10926, 10927, 10928, 10929, 10930, 10931, 10932, 10933, 10934, 10935 };
			m_Bars = new int[] { 9999, 9999, 9999, 9999 };
			bool symbolfound = false;
			if (Core.SE) { }
			else if (Core.AOS)
			{
				symbolpool[0] = -1;
				symbolpool[1] = -1;
			}
			else
			{
				symbolpool[0] = -1;
				symbolpool[1] = -1;
				symbolpool[2] = -1;
				symbolpool[3] = -1;
			}
			for (int i = 0; i < 8; i++)
			{
				symbolfound = false;
				while (!symbolfound)
				{
					int index = Utility.Random(symbolpool.Length);
					if (symbolpool[index] != -1)
					{
						m_Symbols[i] = symbolpool[index];
						symbolpool[index] = -1;
						symbolfound = true;
					}

				}
			}
			for (int i = 0; i < 3; i++)
				m_Bars[i] = m_Symbols[i];
		}

		private void StatScrolls(bool initialize)
		{
			m_Symbols = new int[] { 90300, 90301, 90302, 90303, 90304, 90305, 8511, 90306, 5, 3, 2, 8, 25, 35, -1, 12632256, 6, 1000, 36, 1000, 90301 };
			m_Sounds = new int[] { 525, 524, 497, 1202, 1468, -1, -1, -1, 1202 };
			m_jackpotmultiplier = new int[] { 10000, 2500, 1250, 100, 50, 10, 5, 3, 400, 100, 200 };
			m_JackpotText = new string[] { "====>Harrower Defeated!<====", "***True Harrower***", "*Tentacles Strike*", "Faker!", null, null, null, null, "Watch out!" };
			m_FemaleSounds = new int[] { 783, 823, 783, 794, 794, -1, -1, -1, 783 };
			m_MaleSounds = new int[] { 1054, 1097, 1054, 1066, 1066, -1, -1, -1, 1054 };
			m_JackpotEffect = new int[] { 5, 3, 1, 0, 0, 0, 0, 0, 1 };
			m_Bars = new int[] { 90300, 90301, 90302, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.ExtremelyTight;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.LeftOnly;
				m_AnyBars = true;
				m_PlayerSounds = true;
				Name = "The Harrower";
				Hue = 1000;
				m_Cost = 100;
				m_Rewards = JackpotRewardType.RewardOnly;
				m_CreditCashOut = 750000;
				m_CreditATMLimit = 500000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 5000;
			}
			if (m_Rewards == JackpotRewardType.RewardOnly)
				m_jackpotmultiplier[0] = 0;
			else if (m_Rewards == JackpotRewardType.RewardAndCash)
				m_jackpotmultiplier[0] = 1000;
		}
		private void TailorTreats(bool initialize)
		{
			m_Symbols = new int[] { 90200, 90201, 90202, 3997, 2981, 90203, 90204, 3999, 5, 3, 2, 8, 15, 20, -1, 16711935, 6, 22, 2412, 1153, 90210 };
			m_jackpotmultiplier = new int[] { 10000, 2500, 1250, 100, 50, 10, 5, 3, 400, 100, 200 };
			m_JackpotText = new string[] { "====>Barbed Jackpot!<====", "***Horned Jackpot!***", "*Spined Jackpot!*", "Winner", null, null, null, null, "<Good One!>" };
			m_FemaleSounds = new int[] { 783, 823, 783, 794, 794, -1, -1, -1, 783 };
			m_MaleSounds = new int[] { 1054, 1097, 1054, 1066, 1066, -1, -1, -1, 1054 };
			m_Sounds = new int[] { 492, 61, 1460, 584, 584, -1, -1, -1, 1048 };
			m_JackpotEffect = new int[] { 5, 3, 1, 0, 0, 0, 0, 0, 1 };
			m_Bars = new int[] { 90200, 90201, 90202, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Random;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.LeftOnly;
				m_AnyBars = true;
				m_PlayerSounds = true;
				Name = "Tailor Treats";
				Hue = 1160; // Utility.RandomList(1153, 1158, 1160, 1165, 1170, 1174);
				m_Cost = 100;
				m_Rewards = JackpotRewardType.RewardOnly;
				m_CreditCashOut = 400000;
				m_CreditATMLimit = 300000;
				m_CreditATMIncrements = 10000;
				m_DefaultStartProgressive = 5000;
			}
			if (m_Rewards == JackpotRewardType.RewardOnly)
			{
				m_jackpotmultiplier[0] = 0;
				m_jackpotmultiplier[1] = 0;
				m_jackpotmultiplier[2] = 0;
			}
			else if (m_Rewards == JackpotRewardType.RewardAndCash)
			{
				m_jackpotmultiplier[0] = 1000;
				m_jackpotmultiplier[1] = 750;
				m_jackpotmultiplier[2] = 500;
			}
		}

		private void TrophyHunter(bool initialize)
		{
			m_Symbols = new int[] { 7777, 7776, 7781, 7785, 7782, 7779, 7780, 7822, 0, 3, 3, 8, 25, 25, -1, 16444375, 5, 262, 88, 1153, 4203 };
			m_jackpotmultiplier = new int[] { 4000, 2000, 1000, 200, 20, 10, 5, 3, 0, -1, 200 };
			m_JackpotText = new string[] { "====>Deer Hunter!<====", "***Grizzily Adams***", "**You got the Twin!**", "*Big Fish*", null, null, null, null, null };
			m_FemaleSounds = new int[] { 824, 823, 783, -1, 823, 797, -1, -1, -1 };
			m_MaleSounds = new int[] { 1098, 1097, 1054, -1, 1097, 1069, -1, -1, -1 };
			m_Sounds = new int[] { 1227, 167, 1218, 61, 465, 158, -1, -1, 171 };
			m_JackpotEffect = new int[] { 5, 3, 1, 0, 0, 0, 0, 0, 0 };
			m_Bars = new int[] { 9999, 9999, 9999, 9999 };
			m_TotalSymbols = 8;
			if (initialize)
			{
				m_PaybackType = PaybackType.Normal;
				m_BonusRound = BonusRoundType.None;
				m_ScatterPay = ScatterType.Any;
				m_AnyBars = false;
				m_PlayerSounds = true;
				Name = "Trophy Hunter";
				Hue = 367; // Utility.RandomList(262, 88);
				m_Cost = 25;
				m_Rewards = JackpotRewardType.None;
				m_CreditCashOut = 125000;
				m_CreditATMLimit = 75000;
				m_CreditATMIncrements = 5000;
				m_DefaultStartProgressive = 5000;
			}
		}
		#endregion

		private string CreateProfileStrings(int[] arg)
		{
			string tstring = null;

			for (int i = 0; i <= arg.Length - 1; i++)
			{
				tstring = tstring + arg[i].ToString();
				if (i != arg.Length - 1)
					tstring = tstring + ",";

			}
			return tstring;
		}

		#region Timers
		public void ActivateRandomTimer()
		{
			// reset the timer
			//if ( m_RandomTimer != null )
			//	m_RandomTimer.Stop();

			// start the refractory timer to set proximity activated to false, thus enabling another activation
			if (m_RandomMax > TimeSpan.FromMinutes(0))
			{
				int minSeconds = (int)m_RandomMin.TotalSeconds;
				int maxSeconds = (int)m_RandomMax.TotalSeconds;
				DoTimer1(TimeSpan.FromSeconds(Utility.RandomMinMax(minSeconds, maxSeconds)));
			}
		}

		public void DoTimer1(TimeSpan delay)
		{

			m_RandomTimerEnd = DateTime.Now + delay;
			m_RandomActivated = true;

			if (m_RandomTimer != null)
				m_RandomTimer.Stop();

			m_RandomTimer = new InternalTimer1(this, delay);
			m_RandomTimer.Start();
		}

		private class InternalTimer1 : Timer
		{
			private TurboSlot m_slot;

			public InternalTimer1(TurboSlot slot, TimeSpan delay)
				: base(delay)
			{
				Priority = TimerPriority.OneSecond;
				m_slot = slot;
			}

			protected override void OnTick()
			{
				if (m_slot != null && !m_slot.Deleted)
				{
					//reset the Odds table 
					m_slot.m_RandomActivated = false;
					m_slot.SetupOddsTable(m_slot.SlotPaybackOdds, true);
				}
			}
		}

		public void ActivateBlinkTimer(int hue)
		{
			if (!m_Active || m_Profile) return;
			if (m_OrigHue == -1)
				m_OrigHue = this.Hue;
			if (hue == 0 || hue == this.Hue)
			{
				if (this.Hue != 1150)
					m_BlinkHue = 1150;
				else
					m_BlinkHue = Utility.Random(100);
			}
			else
				m_BlinkHue = hue;
			this.Hue = m_BlinkHue;
			m_BlinkCount = 0;
			DoTimer2(TimeSpan.FromSeconds(1));
		}

		public void BlinkMe()
		{
			m_BlinkCount++;
			if (m_BlinkCount % 2 == 0)
				this.Hue = m_BlinkHue;
			else
				if (m_OrigHue > 0)
					this.Hue = m_OrigHue;
			if (m_BlinkCount > 5)
			{
				this.Hue = m_OrigHue;
				m_OrigHue = -1;
			}
			else
				DoTimer2(TimeSpan.FromSeconds(1));
		}

		public void DoTimer2(TimeSpan delay)
		{
			if (m_BlinkTimer != null)
				m_BlinkTimer.Stop();
			m_BlinkTimer = new InternalTimer2(this, delay);
			m_BlinkTimer.Start();
		}

		private class InternalTimer2 : Timer
		{
			private TurboSlot m_slot;

			public InternalTimer2(TurboSlot slot, TimeSpan delay)
				: base(delay)
			{
				Priority = TimerPriority.OneSecond;
				m_slot = slot;
			}

			protected override void OnTick()
			{
				if (m_slot != null && !m_slot.Deleted)
				{
					m_slot.BlinkMe();
				}
			}
		}
		#endregion

		public TurboSlot(Serial serial)
			: base(serial)
		{
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
				if (m_CardClubOnly)
					list.Add(1060658, "Status\tAvailable (Captain's Cabin Only)");
				else
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
				list.Add(numberpos, "Last Jackpot\t{0}", m_LastWonBy.Name);
				numberpos++;
				list.Add(numberpos, "Date\t{0}", m_LastWonByDate);
				numberpos++;
				list.Add(numberpos, "Amount\t{0}", m_LastWonAmount);
			}
		}

		public override bool HandlesOnMovement { get { return (m_InUseBy != null && m_Active); } }// Tell the core that we implement OnMovement

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)2);

			writer.Write(m_CardClubOnly); // Version 2

			writer.Write(m_MembershipCard); // Version 1

			writer.Write(m_Active); // Version 0
			writer.Write(m_Cost);
			writer.Write(m_Won);
			writer.Write(m_LastPay);
			writer.Write((int)m_SlotTheme);
			writer.Write(m_TotalSpins);
			writer.Write(m_TotalCollected);
			writer.Write(m_TotalWon);
			writer.Write(m_ErrorCode);
			writer.Write(m_InUseBy);
			writer.Write(m_LastPlayed);
			writer.Write(m_LastWonBy);
			writer.Write(m_LastWonByDate);
			writer.Write(m_LastWonAmount);
			writer.Write(m_isProgMaster);
			writer.Write(m_ProgressivePercent);
			writer.Write(m_ProgressiveMaster);
			writer.Write(m_ProgressiveJackpot);
			writer.Write(m_DefaultStartProgressive);
			writer.Write(m_ShowPayback);
			writer.Write((int)m_PaybackType);
			writer.Write((int)m_BonusRound);
			writer.Write((int)m_ScatterPay);
			writer.Write((int)m_Rewards);
			writer.Write(m_AnnounceJackpot);
			writer.Write(m_CreditCashOut);
			writer.Write(m_CreditATMLimit);
			writer.Write(m_CreditATMIncrements);
			writer.Write(m_AnyBars);
			writer.Write(m_PlayerSounds);
			writer.Write(m_OrigHue);
			writer.Write(m_RandomMin);
			writer.Write(m_RandomMax);
			writer.Write(m_ProfReel[0]);
			writer.Write(m_ProfReel[1]);
			writer.Write(m_ProfReel[2]);
			writer.Write(m_ProfPayTable);
			writer.Write(m_ProfSymbols);
			for (int i = 0; i < 11; i++)
				writer.Write(m_jackpotStats[i]);
			writer.Write((int)m_SlotSlaves.Count);
			for (int i = 0; i < m_SlotSlaves.Count; ++i)
				writer.Write((Item)m_SlotSlaves[i]);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					m_CardClubOnly = reader.ReadBool();
					goto case 1;

				case 1:
					m_MembershipCard = reader.ReadBool();
					goto case 0;

				case 0: // Release 0
					{
						m_Active = reader.ReadBool();
						m_Cost = reader.ReadInt();
						m_Won = reader.ReadInt();
						m_LastPay = reader.ReadInt();
						m_SlotTheme = (SlotThemeType)reader.ReadInt();
						m_TotalSpins = reader.ReadInt();
						m_TotalCollected = reader.ReadInt();
						m_TotalWon = reader.ReadInt();
						m_ErrorCode = reader.ReadInt();
						m_InUseBy = reader.ReadMobile();
						m_LastPlayed = reader.ReadDateTime();
						m_LastWonBy = reader.ReadMobile();
						m_LastWonByDate = reader.ReadDateTime();
						m_LastWonAmount = reader.ReadInt();
						m_isProgMaster = reader.ReadBool();
						m_ProgressivePercent = reader.ReadInt();
						m_ProgressiveMaster = reader.ReadItem();
						m_ProgressiveJackpot = reader.ReadInt();
						m_DefaultStartProgressive = reader.ReadInt();
						m_ShowPayback = reader.ReadBool();
						m_PaybackType = (PaybackType)reader.ReadInt();
						m_BonusRound = (BonusRoundType)reader.ReadInt();
						m_ScatterPay = (ScatterType)reader.ReadInt();
						m_Rewards = (JackpotRewardType)reader.ReadInt();
						m_AnnounceJackpot = reader.ReadBool();
						m_CreditCashOut = reader.ReadInt();
						m_CreditATMLimit = reader.ReadInt();
						m_CreditATMIncrements = reader.ReadInt();
						m_AnyBars = reader.ReadBool();
						m_PlayerSounds = reader.ReadBool();
						m_OrigHue = reader.ReadInt();
						this.m_RandomMin = reader.ReadTimeSpan();
						this.m_RandomMax = reader.ReadTimeSpan();
						m_ProfReel[0] = reader.ReadString();
						m_ProfReel[1] = reader.ReadString();
						m_ProfReel[2] = reader.ReadString();
						m_ProfPayTable = reader.ReadString();
						m_ProfSymbols = reader.ReadString();
						for (int i = 0; i < 11; i++)
							m_jackpotStats[i] = reader.ReadInt();
						int count = reader.ReadInt();
						m_SlotSlaves = new ArrayList(count);
						for (int i = 0; i < count; ++i)
						{
							Item item = reader.ReadItem();
							if (item != null)
								m_SlotSlaves.Add(item);
						}
						break;
					}
			}
			if (m_OrigHue != -1 && m_Active)
			{
				this.Hue = m_OrigHue;
				m_OrigHue = -1;
			}
			SetupTheme(m_SlotTheme, false);
		}
	}

	#region Lady Luck Items
	public class LadyLuckRobe : Robe
	{
		[Constructable]
		public LadyLuckRobe()
		{
			LootType = LootType.Blessed;
			Hue = 2213;
			Attributes.Luck = 75 + Utility.Random(50);
			Resistances.Physical = 3;
#if !RUNUO2RC1
			Name = "Lady Luck Robe";
#endif
		}

#if RUNUO2RC1
		public override string DefaultName { get { return "Lady Luck Robe"; } }
#endif

		public override bool Dye(Mobile from, DyeTub sender)
		{
			from.SendLocalizedMessage(sender.FailMessage); // You can't dye this
			return false;
		}

		public new bool Scissor(Mobile from, Scissors scissors)
		{
			from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
			return false;
		}

		public LadyLuckRobe(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}


	public class LadyLuckCloak : BaseCloak
	{
		[Constructable]
		public LadyLuckCloak()
			: base(0x1515, 2213)
		{
			LootType = LootType.Blessed;
			Attributes.Luck = 50 + Utility.Random(25);
			Resistances.Physical = 2;
#if !RUNUO2RC1
			Name = "Lady Luck Cloak";
#endif
		}


#if RUNUO2RC1
		public override string DefaultName { get { return "Lady Luck Cloak"; } }
#endif
		public LadyLuckCloak(Serial serial)
			: base(serial)
		{
		}

		public override bool Dye(Mobile from, DyeTub sender)
		{
			from.SendLocalizedMessage(sender.FailMessage); // You can't dye this
			return false;
		}

		public new bool Scissor(Mobile from, Scissors scissors)
		{
			from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}


	public class LadyLuckSash : BaseMiddleTorso
	{
		[Constructable]
		public LadyLuckSash()
			: base(0x1541, 2213)
		{
			LootType = LootType.Blessed;
			Attributes.Luck = 25 + Utility.Random(25);
			Resistances.Physical = 1;
#if !RUNUO2RC1
			Name = "Lady Luck Sash";
#endif
			Weight = 1.0;
		}

#if RUNUO2RC1
		public override string DefaultName { get { return "Lady Luck Sash"; } }
#endif
		public LadyLuckSash(Serial serial)
			: base(serial)
		{
		}



		public override bool Dye(Mobile from, DyeTub sender)
		{
			from.SendLocalizedMessage(sender.FailMessage); // You can't dye this
			return false;
		}

		public new bool Scissor(Mobile from, Scissors scissors)
		{
			from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
	#endregion

	#region GruesomeBlood
	public class GruesomeBlood : Item   // Same as the other random blood except timer is longer
	{

		[Constructable]
		public GruesomeBlood()
		{
			Movable = false;
			ItemID = Utility.Random(0x122A, 5);
			new InternalTimer(this).Start();
		}

		public GruesomeBlood(Serial serial)
			: base(serial)
		{
			new InternalTimer(this).Start();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		private class InternalTimer : Timer
		{
			private Item m_Blood;

			public InternalTimer(Item blood)
				: base(TimeSpan.FromMinutes(1 + Utility.Random(4)))
			{
				Priority = TimerPriority.OneMinute;

				m_Blood = blood;
			}

			protected override void OnTick()
			{
				m_Blood.Delete();
			}
		}
	}
	#endregion

	#region Casino Token
	public class CasinoToken : Item   // Free plays on Casino games that are aware of Tokens
	{

		[Constructable]
		public CasinoToken() : this(10) { }

		[Constructable]
		public CasinoToken(int NumCredits) : this(NumCredits, 56) { }

		[Constructable]
		public CasinoToken(int NumCredits, int CreditHue)
			: base(10922)
		{
			Light = LightType.Empty;
			Stackable = true;
			Weight = 0.02;
			Hue = CreditHue;
#if !RUNUO2RC1
			Name = "Casino Token";
#endif
			Amount = NumCredits;
		}

#if RUNUO2RC1
		public override string DefaultName { get { return "Casino Token"; } }
#endif
		public CasinoToken(Serial serial)
			: base(serial)
		{
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			if (this.Name == null)
				list.Add(this.LabelNumber);
			else
				list.Add(this.Name);
			list.Add(1060584, this.Amount.ToString()); // uses remaining:
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version         
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}

	#endregion

	#region CasinoMembershipCard
	public class CasinoMembershipCard : Item   // Private Club "Jackpot Winners" Membership card
	{
		private Mobile m_Member;
		private DateTime m_JoinDate;
		private string m_Game = null;
		private int m_Jackpot = 0;

		[Constructable]
		public CasinoMembershipCard()
			: base(0xE17)
		{
			this.Hue = 1150;
#if !RUNUO2RC1
			this.Name = "Captain's Cabin Membership Card";
#endif
			this.m_JoinDate = DateTime.Now;
		}

#if RUNUO2RC1
		public override string DefaultName { get { return "Captain's Cabin Membership Card"; } }
#endif

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile ClubMember
		{
			get { return m_Member; }
			set { m_Member = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime JoinDate
		{
			get { return m_JoinDate; }
			set { m_JoinDate = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public String Game
		{
			get { return m_Game; }
			set { m_Game = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Jackpot
		{
			get { return m_Jackpot; }
			set { m_Jackpot = value; }
		}

		public CasinoMembershipCard(Serial serial)
			: base(serial)
		{
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			if (m_Member != null && !m_Member.Deleted)
			{
				list.Add(1060658, "Member Name\t{0}", m_Member.Name); //  MemberName
				list.Add(1060659, "Joined\t{0}", m_JoinDate); //  Date Jackpot Won
				if (m_Game != null)
				{
					list.Add(1060660, "Game\t{0}", m_Game); //  Slot Machine
					list.Add(1060661, "Jackpot Value\t{0:##,###,##} Gold", m_Jackpot); //  Jackpot Amount
				}

			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version 
			writer.Write(m_Member);
			writer.Write(m_JoinDate);
			writer.Write(m_Game);
			writer.Write(m_Jackpot);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			m_Member = reader.ReadMobile();
			m_JoinDate = reader.ReadDateTime();
			m_Game = reader.ReadString();
			m_Jackpot = reader.ReadInt();
		}
	}
	#endregion
}

/*

MiniHouseDeed( (MiniHouseType)number );
 * list.Add( MiniHouseInfo.GetInfo( m_Type ).LabelNumber );
 * 
 * Common Houses

-Brick House
-Field Stone house
-Small Brick House
-Large House with Patio
-Small Marble Workshop
-Small Stone Workshop
-Stone and Plaster house
-Two-Story Log Cabin
-Two-Story Stone and Plaster house
-Two-Story Wood and Plaster house
-Wooden House

 * 90% of the time above (equal chance)then out of another 100% roll:
 * 
Rare Houses

-Castle .02
-Keep   .04
-Tower  .06
-Thatched Roof Cottage .10
 * 
-Small Stone Tower .15
-Two-story villa .15
-Marble with patio .16
-Sandstone with Patio .16
-Wood and Plaster house .16
 
 *Castle, Thatched Roof Cottage, Keep, and Tower are more valuable then other rare mini houses.

*/