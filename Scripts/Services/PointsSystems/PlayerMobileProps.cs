using Server.Accounting;
using Server.Engines.BulkOrders;
using Server.Engines.CityLoyalty;
using Server.Engines.Points;
using Server.Misc;
using System;

namespace Server.Mobiles
{
    [PropertyObject]
    public class PointsSystemProps
    {
        public override string ToString()
        {
            return "...";
        }

        public PlayerMobile Player { get; set; }

        public PointsSystemProps(PlayerMobile pm)
        {
            Player = pm;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Blackthorn
        {
            get
            {
                return (int)PointsSystem.Blackthorn.GetPoints(Player);
            }
            set
            {
                PointsSystem.Blackthorn.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double CleanUpBrit
        {
            get
            {
                return (int)PointsSystem.CleanUpBritannia.GetPoints(Player);
            }
            set
            {
                PointsSystem.CleanUpBritannia.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double VoidPool
        {
            get
            {
                return (int)PointsSystem.VoidPool.GetPoints(Player);
            }
            set
            {
                PointsSystem.VoidPool.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Casino
        {
            get
            {
                return (int)PointsSystem.CasinoData.GetPoints(Player);
            }
            set
            {
                PointsSystem.CasinoData.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double QueensLoyalty
        {
            get
            {
                return (int)PointsSystem.QueensLoyalty.GetPoints(Player);
            }
            set
            {
                PointsSystem.QueensLoyalty.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ShameCrystals
        {
            get
            {
                return (int)PointsSystem.ShameCrystals.GetPoints(Player);
            }
            set
            {
                PointsSystem.ShameCrystals.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double DespiseCrystals
        {
            get
            {
                return (int)PointsSystem.DespiseCrystals.GetPoints(Player);
            }
            set
            {
                PointsSystem.DespiseCrystals.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ViceVsVirtue
        {
            get
            {
                return (int)PointsSystem.ViceVsVirtue.GetPoints(Player);
            }
            set
            {
                PointsSystem.ViceVsVirtue.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Khaldun
        {
            get
            {
                return (int)PointsSystem.Khaldun.GetPoints(Player);
            }
            set
            {
                PointsSystem.Khaldun.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Doom
        {
            get
            {
                return (int)PointsSystem.TreasuresOfDoom.GetPoints(Player);
            }
            set
            {
                PointsSystem.TreasuresOfDoom.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Doubloons
        {
            get
            {
                return (int)PointsSystem.RisingTide.GetPoints(Player);
            }
            set
            {
                PointsSystem.RisingTide.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Fellowship
        {
            get
            {
                return (int)PointsSystem.FellowshipData.GetPoints(Player);
            }
            set
            {
                PointsSystem.FellowshipData.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double GauntletPoints
        {
            get
            {
                return (int)PointsSystem.DoomGauntlet.GetPoints(Player);
            }
            set
            {
                PointsSystem.DoomGauntlet.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double TOTPoints
        {
            get
            {
                return (int)PointsSystem.TreasuresOfTokuno.GetPoints(Player);
            }
            set
            {
                PointsSystem.TreasuresOfTokuno.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TOTurnIns
        {
            get
            {
                return PointsSystem.TreasuresOfTokuno.GetTurnIns(Player);
            }
            set
            {
                PointsSystem.TreasuresOfTokuno.GetPlayerEntry<TreasuresOfTokuno.TOTEntry>(Player).TurnIns = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double VASPoints
        {
            get
            {
                return (int)PointsSystem.VirtueArtifacts.GetPoints(Player);
            }
            set
            {
                PointsSystem.VirtueArtifacts.SetPoints(Player, value);
            }
        }

        private CityLoyaltyProps _CityLoyaltyProps;

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyProps CityLoyalty
        {
            get
            {
                if (_CityLoyaltyProps == null)
                    _CityLoyaltyProps = new CityLoyaltyProps(Player);

                return _CityLoyaltyProps;
            }
            set
            {
            }
        }
    }

    [PropertyObject]
    public class AccountGoldProps
    {
        public override string ToString()
        {
            if (Player.Account == null)
                return "...";

            return (Player.Account.TotalCurrency * Account.CurrencyThreshold).ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

        public PlayerMobile Player { get; set; }

        public AccountGoldProps(PlayerMobile pm)
        {
            Player = pm;
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public int AccountGold
        {
            get
            {
                if (Player.Account == null)
                    return 0;

                return Player.Account.TotalGold;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int AccountPlatinum
        {
            get
            {
                if (Player.Account == null)
                    return 0;

                return Player.Account.TotalPlat;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double TotalCurrency
        {
            get
            {
                if (Player.Account == null)
                    return 0;

                return Player.Account.TotalCurrency * Account.CurrencyThreshold;
            }
        }
    }

    [PropertyObject]
    public class BODProps
    {
        public override string ToString()
        {
            return "...";
        }

        public PlayerMobile Player { get; set; }

        public BODProps(PlayerMobile pm)
        {
            Player = pm;

            BODContext context = BulkOrderSystem.GetContext(pm, false);

            if (context != null)
            {
                foreach (System.Collections.Generic.KeyValuePair<BODType, BODEntry> kvp in context.Entries)
                {
                    switch (kvp.Key)
                    {
                        case BODType.Smith: Smithy = new BODData(kvp.Key, kvp.Value); break;
                        case BODType.Tailor: Tailor = new BODData(kvp.Key, kvp.Value); break;
                        case BODType.Alchemy: Alchemy = new BODData(kvp.Key, kvp.Value); break;
                        case BODType.Inscription: Inscription = new BODData(kvp.Key, kvp.Value); break;
                        case BODType.Tinkering: Tinkering = new BODData(kvp.Key, kvp.Value); break;
                        case BODType.Cooking: Cooking = new BODData(kvp.Key, kvp.Value); break;
                        case BODType.Fletching: Fletching = new BODData(kvp.Key, kvp.Value); break;
                        case BODType.Carpentry: Carpentry = new BODData(kvp.Key, kvp.Value); break;
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Tailor { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Smithy { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Alchemy { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Carpentry { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Cooking { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Fletching { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Inscription { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Tinkering { get; private set; }
    }

    [PropertyObject]
    public class BODData
    {
        public override string ToString()
        {
            return "...";
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODEntry Entry { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODType Type { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CachedDeeds => Entry == null ? 0 : Entry.CachedDeeds;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastBulkOrder => Entry == null ? DateTime.MinValue : Entry.LastBulkOrder;

        [CommandProperty(AccessLevel.GameMaster)]
        public double BankedPoints => Entry == null ? 0 : Entry.BankedPoints;

        [CommandProperty(AccessLevel.GameMaster)]
        public int PendingRewardPoints => Entry == null ? 0 : Entry.PendingRewardPoints;

        public BODData(BODType type, BODEntry entry)
        {
            Type = type;
            Entry = entry;
        }
    }

    [PropertyObject]
    public class CityLoyaltyProps
    {
        public PlayerMobile Player { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyEntry Moonglow
        {
            get
            {
                return CityLoyaltySystem.Moonglow.GetPlayerEntry<CityLoyaltyEntry>(Player);
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyEntry Britain
        {
            get
            {
                return CityLoyaltySystem.Britain.GetPlayerEntry<CityLoyaltyEntry>(Player);
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyEntry Jhelom
        {
            get
            {
                return CityLoyaltySystem.Jhelom.GetPlayerEntry<CityLoyaltyEntry>(Player);
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyEntry Yew
        {
            get
            {
                return CityLoyaltySystem.Yew.GetPlayerEntry<CityLoyaltyEntry>(Player);
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyEntry Minoc
        {
            get
            {
                return CityLoyaltySystem.Minoc.GetPlayerEntry<CityLoyaltyEntry>(Player);
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyEntry Trinsic
        {
            get
            {
                return CityLoyaltySystem.Trinsic.GetPlayerEntry<CityLoyaltyEntry>(Player);
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyEntry SkaraBrae
        {
            get
            {
                return CityLoyaltySystem.SkaraBrae.GetPlayerEntry<CityLoyaltyEntry>(Player);
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyEntry NewMagincia
        {
            get
            {
                return CityLoyaltySystem.NewMagincia.GetPlayerEntry<CityLoyaltyEntry>(Player);
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltyEntry Vesper
        {
            get
            {
                return CityLoyaltySystem.Vesper.GetPlayerEntry<CityLoyaltyEntry>(Player);
            }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityTradeSystem.CityTradeEntry TradeEntry
        {
            get
            {
                return CityLoyaltySystem.CityTrading.GetPlayerEntry<CityTradeSystem.CityTradeEntry>(Player);
            }
            set { }
        }

        public CityLoyaltyProps(PlayerMobile pm)
        {
            Player = pm;
        }

        public override string ToString()
        {
            CityLoyaltySystem sys = CityLoyaltySystem.GetCitizenship(Player, false);

            if (sys != null)
            {
                return string.Format("Citizenship: {0}", sys.City.ToString());
            }

            return base.ToString();
        }
    }
}
