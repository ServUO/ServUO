using System;

namespace Server.Factions
{
    public class FactionDefinition
    {
        private readonly int m_Sort;
        private readonly int m_HuePrimary;
        private readonly int m_HueSecondary;
        private readonly int m_HueJoin;
        private readonly int m_HueBroadcast;
        private readonly int m_WarHorseBody;
        private readonly int m_WarHorseItem;
        private readonly string m_FriendlyName;
        private readonly string m_Keyword;
        private readonly string m_Abbreviation;
        private readonly TextDefinition m_Name;
        private readonly TextDefinition m_PropName;
        private readonly TextDefinition m_Header;
        private readonly TextDefinition m_About;
        private readonly TextDefinition m_CityControl;
        private readonly TextDefinition m_SigilControl;
        private readonly TextDefinition m_SignupName;
        private readonly TextDefinition m_FactionStoneName;
        private readonly TextDefinition m_OwnerLabel;
        private readonly TextDefinition m_GuardIgnore;
        private readonly TextDefinition m_GuardWarn;
        private readonly TextDefinition m_GuardAttack;
        private readonly StrongholdDefinition m_Stronghold;
        private readonly RankDefinition[] m_Ranks;
        private readonly GuardDefinition[] m_Guards;
        public FactionDefinition(int sort, int huePrimary, int hueSecondary, int hueJoin, int hueBroadcast, int warHorseBody, int warHorseItem, string friendlyName, string keyword, string abbreviation, TextDefinition name, TextDefinition propName, TextDefinition header, TextDefinition about, TextDefinition cityControl, TextDefinition sigilControl, TextDefinition signupName, TextDefinition factionStoneName, TextDefinition ownerLabel, TextDefinition guardIgnore, TextDefinition guardWarn, TextDefinition guardAttack, StrongholdDefinition stronghold, RankDefinition[] ranks, GuardDefinition[] guards)
        {
            this.m_Sort = sort;
            this.m_HuePrimary = huePrimary;
            this.m_HueSecondary = hueSecondary;
            this.m_HueJoin = hueJoin;
            this.m_HueBroadcast = hueBroadcast;
            this.m_WarHorseBody = warHorseBody;
            this.m_WarHorseItem = warHorseItem;
            this.m_FriendlyName = friendlyName;
            this.m_Keyword = keyword;
            this.m_Abbreviation = abbreviation;
            this.m_Name = name;
            this.m_PropName = propName;
            this.m_Header = header;
            this.m_About = about;
            this.m_CityControl = cityControl;
            this.m_SigilControl = sigilControl;
            this.m_SignupName = signupName;
            this.m_FactionStoneName = factionStoneName;
            this.m_OwnerLabel = ownerLabel;
            this.m_GuardIgnore = guardIgnore;
            this.m_GuardWarn = guardWarn;
            this.m_GuardAttack = guardAttack;
            this.m_Stronghold = stronghold;
            this.m_Ranks = ranks;
            this.m_Guards = guards;
        }

        public int Sort
        {
            get
            {
                return this.m_Sort;
            }
        }
        public int HuePrimary
        {
            get
            {
                return this.m_HuePrimary;
            }
        }
        public int HueSecondary
        {
            get
            {
                return this.m_HueSecondary;
            }
        }
        public int HueJoin
        {
            get
            {
                return this.m_HueJoin;
            }
        }
        public int HueBroadcast
        {
            get
            {
                return this.m_HueBroadcast;
            }
        }
        public int WarHorseBody
        {
            get
            {
                return this.m_WarHorseBody;
            }
        }
        public int WarHorseItem
        {
            get
            {
                return this.m_WarHorseItem;
            }
        }
        public string FriendlyName
        {
            get
            {
                return this.m_FriendlyName;
            }
        }
        public string Keyword
        {
            get
            {
                return this.m_Keyword;
            }
        }
        public string Abbreviation
        {
            get
            {
                return this.m_Abbreviation;
            }
        }
        public TextDefinition Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public TextDefinition PropName
        {
            get
            {
                return this.m_PropName;
            }
        }
        public TextDefinition Header
        {
            get
            {
                return this.m_Header;
            }
        }
        public TextDefinition About
        {
            get
            {
                return this.m_About;
            }
        }
        public TextDefinition CityControl
        {
            get
            {
                return this.m_CityControl;
            }
        }
        public TextDefinition SigilControl
        {
            get
            {
                return this.m_SigilControl;
            }
        }
        public TextDefinition SignupName
        {
            get
            {
                return this.m_SignupName;
            }
        }
        public TextDefinition FactionStoneName
        {
            get
            {
                return this.m_FactionStoneName;
            }
        }
        public TextDefinition OwnerLabel
        {
            get
            {
                return this.m_OwnerLabel;
            }
        }
        public TextDefinition GuardIgnore
        {
            get
            {
                return this.m_GuardIgnore;
            }
        }
        public TextDefinition GuardWarn
        {
            get
            {
                return this.m_GuardWarn;
            }
        }
        public TextDefinition GuardAttack
        {
            get
            {
                return this.m_GuardAttack;
            }
        }
        public StrongholdDefinition Stronghold
        {
            get
            {
                return this.m_Stronghold;
            }
        }
        public RankDefinition[] Ranks
        {
            get
            {
                return this.m_Ranks;
            }
        }
        public GuardDefinition[] Guards
        {
            get
            {
                return this.m_Guards;
            }
        }
    }
}