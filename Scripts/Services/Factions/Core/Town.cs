using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Targeting;

namespace Server.Factions
{
    [CustomEnum(new string[] { "Britain", "Magincia", "Minoc", "Moonglow", "Skara Brae", "Trinsic", "Vesper", "Yew" })]
    public abstract class Town : IComparable
    {
        public static readonly TimeSpan TaxChangePeriod = TimeSpan.FromHours(12.0);
        public static readonly TimeSpan IncomePeriod = TimeSpan.FromDays(1.0);
        public const int SilverCaptureBonus = 10000;
        private TownDefinition m_Definition;
        private TownState m_State;
        private Timer m_IncomeTimer;
        private List<VendorList> m_VendorLists;
        private List<GuardList> m_GuardLists;
        public Town()
        {
            this.m_State = new TownState(this);
            this.ConstructVendorLists();
            this.ConstructGuardLists();
            this.StartIncomeTimer();
        }

        public static List<Town> Towns
        {
            get
            {
                return Reflector.Towns;
            }
        }
        public TownDefinition Definition
        {
            get
            {
                return this.m_Definition;
            }
            set
            {
                this.m_Definition = value;
            }
        }
        public TownState State
        {
            get
            {
                return this.m_State;
            }
            set
            {
                this.m_State = value;
                this.ConstructGuardLists();
            }
        }
        public int Silver
        {
            get
            {
                return this.m_State.Silver;
            }
            set
            {
                this.m_State.Silver = value;
            }
        }
        public Faction Owner
        {
            get
            {
                return this.m_State.Owner;
            }
            set
            {
                this.Capture(value);
            }
        }
        public Mobile Sheriff
        {
            get
            {
                return this.m_State.Sheriff;
            }
            set
            {
                this.m_State.Sheriff = value;
            }
        }
        public Mobile Finance
        {
            get
            {
                return this.m_State.Finance;
            }
            set
            {
                this.m_State.Finance = value;
            }
        }
        public int Tax
        {
            get
            {
                return this.m_State.Tax;
            }
            set
            {
                this.m_State.Tax = value;
            }
        }
        public DateTime LastTaxChange
        {
            get
            {
                return this.m_State.LastTaxChange;
            }
            set
            {
                this.m_State.LastTaxChange = value;
            }
        }
        public bool TaxChangeReady
        {
            get
            {
                return (this.m_State.LastTaxChange + TaxChangePeriod) < DateTime.UtcNow;
            }
        }
        public int FinanceUpkeep
        {
            get
            {
                List<VendorList> vendorLists = this.VendorLists;
                int upkeep = 0;

                for (int i = 0; i < vendorLists.Count; ++i)
                    upkeep += vendorLists[i].Vendors.Count * vendorLists[i].Definition.Upkeep;

                return upkeep;
            }
        }
        public int SheriffUpkeep
        {
            get
            {
                List<GuardList> guardLists = this.GuardLists;
                int upkeep = 0;

                for (int i = 0; i < guardLists.Count; ++i)
                    upkeep += guardLists[i].Guards.Count * guardLists[i].Definition.Upkeep;

                return upkeep;
            }
        }
        public int DailyIncome
        {
            get
            {
                return (10000 * (100 + this.m_State.Tax)) / 100;
            }
        }
        public int NetCashFlow
        {
            get
            {
                return this.DailyIncome - this.FinanceUpkeep - this.SheriffUpkeep;
            }
        }
        public TownMonolith Monolith
        {
            get
            {
                List<BaseMonolith> monoliths = BaseMonolith.Monoliths;

                foreach (BaseMonolith monolith in monoliths)
                {
                    if (monolith is TownMonolith)
                    {
                        TownMonolith townMonolith = (TownMonolith)monolith;

                        if (townMonolith.Town == this)
                            return townMonolith;
                    }
                }

                return null;
            }
        }
        public DateTime LastIncome
        {
            get
            {
                return this.m_State.LastIncome;
            }
            set
            {
                this.m_State.LastIncome = value;
            }
        }
        public List<VendorList> VendorLists
        {
            get
            {
                return this.m_VendorLists;
            }
            set
            {
                this.m_VendorLists = value;
            }
        }
        public List<GuardList> GuardLists
        {
            get
            {
                return this.m_GuardLists;
            }
            set
            {
                this.m_GuardLists = value;
            }
        }
        public static Town FromRegion(Region reg)
        {
            if (reg.Map != Faction.Facet)
                return null;

            List<Town> towns = Towns;

            for (int i = 0; i < towns.Count; ++i)
            {
                Town town = towns[i];

                if (reg.IsPartOf(town.Definition.Region))
                    return town;
            }

            return null;
        }

