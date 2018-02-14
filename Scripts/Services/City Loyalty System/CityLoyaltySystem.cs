using System;
using Server;
using Server.Mobiles;
using Server.Engines.Points;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Server.Network;
using Server.Commands;

namespace Server.Engines.CityLoyalty
{
	public enum City
	{
		Moonglow,
		Britain,
		Jhelom,
		Yew,
		Minoc,
		Trinsic,
		SkaraBrae,
		NewMagincia,
		Vesper
	}
	
	public enum LoyaltyRating
	{
		Disfavored,
		Disliked,
		Detested,
		Loathed,
		Despised,
		Reviled, 
		Scorned,
		Shunned,
		Villified,
		Abhorred,
		Unknown,
		Doubted, 
		Distrusted,
		Disgraced,
		Denigrated,
		Commended,
		Esteemed,
		Respected,
		Honored,
		Admired,
		Adored,
		Lauded,
		Exalted,
		Revered,
		Venerated
	}
	
	[Flags]
	public enum CityTitle
	{
        None    = 0x00000000,
		Citizen = 0x00000002,
		Knight	= 0x00000004,
		Baronet = 0x00000008,
		Baron	= 0x00000010,
		Viscount= 0x00000020,
		Earl	= 0x00000040,
		Marquis	= 0x00000080,
		Duke	= 0x00000100,
	}

    public enum TradeDeal
    {
        None = 0,
        GuildOfArcaneArts = 1154044,
        SocietyOfClothiers = 1154045,
        BardicCollegium = 1154046,
        OrderOfEngineers = 1154048,
        GuildOfHealers = 1154049,
        MaritimeGuild = 1154050,
        MerchantsAssociation = 1154051,
        MiningCooperative = 1154052,
        LeageOfRangers = 1154053,
        GuildOfAssassins = 1154054,
        WarriorsGuild = 1154055,
    }
	
	[PropertyObject]
	public class CityLoyaltySystem : PointsSystem
	{
        public static readonly bool Enabled = Config.Get("CityLoyalty.Enabled", true);
		public static readonly int CitizenJoinWait = Config.Get("CityLoyalty.JoinWait", 7);
		public static readonly int BannerCost = Config.Get("CityLoyalty.BannerCost", 250000);
        public static readonly int BannerCooldownDuration = Config.Get("CityLoyalty.BannerCooldown", 24);
        public static readonly int TradeDealCostPeriod = Config.Get("CityLoyalty.TradeDealPeriod", 7);
        public static readonly int TradeDealCooldown = Config.Get("CityLoyalty.TradeDealCooldown", 7);
        public static readonly int TradeDealCost = Config.Get("CityLoyalty.TradeDealCost", 2000000);
        public static readonly int TradeDealUtilizationPeriod = Config.Get("CityLoyalty.TradeDealUtilizationPeriod", 24);
        public static readonly int MaxBallotBoxes = Config.Get("CityLoyalty.MaxBallotBoxes", 10);
        public static readonly int AnnouncementPeriod = Config.Get("CityLoyalty.AnnouncementPeriod", 48);

        public override TextDefinition Name { get { return new TextDefinition(String.Format("{0}", this.City.ToString())); } }
        public override bool AutoAdd { get { return false; } }
        public override double MaxPoints { get { return double.MaxValue; } }
        public override PointsType Loyalty { get { return PointsType.None; } }
        public override bool ShowOnLoyaltyGump { get { return false; } }

        public override string ToString()
        {
            return Name.String;
        }

