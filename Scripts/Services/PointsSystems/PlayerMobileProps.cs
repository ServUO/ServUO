using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Engines.Points;
using Server.Accounting;
using Server.Engines.BulkOrders;

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

            var context = BulkOrderSystem.GetContext(pm, false);

            if (context != null)
            {
                foreach (var kvp in context.Entries)
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

        public void CheckChanges()
        {
            var context = BulkOrderSystem.GetContext(Player, false);

            if (context != null)
            {
                foreach (var kvp in context.Entries)
                {
                    switch (kvp.Key)
                    {
                        case BODType.Smith:
                            if (Smithy == null)
                                Smithy = new BODData(kvp.Key, kvp.Value);
                            else
                                Smithy.CheckChanges(kvp.Value);
                            break;
                        case BODType.Tailor:
                            if (Tailor == null)
                                Tailor = new BODData(kvp.Key, kvp.Value);
                            else
                                Tailor.CheckChanges(kvp.Value);
                            break;
                        case BODType.Alchemy:
                            if (Alchemy == null)
                                Alchemy = new BODData(kvp.Key, kvp.Value);
                            else
                                Alchemy.CheckChanges(kvp.Value);
                            break;
                        case BODType.Inscription:
                            if (Inscription == null)
                                Inscription = new BODData(kvp.Key, kvp.Value);
                            else
                                Inscription.CheckChanges(kvp.Value);
                            break;
                        case BODType.Tinkering:
                            if (Tinkering == null)
                                Tinkering = new BODData(kvp.Key, kvp.Value);
                            else
                                Tinkering.CheckChanges(kvp.Value);
                            break;
                        case BODType.Cooking:
                            if (Cooking == null)
                                Cooking = new BODData(kvp.Key, kvp.Value);
                            else
                                Cooking.CheckChanges(kvp.Value);
                            break;
                        case BODType.Fletching:
                            if (Fletching == null)
                                Fletching = new BODData(kvp.Key, kvp.Value);
                            else
                                Fletching.CheckChanges(kvp.Value);
                            break;
                        case BODType.Carpentry:
                            if (Carpentry == null)
                                Carpentry = new BODData(kvp.Key, kvp.Value);
                            else
                                Carpentry.CheckChanges(kvp.Value);
                            break;
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Tailor { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Smithy { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Alchemy { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Carpentry { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Cooking { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Fletching { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Inscription { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODData Tinkering { get; set; }
    }

    [PropertyObject]
    public class BODData
    {
        public override string ToString()
        {
            return "...";
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BODType Type { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CachedDeeds { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastBulkOrder { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double BankedPoints { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PendingRewardPoints { get; set; }

        public BODData(BODType type, BODEntry entry)
        {
            Type = type;

            CachedDeeds = entry == null ? 0 : entry.CachedDeeds;
            LastBulkOrder = entry == null ? DateTime.MinValue : entry.LastBulkOrder;
            BankedPoints = entry == null ? 0 : entry.BankedPoints;
            PendingRewardPoints = entry == null ? 0 : entry.PendingRewardPoints;
        }

        public void CheckChanges(BODEntry entry)
        {
            if (entry == null)
                return;

            CachedDeeds = entry.CachedDeeds;
            LastBulkOrder = entry.LastBulkOrder;
            BankedPoints = entry.BankedPoints;
            PendingRewardPoints = entry.PendingRewardPoints;
        }
    }
}