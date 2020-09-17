using Server.Accounting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Prompts;
using Server.Targeting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Server.Mobiles
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlayerVendorTargetAttribute : Attribute
    {
    }

    public class VendorItem
    {
        private string m_Description;

        public VendorItem(Item item, int price, string description, DateTime created)
        {
            Item = item;
            Price = price;

            if (description != null)
                m_Description = description;
            else
                m_Description = "";

            Created = created;

            Valid = true;
        }

        public Item Item { get; }
        public int Price { get; }
        public string FormattedPrice => Price.ToString("N0", CultureInfo.GetCultureInfo("en-US"));
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                if (value != null)
                    m_Description = value;
                else
                    m_Description = "";

                if (Valid)
                    Item.InvalidateProperties();
            }
        }
        public DateTime Created { get; }
        public bool IsForSale => Price >= 0;
        public bool IsForFree => Price == 0;
        public bool Valid { get; private set; }

        public void Invalidate()
        {
            Valid = false;
        }
    }

    public class VendorBackpack : Backpack
    {
        public VendorBackpack()
        {
            Layer = Layer.Backpack;
            Weight = 1.0;
        }

        public VendorBackpack(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultMaxWeight => 0;

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (!base.CheckHold(m, item, message, checkItems, plusItems, plusWeight))
                return false;

            if (Parent is PlayerVendor && item is Container && ((Container)item).Items.OfType<Container>().Any())
            {
                ((PlayerVendor)Parent).SayTo(m, 1017381); // You cannot place a container that has other containers in it on a vendor.

                return false;
            }

            if (Parent is CommissionPlayerVendor)
            {
                BaseHouse house = ((PlayerVendor)Parent).House;

                if (house != null && !house.CheckAosStorage(1 + item.TotalItems + plusItems))
                {
                    if (message)
                        m.SendLocalizedMessage(1061839); // This action would exceed the secure storage limit of the house.

                    return false;
                }
            }

            return true;
        }

        public override bool IsAccessibleTo(Mobile m)
        {
            return true;
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (!base.CheckItemUse(from, item))
                return false;

            if (item is Container || item is Engines.BulkOrders.BulkOrderBook || item is RecipeBook)
                return true;

            from.SendLocalizedMessage(500447); // That is not accessible.
            return false;
        }

        public override bool CheckTarget(Mobile from, Target targ, object targeted)
        {
            if (!base.CheckTarget(from, targ, targeted))
                return false;

            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return targ.GetType().IsDefined(typeof(PlayerVendorTargetAttribute), false);
        }

        public override void GetChildContextMenuEntries(Mobile from, List<ContextMenuEntry> list, Item item)
        {
            base.GetChildContextMenuEntries(from, list, item);

            PlayerVendor pv = RootParent as PlayerVendor;

            if (pv == null || pv.IsOwner(from))
                return;

            VendorItem vi = pv.GetVendorItem(item);

            if (vi != null)
                list.Add(new BuyEntry(item));
        }

        public override void GetChildNameProperties(ObjectPropertyList list, Item item)
        {
            base.GetChildNameProperties(list, item);

            PlayerVendor pv = RootParent as PlayerVendor;

            if (pv == null)
                return;

            VendorItem vi = pv.GetVendorItem(item);

            if (vi == null)
                return;

            if (!vi.IsForSale)
                list.Add(1043307); // Price: Not for sale.
            else if (vi.IsForFree)
                list.Add(1043306); // Price: FREE!
            else
                list.Add(1043304, vi.FormattedPrice); // Price: ~1_COST~
        }

        public override void GetChildProperties(ObjectPropertyList list, Item item)
        {
            base.GetChildProperties(list, item);

            PlayerVendor pv = RootParent as PlayerVendor;

            if (pv == null)
                return;

            VendorItem vi = pv.GetVendorItem(item);

            if (vi != null && vi.Description != null && vi.Description.Length > 0)
                list.Add(1043305, vi.Description); // <br>Seller's Description:<br>"~1_DESC~"
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private class BuyEntry : ContextMenuEntry
        {
            private readonly Item m_Item;

            public BuyEntry(Item item)
                : base(6103)
            {
                m_Item = item;
            }

            public override bool NonLocalUse => true;

            public override void OnClick()
            {
                if (m_Item.Deleted)
                    return;

                PlayerVendor.TryToBuy(m_Item, Owner.From);
            }
        }
    }

    public class PlayerVendor : Mobile
    {
        private Hashtable m_SellItems;
        private BaseHouse m_House;
        private string m_ShopName;

        public double CommissionPerc => 5.25;
        public virtual bool IsCommission => false;

        public PlayerVendor(Mobile owner, BaseHouse house)
        {
            Owner = owner;
            House = house;

            BankAccount = 0;
            HoldGold = 3;

            VendorSearch = true;

            ShopName = "Shop Not Yet Named";

            m_SellItems = new Hashtable();

            CantWalk = true;

            InitStats(100, 100, 100);
            InitBody();
            InitOutfit();

            if (!IsCommission)
            {
                NextPayTime = DateTime.UtcNow + PayTimer.GetInterval();
            }

            PlayerVendors.Add(this);
        }

        public PlayerVendor(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool VendorSearch { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BankAccount { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HoldGold { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ShopName
        {
            get
            {
                return m_ShopName;
            }
            set
            {
                if (value == null)
                    m_ShopName = "";
                else
                    m_ShopName = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextPayTime { get; set; }

        public PlayerVendorPlaceholder Placeholder { get; set; }

        public BaseHouse House
        {
            get
            {
                return m_House;
            }
            set
            {
                if (m_House != null)
                    m_House.PlayerVendors.Remove(this);

                if (value != null)
                    value.PlayerVendors.Add(this);

                m_House = value;
            }
        }

        public int ChargePerDay => ChargePerRealWorldDay / 12;

        public int ChargePerRealWorldDay
        {
            get
            {
                long total = 0;
                foreach (VendorItem vi in m_SellItems.Values)
                {
                    total += vi.Price;
                }

                int perDay = (int)(60 + (total / 500) * 3);

                var trinket = GetMerchantsTrinket();

                if (trinket != null)
                {
                    return perDay - (int)(perDay * ((double)trinket.Bonus / 100));
                }

                return perDay;
            }
        }

        public MerchantsTrinket GetMerchantsTrinket()
        {
            var trinket = FindItemOnLayer(Layer.Earrings) as MerchantsTrinket;

            if (trinket != null && trinket.UsesRemaining > 0)
            {
                return trinket;
            }

            return null;
        }

        public static void TryToBuy(Item item, Mobile from)
        {
            PlayerVendor vendor = item.RootParent as PlayerVendor;

            if (vendor == null || !vendor.CanInteractWith(from, false))
                return;

            if (vendor.IsOwner(from))
            {
                vendor.SayTo(from, 503212); // You own this shop, just take what you want.
                return;
            }

            VendorItem vi = vendor.GetVendorItem(item);

            if (vi == null)
            {
                vendor.SayTo(from, 503216); // You can't buy that.
            }
            else if (!vi.IsForSale)
            {
                vendor.SayTo(from, 503202); // This item is not for sale.
            }
            else if (vi.Created + TimeSpan.FromMinutes(1.0) > DateTime.UtcNow)
            {
                from.SendLocalizedMessage(1071949); // You cannot buy this item right now.  Please wait one minute and try again.
            }
            else
            {
                from.CloseGump(typeof(PlayerVendorBuyGump));
                from.SendGump(new PlayerVendorBuyGump(vendor, vi));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(3); // version

            writer.Write(VendorSearch);
            writer.Write(m_ShopName);
            writer.WriteDeltaTime(NextPayTime);
            writer.Write(House);

            writer.Write(Owner);
            writer.Write(BankAccount);
            writer.Write(HoldGold);

            writer.Write(m_SellItems.Count);
            foreach (VendorItem vi in m_SellItems.Values)
            {
                writer.Write(vi.Item);
                writer.Write(vi.Price);
                writer.Write(vi.Description);

                writer.Write(vi.Created);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                case 2:
                    {
                        VendorSearch = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                    {
                        if(version < 3)
                        {
                            reader.ReadBool();
                        }
                        
                        m_ShopName = reader.ReadString();
                        NextPayTime = reader.ReadDeltaTime();
                        House = (BaseHouse)reader.ReadItem();

                        goto case 0;
                    }
                case 0:
                    {
                        Owner = reader.ReadMobile();
                        BankAccount = reader.ReadInt();
                        HoldGold = reader.ReadInt();

                        m_SellItems = new Hashtable();

                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            Item item = reader.ReadItem();

                            int price = reader.ReadInt();
                            if (price > 175000000)
                                price = 175000000;

                            string description = reader.ReadString();

                            DateTime created = version < 1 ? DateTime.UtcNow : reader.ReadDateTime();

                            if (item != null)
                            {
                                SetVendorItem(item, version < 1 && price <= 0 ? -1 : price, description, created);
                            }
                        }

                        break;
                    }
            }

            if (version == 1)
            {
                VendorSearch = true;
            }

            Blessed = false;

            PlayerVendors.Add(this);
        }

        public void InitBody()
        {
            Hue = Utility.RandomSkinHue();
            SpeechHue = 0x3B2;

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }
        }

        public virtual void InitOutfit()
        {
            Item item = new FancyShirt(Utility.RandomNeutralHue())
            {
                Layer = Layer.InnerTorso
            };
            AddItem(item);
            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new BodySash(Utility.RandomNeutralHue()));
            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new Cloak(Utility.RandomNeutralHue()));

            Utility.AssignRandomHair(this);

            Container pack = new VendorBackpack
            {
                Movable = false
            };
            AddItem(pack);
        }

        public virtual bool IsOwner(Mobile m)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (House != null)
            {
                return House.IsOwner(m);
            }
            else
            {
                return m == Owner;
            }
        }

        public virtual void Destroy(bool toBackpack)
        {
            Return();

            /* Possible cases regarding item return:
            * 
            * 1. No item must be returned
            *       -> do nothing.
            * 2. ( toBackpack is false OR the vendor is in the internal map ) AND the vendor is associated with a AOS house
            *       -> put the items into the moving crate or a vendor inventory,
            *          depending on whether the vendor owner is also the house owner.
            * 3. ( toBackpack is true OR the vendor isn't associated with any AOS house ) AND the vendor isn't in the internal map
            *       -> put the items into a backpack.
            * 4. The vendor isn't associated with any house AND it's in the internal map
            *       -> do nothing (we can't do anything).
            */

            List<Item> list = GetItems();

            if (list.Count > 0 || HoldGold > 0) // No case 1
            {
                if ((!toBackpack || Map == Map.Internal) && House != null) // Case 2
                {
                    if (House.IsOwner(Owner)) // Move to moving crate
                    {
                        if (House.MovingCrate == null)
                            House.MovingCrate = new MovingCrate(House);

                        if (HoldGold > 0)
                        {
                            if (AccountGold.Enabled)
                            {
                                Banker.Deposit(Owner, HoldGold, true);
                            }
                            else
                            {
                                Banker.Deposit(House.MovingCrate, HoldGold);
                            }
                            
                            HoldGold = 0;
                        }

                        foreach (Item item in list)
                        {
                            House.MovingCrate.DropItem(item);
                        }
                    }
                    else // Move to vendor inventory
                    {
                        VendorInventory inventory = new VendorInventory(House, Owner, Name, ShopName)
                        {
                            Gold = HoldGold
                        };
                        HoldGold = 0;

                        foreach (Item item in list)
                        {
                            inventory.AddItem(item);
                        }

                        House.VendorInventories.Add(inventory);
                    }
                }
                else if ((toBackpack || House == null) && Map != Map.Internal) // Case 3 - Move to backpack
                {
                    Container backpack = new Backpack();

                    if (HoldGold > 0)
                    {
                        if (AccountGold.Enabled && Owner != null)
                        {
                            Banker.Deposit(Owner, HoldGold, true);
                        }
                        else
                        {
                            Banker.Deposit(backpack, HoldGold);
                        }
                        
                        HoldGold = 0;
                    }

                    foreach (Item item in list)
                    {
                        backpack.DropItem(item);
                    }

                    backpack.MoveToWorld(Location, Map);
                }
            }

            Delete();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            House = null;

            if (Placeholder != null)
            {
                Placeholder.Delete();
            }
            
            if(PlayerVendors.Contains(this))
            {
                PlayerVendors.Remove(this);
            }
        }

        public override bool IsSnoop(Mobile from)
        {
            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1062449, ShopName); // Shop Name: ~1_NAME~
        }

        public VendorItem GetVendorItem(Item item)
        {
            return (VendorItem)m_SellItems[item];
        }

        public override void OnSubItemAdded(Item item)
        {
            base.OnSubItemAdded(item);

            if (GetVendorItem(item) == null && CanBeVendorItem(item))
            {
                // TODO: default price should be dependent to the type of object
                SetVendorItem(item, 999, "");
            }
        }

        public override void OnSubItemRemoved(Item item)
        {
            base.OnSubItemRemoved(item);

            if (item.GetBounce() == null)
                RemoveVendorItem(item);
        }

        public override void OnSubItemBounceCleared(Item item)
        {
            base.OnSubItemBounceCleared(item);

            if (!CanBeVendorItem(item))
                RemoveVendorItem(item);
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (item == Backpack)
            {
                foreach (Item subItem in item.Items)
                {
                    RemoveVendorItem(subItem);
                }
            }
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (!IsOwner(from))
            {
                SayTo(from, 503209); // I can only take item from the shop owner.
                return false;
            }

            if (item is SecretChest && ((SecretChest)item).Locked)
            {
                SayTo(from, 1151612); // I cannot accept a number key locked item.
                return false;
            }

            if (item is Gold)
            {
                if (HoldGold < 1000000)
                {
                    SayTo(from, 503210); // I'll take that to fund my services.

                    HoldGold += item.Amount;
                    item.Delete();

                    return true;
                }
                else
                {
                    from.SendLocalizedMessage(1062493); // Your vendor has sufficient funds for operation and cannot accept this gold.

                    return false;
                }                        
            }
            else
            {
                bool newItem = (GetVendorItem(item) == null);

                if (Backpack != null && Backpack.TryDropItem(from, item, false))
                {
                    if (newItem)
                        OnItemGiven(from, item);

                    return true;
                }
                else
                {
                    SayTo(from, 503211); // I can't carry any more.
                    return false;
                }
            }
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            if (IsOwner(from))
            {
                if (item is SecretChest && ((SecretChest)item).Locked)
                {
                    SayTo(from, 1151612); // I cannot accept a number key locked item.
                    return false;
                }

                if (GetVendorItem(item) == null)
                {
                    // We must wait until the item is added
                    Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(NonLocalDropCallback), new object[] { from, item });
                }

                return true;
            }
            else
            {
                SayTo(from, 503209); // I can only take item from the shop owner.
                return false;
            }
        }

        public override bool AllowEquipFrom(Mobile from)
        {
            if (IsOwner(from))
                return true;

            return base.AllowEquipFrom(from);
        }

        public override bool CheckNonlocalLift(Mobile from, Item item)
        {
            if (item.IsChildOf(Backpack))
            {
                if (IsOwner(from))
                {
                    return true;
                }
                else
                {
                    SayTo(from, 503223); // If you'd like to purchase an item, just ask.
                    return false;
                }
            }
            else if (IsOwner(from))
            {
                return true;
            }

            return base.CheckNonlocalLift(from, item);
        }

        public bool CanInteractWith(Mobile from, bool ownerOnly)
        {
            if (!from.CanSee(this) || !Utility.InUpdateRange(from, this) || !from.CheckAlive())
                return false;

            if (ownerOnly)
                return IsOwner(from);

            if (House != null && House.IsBanned(from) && !IsOwner(from))
            {
                from.SendLocalizedMessage(1062674); // You can't shop from this home as you have been banned from this establishment.
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsOwner(from))
            {
                SendOwnerGump(from);
            }
            else if (CanInteractWith(from, false))
            {
                OpenBackpack(from);
            }
        }

        public override void DisplayPaperdollTo(Mobile m)
        {
            base.DisplayPaperdollTo(m);

            if (CanInteractWith(m, false))
            {
                OpenBackpack(m);
            }
        }

        public void SendOwnerGump(Mobile to)
        {
            to.CloseGump(typeof(NewPlayerVendorOwnerGump));
            to.CloseGump(typeof(NewPlayerVendorCustomizeGump));

            to.SendGump(new NewPlayerVendorOwnerGump(this));
        }

        public void OpenBackpack(Mobile from)
        {
            if (Backpack != null)
            {
                SayTo(from, IsOwner(from) ? 1010642 : 503208); // Take a look at my/your goods.

                Backpack.DisplayTo(from);
            }
        }

        public void CollectGold(Mobile to)
        {
            if (HoldGold > 0)
            {
                SayTo(to, 1079008, HoldGold.ToString()); // How much of the ~1_gold~ gold that I'm holding would you like?
                to.SendLocalizedMessage(1079007); // Enter the amount of gold you wish to withdraw(ESC = CANCEL):

                to.Prompt = new CollectGoldPrompt(this);
            }
            else
            {
                SayTo(to, 503215); // I am holding no gold for you.
            }
        }

        public void DepositeGold(Mobile to)
        {
            to.SendLocalizedMessage(1156105); // Enter the amount of gold you wish to deposit (ESC = CANCEL):

            to.Prompt = new DepositGoldPrompt(this);
        }

        private class DepositGoldPrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;

            public DepositGoldPrompt(PlayerVendor vendor)
            {
                m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_Vendor.CanInteractWith(from, true))
                    return;

                text = text.Trim();

                int amount;

                if (!int.TryParse(text, out amount))
                    amount = 0;

                TakeGold(from, amount);
            }

            public override void OnCancel(Mobile from)
            {
                if (!m_Vendor.CanInteractWith(from, true))
                    return;

                TakeGold(from, 0);
            }

            public void TakeGold(Mobile to, int amount)
            {
                if (amount <= 0 || amount > Banker.GetBalance(to))
                {
                    to.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                }
                else if (m_Vendor.HoldGold + amount > 1000000)
                {
                    to.SendLocalizedMessage(1062671); // That would exceed your vendors account limit (1 million gold).
                }
                else
                {
                    Banker.Withdraw(to, amount, true);
                    m_Vendor.HoldGold += amount;

                    m_Vendor.SayTo(to, 503210); // I'll take that to fund my services.
                }
            }
        }

        public int GiveGold(Mobile to, int amount)
        {
            if (amount <= 0)
                return 0;

            if (amount > HoldGold)
            {
                SayTo(to, 1071950, HoldGold.ToString()); // I'm sorry, but I'm only holding ~1_VAL~ gold for you.
                return 0;
            }

            int amountGiven = Banker.DepositUpTo(to, amount, amount > 0);
            HoldGold -= amountGiven;

            if (amountGiven == 0)
            {
                SayTo(to, 1070755); // Your bank box cannot hold the gold you are requesting.  I will keep the gold until you can take it.
            }
            else if (amount > amountGiven)
            {
                SayTo(to, 1070756); // I can only give you part of the gold now, as your bank box is too full to hold the full amount.
            }
            else if (HoldGold > 0)
            {
                SayTo(to, 1042639); // Your gold has been transferred.
            }
            else
            {
                SayTo(to, 503234); // All the gold I have been carrying for you has been deposited into your bank account.
            }

            return amountGiven;
        }

        public void Dismiss(Mobile from)
        {
            Container pack = Backpack;

            if (pack != null && pack.Items.Count > 0)
            {
                SayTo(from, 1038325); // You cannot dismiss me while I am holding your goods.
                return;
            }

            if (HoldGold > 0)
            {
                GiveGold(from, HoldGold);

                if (HoldGold > 0)
                    return;
            }

            Destroy(true);
        }

        public void Rename(Mobile from)
        {
            from.SendLocalizedMessage(1062494); // Enter a new name for your vendor (20 characters max):

            from.Prompt = new VendorNamePrompt(this);
        }

        public void RenameShop(Mobile from)
        {
            from.SendLocalizedMessage(1062433); // Enter a new name for your shop (20 chars max):

            from.Prompt = new ShopNamePrompt(this);
        }

        public bool CheckTeleport(Mobile to)
        {
            if (Deleted || !IsOwner(to) || House == null || Map == Map.Internal)
                return false;

            if (House.IsInside(to) || to.Map != House.Map || !House.InRange(to, 5))
                return false;

            if (Placeholder == null)
            {
                Placeholder = new PlayerVendorPlaceholder(this);
                Placeholder.MoveToWorld(Location, Map);

                MoveToWorld(to.Location, to.Map);

                to.SendLocalizedMessage(1062431); // This vendor has been moved out of the house to your current location temporarily.  The vendor will return home automatically after two minutes have passed once you are done managing its inventory or customizing it.
            }
            else
            {
                Placeholder.RestartTimer();

                to.SendLocalizedMessage(1062430); // This vendor is currently temporarily in a location outside its house.  The vendor will return home automatically after two minutes have passed once you are done managing its inventory or customizing it.
            }

            return true;
        }

        public void Return()
        {
            if (Placeholder != null)
                Placeholder.Delete();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && Placeholder != null && IsOwner(from))
            {
                list.Add(new ReturnVendorEntry(this));
            }

            base.GetContextMenuEntries(from, list);
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return from.Alive && from.GetDistanceToSqrt(this) <= 3;
        }

        public bool WasNamed(string speech)
        {
            return Name != null && Insensitive.StartsWith(speech, Name);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Handled || !from.Alive || from.GetDistanceToSqrt(this) > 3)
                return;

            if (e.HasKeyword(0x3C) || (e.HasKeyword(0x171) && WasNamed(e.Speech))) // vendor buy, *buy*
            {
                if (IsOwner(from))
                {
                    SayTo(from, 503212); // You own this shop, just take what you want.
                }
                else if (House == null || !House.IsBanned(from))
                {
                    from.SendLocalizedMessage(503213); // Select the item you wish to buy.
                    from.Target = new PVBuyTarget();

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x3D) || (e.HasKeyword(0x172) && WasNamed(e.Speech))) // vendor browse, *browse
            {
                if (House != null && House.IsBanned(from) && !IsOwner(from))
                {
                    SayTo(from, 1062674); // You can't shop from this home as you have been banned from this establishment.
                }
                else
                {
                    if (WasNamed(e.Speech))
                        OpenBackpack(from);
                    else
                    {
                        IPooledEnumerable mobiles = e.Mobile.GetMobilesInRange(2);

                        foreach (Mobile m in mobiles)
                            if (m is PlayerVendor && m.CanSee(e.Mobile) && m.InLOS(e.Mobile))
                                ((PlayerVendor)m).OpenBackpack(from);

                        mobiles.Free();
                    }

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x3E) || (e.HasKeyword(0x173) && WasNamed(e.Speech))) // vendor collect, *collect
            {
                if (IsOwner(from))
                {
                    CollectGold(from);

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x3F) || (e.HasKeyword(0x174) && WasNamed(e.Speech))) // vendor status, *status
            {
                if (IsOwner(from))
                {
                    SendOwnerGump(from);

                    e.Handled = true;
                }
                else
                {
                    SayTo(from, 503226); // What do you care? You don't run this shop.	
                }
            }
            else if (e.HasKeyword(0x40) || (e.HasKeyword(0x175) && WasNamed(e.Speech))) // vendor dismiss, *dismiss
            {
                if (IsOwner(from))
                {
                    Dismiss(from);

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x41) || (e.HasKeyword(0x176) && WasNamed(e.Speech))) // vendor cycle, *cycle
            {
                if (IsOwner(from))
                {
                    Direction = GetDirectionTo(from);

                    e.Handled = true;
                }
            }
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        protected List<Item> GetItems()
        {
            List<Item> list = new List<Item>();

            foreach (Item item in Items)
                if (item.Movable && item != Backpack && item.Layer != Layer.Hair && item.Layer != Layer.FacialHair)
                    list.Add(item);

            if (Backpack != null)
                list.AddRange(Backpack.Items);

            return list;
        }

        private void FixDresswear()
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                Item item = Items[i] as Item;

                if (item is BaseHat)
                    item.Layer = Layer.Helm;
                else if (item is BaseMiddleTorso)
                    item.Layer = Layer.MiddleTorso;
                else if (item is BaseOuterLegs)
                    item.Layer = Layer.OuterLegs;
                else if (item is BaseOuterTorso)
                    item.Layer = Layer.OuterTorso;
                else if (item is BasePants)
                    item.Layer = Layer.Pants;
                else if (item is BaseShirt)
                    item.Layer = Layer.Shirt;
                else if (item is BaseWaist)
                    item.Layer = Layer.Waist;
                else if (item is BaseShoes)
                {
                    if (item is Sandals)
                        item.Hue = 0;

                    item.Layer = Layer.Shoes;
                }
            }
        }

        private VendorItem SetVendorItem(Item item, int price, string description)
        {
            return SetVendorItem(item, price, description, DateTime.UtcNow);
        }

        private VendorItem SetVendorItem(Item item, int price, string description, DateTime created)
        {
            RemoveVendorItem(item);

            VendorItem vi = new VendorItem(item, price, description, created);
            m_SellItems[item] = vi;

            item.InvalidateProperties();

            return vi;
        }

        private void RemoveVendorItem(Item item)
        {
            VendorItem vi = GetVendorItem(item);

            if (vi != null)
            {
                vi.Invalidate();
                m_SellItems.Remove(item);

                foreach (Item subItem in item.Items)
                {
                    RemoveVendorItem(subItem);
                }

                item.InvalidateProperties();
            }
        }

        private bool CanBeVendorItem(Item item)
        {
            Item parent = item.Parent as Item;

            if (parent == Backpack)
                return true;

            if (parent is Container)
            {
                VendorItem parentVI = GetVendorItem(parent);

                if (parentVI != null)
                    return !parentVI.IsForSale;
            }

            return false;
        }

        private void NonLocalDropCallback(object state)
        {
            object[] aState = (object[])state;

            Mobile from = (Mobile)aState[0];
            Item item = (Item)aState[1];

            OnItemGiven(from, item);
        }

        private void OnItemGiven(Mobile from, Item item)
        {
            VendorItem vi = GetVendorItem(item);

            if (vi != null)
            {
                string name;
                if (!string.IsNullOrEmpty(item.Name))
                    name = item.Name;
                else
                    name = "#" + item.LabelNumber.ToString();

                from.SendLocalizedMessage(1043303, name); // Type in a price and description for ~1_ITEM~ (ESC=not for sale)
                from.Prompt = new VendorPricePrompt(this, vi);
            }
        }

        private class ReturnVendorEntry : ContextMenuEntry
        {
            private readonly PlayerVendor m_Vendor;

            public ReturnVendorEntry(PlayerVendor vendor)
                : base(6214)
            {
                m_Vendor = vendor;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (!m_Vendor.Deleted && m_Vendor.IsOwner(from) && from.CheckAlive())
                    m_Vendor.Return();
            }
        }

        [PlayerVendorTarget]
        private class PVBuyTarget : Target
        {
            public PVBuyTarget()
                : base(3, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Item)
                {
                    TryToBuy((Item)targeted, from);
                }
            }
        }

        private class VendorPricePrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;
            private readonly VendorItem m_VI;

            public VendorPricePrompt(PlayerVendor vendor, VendorItem vi)
            {
                m_Vendor = vendor;
                m_VI = vi;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_VI.Valid || !m_Vendor.CanInteractWith(from, true))
                    return;

                string firstWord;

                int sep = text.IndexOfAny(new char[] { ' ', ',' });
                if (sep >= 0)
                    firstWord = text.Substring(0, sep);
                else
                    firstWord = text;

                int price;
                string description;

                if (int.TryParse(firstWord, out price))
                {
                    if (sep >= 0)
                        description = text.Substring(sep + 1).Trim();
                    else
                        description = "";
                }
                else
                {
                    price = -1;
                    description = text.Trim();
                }

                SetInfo(from, price, Utility.FixHtml(description));
            }

            public override void OnCancel(Mobile from)
            {
                if (!m_VI.Valid || !m_Vendor.CanInteractWith(from, true))
                    return;

                SetInfo(from, -1, "");
            }

            private void SetInfo(Mobile from, int price, string description)
            {
                Item item = m_VI.Item;

                bool setPrice = false;

                if (price < 0) // Not for sale
                {
                    price = -1;

                    if (item is Container)
                    {
                        if (item is LockableContainer && ((LockableContainer)item).Locked)
                            m_Vendor.SayTo(from, 1043298); // Locked items may not be made not-for-sale.
                        else if (item.Items.Count > 0)
                            m_Vendor.SayTo(from, 1043299); // To be not for sale, all items in a container must be for sale.
                        else
                            setPrice = true;
                    }
                    else if (item is BaseBook || item is Engines.BulkOrders.BulkOrderBook || item is RecipeBook)
                    {
                        setPrice = true;
                    }
                    else
                    {
                        m_Vendor.SayTo(from, 1043301); // Only the following may be made not-for-sale: books, containers, keyrings, and items in for-sale containers.
                    }
                }
                else
                {
                    if (price > 175000000)
                    {
                        price = 175000000;
                        from.SendLocalizedMessage(1071986); // You cannot price items above 175,000,000 gold. The price has been set to the maximum. Note that if you price an item above 125,000,000 gold, players without expanded bank storage may not be able to purchase the item.
                    }

                    setPrice = true;
                }

                if (setPrice)
                {
                    m_Vendor.SetVendorItem(item, price, description);
                }
                else
                {
                    m_VI.Description = description;
                }
            }
        }

        private class CollectGoldPrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;

            public CollectGoldPrompt(PlayerVendor vendor)
            {
                m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_Vendor.CanInteractWith(from, true))
                    return;

                text = text.Trim();

                int amount;

                if (!int.TryParse(text, out amount))
                    amount = 0;

                GiveGold(from, amount);
            }

            public override void OnCancel(Mobile from)
            {
                if (!m_Vendor.CanInteractWith(from, true))
                    return;

                GiveGold(from, 0);
            }

            private void GiveGold(Mobile to, int amount)
            {
                if (amount <= 0)
                {
                    m_Vendor.SayTo(to, 1071951); // Very well. I will hold on to the money for now then.
                }
                else
                {
                    m_Vendor.GiveGold(to, amount);
                }
            }
        }

        private class VendorNamePrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;

            public VendorNamePrompt(PlayerVendor vendor)
            {
                m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_Vendor.CanInteractWith(from, true))
                    return;

                string name = text.Trim();

                if (!NameVerification.Validate(name, 1, 20, true, true, true, 0, NameVerification.Empty))
                {
                    m_Vendor.SayTo(from, 501173); // That name is disallowed.
                    return;
                }

                m_Vendor.Name = Utility.FixHtml(name);

                from.SendLocalizedMessage(1062496); // Your vendor has been renamed.

                from.SendGump(new NewPlayerVendorOwnerGump(m_Vendor));
            }
        }

        private class ShopNamePrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;
            public ShopNamePrompt(PlayerVendor vendor)
            {
                m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!m_Vendor.CanInteractWith(from, true))
                    return;

                string name = text.Trim();

                if (!NameVerification.Validate(name, 1, 20, true, true, true, 0, NameVerification.Empty))
                {
                    m_Vendor.SayTo(from, 501173); // That name is disallowed.
                    return;
                }

                m_Vendor.ShopName = Utility.FixHtml(name);

                from.SendGump(new NewPlayerVendorOwnerGump(m_Vendor));
            }
        }

        public static List<PlayerVendor> PlayerVendors { get; set; } = new List<PlayerVendor>();
    }

    public class PlayerVendorPlaceholder : Item
    {
        private readonly ExpireTimer m_Timer;

        public PlayerVendorPlaceholder(PlayerVendor vendor)
            : base(0x1F28)
        {
            Hue = 0x672;
            Movable = false;

            Vendor = vendor;

            m_Timer = new ExpireTimer(this);
            m_Timer.Start();
        }

        public PlayerVendorPlaceholder(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerVendor Vendor { get; private set; }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Vendor != null)
                list.Add(1062498, Vendor.Name); // reserved for vendor ~1_NAME~
        }

        public void RestartTimer()
        {
            m_Timer.Stop();
            m_Timer.Start();
        }

        public override void OnDelete()
        {
            if (Vendor != null && !Vendor.Deleted)
            {
                Vendor.MoveToWorld(Location, Map);
                Vendor.Placeholder = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);

            writer.Write(Vendor);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            Vendor = (PlayerVendor)reader.ReadMobile();

            Timer.DelayCall(TimeSpan.Zero, Delete);
        }

        private class ExpireTimer : Timer
        {
            private readonly PlayerVendorPlaceholder m_Placeholder;
            public ExpireTimer(PlayerVendorPlaceholder placeholder)
                : base(TimeSpan.FromMinutes(2.0))
            {
                m_Placeholder = placeholder;

                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_Placeholder.Delete();
            }
        }
    }

    public class PayTimer : Timer
    {
        public static void Initialize()
        {
            var timer = new PayTimer();
            timer.Start();
        }  

        public PayTimer()
            : base(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1))
        {
        }

        public static TimeSpan GetInterval()
        {
            return TimeSpan.FromDays(1.0);
        }

        protected override void OnTick()
        {
            var list = PlayerVendor.PlayerVendors.Where(v => !v.Deleted && !v.IsCommission && v.NextPayTime <= DateTime.UtcNow).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                var vendor = list[i];
                vendor.NextPayTime = DateTime.UtcNow + GetInterval();

                var pay = vendor.ChargePerRealWorldDay;
                var totalGold = vendor.HoldGold;

                if (pay > totalGold)
                {
                    vendor.Destroy(false);
                }
                else
                {
                    vendor.HoldGold -= pay;

                    var trinket = vendor.GetMerchantsTrinket();

                    if (trinket != null)
                    {
                        trinket.UsesRemaining--;
                    }
                }
            }

            ColUtility.Free(list);

            var rentals = PlayerVendor.PlayerVendors.OfType<RentedVendor>().Where(rv => !rv.Deleted && rv.RentalExpireTime <= DateTime.UtcNow).ToList();

            for (int i = 0; i < rentals.Count; i++)
            {
                var vendor = rentals[i];
                int renewalPrice = vendor.RenewalPrice;

                if (vendor.Renew && vendor.HoldGold >= renewalPrice)
                {
                    vendor.HoldGold -= renewalPrice;
                    vendor.RentalGold += renewalPrice;

                    vendor.RentalPrice = renewalPrice;

                    vendor.RentalExpireTime = DateTime.UtcNow + vendor.RentalDuration.Duration;
                }
                else
                {
                    vendor.Destroy(false);
                }
            }

            ColUtility.Free(rentals);
        }
    }
}