		[CommandProperty(AccessLevel.GameMaster)]
		public City City { get; private set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int CompletedTrades { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public CityDefinition Definition { get; set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public long Treasury { get; set; }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public CityElection Election { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TradeDeal ActiveTradeDeal { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TradeDealStart { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextTradeDealCheck { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanUtilize { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public GuardCaptain Captain { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityHerald Herald { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TradeMinister Minister { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityStone Stone { get; set; }

        private Mobile _Governor;
        private Mobile _GovernorElect;
        private bool _PendingGovernor;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile GovernorElect 
        {
            get { return _GovernorElect; }
            set
            {
                if (value != null && Governor != null)
                    Governor = null;

                _GovernorElect = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Governor
        {
            get { return _Governor; }
            set
            {
                if (_Governor != null && _Governor != value && _Governor.NetState != null)
                    _Governor.SendLocalizedMessage(1154071); // King Blackthorn thanks you for your service. You have been removed from the Office of the Governor.

                if (value == _GovernorElect)
                    _GovernorElect = null;

                if(value != null && value != _Governor)
                    HeraldMessage(1154070, value.Name); // Hear Ye! Hear Ye! ~1_NAME~ hath accepted the Office of Governor! King Blackthorn congratulates Governor ~1_NAME~! 

                _PendingGovernor = false;
                _Governor = value;

                if (Stone != null)
                    Stone.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PendingGovernor
        {
            get { return _PendingGovernor; }
            set
            {
                if (value && _GovernorElect != null)
                    _GovernorElect = null;

                _PendingGovernor = value;

                if (Stone != null)
                    Stone.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Headline { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Body { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PostedOn { get; set; }

        private Dictionary<Mobile, DateTime> CitizenWait { get; set; }

		public CityLoyaltySystem(City city)
		{
            City = city;

            if (Cities == null)
                Cities = new List<CityLoyaltySystem>();

            Election = new CityElection(this);
            CitizenWait = new Dictionary<Mobile, DateTime>();

			Cities.Add(this);
		}

        public bool IsGovernor(Mobile m)
        {
            return m.AccessLevel > AccessLevel.GameMaster || m == Governor;
        }

        public override PointsEntry GetSystemEntry(PlayerMobile pm)
        {
            return new CityLoyaltyEntry(pm, this.City);
        }
		
		public bool IsCitizen(Mobile from, bool staffIsCitizen = true)
		{
            if (from.AccessLevel > AccessLevel.Player && staffIsCitizen)
                return true;

            CityLoyaltyEntry entry = GetPlayerEntry<CityLoyaltyEntry>(from);
			
			return entry != null && entry.IsCitizen;
		}

        public int GetCitizenCount()
        {
            int count = 0;

            foreach (CityLoyaltyEntry entry in PlayerTable.OfType<CityLoyaltyEntry>())
            {
                if (entry.IsCitizen)
                    count++;
            }

            return count;
        }
		
		public void DeclareCitizen(Mobile from)
		{
            CityLoyaltyEntry entry = GetPlayerEntry<CityLoyaltyEntry>(from, true);
			
			entry.DeclareCitizenship();
		}
		
		public void RenounceCitizenship(Mobile from)
		{
            CityLoyaltyEntry entry = GetPlayerEntry<CityLoyaltyEntry>(from, true);

            if (entry != null)
            {
                entry.RenounceCitizenship();

                if (from == Governor)
                {
                    _Governor = null;

                    if (Stone != null)
                        Stone.InvalidateProperties();
                }

                if (from == GovernorElect)
                {
                    _GovernorElect = null;

                    if (Stone != null)
                        Stone.InvalidateProperties();
                }

                CitizenWait[from] = DateTime.UtcNow + TimeSpan.FromDays(CitizenJoinWait);
            }
		}
		
		public virtual void AwardHate(Mobile from, double hate)
		{
            CityLoyaltyEntry entry = GetPlayerEntry<CityLoyaltyEntry>(from, true);
			
			if(entry.Love > 10)
			{
				double convert = entry.Hate / 75;
				entry.Love -= (int)convert;
				entry.Hate += (int)convert;
			}

            entry.Hate += (int)hate;
            from.SendLocalizedMessage(1152321, Definition.Name); // Your deeds in the city of ~1_name~ are worthy of censure.

            if (from == Governor && entry.LoyaltyRating < LoyaltyRating.Unknown)
                Governor = null;

            if (from == GovernorElect && entry.LoyaltyRating < LoyaltyRating.Unknown)
                GovernorElect = null;
		}
		
		public virtual void AwardLove(Mobile from, double love, bool message = true)
		{
            CityLoyaltyEntry entry = GetPlayerEntry<CityLoyaltyEntry>(from, true);
			
			if(entry.Hate > 10)
			{
				double convert = entry.Hate / 75;
                entry.Neutrality += (int)convert;
                entry.Hate -= (int)convert;
			}
			
			foreach(CityLoyaltySystem sys in Cities.Where(s => s.City != this.City))
			{
                CityLoyaltyEntry e = sys.GetPlayerEntry<CityLoyaltyEntry>(from, true);

				if(e.Love > 10)
				{
					double convert = e.Love / 75;
                    e.Love -= (int)convert;
                    e.Neutrality += (int)convert;
				}
			}

            if(message)
                from.SendLocalizedMessage(1152320, Definition.Name); // Your deeds in the city of ~1_name~ are worthy of praise.

			entry.Love += (int)love;
		}
		
		public virtual LoyaltyRating GetLoyaltyRating(Mobile from)
		{
            return GetLoyaltyRating(from, GetPlayerEntry<CityLoyaltyEntry>(from as PlayerMobile));
		}

        public virtual LoyaltyRating GetLoyaltyRating(Mobile from, CityLoyaltyEntry entry)
        {
            if (entry != null)
            {
                int love = entry.Love;
                int hate = entry.Hate;
                int neut = entry.Neutrality;

                if (hate > 0 && hate > love && hate > neut)
                {
                    return GetHateRating(hate);
                }
                else if (neut > 0 && neut > love && neut > hate)
                {
                    return GetNeutralRating(neut);
                }
                else if (love > 0)
                {
                    return GetLoveRating(love);
                }
            }

            return LoyaltyRating.Unknown;
        }
		
		public virtual LoyaltyRating GetHateRating(int points)
		{
			return GetRating(points, _LoveHatePointsTable, _HateLoyaltyTable);
		}
		
		public virtual LoyaltyRating GetNeutralRating(int points)
		{
			return GetRating(points, _NuetralPointsTable, _NuetralLoyaltyTable);
		}
		
		public virtual LoyaltyRating GetLoveRating(int points)
		{
			return GetRating(points, _LoveHatePointsTable, _LoveLoyaltyTable);
		}
		
		private LoyaltyRating GetRating(int points, int[][] table, LoyaltyRating[][] loyaltytable)
		{
            LoyaltyRating rating = LoyaltyRating.Unknown;

			for(int i = 0; i < table.Length; i++)
			{
				for(int j = 0; j < table[i].Length; j++)
				{
					if(points >= table[i][j])
						rating = loyaltytable[i][j];
				}
			}
			
			return rating;
		}

        public void AddToTreasury(Mobile m, int amount, bool givelove = false)
        {
            Treasury += amount;

            if (Stone != null)
                Stone.InvalidateProperties();

            if (givelove)
            {
                AwardLove(m, amount / 250);
            }
        }

		public virtual bool HasTitle(Mobile from, CityTitle title)
		{
			CityLoyaltyEntry entry = GetPlayerEntry<CityLoyaltyEntry>(from);
			
			if(entry == null)
				return false;
			
			return (entry.Titles & title) != 0;
		}
		
		public virtual void AddTitle(Mobile from, CityTitle title)
		{
            CityLoyaltyEntry entry = GetPlayerEntry<CityLoyaltyEntry>(from);
			
			if(entry == null)
				return;

            entry.AddTitle(title);
		}
		
		public int GetTitleCost(CityTitle title)
		{
			switch(title)
			{
				default:
				case CityTitle.Citizen: return 0;
				case CityTitle.Knight:	return 10000;
				case CityTitle.Baronet:	return 100000;
				case CityTitle.Baron:	return 1000000;
				case CityTitle.Viscount:return 2000000;
				case CityTitle.Earl:	return 5000000;
				case CityTitle.Marquis:	return 10000000;
				case CityTitle.Duke:	return 50000000;
			}
		}
		
		public bool HasMinRating(Mobile from, CityTitle title)
		{
            CityLoyaltyEntry entry = GetPlayerEntry<CityLoyaltyEntry>(from);
			
			if(entry == null)
				return false;
				
			return entry.LoyaltyRating >= GetMinimumRating(title);
		}
		
		public LoyaltyRating GetMinimumRating(CityTitle title)
		{
			switch(title)
			{
                default:
				case CityTitle.Citizen: return LoyaltyRating.Disfavored;
				case CityTitle.Knight: return LoyaltyRating.Commended;
				case CityTitle.Baronet: return LoyaltyRating.Esteemed;
				case CityTitle.Baron: return LoyaltyRating.Respected;
				case CityTitle.Viscount: return LoyaltyRating.Admired;
				case CityTitle.Earl: return LoyaltyRating.Adored;
				case CityTitle.Marquis: return LoyaltyRating.Revered;
				case CityTitle.Duke: return LoyaltyRating.Venerated;
			}
		}

        public void PayTradeDealCost()
        {
            if (Treasury >= TradeDealCost)
                Treasury -= TradeDealCost;
            else
            {
                OnNewTradeDeal(TradeDeal.None);
            }
        }

        public void OnNewTradeDeal(TradeDeal newtradedeal)
        {
            if(ActiveTradeDeal == TradeDeal.None)
            {
                NextTradeDealCheck = DateTime.UtcNow + TimeSpan.FromDays(TradeDealCostPeriod);
            }
            else if (newtradedeal == TradeDeal.None)
            {
                NextTradeDealCheck = DateTime.MinValue;
                TradeDealStart = DateTime.MinValue;
            }
            else
                TradeDealStart = DateTime.UtcNow;

            ActiveTradeDeal = newtradedeal;

            if (Stone != null)
                Stone.InvalidateProperties();

            foreach (CityLoyaltyEntry player in PlayerTable.OfType<CityLoyaltyEntry>())
            {
                if (player.UtilizingTradeDeal)
                    player.UtilizingTradeDeal = false;
            }
        }

        public void TryUtilizeTradeDeal(Mobile m)
        {
            CityLoyaltyEntry entry = GetPlayerEntry<CityLoyaltyEntry>(m, true);

            if (entry != null)
            {
                if (ActiveTradeDeal == TradeDeal.None)
                {
                    HeraldMessage(m, 1154064); // The City doth nay currently have a Trade Deal in place, perhaps ye should petition the Governor to make such a deal...
                }
                else if (entry.UtilizingTradeDeal)
                {
                    HeraldMessage(m, 1154063); // Thou hath already claimed the benefit of the Trade Deal today!
                }
                else if (entry.LoyaltyRating < LoyaltyRating.Respected)
                {
                    HeraldMessage(m, 1154062); // Begging thy pardon, but thou must be at least Respected within the City to claim the benefits of a Trade Deal!
                }
                else
                {
                    entry.UtilizingTradeDeal = true;
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.CityTradeDeal, 1154168, 1154169, new TextDefinition((int)ActiveTradeDeal), true));

                    m.SendLocalizedMessage(1154075); // You gain the benefit of your City's Trade Deal!
                }
            }
        }

        public void HeraldMessage(int message)
        {
            HeraldMessage(message, "");
        }

        public void HeraldMessage(string message)
        {
            if (Herald != null)
                Herald.Say(message);
        }

        public void HeraldMessage(int message, string args)
        {
            if (Herald != null)
            {
                Herald.Say(message, args);
            }
        }

        public void HeraldMessage(Mobile to, int message)
        {
            if (Herald != null)
                Herald.SayTo(to, message);
            else
                to.SendLocalizedMessage(message);
        }

        public bool CanAdd(Mobile from)
        {
            if (CitizenWait.ContainsKey(from))
            {
                if (CitizenWait[from] < DateTime.UtcNow)
                {
                    RemoveWaitTime(from);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public int NextJoin(Mobile from)
        {
            if (CitizenWait.ContainsKey(from))
            {
                return (int)(CitizenWait[from] - DateTime.UtcNow).TotalDays;
            }

            return 0;
        }

        public void RemoveWaitTime(Mobile from)
        {
            if (CitizenWait.ContainsKey(from))
            {
                CitizenWait.Remove(from);
            }
        }

        public static void Initialize()
        {
            EventSink.Login += OnLogin;
            Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), OnTick);

            CommandSystem.Register("ElectionStartTime", AccessLevel.Administrator, e => e.Mobile.SendGump(new ElectionStartTimeGump(e.Mobile as PlayerMobile)));
            CommandSystem.Register("RemoveWait", AccessLevel.Administrator, e =>
                {
                    foreach (var city in Cities)
                    {
                        city.RemoveWaitTime(e.Mobile);
                    }
                });

            CommandSystem.Register("SystemInfo", AccessLevel.Administrator, e => 
            {
                e.Mobile.CloseGump(typeof(SystemInfoGump));
                e.Mobile.SendGump(new SystemInfoGump());
            });
        }

		private static DateTime _NextAtrophy;
		
		public static List<CityLoyaltySystem> Cities { get; private set; }

        public static bool HasTradeDeal(Mobile m, TradeDeal deal)
        {
            CityLoyaltySystem sys = GetCitizenship(m);

            if (sys != null)
            {
                CityLoyaltyEntry entry = sys.GetPlayerEntry<CityLoyaltyEntry>(m, true);

                return sys.ActiveTradeDeal == deal && entry != null && entry.UtilizingTradeDeal;
            }

            return false;
        }

        public static void OnLogin(LoginEventArgs e)
        {
            if (!Enabled)
                return;

            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            CityLoyaltySystem sys = GetCitizenship(pm);

            if (sys != null && sys.ActiveTradeDeal != TradeDeal.None)
            {
                CityLoyaltyEntry entry = sys.GetPlayerEntry<CityLoyaltyEntry>(pm, true);

                if(entry != null && entry.UtilizingTradeDeal)
                    BuffInfo.AddBuff(pm, new BuffInfo(BuffIcon.CityTradeDeal, 1154168, 1154169, new TextDefinition((int)sys.ActiveTradeDeal), true));
            }
        }

        public static void OnBODTurnIn(Mobile from, int gold)
        {
            if (!Enabled)
                return;

            CityLoyaltySystem city = Cities.FirstOrDefault(c => c.Definition.Region != null && c.Definition.Region.IsPartOf(from.Region));

            if (city != null)
            {
                city.AwardLove(from, Math.Max(10, gold / 100));
            }
        }

        public static void OnSpawnCreatureKilled(BaseCreature killed, int spawnLevel)
        {
            if (!Enabled || killed == null)
                return;

            List<DamageStore> rights = killed.GetLootingRights();

            rights.ForEach(store =>
                {
                    CityLoyaltySystem city = CityLoyaltySystem.GetCitizenship(store.m_Mobile);

                    if (city != null)
                        city.AwardLove(store.m_Mobile, 1 * (spawnLevel + 1), 0.10 > Utility.RandomDouble());
                });
        }

        public static bool CanAddCitizen(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            foreach (var city in Cities)
            {
                if (!city.CanAdd(from))
                    return false;
            }
            return true;
        }

        public static int NextJoinCity(Mobile from)
        {
            foreach (var city in Cities)
            {
                if (!city.CanAdd(from))
                {
                    return city.NextJoin(from);
                }
            }

            return 0;
        }

        public static void OnTick()
        {
            foreach (var sys in Cities)
            {
                List<Mobile> list = new List<Mobile>(sys.CitizenWait.Keys);

                foreach (var m in list)
                {
                    if (sys.CitizenWait[m] < DateTime.UtcNow)
                    {
                        sys.RemoveWaitTime(m);
                    }
                }

                ColUtility.Free(list);

                if (DateTime.UtcNow > _NextAtrophy)
                {
                    sys.PlayerTable.ForEach(t =>
                    {
                        CityLoyaltyEntry entry = t as CityLoyaltyEntry;

                        if (entry != null)
                        {
                            PlayerMobile owner = entry.Player;

                            entry.Neutrality -= entry.Neutrality / 50;
                            entry.Hate -= entry.Hate / 50;

                            if (owner != null && owner.LastOnline + TimeSpan.FromHours(40) < DateTime.UtcNow)
                                entry.Love -= entry.Love / 75;
                        }
                    });

                    _NextAtrophy = DateTime.UtcNow + TimeSpan.FromDays(1);
                }

                if (sys.NextTradeDealCheck != DateTime.MinValue && sys.NextTradeDealCheck < DateTime.UtcNow)
                {
                    sys.PayTradeDealCost();
                }

                foreach (CityLoyaltyEntry entry in sys.PlayerTable.OfType<CityLoyaltyEntry>())
                {
                    if (entry.TradeDealExpired)
                    {
                        entry.CheckTradeDeal();
                    }
                }

                if (sys.Election != null)
                {
                    sys.Election.OnTick();
                }
                else
                    sys.Election = new CityElection(sys);
            }

            CityTradeSystem.OnTick();
        }
		
		public static bool HasCitizenship(Mobile from)
		{
			foreach(CityLoyaltySystem sys in Cities)
			{
				if(sys.IsCitizen(from))
					return true;
			}
			
			return false;
		}

        public static bool HasCitizenship(Mobile from, City city)
        {
            if (Cities == null)
                return false;

            CityLoyaltySystem sys = Cities.FirstOrDefault(s => s.City == city);

            return sys != null && sys.IsCitizen(from);
        }
		
		public static CityLoyaltySystem GetCitizenship(Mobile from, bool staffIsCitizen = true)
		{
            if (Cities == null)
                return null;

            return Cities.FirstOrDefault(sys => sys.IsCitizen(from, staffIsCitizen));
		}

        public static bool ApplyCityTitle(PlayerMobile pm, ObjectPropertyList list, string prefix, int loc)
        {
            if (loc == 1154017)
            {
                CityLoyaltySystem city = GetCitizenship(pm);

                if (city != null)
                {
                    CityLoyaltyEntry entry = city.GetPlayerEntry<CityLoyaltyEntry>(pm, true);

                    if (entry != null && !String.IsNullOrEmpty(entry.CustomTitle))
                    {
                        prefix = String.Format("{0} {1} the {2}", prefix, pm.Name, entry.CustomTitle);
                        list.Add(1154017, String.Format("{0}\t{1}", prefix, city.Definition.Name)); // ~1_TITLE~ of ~2_CITY~
                        return true;
                    }
                }
            }
            else
            {
                list.Add(1151487, "{0} \t{1} the \t#{2}", prefix, pm.Name, loc); // ~1NT_PREFIX~~2NT_NAME~~3NT_SUFFIX~
                return true;
            }

            return false;
        }

        public static bool HasCustomTitle(PlayerMobile pm, out string str)
        {
            CityLoyaltySystem city = GetCitizenship(pm);
            str = null;

            if (city != null)
            {
                CityLoyaltyEntry entry = city.GetPlayerEntry<CityLoyaltyEntry>(pm, true);

                if (entry != null && !String.IsNullOrEmpty(entry.CustomTitle))
                    str = String.Format("{0}\t{1}", entry.CustomTitle, city.Definition.Name);
            }

            return str != null;
        }

		public static City GetRandomCity()
		{
			switch(Utility.Random(11))
			{
				default:
				case 0: return City.Moonglow;
				case 1: return City.Britain;
				case 2: return City.Jhelom;
				case 3: return City.Yew;
				case 4: return City.Minoc;
				case 5: return City.Trinsic;
				case 6: return City.SkaraBrae;
				case 7: return City.NewMagincia;
				case 10: return City.Vesper;
			}
		}
		
		public static int GetTitleLocalization(Mobile from, CityTitle title, City city)
		{
            return (1152739 + (int)city * 16) + TitleIndex(title, from.Female);
		}

        private static int TitleIndex(CityTitle title, bool female)
        {
            switch(title)
			{
                default:
                case CityTitle.Citizen: return !female ? 0 : 1;
				case CityTitle.Knight:	return !female ? 2 : 3;
                case CityTitle.Baronet: return !female ? 4 : 5;
				case CityTitle.Baron:	return !female ? 6 : 7;
				case CityTitle.Viscount:return !female ? 8 : 9;
				case CityTitle.Earl:	return !female ? 10 : 11;
                case CityTitle.Marquis: return !female ? 12 : 13;
				case CityTitle.Duke:	return !female ? 14 : 15;
			}
        }
		
		public static int BannerLocalization(City city)
		{
			switch(city)
			{
				default: return 0;
				case City.Moonglow: return 1098171;
				case City.Britain: return 1098172;
				case City.Jhelom: return 1098173;
				case City.Yew: return 1098174;
				case City.Minoc: return 1098175;
				case City.Trinsic: return 1098170;
				case City.SkaraBrae: return 1098178;
				case City.NewMagincia: return 1098177;
				case City.Vesper: return 1098176;
			}
		}

        public static int GetCityLocalization(City city)
        {
            switch (city)
            {
                default: return 0;
                case City.Moonglow: return 1011344;
                case City.Britain: return 1011028;
                case City.Jhelom: return 1011343;
                case City.Yew: return 1011032;
                case City.Minoc: return 1011031;
                case City.Trinsic: return 1011029;
                case City.SkaraBrae: return 1011347;
                case City.NewMagincia: return 1011345;
                case City.Vesper: return 1011030;
            }
        }
		
		public static int CityLocalization(City city)
		{
			return GetCityInstance(city).Definition.LocalizedName;
		}

        public static int RatingLocalization(LoyaltyRating rating)
        {
            switch (rating)
            {
                default: return 1152115; // Unknown
                case LoyaltyRating.Disfavored: return 1152118;
                case LoyaltyRating.Disliked: return 1152122;
                case LoyaltyRating.Detested: return 1152123;
                case LoyaltyRating.Loathed: return 1152128;
                case LoyaltyRating.Despised: return 1152129;
                case LoyaltyRating.Reviled: return 1152130;
                case LoyaltyRating.Scorned: return 1152136;
                case LoyaltyRating.Shunned: return 1152137;
                case LoyaltyRating.Villified: return 1152138;
                case LoyaltyRating.Abhorred: return 1152139;
                case LoyaltyRating.Unknown: return 1152115;
                case LoyaltyRating.Doubted: return 1152117;
                case LoyaltyRating.Distrusted: return 1152121;
                case LoyaltyRating.Disgraced: return 1152127;
                case LoyaltyRating.Denigrated: return 1152135;
                case LoyaltyRating.Commended: return 1152116;
                case LoyaltyRating.Esteemed: return 1152120;
                case LoyaltyRating.Respected: return 1152119;
                case LoyaltyRating.Honored: return 1152126;
                case LoyaltyRating.Admired: return 1152125;
                case LoyaltyRating.Adored: return 1152124;
                case LoyaltyRating.Lauded: return 1152134;
                case LoyaltyRating.Exalted: return 1152133;
                case LoyaltyRating.Revered: return 1152132;
                case LoyaltyRating.Venerated: return 1152131;
            }
        }
		
		public static int[][] _LoveHatePointsTable =
		{
			new int[] { 250 }, 								// Tier 1
			new int[] { 500, 1000 }, 						// Tier 2
			new int[] { 5000, 10000, 25000 }, 				// Tier 3
			new int[] { 100000, 250000, 500000, 1000000  }, // Tier 4
		};
		
		public static int[][] _NuetralPointsTable =
		{
			new int[] { 250 }, 								// Tier 1
			new int[] { 1000 }, 							// Tier 2
			new int[] { 25000 }, 							// Tier 3
			new int[] { 1000000 }, 							// Tier 4
		};
		
		public static LoyaltyRating[][] _LoveLoyaltyTable =
		{
			new LoyaltyRating[] { LoyaltyRating.Commended }, 																		// Tier 1
			new LoyaltyRating[] { LoyaltyRating.Esteemed, LoyaltyRating.Respected }, 												// Tier 2
			new LoyaltyRating[] { LoyaltyRating.Honored, LoyaltyRating.Admired, LoyaltyRating.Adored }, 							// Tier 3
			new LoyaltyRating[] { LoyaltyRating.Lauded, LoyaltyRating.Exalted, LoyaltyRating.Revered, LoyaltyRating.Venerated  },// Tier 4
		};
		
		public static LoyaltyRating[][] _HateLoyaltyTable =
		{
			new LoyaltyRating[] { LoyaltyRating.Disfavored }, 																		// Tier 1
			new LoyaltyRating[] { LoyaltyRating.Disliked, LoyaltyRating.Detested }, 													// Tier 2
			new LoyaltyRating[] { LoyaltyRating.Loathed, LoyaltyRating.Despised, LoyaltyRating.Reviled }, 							// Tier 3
			new LoyaltyRating[] { LoyaltyRating.Scorned, LoyaltyRating.Shunned, LoyaltyRating.Villified, LoyaltyRating.Abhorred  }, 	// Tier 4
		};
		
		public static LoyaltyRating[][] _NuetralLoyaltyTable =
		{
			new LoyaltyRating[] { LoyaltyRating.Doubted }, 							// Tier 1
			new LoyaltyRating[] { LoyaltyRating.Distrusted }, 						// Tier 2
			new LoyaltyRating[] { LoyaltyRating.Disgraced }, 							// Tier 3
			new LoyaltyRating[] { LoyaltyRating.Denigrated }, 						// Tier 4
		};
		
		public static bool IsLove(LoyaltyRating rating)
		{
			foreach(LoyaltyRating[] ratings in _LoveLoyaltyTable)
			{
				foreach(LoyaltyRating r in ratings)
				{
					if(r == rating)
						return true;
				}
			}
			
			return false;
		}
		
		public static CityLoyaltySystem GetCityInstance(City city)
		{
		    switch(city)
			{
				default: return null;
				case City.Moonglow: return Moonglow;
				case City.Britain: return Britain;
				case City.Jhelom: return Jhelom;
				case City.Yew: return Yew;
				case City.Minoc: return Minoc;
				case City.Trinsic: return Trinsic;
				case City.SkaraBrae: return SkaraBrae;
				case City.NewMagincia: return NewMagincia;
				case City.Vesper: return Vesper;
			}
		}

        public static bool IsSetup()
        {
            return Cities.FirstOrDefault(c => c.CanUtilize) != null;
        }
		
		public static void OnTradeComplete(Mobile from, TradeEntry entry)
		{
            var dest = GetCityInstance(entry.Destination);
            var origin = GetCityInstance(entry.Origin);
            int gold = entry.CalculateGold();

            origin.AddToTreasury(from, gold);
            from.SendLocalizedMessage(1154761, String.Format("{0}\t{1}", gold.ToString("N0", CultureInfo.GetCultureInfo("en-US")), origin.Definition.Name)); // ~1_val~ gold has been deposited into the ~2_NAME~ City treasury for your efforts!

            origin.AwardLove(from, 150);
            dest.AwardLove(from, 150);

            origin.CompletedTrades++;
		}

        public static void OnSlimTradeComplete(Mobile from, TradeEntry entry)
        {
            var dest = GetCityInstance(entry.Destination);
            var origin = GetCityInstance(entry.Origin);

            origin.AwardHate(from, 25);
            dest.AwardHate(from, 25);
        }

        public static Moonglow Moonglow { get; set; }
        public static Britain Britain { get; set; }
        public static Jhelom Jhelom { get; set; }
        public static Yew Yew { get; set; }
        public static Minoc Minoc { get; set; }
        public static Trinsic Trinsic { get; set; }
        public static SkaraBrae SkaraBrae { get; set; }
        public static NewMagincia NewMagincia { get; set; }
        public static Vesper Vesper { get; set; }

        public static CityTradeSystem CityTrading { get; set; }

        public static void ConstructSystems()
        {
            Moonglow = new Moonglow();
            Britain = new Britain();
            Jhelom = new Jhelom();
            Yew = new Yew();
            Minoc = new Minoc();
            Trinsic = new Trinsic();
            SkaraBrae = new SkaraBrae();
            NewMagincia = new NewMagincia();
            Vesper = new Vesper();

            CityTrading = new CityTradeSystem();
        }
		
		public override void Serialize(GenericWriter writer)
		{
            writer.Write((int)City);

			base.Serialize(writer);
			writer.Write(1);

            writer.Write(CitizenWait.Count);
            foreach (var kvp in CitizenWait)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }

			writer.Write(CompletedTrades);
            writer.Write(Governor);
            writer.Write(GovernorElect);
            writer.Write(PendingGovernor);
            writer.Write(Treasury);
            writer.Write((int)ActiveTradeDeal);
            writer.Write(TradeDealStart);
            writer.Write(NextTradeDealCheck);
            writer.Write(CanUtilize);

            writer.Write(Headline);
            writer.Write(Body);
            writer.Write(PostedOn);

            if (Election != null)
            {
                writer.Write(0);
                Election.Serialize(writer);
            }
            else
                writer.Write(1);
		}
		
		public override void Deserialize(GenericReader reader)
		{
            City = (City)reader.ReadInt();

			base.Deserialize(reader);
			int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            Mobile m = reader.ReadMobile();
                            DateTime dt = reader.ReadDateTime();

                            if (m != null && dt > DateTime.UtcNow)
                                CitizenWait[m] = dt;
                        }
                    }
                    goto case 0;
                case 0:
                    {
                        CompletedTrades = reader.ReadInt();
                        Governor = reader.ReadMobile();
                        GovernorElect = reader.ReadMobile();
                        PendingGovernor = reader.ReadBool();
                        Treasury = reader.ReadLong();
                        ActiveTradeDeal = (TradeDeal)reader.ReadInt();
                        TradeDealStart = reader.ReadDateTime();
                        NextTradeDealCheck = reader.ReadDateTime();
                        CanUtilize = reader.ReadBool();

                        Headline = reader.ReadString();
                        Body = reader.ReadString();
                        PostedOn = reader.ReadDateTime();

                        if (reader.ReadInt() == 0)
                            Election = new CityElection(this, reader);
                        else
                            Election = new CityElection(this);
                    }
                    break;
            }

            if (version == 0 && this.City == City.Britain)
            {
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    Mobile m = reader.ReadMobile();
                    DateTime dt = reader.ReadDateTime();

                    if (m != null && dt > DateTime.UtcNow)
                        CitizenWait[m] = dt;
                }
            }
		}
	}
	
	public class Moonglow : CityLoyaltySystem
	{
        public override PointsType Loyalty { get { return PointsType.Moonglow; } }

		public Moonglow() : base(City.Moonglow)
		{
			Definition = new CityDefinition (
							 City.Moonglow,
							 new Point3D(4480, 1173, 0),
							 new Point3D(4416, 1044, -2),
                             new Point3D(4480, 1172, 0),
                             new Point3D(4551, 1051, 0),
                             new Point3D(4474, 1176, 0),
							 "Moonglow",
							 1114143,
							 1154524
							 );
		}
	}
	
	public class Britain : CityLoyaltySystem
	{
        public override PointsType Loyalty { get { return PointsType.Britain; } }

		public Britain() : base(City.Britain)
		{
			Definition = new CityDefinition (
							 City.Britain,
                             new Point3D(1445, 1694, 0),
                             new Point3D(1436, 1760, -2),
                             new Point3D(1446, 1694, 0),
                             new Point3D(1417, 1715, 20),
                             new Point3D(1437, 1693, 0),
							 "Britain",
							 1114148,
							 1154521
							 );
		}
	}
	
	public class Jhelom : CityLoyaltySystem
	{
        public override PointsType Loyalty { get { return PointsType.Jhelom; } }

		public Jhelom() : base(City.Jhelom)
		{
			Definition = new CityDefinition (
							 City.Jhelom,
							 new Point3D(1336, 3769, 0),
							 new Point3D(1377, 3879, 0),
                             new Point3D(1336, 3770, 0),
                             new Point3D(1379, 3797, 0),
                             new Point3D(1326, 3776, 0),
							 "Jhelom",
							 1114146,
							 1154522
							 );
		}
	}
	
	public class Yew : CityLoyaltySystem
	{
        public override PointsType Loyalty { get { return PointsType.Yew; } }

		public Yew() : base(City.Yew)
		{
			Definition = new CityDefinition (
							 City.Yew,
							 new Point3D(632, 863, 0),
							 new Point3D(621, 1043, 0),
                             new Point3D(631, 863, 0),
                             new Point3D(385, 914, 0),
                             new Point3D(633, 856, 0),
							 "Yew",
							 1114138,
							 1154529
							 );
		}
	}
	
	public class Minoc : CityLoyaltySystem
	{
        public override PointsType Loyalty { get { return PointsType.Minoc; } }

		public Minoc() : base(City.Minoc)
		{
			Definition = new CityDefinition (
							 City.Minoc,
							 new Point3D(2514, 558, 0),
							 new Point3D(2499, 398, 15),
                             new Point3D(2514, 559, 0),
                             new Point3D(2424, 533, 0),
                             new Point3D(2508, 560, 0),
							 "Minoc",
							 1114139,
							 1154523
							 );
		}
	}
	
	public class Trinsic : CityLoyaltySystem
	{
        public override PointsType Loyalty { get { return PointsType.Trinsic; } }

		public Trinsic() : base(City.Trinsic)
		{
			Definition = new CityDefinition (
							 City.Trinsic,
							 new Point3D(1907, 2682, 0),
                             new Point3D(2061, 2855, -2), 
                             new Point3D(1907, 2683, 0),
                             new Point3D(1851, 2772, 0),
                             new Point3D(1904, 2690, 7),
							 "Trinsic",
							 1114142,
							 1154527
							 );
		}
	}
	
	public class SkaraBrae : CityLoyaltySystem
	{
        public override PointsType Loyalty { get { return PointsType.SkaraBrae; } }

		public SkaraBrae() : base(City.SkaraBrae)
		{
            CityLoyaltySystem.SkaraBrae = this;
			Definition = new CityDefinition (
							 City.SkaraBrae,
							 new Point3D(587, 2153, 0),
							 new Point3D(645, 2228, -2),
                             new Point3D(586, 2153, 0),
                             new Point3D(571, 2210, 0),
                             new Point3D(590, 2152, 0),
							 "Skara Brae",
							 1114145,
							 1154526
							 );
		}
	}
	
	public class NewMagincia : CityLoyaltySystem
	{
        public override PointsType Loyalty { get { return PointsType.NewMagincia; } }

		public NewMagincia() : base(City.NewMagincia)
		{
            CityLoyaltySystem.NewMagincia = this;
			Definition = new CityDefinition (
							 City.NewMagincia,
							 new Point3D(3795, 2247, 20),
							 new Point3D(3677, 2254, 20),
                             new Point3D(3796, 2247, 20),
                             new Point3D(3680, 2269, 26),
                             new Point3D(3781, 2256, 20),
							 "Magincia",
							 1114141,
							 1154525
							 );
		}
	}
	
	public class Vesper : CityLoyaltySystem
	{
        public override PointsType Loyalty { get { return PointsType.Vesper; } }

		public Vesper() : base(City.Vesper)
		{
            CityLoyaltySystem.Vesper = this;
			Definition = new CityDefinition (
							 City.Vesper,
							 new Point3D(2891, 683, 0),
							 new Point3D(3004, 834, 0),
                             new Point3D(2891, 682, 0),
                             new Point3D(3004, 822, 0),
                             new Point3D(2894, 680, 0),
							 "Vesper",
							 1114140,
							 1154528
							 );
		}
	}
}