        public static void Initialize()
        {
            List<Town> towns = Towns;

            for (int i = 0; i < towns.Count; ++i)
            {
                towns[i].Sheriff = towns[i].Sheriff;
                towns[i].Finance = towns[i].Finance;
            }

            CommandSystem.Register("GrantTownSilver", AccessLevel.Administrator, new CommandEventHandler(GrantTownSilver_OnCommand));
        }

        public static void WriteReference(GenericWriter writer, Town town)
        {
            int idx = Towns.IndexOf(town);

            writer.WriteEncodedInt((int)(idx + 1));
        }

        public static Town ReadReference(GenericReader reader)
        {
            int idx = reader.ReadEncodedInt() - 1;

            if (idx >= 0 && idx < Towns.Count)
                return Towns[idx];

            return null;
        }

        public static Town Parse(string name)
        {
            List<Town> towns = Towns;

            for (int i = 0; i < towns.Count; ++i)
            {
                Town town = towns[i];

                if (Insensitive.Equals(town.Definition.FriendlyName, name))
                    return town;
            }

            return null;
        }

        public static void GrantTownSilver_OnCommand(CommandEventArgs e)
        {
            Town town = FromRegion(e.Mobile.Region);

            if (town == null)
                e.Mobile.SendMessage("You are not in a faction town.");
            else if (e.Length == 0)
                e.Mobile.SendMessage("Format: GrantTownSilver <amount>");
            else
            {
                town.Silver += e.GetInt32(0);
                e.Mobile.SendMessage("You have granted {0:N0} silver to the town. It now has {1:N0} silver.", e.GetInt32(0), town.Silver);
            }
        }

        public void BeginOrderFiring(Mobile from)
        {
            bool isFinance = this.IsFinance(from);
            bool isSheriff = this.IsSheriff(from);
            string type = null;

            // NOTE: Messages not OSI-accurate, intentional
            if (isFinance && isSheriff) // GM only
                type = "vendor or guard";
            else if (isFinance)
                type = "vendor";
            else if (isSheriff)
                type = "guard";

            from.SendMessage("Target the {0} you wish to dismiss.", type);
            from.BeginTarget(12, false, TargetFlags.None, new TargetCallback(EndOrderFiring));
        }

        public void EndOrderFiring(Mobile from, object obj)
        {
            bool isFinance = this.IsFinance(from);
            bool isSheriff = this.IsSheriff(from);
            string type = null;

            if (isFinance && isSheriff) // GM only
                type = "vendor or guard";
            else if (isFinance)
                type = "vendor";
            else if (isSheriff)
                type = "guard";

            if (obj is BaseFactionVendor)
            {
                BaseFactionVendor vendor = (BaseFactionVendor)obj;

                if (vendor.Town == this && isFinance)
                    vendor.Delete();
            }
            else if (obj is BaseFactionGuard)
            {
                BaseFactionGuard guard = (BaseFactionGuard)obj;

                if (guard.Town == this && isSheriff)
                    guard.Delete();
            }
            else
            {
                from.SendMessage("That is not a {0}!", type);
            }
        }

