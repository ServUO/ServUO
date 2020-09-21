using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Accounting;
using Server.Engines.NewMagincia;
using Server.Engines.UOStore;
using Server.ContextMenus;
using Server.Spells;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Server.AccountVault
{
    [Flipable(0x9FE7, 0x9FE8)]
    public class AccountVault : Container
    {
        public override int DefaultMaxWeight => 0; // unlimited weight
        public override bool DisplaysContent => false;
        public override bool DisplayWeight => false;
        public override bool IsDecoContainer => false;

        private int _Balance;

        [CommandProperty(AccessLevel.GameMaster, true)]
        public string Account { get; set; }

        [CommandProperty(AccessLevel.GameMaster, true)]
        public DateTime NextRent { get; set; }

        [CommandProperty(AccessLevel.GameMaster, true)]
        public int Balance
        {
            get { return _Balance; }
            set
            {
                _Balance = value;

                if (PastDue && Balance >= SystemSettings.RentValue)
                {
                    ChargeRent();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster, true)]
        public bool PastDue { get; set; }

        [CommandProperty(AccessLevel.GameMaster, true)]
        public int Index { get; set; } = -1;

        private AccountVaultContainer _Container;

        [CommandProperty(AccessLevel.GameMaster, true)]
        public AccountVaultContainer Container
        {
            get
            {
                if (Account == null)
                {
                    return _Container;
                }

                if (_Container == null)
                {
                    _Container = new AccountVaultContainer(Index);
                    DropItem(_Container);
                }

                return _Container;
            }
        }

        [Constructable]
        public AccountVault()
            : this(0x9FE7)
        {
        }

        [Constructable]
        public AccountVault(int id)
            : base(id)
        {
            AddVault();
            Movable = false;
        }

        public AccountVault(Serial serial)
            : base(serial)
        {
            AddVault();
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (Account != null)
            {
                list.Add(1157999, Index.ToString());
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!SystemSettings.Enabled)
            {
                return;
            }

            var pm = from as PlayerMobile;

            if (pm == null || pm.Account == null || !InRange(from.Location, 3))
            {
                return;
            }

            if (from.AccessLevel > AccessLevel.Player)
            {
                base.OnDoubleClick(from);
                return;
            }

            if (Account == null && !HasVault(from))
            {
                TryRentVault(from as PlayerMobile, this);
            }
            else if (from.Criminal)
            {
                from.SendLocalizedMessage(1158195); // Thou art a criminal and cannot access thy vault.
            }
            else if (HasAccess(from) && Container != null && from.NetState != null)
            {
                ProcessOpeners(from);

                from.Send(Container.OPLPacket);
                from.NetState.Send(new ContainerContentUpdate(Container));

                Timer.DelayCall(TimeSpan.FromMilliseconds(250), () =>
                {
                    Container.DisplayTo(pm);

                    var manager = FindNearest<VaultManager>(pm, m => m.InRange(pm.Location, Core.GlobalUpdateRange));

                    if (manager != null)
                    {
                        manager.SayTo(pm, 1158644, string.Format("{0}\t{1}", TotalItems, TotalWeight), 0x3B2); // Vault container has ~1_VAL~ items, ~2_VAL~ stones
                    }
                });
            }
        }

        public bool HasAccess(Mobile from)
        {
            return Account != null && from.Account != null && from.Account.Username == Account;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.WriteItem(_Container);
            writer.Write(PastDue);
            writer.Write(Index);

            writer.Write(Account);
            writer.Write(Balance);
            writer.Write(NextRent);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt(); // version

            switch (version)
            {
                case 1:
                    _Container = reader.ReadItem<AccountVaultContainer>();
                    PastDue = reader.ReadBool();
                    Index = reader.ReadInt();
                    goto case 0;
                case 0:
                    Account = reader.ReadString();
                    Balance = reader.ReadInt();
                    NextRent = reader.ReadDateTime();
                    break;
            }

            if (version == 0)
            {
                Timer.DelayCall(() =>
                {
                    if (Account != null)
                    {
                        var cont = Container;
                        var list = new List<Item>(Items);

                        for (int i = 0; i < list.Count; i++)
                        {
                            cont.DropItem(list[i]);
                        }

                        DropItem(cont);
                    }

                    AssignIndex();
                });
            }

            AssignIndex();
        }

        public override void OnDelete()
        {
            RemoveVault();

            base.OnDelete();
        }

        private void AddVault()
        {
            if (!Vaults.Contains(this))
            {
                Vaults.Add(this);
            }
        }

        private void RemoveVault()
        {
            ClearVault();

            Vaults.Remove(this);
        }

        public void TakeOwnership(PlayerMobile from)
        {
            SystemSettings.WithdrawBalance(from);

            NextRent = DateTime.UtcNow + SystemSettings.RentTimeSpan;
            Account = from.Account.Username;

            InvalidateProperties();

            BaseGump.SendGump(new VaultActionsGump(from, this));
            BaseGump.SendGump(new NewVaultPurchaseGump(from, this));
        }

        public void MoveTo(Mobile from)
        {
            var map = Map;

            if (map != null && map != Map.Internal)
            {
                Point3D point;

                for (int i = 0; i < 20; i++)
                {
                    point = new Point3D(Utility.RandomMinMax(X - 1, X + 1), Utility.RandomMinMax(Y - 1, Y + 1), Z);

                    SpellHelper.AdjustField(ref point, map, 16, false);

                    if (map.CanFit(point.X, point.Y, point.Z, 16, false, false, true))
                    {
                        from.MoveToWorld(point, map);

                        Timer.DelayCall(TimeSpan.FromMilliseconds(250), () =>
                        {
                            OnDoubleClick(from);
                        });

                        break;
                    }
                }
            }
        }

        internal void ClearVault()
        {
            if (_Container != null)
            {
                var auctionVault = AuctionVault.GetFirstAvailable();

                if (auctionVault != null)
                {
                    var auction = auctionVault.Auction;

                    auction.AuctionItem = _Container;

                    _Container.AuctionItem = true;
                    _Container.Movable = false;
                    _Container.MoveToWorld(new Point3D(auctionVault.X, auctionVault.Y, auctionVault.Z + 7), auctionVault.Map);

                    auction.Description = "Contents of an unpaid vault.";
                    auction.Duration = SystemSettings.AuctionDuration;
                    auction.StartBid = 1000;
                    auction.OnBegin();

                    var m = GetAccountMobile();

                    if (m != null)
                    {
                        var region = Region.Find(GetWorldLocation(), Map).Name;

                        if (!string.IsNullOrEmpty(region))
                        {
                            MaginciaLottoSystem.SendMessageTo(
                                m,
                                new NewMaginciaMessage(
                                    1158081, // Rent Past Due
                                    1158053, // Your vault contents have been put up for auction and no longer can be claimed. This vault was located in ~1_CITY~.
                                    SystemSettings.PastDuePeriod,
                                    region,
                                    true));
                        }
                        else
                        {
                            MaginciaLottoSystem.SendMessageTo(
                                m,
                                new NewMaginciaMessage(
                                    1158081, // Rent Past Due
                                    string.Format("Your vault contents have been put up for auction and no longer can be claimed. This vault is located at {0} [{1}].", GetWorldLocation().ToString(), Map.ToString()),
                                    SystemSettings.PastDuePeriod,
                                    true));
                        }
                    }

                    _Container = null;
                }
                else
                {
                    // None available, so we'll delay a day
                    NextRent = DateTime.UtcNow + TimeSpan.FromDays(1);
                }
            }

            Account = null;
            NextRent = DateTime.MinValue;

            ColUtility.SafeDelete(Items);

            InvalidateProperties();
            Hue = 0;
            Balance = 0;
            PastDue = false;
        }

        internal void ClaimVault(Mobile m)
        {
            if (Account == null || m.Account == null || m.Account.Username != Account)
                return;

            var cont = Container;
            var items = new List<Item>(Items.Where(i => i != cont && !i.IsChildOf(cont)));

            foreach (var item in items)
            {
                cont.DropItem(item);
            }

            ColUtility.Free(items);
            _Container = null;

            cont.Index = Index;

            if (m.Backpack == null || !m.Backpack.TryDropItem(m, cont, false))
            {
                m.BankBox.DropItem(cont);
            }

            ClearVault();
        }

        public void OnTick()
        {
            if (DateTime.UtcNow >= NextRent)
            {
                if (Balance >= SystemSettings.RentValue)
                {
                    ChargeRent();
                }
                else
                {
                    if (PastDue)
                    {
                        ClearVault();
                    }
                    else
                    {
                        PastDue = true;
                        DoWarning();
                        NextRent = DateTime.UtcNow + SystemSettings.PastDuePeriod;
                    }
                }
            }
        }

        private void ChargeRent()
        {
            Balance -= SystemSettings.RentValue;
            NextRent = DateTime.UtcNow + SystemSettings.RentTimeSpan;

            if (PastDue)
            {
                PastDue = false;
            }
        }

        private void DoWarning()
        {
            var m = GetAccountMobile();

            if (m != null)
            {
                var region = Region.Find(GetWorldLocation(), Map).Name;

                if (!string.IsNullOrEmpty(region))
                {
                    MaginciaLottoSystem.SendMessageTo(
                        m,
                        new NewMaginciaMessage(
                            1158081, // Rent Past Due
                            1158041, //Rent is past due for your Vault and your items will be lost after 168 hours unless you claim your Vault from the Vault Manager. This vault is located in ~1_CITY~.
                            SystemSettings.PastDuePeriod,
                            region,
                            true));
                }
                else
                {
                    MaginciaLottoSystem.SendMessageTo(
                        m,
                        new NewMaginciaMessage(
                            1158081, // Rent Past Due
                            string.Format("Rent is past due for your Vault and your items will be lost after 168 hours unless you claim your Vault from the Vault Manager. This vault is located at {0} [{1}].", GetWorldLocation().ToString(), Map.ToString()),
                            SystemSettings.PastDuePeriod,
                            true));
                }
            }
        }

        private Mobile GetAccountMobile()
        {
            if (Account == null)
            {
                return null;
            }

            var account = Accounts.GetAccount(Account);

            if (account != null)
            {
                for (int i = 0; i < account.Length; i++)
                {
                    var m = account[i];

                    if (m != null)
                    {
                        return m;
                    }
                }
            }

            return null;
        }

        public override void OnMapChange()
        {
            AssignIndex();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            var old = Region.Find(oldLocation, Map);

            if (old != Region.Find(GetWorldLocation(), Map))
            {
                AssignIndex();
            }
        }

        public void AssignIndex()
        {
            if (World.Loading)
            {
                return;
            }

            var region = Region.Find(GetWorldLocation(), Map);
            var list = new List<AccountVault>();

            for (int i = 0; i < Vaults.Count; i++)
            {
                var vault = Vaults[i];

                if (Region.Find(vault.GetWorldLocation(), vault.Map).IsPartOf(region))
                {
                    list.Add(vault);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (!list.Any(vault => vault.Index == i))
                {
                    Index = i;
                    break;
                }
            }

            Index = list.Count;
        }

        #region Static Properties and Methods
        public static List<AccountVault> Vaults { get; set; } = new List<AccountVault>();

        public static bool HasVault(Mobile m)
        {
            return GetVault(m) != null;
        }

        public static AccountVault GetVault(Mobile m)
        {
            if (m.Account == null || m.Map == Map.Internal)
            {
                return null;
            }

            var reg = Region.Find(m.Location, m.Map).Name;

            if (string.IsNullOrEmpty(reg))
            {
                return null;
            }

            return Vaults.FirstOrDefault(v => v.Account == m.Account.Username && Region.Find(v.GetWorldLocation(), v.Map).IsPartOf(reg) && v.Map == m.Map);
        }

        public static void TryRentVault(PlayerMobile pm, AccountVault vault)
        {
            TryRentVault(pm, vault, null);
        }

        public static void TryRentVault(PlayerMobile pm, VaultManager manager)
        {
            TryRentVault(pm, null, manager);
        }

        public static void TryRentVault(PlayerMobile pm, AccountVault vault, VaultManager manager)
        {
            if (pm != null && pm.Account != null && !HasVault(pm))
            {
                if (vault == null)
                {
                    vault = FindNearest<AccountVault>(pm, v => v.Account == null);
                }

                if (vault != null)
                {
                    if (!ValidateLocation(vault))
                    {
                        Utility.WriteConsoleColor(ConsoleColor.Red, "Invalid Account Vault Location: {0} [{1}]", vault.Location.ToString(), vault.Map != null ? vault.Map.ToString() : "null map");
                        pm.SendMessage("You cannot rent an account vault in this location!");
                    }
                    else if (!SystemSettings.HasBalance(pm))
                    {
                        if (manager == null)
                        {
                            manager = FindNearest<VaultManager>(pm);
                        }

                        if (SystemSettings.UseTokens)
                        {
                            if (manager != null)
                            {
                                manager.SayTo(pm, 1158313, 0x3B2); // But thou hast not the vault tokens! Please visit the Ultima Store for more details.
                            }
                            else
                            {
                                pm.SendLocalizedMessage(1158313); // But thou hast not the vault tokens! Please visit the Ultima Store for more details.
                            }

                            var entry = UltimaStore.GetEntry(typeof(VaultToken));

                            if (entry != null)
                            {
                                BaseGump.SendGump(new PromoItemGump(pm, entry, 0x9CCB));
                            }
                        }
                        else if (manager != null)
                        {
                            manager.SayTo(pm, 0x3B2, "But thou hast not the gold! You need {0} to rent a vault!", SystemSettings.RentGoldValue.ToString("N0", CultureInfo.GetCultureInfo("en-US")));
                        }
                        else
                        {
                            pm.SendMessage("But thou hast not the gold! You need {0} to rent a vault!", SystemSettings.RentGoldValue.ToString("N0", CultureInfo.GetCultureInfo("en-US")));
                        }
                    }
                    else
                    {
                        BaseGump.SendGump(new PetTrainingStyleConfirmGump(pm, 1074974, SystemSettings.RentMessage(), () =>
                        {
                            if (CanAssignVault(pm, vault))
                            {
                                vault.TakeOwnership(pm);
                            }
                        }));
                    }
                }
                else
                {
                    // TODO: Message?
                }
            }
        }

        public static bool CanAssignVault(PlayerMobile pm, AccountVault vault)
        {
            if (pm.Account == null)
            {
                pm.SendMessage("An error occured why purchasing your vault. Try again later.");
            }
            else if (HasVault(pm))
            {
                pm.SendLocalizedMessage(1158080); // You may only rent one vault at a time at this location.
            }
            else if (vault.Account != null)
            {
                pm.SendMessage("That vault has already been claimed!");
            }
            else if (!SystemSettings.HasBalance(pm))
            {
                pm.SendMessage("You lack the {0} to rent the account vault for the first month!", SystemSettings.UseTokens ? "vault credits" : "gold");
            }
            else if (pm.Criminal)
            {
                pm.SendLocalizedMessage(1158195); // Thou art a criminal and cannot access thy vault.
            }
            else
            {
                return true;
            }

            return false;
        }

        public static T FindNearest<T>(IEntity e, Func<T, bool> predicate = null) where T : IEntity
        {
            IPooledEnumerable eable = null;
            var loc = e.Location;
            var map = e.Map;

            for (int i = 0; i < 50; i++)
            {
                eable = map.GetObjectsInRange(loc, i);

                var toFind = eable.OfType<T>().FirstOrDefault(entity => predicate == null || predicate(entity));

                if (toFind != null)
                {
                    eable.Free();
                    return toFind;
                }
            }

            if (eable != null)
            {
                eable.Free();
            }

            return default(T);
        }

        public static bool ValidateLocation(AccountVault v)
        {
            var reg = Region.Find(v.GetWorldLocation(), v.Map);

            if (SystemSettings.VaultRegions.Any(r => reg.IsPartOf(r)))
            {
                return ValidateMap(v);
            }

            return false;
        }

        public static bool ValidateMap(AccountVault v)
        {
            if (Siege.SiegeShard)
            {
                return v.Map != Map.Trammel;
            }

            return v.Map != Map.Felucca;
        }
        #endregion
    }

    public class AccountVaultContainer : WoodenChest
    {
        private int _Index = -1;
        private bool _AuctionItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Index { get { return _Index; } set { _Index = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AuctionItem { get { return _AuctionItem; } set { _AuctionItem = value; InvalidateProperties(); } }

        public override int DefaultMaxWeight => DisplaysContent ? GlobalMaxWeight : 0;
        public override bool DisplayWeight => DisplaysContent;
        public override bool DisplaysContent => Vault == null && !AuctionItem;

        public VaultAuctionClaimTimer DeadlineTimer { get; set; }

        public AccountVault Vault => RootParent as AccountVault;

        public AccountVaultContainer(int index)
        {
            _Index = index;
        }

        public AccountVaultContainer(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (_Index > -1)
            {
                list.Add(1157997, _Index.ToString());
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> entries)
        {
            base.GetContextMenuEntries(from, entries);

            if (_AuctionItem)
            {
                entries.Add(new ViewContents(this));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player || (!_AuctionItem && Vault == null))
            {
                base.OnDoubleClick(from);
            }
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (_AuctionItem && from.AccessLevel == AccessLevel.Player)
            {
                return false;
            }

            return base.CheckLift(from, item, ref reject);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (_AuctionItem && from.AccessLevel == AccessLevel.Player)
            {
                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item dropped, Point3D p)
        {
            if (_AuctionItem && from.AccessLevel == AccessLevel.Player)
            {
                return false;
            }

            return base.OnDragDropInto(from, dropped, p);
        }

        public override void Delete()
        {
            base.Delete();

            StopTimer();
        }

        private class ViewContents : ContextMenuEntry
        {
            public Container Container { get; set; }

            public ViewContents(Container c)
                : base(1158005, 2) // Vault Contents
            {
                Container = c;
            }

            public override void OnClick()
            {
                var pm = Owner.From as PlayerMobile;

                if (pm != null && Container != null && !Container.Deleted)
                {
                    BaseGump.SendGump(new ContainerDisplayGump(pm, Container, 1158005));
                }
            }
        }

        public bool TryClaim(Mobile from)
        {
            ConvertCommodities();
            AuctionItem = false;

            if (from.Backpack == null || !from.Backpack.TryDropItem(from, this, false))
            {
                StartTimer(from, DateTime.UtcNow + SystemSettings.ClaimPeriod);

                Internalize();

                return false;
            }

            Movable = true;
            return true;
        }

        private void ConvertCommodities()
        {
            var items = new List<Item>(Items);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (item is ICommodity && ((ICommodity)item).IsDeedable)
                {
                    var deed = new CommodityDeed();

                    if (deed.SetCommodity(item))
                    {
                        DropItem(deed);
                    }
                    else
                    {
                        deed.Delete();
                    }
                }
            }

            ColUtility.Free(items);
        }

        public void StartTimer(Mobile from, DateTime deadline)
        {
            StopTimer();

            DeadlineTimer = new VaultAuctionClaimTimer(from, this, deadline);
            DeadlineTimer.Start();
        }

        public void StopTimer()
        {
            if (DeadlineTimer != null)
            {
                DeadlineTimer.Stop();
                DeadlineTimer = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Index);
            writer.Write(_AuctionItem);
            writer.Write(DeadlineTimer != null ? 0 : 1);

            if (DeadlineTimer != null)
            {
                writer.Write(DeadlineTimer.Winner);
                writer.Write(DeadlineTimer.Deadline);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            _Index = reader.ReadInt();
            _AuctionItem = reader.ReadBool();

            if (reader.ReadInt() == 0)
            {
                var m = reader.ReadMobile();
                var dt = reader.ReadDateTime();

                if (m != null)
                {
                    DeadlineTimer = new VaultAuctionClaimTimer(m, this, dt);
                    DeadlineTimer.Start();
                }
            }
        }
    }
}
