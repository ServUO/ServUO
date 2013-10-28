using System;
using System.Collections;
using System.Collections.Generic;
using Server;

namespace CustomsFramework.Systems.VIPSystem
{
    public enum BonusName
    {
        ResProtection = 0,
        ToolbarAccess = 1,
        BasicCommands = 2,
        GainIncrease = 3,
        FreeCorpseReturn = 4,

        FullLRC = 5,
        BankIncrease = 6,
        LifeStoneNoUses = 7,
        LootGoldFromGround = 8,
        DoubleResources = 9,

        LootGoldFromCorpses = 10,
        GlobalBankCommands = 11,
        SmartGrabBags = 12,
        FreeHouseDecoration = 13, // Added
        UnlimitedTools = 14
    }

    [PropertyObject]
    public class Bonus
    {
        private readonly Bonuses _Bonuses;
        private readonly BonusInfo _Info;
        private bool _Enabled;
        private DateTime _TimeStarted;
        private TimeSpan _ServicePeriod;
        public Bonus(Bonuses bonuses, BonusInfo info, GenericReader reader)
        {
            this._Bonuses = bonuses;
            this._Info = info;

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        this._Enabled = reader.ReadBool();
                        this._TimeStarted = reader.ReadDateTime();
                        this._ServicePeriod = reader.ReadTimeSpan();
                        break;
                    }
            }
        }

        public Bonus(Bonuses bonuses, BonusInfo info, bool enabled)
        {
            this._Bonuses = bonuses;
            this._Info = info;
            this._Enabled = enabled;
            this._TimeStarted = DateTime.MinValue;
            this._ServicePeriod = TimeSpan.Zero;
        }

        public Bonus(Bonuses bonuses, BonusInfo info)
        {
            this._Bonuses = bonuses;
            this._Info = info;
            this._Enabled = false;
            this._TimeStarted = DateTime.MinValue;
            this._ServicePeriod = TimeSpan.Zero;
        }

        public Bonuses Bonuses
        {
            get
            {
                return this._Bonuses;
            }
        }
        public BonusName BonusName
        {
            get
            {
                return (BonusName)this._Info.BonusID;
            }
        }
        public int BonusID
        {
            get
            {
                return this._Info.BonusID;
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public string Name
        {
            get
            {
                return this._Info.BonusName;
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public bool Enabled
        {
            get
            {
                return this._Enabled;
            }
            set
            {
                this._Enabled = value;
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public DateTime TimeStarted
        {
            get
            {
                return this._TimeStarted;
            }
            set
            {
                this._TimeStarted = value;
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public TimeSpan ServicePeriod
        {
            get
            {
                return this._ServicePeriod;
            }
            set
            {
                this._ServicePeriod = value;
            }
        }
        public override string ToString()
        {
            return String.Format("[{0}: {1}]", this.Name, this.Enabled);
        }

        public void Serialize(GenericWriter writer)
        {
            Utilities.WriteVersion(writer, 0);

            // Version 0
            writer.Write(this._Enabled);
            writer.Write(this._TimeStarted);
            writer.Write(this._ServicePeriod);
        }
    }

    public class BonusInfo
    {
        private static BonusInfo[] _Table = new BonusInfo[15]
        {
            new BonusInfo(0, "Ressurection Protection", "An optional 30 second ressurection protection."),
            new BonusInfo(1, "Toolbar Access", "Access to the [Toolbar command, for quick easy commands."),
            new BonusInfo(2, "Basic Commands", "Access to basic VIP commands."),
            new BonusInfo(3, "Skill & Stat Gain Increase", "Faster skill and stat gains."),
            new BonusInfo(4, "Free Corpse Retrieval", "No fee for getting your corpse back."),
            new BonusInfo(5, "Full LRC", "No regs needed to cast spells."),
            new BonusInfo(6, "Bank Size Increase", "Get more room in your bank box."),
            new BonusInfo(7, "Unlimited Life Stone Uses", "Life Stones will not consume uses when you use them."),
            new BonusInfo(8, "Loot Gold From Ground", "Ledger will loot gold you walk over, automatically."),
            new BonusInfo(9, "Double Resource Gain", "Double resources, no matter what map you're on."),
            new BonusInfo(10, "Loot Gold From Corpses", "Ledger will loot gold from near-by kills, automatically."),
            new BonusInfo(11, "Global Bank Commands", "Access your bank account from anywhere."),
            new BonusInfo(12, "Smart Grab Bags", "Setup grab bags with separate lists."),
            new BonusInfo(13, "Free House Decoration", "House commits don't cost anything."),
            new BonusInfo(14, "Unlimited Tool Uses", "Non-crafting tools will not consume uses when you use them."),
        };
        private int _BonusID;
        private string _BonusName;
        private string _BonusDescription;
        public BonusInfo(int bonusID, string name, string description)
        {
            this._BonusID = bonusID;
            this._BonusName = name;
            this._BonusDescription = description;
        }

        public static BonusInfo[] Table
        {
            get
            {
                return _Table;
            }
            set
            {
                _Table = value;
            }
        }
        public int BonusID
        {
            get
            {
                return this._BonusID;
            }
            set
            {
                this._BonusID = value;
            }
        }
        public string BonusName
        {
            get
            {
                return this._BonusName;
            }
            set
            {
                this._BonusName = value;
            }
        }
        public string BonusDescription
        {
            get
            {
                return this._BonusDescription;
            }
            set
            {
                this._BonusDescription = value;
            }
        }
    }

    [PropertyObject]
    public class Bonuses : IEnumerable<Bonus>
    {
        private Bonus[] _Bonuses;
        public Bonuses()
        {
            BonusInfo[] info = BonusInfo.Table;
            this._Bonuses = new Bonus[info.Length];

            for (int i = 0; i < info.Length; ++i)
                this._Bonuses[i] = new Bonus(this, info[i]);
        }

        public Bonuses(GenericReader reader)
        {
            this.Deserialize(reader);
        }

        [CommandProperty(AccessLevel.Developer)]
        public Bonus ResProtection
        {
            get
            {
                return this[BonusName.ResProtection];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus ToolbarAccess
        {
            get
            {
                return this[BonusName.ToolbarAccess];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus BasicCommands
        {
            get
            {
                return this[BonusName.BasicCommands];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus GainIncrease
        {
            get
            {
                return this[BonusName.GainIncrease];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus FreeCorpseReturn
        {
            get
            {
                return this[BonusName.FreeCorpseReturn];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus FullLRC
        {
            get
            {
                return this[BonusName.FullLRC];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus BankIncrease
        {
            get
            {
                return this[BonusName.BankIncrease];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus LifeStoneNoUses
        {
            get
            {
                return this[BonusName.LifeStoneNoUses];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus LootGoldFromGround
        {
            get
            {
                return this[BonusName.LootGoldFromGround];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus DoubleResources
        {
            get
            {
                return this[BonusName.DoubleResources];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus LootGoldFromCorpses
        {
            get
            {
                return this[BonusName.LootGoldFromCorpses];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus GlobalBankCommands
        {
            get
            {
                return this[BonusName.GlobalBankCommands];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus SmartGrabBags
        {
            get
            {
                return this[BonusName.SmartGrabBags];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus FreeHouseDecoration
        {
            get
            {
                return this[BonusName.FreeHouseDecoration];
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public Bonus UnlimitedTools
        {
            get
            {
                return this[BonusName.UnlimitedTools];
            }
            set
            {
            }
        }
        public int Length
        {
            get
            {
                return this._Bonuses.Length;
            }
        }
        public Bonus this[BonusName name]
        {
            get
            {
                return this[(int)name];
            }
        }
        public Bonus this[int bonusID]
        {
            get
            {
                if (bonusID < 0 || bonusID >= this._Bonuses.Length)
                    return null;

                Bonus bonus = this._Bonuses[bonusID];

                if (bonus == null)
                    this._Bonuses[bonusID] = bonus = new Bonus(this, BonusInfo.Table[bonusID]);

                return bonus;
            }
        }
        public override string ToString()
        {
            return "...";
        }

        public void StartBonuses()
        {
            VIPCore core = World.GetCore(typeof(VIPCore)) as VIPCore;

            if (core != null)
            {
                foreach (Bonus bonus in this._Bonuses)
                {
                    if (bonus.Enabled)
                    {
                        bonus.TimeStarted = DateTime.UtcNow;
                        bonus.ServicePeriod = core.ServiceTimespan;
                    }
                }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            Utilities.WriteVersion(writer, 0);

            // Version 0
            writer.Write(this._Bonuses.Length);

            for (int i = 0; i < this._Bonuses.Length; ++i)
            {
                this._Bonuses[i].Serialize(writer);
            }
        }

        public IEnumerator<Bonus> GetEnumerator()
        {
            foreach (Bonus bonus in this._Bonuses)
            {
                yield return bonus;
            }
        }

        private void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        BonusInfo[] info = BonusInfo.Table;
                        this._Bonuses = new Bonus[info.Length];

                        int count = reader.ReadInt();

                        for (int i = 0; i < count; ++i)
                        {
                            this._Bonuses[i] = new Bonus(this, info[i], reader);
                        }

                        break;
                    }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}