        public void StartIncomeTimer()
        {
            if (this.m_IncomeTimer != null)
                this.m_IncomeTimer.Stop();

            this.m_IncomeTimer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), new TimerCallback(CheckIncome));
        }

        public void StopIncomeTimer()
        {
            if (this.m_IncomeTimer != null)
                this.m_IncomeTimer.Stop();

            this.m_IncomeTimer = null;
        }

        public void CheckIncome()
        {
            if ((this.LastIncome + IncomePeriod) > DateTime.UtcNow || this.Owner == null)
                return;

            this.ProcessIncome();
        }

        public void ProcessIncome()
        {
            this.LastIncome = DateTime.UtcNow;

            int flow = this.NetCashFlow;

            if ((this.Silver + flow) < 0)
            {
                ArrayList toDelete = this.BuildFinanceList();

                while ((this.Silver + flow) < 0 && toDelete.Count > 0)
                {
                    int index = Utility.Random(toDelete.Count);
                    Mobile mob = (Mobile)toDelete[index];

                    mob.Delete();

                    toDelete.RemoveAt(index);
                    flow = this.NetCashFlow;
                }
            }

            this.Silver += flow;
        }

        public ArrayList BuildFinanceList()
        {
            ArrayList list = new ArrayList();

            List<VendorList> vendorLists = this.VendorLists;

            for (int i = 0; i < vendorLists.Count; ++i)
                list.AddRange(vendorLists[i].Vendors);

            List<GuardList> guardLists = this.GuardLists;

            for (int i = 0; i < guardLists.Count; ++i)
                list.AddRange(guardLists[i].Guards);

            return list;
        }

        public void ConstructGuardLists()
        {
            GuardDefinition[] defs = (this.Owner == null ? new GuardDefinition[0] : this.Owner.Definition.Guards);

            this.m_GuardLists = new List<GuardList>();

            for (int i = 0; i < defs.Length; ++i)
                this.m_GuardLists.Add(new GuardList(defs[i]));
        }

        public GuardList FindGuardList(Type type)
        {
            List<GuardList> guardLists = this.GuardLists;

            for (int i = 0; i < guardLists.Count; ++i)
            {
                GuardList guardList = guardLists[i];

                if (guardList.Definition.Type == type)
                    return guardList;
            }

            return null;
        }

        public void ConstructVendorLists()
        {
            VendorDefinition[] defs = VendorDefinition.Definitions;

            this.m_VendorLists = new List<VendorList>();

            for (int i = 0; i < defs.Length; ++i)
                this.m_VendorLists.Add(new VendorList(defs[i]));
        }

        public VendorList FindVendorList(Type type)
        {
            List<VendorList> vendorLists = this.VendorLists;

            for (int i = 0; i < vendorLists.Count; ++i)
            {
                VendorList vendorList = vendorLists[i];

                if (vendorList.Definition.Type == type)
                    return vendorList;
            }

            return null;
        }

        public bool RegisterGuard(BaseFactionGuard guard)
        {
            if (guard == null)
                return false;

            GuardList guardList = this.FindGuardList(guard.GetType());

            if (guardList == null)
                return false;

            guardList.Guards.Add(guard);
            return true;
        }

        public bool UnregisterGuard(BaseFactionGuard guard)
        {
            if (guard == null)
                return false;

            GuardList guardList = this.FindGuardList(guard.GetType());

            if (guardList == null)
                return false;

            if (!guardList.Guards.Contains(guard))
                return false;

            guardList.Guards.Remove(guard);
            return true;
        }

        public bool RegisterVendor(BaseFactionVendor vendor)
        {
            if (vendor == null)
                return false;

            VendorList vendorList = this.FindVendorList(vendor.GetType());

            if (vendorList == null)
                return false;

            vendorList.Vendors.Add(vendor);
            return true;
        }

        public bool UnregisterVendor(BaseFactionVendor vendor)
        {
            if (vendor == null)
                return false;

            VendorList vendorList = this.FindVendorList(vendor.GetType());

            if (vendorList == null)
                return false;

            if (!vendorList.Vendors.Contains(vendor))
                return false;

            vendorList.Vendors.Remove(vendor);
            return true;
        }

        public bool IsSheriff(Mobile mob)
        {
            if (mob == null || mob.Deleted)
                return false;

            return (mob.AccessLevel >= AccessLevel.GameMaster || mob == this.Sheriff);
        }

        public bool IsFinance(Mobile mob)
        {
            if (mob == null || mob.Deleted)
                return false;

            return (mob.AccessLevel >= AccessLevel.GameMaster || mob == this.Finance);
        }

        public void Capture(Faction f)
        {
            if (this.m_State.Owner == f)
                return;

            if (this.m_State.Owner == null) // going from unowned to owned
            {
                this.LastIncome = DateTime.UtcNow;
                f.Silver += SilverCaptureBonus;
            }
            else if (f == null) // going from owned to unowned
            {
                this.LastIncome = DateTime.MinValue;
            }
            else // otherwise changing hands, income timer doesn't change
            {
                f.Silver += SilverCaptureBonus;
            }

            this.m_State.Owner = f;

            this.Sheriff = null;
            this.Finance = null;

            TownMonolith monolith = this.Monolith;

            if (monolith != null)
                monolith.Faction = f;

            List<VendorList> vendorLists = this.VendorLists;

            for (int i = 0; i < vendorLists.Count; ++i)
            {
                VendorList vendorList = vendorLists[i];
                List<BaseFactionVendor> vendors = vendorList.Vendors;

                for (int j = vendors.Count - 1; j >= 0; --j)
                    vendors[j].Delete();
            }

            List<GuardList> guardLists = this.GuardLists;

            for (int i = 0; i < guardLists.Count; ++i)
            {
                GuardList guardList = guardLists[i];
                List<BaseFactionGuard> guards = guardList.Guards;

                for (int j = guards.Count - 1; j >= 0; --j)
                    guards[j].Delete();
            }

            this.ConstructGuardLists();
        }

        public int CompareTo(object obj)
        {
            return this.m_Definition.Sort - ((Town)obj).m_Definition.Sort;
        }

        public override string ToString()
        {
            return this.m_Definition.FriendlyName;
        }
    }
}