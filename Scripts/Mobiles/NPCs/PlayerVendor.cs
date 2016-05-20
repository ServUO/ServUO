using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Prompts;
using Server.Targeting;

namespace Server.Mobiles
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlayerVendorTargetAttribute : Attribute
    {
        public PlayerVendorTargetAttribute()
        {
        }
    }

    public class VendorItem
    {
        private readonly Item m_Item;
        private readonly int m_Price;
        private readonly DateTime m_Created;
        private string m_Description;
        private bool m_Valid;
        public VendorItem(Item item, int price, string description, DateTime created)
        {
            this.m_Item = item;
            this.m_Price = price;

            if (description != null)
                this.m_Description = description;
            else
                this.m_Description = "";

            this.m_Created = created;

            this.m_Valid = true;
        }

        public Item Item
        {
            get
            {
                return this.m_Item;
            }
        }
        public int Price
        {
            get
            {
                return this.m_Price;
            }
        }
        public string FormattedPrice
        {
            get
            {
                if (Core.ML)
                    return this.m_Price.ToString("N0", CultureInfo.GetCultureInfo("en-US"));

                return this.m_Price.ToString();
            }
        }
        public string Description
        {
            get
            {
                return this.m_Description;
            }
            set
            {
                if (value != null)
                    this.m_Description = value;
                else
                    this.m_Description = "";

                if (this.Valid)
                    this.Item.InvalidateProperties();
            }
        }
        public DateTime Created
        {
            get
            {
                return this.m_Created;
            }
        }
        public bool IsForSale
        {
            get
            {
                return this.Price >= 0;
            }
        }
        public bool IsForFree
        {
            get
            {
                return this.Price == 0;
            }
        }
        public bool Valid
        {
            get
            {
                return this.m_Valid;
            }
        }
        public void Invalidate()
        {
            this.m_Valid = false;
        }
    }

    public class VendorBackpack : Backpack
    {
        public VendorBackpack()
        {
            this.Layer = Layer.Backpack;
            this.Weight = 1.0;
        }

        public VendorBackpack(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultMaxWeight
        {
            get
            {
                return 0;
            }
        }
        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (!base.CheckHold(m, item, message, checkItems, plusItems, plusWeight))
                return false;

            if (Ethics.Ethic.IsImbued(item, true))
            {
                if (message)
                    m.SendMessage("Imbued items may not be sold here.");

                return false;
            }

            if (!BaseHouse.NewVendorSystem && this.Parent is PlayerVendor)
            {
                BaseHouse house = ((PlayerVendor)this.Parent).House;

                if (house != null && house.IsAosRules && !house.CheckAosStorage(1 + item.TotalItems + plusItems))
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

            if (item is Container || item is Engines.BulkOrders.BulkOrderBook)
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

            PlayerVendor pv = this.RootParent as PlayerVendor;

            if (pv == null || pv.IsOwner(from))
                return;

            VendorItem vi = pv.GetVendorItem(item);

            if (vi != null)
                list.Add(new BuyEntry(item));
        }

        public override void GetChildNameProperties(ObjectPropertyList list, Item item)
        {
            base.GetChildNameProperties(list, item);

            PlayerVendor pv = this.RootParent as PlayerVendor;

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

            PlayerVendor pv = this.RootParent as PlayerVendor;

            if (pv == null)
                return;

            VendorItem vi = pv.GetVendorItem(item);

            if (vi != null && vi.Description != null && vi.Description.Length > 0)
                list.Add(1043305, vi.Description); // <br>Seller's Description:<br>"~1_DESC~"
        }

        public override void OnSingleClickContained(Mobile from, Item item)
        {
            if (this.RootParent is PlayerVendor)
            {
                PlayerVendor vendor = (PlayerVendor)this.RootParent;

                VendorItem vi = vendor.GetVendorItem(item);

                if (vi != null)
                {
                    if (!vi.IsForSale)
                        item.LabelTo(from, 1043307); // Price: Not for sale.
                    else if (vi.IsForFree)
                        item.LabelTo(from, 1043306); // Price: FREE!
                    else
                        item.LabelTo(from, 1043304, vi.FormattedPrice); // Price: ~1_COST~

                    if (!String.IsNullOrEmpty(vi.Description))
                    {
                        // The localized message (1043305) is no longer valid - <br>Seller's Description:<br>"~1_DESC~"
                        item.LabelTo(from, "Description: {0}", vi.Description);
                    }
                }
            }

            base.OnSingleClickContained(from, item);
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

        private class BuyEntry : ContextMenuEntry
        {
            private readonly Item m_Item;
            public BuyEntry(Item item)
                : base(6103)
            {
                this.m_Item = item;
            }

            public override bool NonLocalUse
            {
                get
                {
                    return true;
                }
            }
            public override void OnClick()
            {
                if (this.m_Item.Deleted)
                    return;

                PlayerVendor.TryToBuy(this.m_Item, this.Owner.From);
            }
        }
    }

    public class PlayerVendor : Mobile
    {
        private Hashtable m_SellItems;
        private Mobile m_Owner;
        private BaseHouse m_House;
        private int m_BankAccount;
        private int m_HoldGold;
        private string m_ShopName;
        private Timer m_PayTimer;
        private DateTime m_NextPayTime;
        private PlayerVendorPlaceholder m_Placeholder;
        public PlayerVendor(Mobile owner, BaseHouse house)
        {
            this.Owner = owner;
            this.House = house;

            if (BaseHouse.NewVendorSystem)
            {
                this.m_BankAccount = 0;
                this.m_HoldGold = 4;
            }
            else
            {
                this.m_BankAccount = 1000;
                this.m_HoldGold = 0;
            }

            this.ShopName = "Shop Not Yet Named";

            this.m_SellItems = new Hashtable();

            this.CantWalk = true;

            if (!Core.AOS)
                this.NameHue = 0x35;

            this.InitStats(100, 100, 100);
            this.InitBody();
            this.InitOutfit();

            TimeSpan delay = PayTimer.GetInterval();

            this.m_PayTimer = new PayTimer(this, delay);
            this.m_PayTimer.Start();

            this.m_NextPayTime = DateTime.UtcNow + delay;
        }

        public PlayerVendor(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int BankAccount
        {
            get
            {
                return this.m_BankAccount;
            }
            set
            {
                this.m_BankAccount = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int HoldGold
        {
            get
            {
                return this.m_HoldGold;
            }
            set
            {
                this.m_HoldGold = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string ShopName
        {
            get
            {
                return this.m_ShopName;
            }
            set
            {
                if (value == null)
                    this.m_ShopName = "";
                else
                    this.m_ShopName = value;

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextPayTime
        {
            get
            {
                return this.m_NextPayTime;
            }
        }
        public PlayerVendorPlaceholder Placeholder
        {
            get
            {
                return this.m_Placeholder;
            }
            set
            {
                this.m_Placeholder = value;
            }
        }
        public BaseHouse House
        {
            get
            {
                return this.m_House;
            }
            set
            {
                if (this.m_House != null)
                    this.m_House.PlayerVendors.Remove(this);

                if (value != null)
                    value.PlayerVendors.Add(this);

                this.m_House = value;
            }
        }
        public int ChargePerDay
        {
            get
            { 
                if (BaseHouse.NewVendorSystem)
                {
                    return this.ChargePerRealWorldDay / 12;
                }
                else
                {
                    long total = 0;
                    foreach (VendorItem vi in this.m_SellItems.Values)
                    {
                        total += vi.Price;
                    }

                    total -= 500;

                    if (total < 0)
                        total = 0;

                    return (int)(20 + (total / 500));
                }
            }
        }
        public int ChargePerRealWorldDay
        {
            get
            {
                if (BaseHouse.NewVendorSystem)
                {
                    long total = 0;
                    foreach (VendorItem vi in this.m_SellItems.Values)
                    {
                        total += vi.Price;
                    }

                    return (int)(60 + (total / 500) * 3);
                }
                else
                {
                    return this.ChargePerDay * 12;
                }
            }
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
                from.SendMessage("You cannot buy this item right now.  Please wait one minute and try again.");
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

            writer.Write((int)1); // version

            writer.Write((bool)BaseHouse.NewVendorSystem);
            writer.Write((string)this.m_ShopName);
            writer.WriteDeltaTime((DateTime)this.m_NextPayTime);
            writer.Write((Item)this.House);

            writer.Write((Mobile)this.m_Owner);
            writer.Write((int)this.m_BankAccount);
            writer.Write((int)this.m_HoldGold);

            writer.Write((int)this.m_SellItems.Count);
            foreach (VendorItem vi in this.m_SellItems.Values)
            {
                writer.Write((Item)vi.Item);
                writer.Write((int)vi.Price);
                writer.Write((string)vi.Description);

                writer.Write((DateTime)vi.Created);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            bool newVendorSystem = false;

            switch ( version )
            {
                case 1:
                    {
                        newVendorSystem = reader.ReadBool();
                        this.m_ShopName = reader.ReadString();
                        this.m_NextPayTime = reader.ReadDeltaTime();
                        this.House = (BaseHouse)reader.ReadItem();

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Owner = reader.ReadMobile();
                        this.m_BankAccount = reader.ReadInt();
                        this.m_HoldGold = reader.ReadInt();

                        this.m_SellItems = new Hashtable();

                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            Item item = reader.ReadItem();

                            int price = reader.ReadInt();
                            if (price > 100000000)
                                price = 100000000;

                            string description = reader.ReadString();

                            DateTime created = version < 1 ? DateTime.UtcNow : reader.ReadDateTime();

                            if (item != null)
                            {
                                this.SetVendorItem(item, version < 1 && price <= 0 ? -1 : price, description, created);
                            }
                        }

                        break;	
                    }
            }

            bool newVendorSystemActivated = BaseHouse.NewVendorSystem && !newVendorSystem;

            if (version < 1 || newVendorSystemActivated)
            {
                if (version < 1)
                {
                    this.m_ShopName = "Shop Not Yet Named";
                    Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(UpgradeFromVersion0), newVendorSystemActivated);
                }
                else
                {
                    Timer.DelayCall(TimeSpan.Zero, new TimerCallback(FixDresswear));
                }

                this.m_NextPayTime = DateTime.UtcNow + PayTimer.GetInterval();

                if (newVendorSystemActivated)
                {
                    this.m_HoldGold += this.m_BankAccount;
                    this.m_BankAccount = 0;
                }
            }

            TimeSpan delay = this.m_NextPayTime - DateTime.UtcNow;

            this.m_PayTimer = new PayTimer(this, delay > TimeSpan.Zero ? delay : TimeSpan.Zero);
            this.m_PayTimer.Start();

            this.Blessed = false;

            if (Core.AOS && this.NameHue == 0x35)
                this.NameHue = -1;
        }

        public void InitBody()
        {
            this.Hue = Utility.RandomSkinHue();
            this.SpeechHue = 0x3B2;

            if (!Core.AOS)
                this.NameHue = 0x35;

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
            }
        }

        public virtual void InitOutfit()
        {
            Item item = new FancyShirt(Utility.RandomNeutralHue());
            item.Layer = Layer.InnerTorso;
            this.AddItem(item);
            this.AddItem(new LongPants(Utility.RandomNeutralHue()));
            this.AddItem(new BodySash(Utility.RandomNeutralHue()));
            this.AddItem(new Boots(Utility.RandomNeutralHue()));
            this.AddItem(new Cloak(Utility.RandomNeutralHue()));

            Utility.AssignRandomHair(this);

            Container pack = new VendorBackpack();
            pack.Movable = false;
            this.AddItem(pack);
        }

        public virtual bool IsOwner(Mobile m)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (BaseHouse.NewVendorSystem && this.House != null)
            {
                return this.House.IsOwner(m);
            }
            else
            {
                return m == this.Owner;
            }
        }

        public virtual void Destroy(bool toBackpack)
        {
            this.Return();

            if (!BaseHouse.NewVendorSystem)
                this.FixDresswear();

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

            List<Item> list = this.GetItems();

            if (list.Count > 0 || this.HoldGold > 0) // No case 1
            {
                if ((!toBackpack || this.Map == Map.Internal) && this.House != null && this.House.IsAosRules) // Case 2
                {
                    if (this.House.IsOwner(this.Owner)) // Move to moving crate
                    {
                        if (this.House.MovingCrate == null)
                            this.House.MovingCrate = new MovingCrate(this.House);

                        if (this.HoldGold > 0)
                            Banker.Deposit(this.House.MovingCrate, this.HoldGold);

                        foreach (Item item in list)
                        {
                            this.House.MovingCrate.DropItem(item);
                        }
                    }
                    else // Move to vendor inventory
                    {
                        VendorInventory inventory = new VendorInventory(this.House, this.Owner, this.Name, this.ShopName);
                        inventory.Gold = this.HoldGold;

                        foreach (Item item in list)
                        {
                            inventory.AddItem(item);
                        }

                        this.House.VendorInventories.Add(inventory);
                    }
                }
                else if ((toBackpack || this.House == null || !this.House.IsAosRules) && this.Map != Map.Internal) // Case 3 - Move to backpack
                {
                    Container backpack = new Backpack();

                    if (this.HoldGold > 0)
                        Banker.Deposit(backpack, this.HoldGold);

                    foreach (Item item in list)
                    {
                        backpack.DropItem(item);
                    }

                    backpack.MoveToWorld(this.Location, this.Map);
                }
            }

            this.Delete();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            this.m_PayTimer.Stop();

            this.House = null;

            if (this.Placeholder != null)
                this.Placeholder.Delete();
        }

        public override bool IsSnoop(Mobile from)
        {
            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (BaseHouse.NewVendorSystem)
            {
                list.Add(1062449, this.ShopName); // Shop Name: ~1_NAME~
            }
        }

        public VendorItem GetVendorItem(Item item)
        {
            return (VendorItem)this.m_SellItems[item];
        }

        public override void OnSubItemAdded(Item item)
        {
            base.OnSubItemAdded(item);

            if (this.GetVendorItem(item) == null && this.CanBeVendorItem(item))
            {
                // TODO: default price should be dependent to the type of object
                this.SetVendorItem(item, 999, "");
            }
        }

        public override void OnSubItemRemoved(Item item)
        {
            base.OnSubItemRemoved(item);

            if (item.GetBounce() == null)
                this.RemoveVendorItem(item);
        }

        public override void OnSubItemBounceCleared(Item item)
        {
            base.OnSubItemBounceCleared(item);

            if (!this.CanBeVendorItem(item))
                this.RemoveVendorItem(item);
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (item == this.Backpack)
            {
                foreach (Item subItem in item.Items)
                {
                    this.RemoveVendorItem(subItem);
                }
            }
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (!this.IsOwner(from))
            {
                this.SayTo(from, 503209); // I can only take item from the shop owner.
                return false;
            }

            if (item is Gold)
            {
                if (BaseHouse.NewVendorSystem)
                {
                    if (this.HoldGold < 1000000)
                    {
                        this.SayTo(from, 503210); // I'll take that to fund my services.

                        this.HoldGold += item.Amount;
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
                    if (this.BankAccount < 1000000)
                    {
                        this.SayTo(from, 503210); // I'll take that to fund my services.

                        this.BankAccount += item.Amount;
                        item.Delete();

                        return true;
                    }
                    else
                    {
                        from.SendLocalizedMessage(1062493); // Your vendor has sufficient funds for operation and cannot accept this gold.

                        return false;
                    }
                }
            }
            else
            {
                bool newItem = (this.GetVendorItem(item) == null);

                if (this.Backpack != null && this.Backpack.TryDropItem(from, item, false))
                {
                    if (newItem)
                        this.OnItemGiven(from, item);

                    return true;
                }
                else
                {
                    this.SayTo(from, 503211); // I can't carry any more.
                    return false;
                }
            }
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            if (this.IsOwner(from))
            {
                if (this.GetVendorItem(item) == null)
                {
                    // We must wait until the item is added
                    Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(NonLocalDropCallback), new object[] { from, item });
                }

                return true;
            }
            else
            {
                this.SayTo(from, 503209); // I can only take item from the shop owner.
                return false;
            }
        }

        public override bool AllowEquipFrom(Mobile from)
        {
            if (BaseHouse.NewVendorSystem && this.IsOwner(from))
                return true;

            return base.AllowEquipFrom(from);
        }

        public override bool CheckNonlocalLift(Mobile from, Item item)
        {
            if (item.IsChildOf(this.Backpack))
            {
                if (this.IsOwner(from))
                {
                    return true;
                }
                else
                {
                    this.SayTo(from, 503223); // If you'd like to purchase an item, just ask.
                    return false;
                }
            }
            else if (BaseHouse.NewVendorSystem && this.IsOwner(from))
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
                return this.IsOwner(from);

            if (this.House != null && this.House.IsBanned(from) && !this.IsOwner(from))
            {
                from.SendLocalizedMessage(1062674); // You can't shop from this home as you have been banned from this establishment.
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsOwner(from))
            {
                this.SendOwnerGump(from);
            }
            else if (this.CanInteractWith(from, false))
            {
                this.OpenBackpack(from);
            }
        }

        public override void DisplayPaperdollTo(Mobile m)
        {
            if (BaseHouse.NewVendorSystem)
            {
                base.DisplayPaperdollTo(m);
            }
            else if (this.CanInteractWith(m, false))
            {
                this.OpenBackpack(m);
            }
        }

        public void SendOwnerGump(Mobile to)
        {
            if (BaseHouse.NewVendorSystem)
            {
                to.CloseGump(typeof(NewPlayerVendorOwnerGump));
                to.CloseGump(typeof(NewPlayerVendorCustomizeGump));

                to.SendGump(new NewPlayerVendorOwnerGump(this));
            }
            else
            {
                to.CloseGump(typeof(PlayerVendorOwnerGump));
                to.CloseGump(typeof(PlayerVendorCustomizeGump));

                to.SendGump(new PlayerVendorOwnerGump(this));
            }
        }

        public void OpenBackpack(Mobile from)
        {
            if (this.Backpack != null)
            {
                this.SayTo(from, this.IsOwner(from) ? 1010642 : 503208); // Take a look at my/your goods.

                this.Backpack.DisplayTo(from);
            }
        }

        public void CollectGold(Mobile to)
        {
            if (this.HoldGold > 0)
            {
                this.SayTo(to, "How much of the {0} that I'm holding would you like?", this.HoldGold.ToString());
                to.SendMessage("Enter the amount of gold you wish to withdraw (ESC = CANCEL):");

                to.Prompt = new CollectGoldPrompt(this);
            }
            else
            {
                this.SayTo(to, 503215); // I am holding no gold for you.
            }
        }

        public int GiveGold(Mobile to, int amount)
        {
            if (amount <= 0)
                return 0;

            if (amount > this.HoldGold)
            {
                this.SayTo(to, "I'm sorry, but I'm only holding {0} gold for you.", this.HoldGold.ToString());
                return 0;
            }

            int amountGiven = Banker.DepositUpTo(to, amount);
            this.HoldGold -= amountGiven;

            if (amountGiven > 0)
            {
                to.SendLocalizedMessage(1060397, amountGiven.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.
            }

            if (amountGiven == 0)
            {
                this.SayTo(to, 1070755); // Your bank box cannot hold the gold you are requesting.  I will keep the gold until you can take it.
            }
            else if (amount > amountGiven)
            {
                this.SayTo(to, 1070756); // I can only give you part of the gold now, as your bank box is too full to hold the full amount.
            }
            else if (this.HoldGold > 0)
            {
                this.SayTo(to, 1042639); // Your gold has been transferred.
            }
            else
            {
                this.SayTo(to, 503234); // All the gold I have been carrying for you has been deposited into your bank account.
            }

            return amountGiven;
        }

        public void Dismiss(Mobile from)
        {
            Container pack = this.Backpack;

            if (pack != null && pack.Items.Count > 0)
            {
                this.SayTo(from, 1038325); // You cannot dismiss me while I am holding your goods.
                return;
            }

            if (this.HoldGold > 0)
            {
                this.GiveGold(from, this.HoldGold);

                if (this.HoldGold > 0)
                    return;
            }

            this.Destroy(true);
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
            if (this.Deleted || !this.IsOwner(to) || this.House == null || this.Map == Map.Internal)
                return false;

            if (this.House.IsInside(to) || to.Map != this.House.Map || !this.House.InRange(to, 5))
                return false;

            if (this.Placeholder == null)
            {
                this.Placeholder = new PlayerVendorPlaceholder(this);
                this.Placeholder.MoveToWorld(this.Location, this.Map);

                this.MoveToWorld(to.Location, to.Map);

                to.SendLocalizedMessage(1062431); // This vendor has been moved out of the house to your current location temporarily.  The vendor will return home automatically after two minutes have passed once you are done managing its inventory or customizing it.
            }
            else
            {
                this.Placeholder.RestartTimer();

                to.SendLocalizedMessage(1062430); // This vendor is currently temporarily in a location outside its house.  The vendor will return home automatically after two minutes have passed once you are done managing its inventory or customizing it.
            }

            return true;
        }

        public void Return()
        {
            if (this.Placeholder != null)
                this.Placeholder.Delete();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && this.Placeholder != null && this.IsOwner(from))
            {
                list.Add(new ReturnVendorEntry(this));
            }

            base.GetContextMenuEntries(from, list);
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return (from.Alive && from.GetDistanceToSqrt(this) <= 3);
        }

        public bool WasNamed(string speech)
        {
            return this.Name != null && Insensitive.StartsWith(speech, this.Name);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Handled || !from.Alive || from.GetDistanceToSqrt(this) > 3)
                return;

            if (e.HasKeyword(0x3C) || (e.HasKeyword(0x171) && this.WasNamed(e.Speech))) // vendor buy, *buy*
            {
                if (this.IsOwner(from))
                {
                    this.SayTo(from, 503212); // You own this shop, just take what you want.
                }
                else if (this.House == null || !this.House.IsBanned(from))
                {
                    from.SendLocalizedMessage(503213); // Select the item you wish to buy.
                    from.Target = new PVBuyTarget();

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x3D) || (e.HasKeyword(0x172) && this.WasNamed(e.Speech))) // vendor browse, *browse
            {
                if (this.House != null && this.House.IsBanned(from) && !this.IsOwner(from))
                {
                    this.SayTo(from, 1062674); // You can't shop from this home as you have been banned from this establishment.
                }
                else
                {
                    if (this.WasNamed(e.Speech))
                        this.OpenBackpack(from);
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
            else if (e.HasKeyword(0x3E) || (e.HasKeyword(0x173) && this.WasNamed(e.Speech))) // vendor collect, *collect
            {
                if (this.IsOwner(from))
                {
                    this.CollectGold(from);

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x3F) || (e.HasKeyword(0x174) && this.WasNamed(e.Speech))) // vendor status, *status
            {
                if (this.IsOwner(from))
                {
                    this.SendOwnerGump(from);

                    e.Handled = true;
                }
                else
                {
                    this.SayTo(from, 503226); // What do you care? You don't run this shop.	
                }
            }
            else if (e.HasKeyword(0x40) || (e.HasKeyword(0x175) && this.WasNamed(e.Speech))) // vendor dismiss, *dismiss
            {
                if (this.IsOwner(from))
                {
                    this.Dismiss(from);

                    e.Handled = true;
                }
            }
            else if (e.HasKeyword(0x41) || (e.HasKeyword(0x176) && this.WasNamed(e.Speech))) // vendor cycle, *cycle
            {
                if (this.IsOwner(from))
                {
                    this.Direction = this.GetDirectionTo(from);

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

            foreach (Item item in this.Items)
                if (item.Movable && item != this.Backpack && item.Layer != Layer.Hair && item.Layer != Layer.FacialHair)
                    list.Add(item);

            if (this.Backpack != null)
                list.AddRange(this.Backpack.Items);

            return list;
        }

        private void UpgradeFromVersion0(object newVendorSystem)
        {
            List<Item> toRemove = new List<Item>();

            foreach (VendorItem vi in this.m_SellItems.Values)
                if (!this.CanBeVendorItem(vi.Item))
                    toRemove.Add(vi.Item);
                else
                    vi.Description = Utility.FixHtml(vi.Description);

            foreach (Item item in toRemove)
                this.RemoveVendorItem(item);

            this.House = BaseHouse.FindHouseAt(this);

            if ((bool)newVendorSystem)
                this.ActivateNewVendorSystem();
        }

        private void ActivateNewVendorSystem()
        {
            this.FixDresswear();

            if (this.House != null && !this.House.IsOwner(this.Owner))
                this.Destroy(false);
        }

        private void FixDresswear()
        {
            for (int i = 0; i < this.Items.Count; ++i)
            {
                Item item = this.Items[i] as Item;

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
            return this.SetVendorItem(item, price, description, DateTime.UtcNow);
        }

        private VendorItem SetVendorItem(Item item, int price, string description, DateTime created)
        {
            this.RemoveVendorItem(item);

            VendorItem vi = new VendorItem(item, price, description, created);
            this.m_SellItems[item] = vi;

            item.InvalidateProperties();

            return vi;
        }

        private void RemoveVendorItem(Item item)
        {
            VendorItem vi = this.GetVendorItem(item);

            if (vi != null)
            {
                vi.Invalidate();
                this.m_SellItems.Remove(item);

                foreach (Item subItem in item.Items)
                {
                    this.RemoveVendorItem(subItem);
                }

                item.InvalidateProperties();
            }
        }

        private bool CanBeVendorItem(Item item)
        {
            Item parent = item.Parent as Item;

            if (parent == this.Backpack)
                return true;

            if (parent is Container)
            {
                VendorItem parentVI = this.GetVendorItem(parent);

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

            this.OnItemGiven(from, item);
        }

        private void OnItemGiven(Mobile from, Item item)
        {
            VendorItem vi = this.GetVendorItem(item);

            if (vi != null)
            {
                string name;
                if (!String.IsNullOrEmpty(item.Name))
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
                this.m_Vendor = vendor;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (!this.m_Vendor.Deleted && this.m_Vendor.IsOwner(from) && from.CheckAlive())
                    this.m_Vendor.Return();
            }
        }

        private class PayTimer : Timer
        {
            private readonly PlayerVendor m_Vendor;
            public PayTimer(PlayerVendor vendor, TimeSpan delay)
                : base(delay, GetInterval())
            {
                this.m_Vendor = vendor;

                this.Priority = TimerPriority.OneMinute;
            }

            public static TimeSpan GetInterval()
            {
                if (BaseHouse.NewVendorSystem)
                    return TimeSpan.FromDays(1.0);
                else
                    return TimeSpan.FromMinutes(Clock.MinutesPerUODay);
            }

            protected override void OnTick()
            {
                this.m_Vendor.m_NextPayTime = DateTime.UtcNow + this.Interval;

                int pay;
                int totalGold;
                if (BaseHouse.NewVendorSystem)
                {
                    pay = this.m_Vendor.ChargePerRealWorldDay;
                    totalGold = this.m_Vendor.HoldGold;
                }
                else
                {
                    pay = this.m_Vendor.ChargePerDay;
                    totalGold = this.m_Vendor.BankAccount + this.m_Vendor.HoldGold;
                }

                if (pay > totalGold)
                {
                    this.m_Vendor.Destroy(!BaseHouse.NewVendorSystem);
                }
                else
                {
                    if (!BaseHouse.NewVendorSystem)
                    {
                        if (this.m_Vendor.BankAccount >= pay)
                        {
                            this.m_Vendor.BankAccount -= pay;
                            pay = 0;
                        }
                        else
                        {
                            pay -= this.m_Vendor.BankAccount;
                            this.m_Vendor.BankAccount = 0;
                        }
                    }

                    this.m_Vendor.HoldGold -= pay;
                }
            }
        }

        [PlayerVendorTarget]
        private class PVBuyTarget : Target
        {
            public PVBuyTarget()
                : base(3, false, TargetFlags.None)
            {
                this.AllowNonlocal = true;
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
                this.m_Vendor = vendor;
                this.m_VI = vi;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!this.m_VI.Valid || !this.m_Vendor.CanInteractWith(from, true))
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

                this.SetInfo(from, price, Utility.FixHtml(description));
            }

            public override void OnCancel(Mobile from)
            {
                if (!this.m_VI.Valid || !this.m_Vendor.CanInteractWith(from, true))
                    return;

                this.SetInfo(from, -1, "");
            }

            private void SetInfo(Mobile from, int price, string description)
            {
                Item item = this.m_VI.Item;

                bool setPrice = false;

                if (price < 0) // Not for sale
                {
                    price = -1;

                    if (item is Container)
                    {
                        if (item is LockableContainer && ((LockableContainer)item).Locked)
                            this.m_Vendor.SayTo(from, 1043298); // Locked items may not be made not-for-sale.
                        else if (item.Items.Count > 0)
                            this.m_Vendor.SayTo(from, 1043299); // To be not for sale, all items in a container must be for sale.
                        else
                            setPrice = true;
                    }
                    else if (item is BaseBook || item is Engines.BulkOrders.BulkOrderBook)
                    {
                        setPrice = true;
                    }
                    else
                    {
                        this.m_Vendor.SayTo(from, 1043301); // Only the following may be made not-for-sale: books, containers, keyrings, and items in for-sale containers.
                    }
                }
                else
                {
                    if (price > 100000000)
                    {
                        price = 100000000;
                        from.SendMessage("You cannot price items above 100,000,000 gold.  The price has been adjusted.");
                    }

                    setPrice = true;
                }

                if (setPrice)
                {
                    this.m_Vendor.SetVendorItem(item, price, description);
                }
                else
                {
                    this.m_VI.Description = description;
                }
            }
        }

        private class CollectGoldPrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;
            public CollectGoldPrompt(PlayerVendor vendor)
            {
                this.m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!this.m_Vendor.CanInteractWith(from, true))
                    return;

                text = text.Trim();

                int amount;

                if (!int.TryParse(text, out amount))
                    amount = 0;

                this.GiveGold(from, amount);
            }

            public override void OnCancel(Mobile from)
            {
                if (!this.m_Vendor.CanInteractWith(from, true))
                    return;

                this.GiveGold(from, 0);
            }

            private void GiveGold(Mobile to, int amount)
            {
                if (amount <= 0)
                {
                    this.m_Vendor.SayTo(to, "Very well. I will hold on to the money for now then.");
                }
                else
                {
                    this.m_Vendor.GiveGold(to, amount);
                }
            }
        }

        private class VendorNamePrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;
            public VendorNamePrompt(PlayerVendor vendor)
            {
                this.m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!this.m_Vendor.CanInteractWith(from, true))
                    return;

                string name = text.Trim();

                if (!NameVerification.Validate(name, 1, 20, true, true, true, 0, NameVerification.Empty))
                {
                    this.m_Vendor.SayTo(from, "That name is unacceptable.");
                    return;
                }

                this.m_Vendor.Name = Utility.FixHtml(name);

                from.SendLocalizedMessage(1062496); // Your vendor has been renamed.

                from.SendGump(new NewPlayerVendorOwnerGump(this.m_Vendor));
            }
        }

        private class ShopNamePrompt : Prompt
        {
            private readonly PlayerVendor m_Vendor;
            public ShopNamePrompt(PlayerVendor vendor)
            {
                this.m_Vendor = vendor;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (!this.m_Vendor.CanInteractWith(from, true))
                    return;

                string name = text.Trim();

                if (!NameVerification.Validate(name, 1, 20, true, true, true, 0, NameVerification.Empty))
                {
                    this.m_Vendor.SayTo(from, "That name is unacceptable.");
                    return;
                }

                this.m_Vendor.ShopName = Utility.FixHtml(name);

                from.SendGump(new NewPlayerVendorOwnerGump(this.m_Vendor));
            }
        }
    }

    public class PlayerVendorPlaceholder : Item
    {
        private readonly ExpireTimer m_Timer;
        private PlayerVendor m_Vendor;
        public PlayerVendorPlaceholder(PlayerVendor vendor)
            : base(0x1F28)
        {
            this.Hue = 0x672;
            this.Movable = false;

            this.m_Vendor = vendor;

            this.m_Timer = new ExpireTimer(this);
            this.m_Timer.Start();
        }

        public PlayerVendorPlaceholder(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerVendor Vendor
        {
            get
            {
                return this.m_Vendor;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Vendor != null)
                list.Add(1062498, this.m_Vendor.Name); // reserved for vendor ~1_NAME~
        }

        public void RestartTimer()
        {
            this.m_Timer.Stop();
            this.m_Timer.Start();
        }

        public override void OnDelete()
        {
            if (this.m_Vendor != null && !this.m_Vendor.Deleted)
            {
                this.m_Vendor.MoveToWorld(this.Location, this.Map);
                this.m_Vendor.Placeholder = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0);

            writer.Write((Mobile)this.m_Vendor);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Vendor = (PlayerVendor)reader.ReadMobile();

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
        }

        private class ExpireTimer : Timer
        {
            private readonly PlayerVendorPlaceholder m_Placeholder;
            public ExpireTimer(PlayerVendorPlaceholder placeholder)
                : base(TimeSpan.FromMinutes(2.0))
            {
                this.m_Placeholder = placeholder;

                this.Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                this.m_Placeholder.Delete();
            }
        }
    }
}
