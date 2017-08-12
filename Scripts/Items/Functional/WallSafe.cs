using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;
using System.Globalization;
using Server.Gumps;
using Server.Accounting;
using Server.ContextMenus;

namespace Server.Items
{
    [Flipable(0x8B8F, 0x8B90)]
    public class WallSafe : Item, IAddon, ISecurable, IChopable
    {
        public const int MaxGold = 100000000;
        public const int HistoryMax = 15;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HoldAmount { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get;
            set;
        }

        public Item Deed { get { return new WallSafeDeed(); } }

        public override int LabelNumber { get { return 1119751; } } // Wall Safe
        public override bool ForceShowProperties { get { return true; } }

        public List<string> History { get; set; }

        [Constructable]
        public WallSafe(Mobile m) : base(0x8B8F)
        {
            Owner = m;
            Movable = false;

            Level = SecureLevel.CoOwners;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public void OnChop(Mobile from)
        {
            if (HoldAmount > 0)
                from.SendLocalizedMessage(1155841); // You can't use a bladed item on a wall safe with a balance.
            else
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null && (house.IsOwner(from) || (house.Addons.ContainsKey(this) && house.Addons[this] == from)))
                {
                    Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                    from.SendLocalizedMessage(500461); // You destroy the item.

                    Delete();

                    house.Addons.Remove(this);

                    Item deed = Deed;

                    if (deed != null)
                        from.AddToBackpack(deed);
                }
            }
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            if (!map.CanFit(p.X, p.Y, p.Z, this.ItemData.Height))
                return false;

            if (this.ItemID == 0x2375)
                return BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map); // North wall
            else
                return BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map); // West wall
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(this.Location, 3))
            {
                BaseHouse house = BaseHouse.FindHouseAt(m);

                if (house != null && house.HasSecureAccess(m, Level))
                {
                    m.SendGump(new WallSafeGump((PlayerMobile)m, this));
                }
                else
                    m.SendLocalizedMessage(1061637); // You are not allowed to access this.
            }
        }

        public void AddHistory(string str)
        {
            if (History == null)
                History = new List<string>();

            History.Add(str);

            if (History.Count > HistoryMax)
                History.RemoveAt(0);
        }

        public WallSafe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(Owner);
            writer.Write(HoldAmount);
            writer.Write((int)Level);

            writer.Write(History == null ? 0 : History.Count);

            if (History != null)
                History.ForEach(s => writer.Write(s));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Owner = reader.ReadMobile();
            HoldAmount = reader.ReadInt();
            Level = (SecureLevel)reader.ReadInt();

            int count = reader.ReadInt();

            if (count > 0)
            {
                History = new List<string>();

                for (int i = 0; i < count; i++)
                {
                    string str = reader.ReadString();
                    History.Add(str);
                }
            }
        }
    }

    public class WallSafeDeed : Item
    {
        public override int LabelNumber { get { return 1155857; } } // Currency Wall Safe

        [Constructable]
        public WallSafeDeed() : base(0x14F0)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.BeginTarget(8, false, Server.Targeting.TargetFlags.None, (from, targeted) =>
                    {
                        if (targeted is IPoint3D)
                        {
                            IPoint3D p = targeted as IPoint3D;
                            BaseHouse house = BaseHouse.FindHouseAt(new Point3D(p), m.Map, 16);

                            if (house != null && BaseHouse.FindHouseAt(m) == house && house.IsCoOwner(m))
                            {
                                Point3D pnt = new Point3D(p.X, p.Y, m.Z);

                                bool northWall = BaseAddon.IsWall(pnt.X, pnt.Y - 1, pnt.Z, m.Map);
                                bool westWall = BaseAddon.IsWall(pnt.X - 1, pnt.Y, pnt.Z, m.Map);

                                if (northWall && westWall)
                                {
                                    switch (from.Direction & Direction.Mask)
                                    {
                                        case Direction.North:
                                        case Direction.South: northWall = true; westWall = false; break;

                                        case Direction.East:
                                        case Direction.West: northWall = false; westWall = true; break;

                                        default: from.SendMessage("Turn to face the wall on which to place the safe."); return;
                                    }
                                }

                                int itemID = 0;

                                if (northWall)
                                    itemID = 0x8B8F;
                                else if(westWall)
                                    itemID = 0x8B90;
                                else
                                    m.SendLocalizedMessage(500268); // This object needs to be mounted on something.

                                if (itemID != 0)
                                {
                                    Item safe = new WallSafe(m);
                                    safe.MoveToWorld(pnt, m.Map);

                                    safe.ItemID = itemID;

                                    house.Addons[safe] = m;

                                    Delete();
                                }
                            }
                            else
                                m.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                        }
                    });
            }
            else
                m.SendLocalizedMessage(1080058); // This must be in your backpack to use it.
        }

        public WallSafeDeed(Serial serial) : base(serial)
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

    public class WallSafeGump : Gump
    {
        public WallSafe Safe { get; set; }
        public PlayerMobile User { get; set; }

        public WallSafeGump(PlayerMobile pm, WallSafe safe)
            : base(50, 50)
        {
            User = pm;
            Safe = safe;

            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            AddBackground(0, 0, 400, 500, 83);

            AddHtmlLocalized(0, 10, 400, 16, 1113302, "#1155860", 0xFFFF, false, false);

            int secureAmount = 0;
            Account acct = User.Account as Account;

            if (acct != null)
                secureAmount = acct.GetSecureAccountAmount(User);

            AddHtmlLocalized(20, 35, 380, 16, 1155859, Safe.HoldAmount.ToString("N0", CultureInfo.GetCultureInfo("en-US")), 0xFFFF, false, false); // Gold Deposited: ~1_AMOUNT~
            AddHtmlLocalized(20, 65, 380, 16, 1155864, String.Format("{0}\t{1}", User.Name, secureAmount.ToString("N0", CultureInfo.GetCultureInfo("en-US"))), 0xFFFF, false, false); // ~1_NAME~'s Secure Account: ~2_AMOUNT~

            AddHtmlLocalized(20, 125, 100, 16, 1155861, 0xFFFF, false, false); // Deposit
            AddButton(75, 125, 4005, 4006, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(220, 125, 100, 16, 1155862, 0xFFFF, false, false); // Withdraw
            AddButton(300, 125, 4005, 4006, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(20, 165, 200, 16, 1155863, 0xFFFF, false, false); // Sale Transactions:

            if (Safe.History != null && Safe.History.Count > 0)
            {
                int y = 195;
                for (int i = Safe.History.Count - 1; i >= 0 && i < WallSafe.HistoryMax; i--)
                {
                    AddHtml(20, y, 380, 16, Safe.History[i], false, false);
                    y += 20;
                }
            }
        }

        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        {
            Account account = User.Account as Account;
            int secureAmount = 0;

            if (account != null)
                secureAmount = account.GetSecureAccountAmount(User);

            switch (info.ButtonID)
            {
                case 1:
                    User.SendLocalizedMessage(1155865); // Enter amount to deposit:
                    User.BeginPrompt<Account>(
                    (from, text, acct) =>
                    {
                        int v = 0;

                        if (text != null && !String.IsNullOrEmpty(text))
                        {
                            v = Utility.ToInt32(text);

                            if (v <= 0 || v > secureAmount)
                                from.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                            else if (acct != null)
                            {
                                int left = WallSafe.MaxGold - Safe.HoldAmount;

                                if (v > left)
                                {
                                    Safe.HoldAmount = WallSafe.MaxGold;
                                    acct.WithdrawFromSecure(User, left);
                                    Safe.AddHistory(String.Format("<basefont color=green>{0} +{1}", User.Name, left.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
                                }
                                else
                                {
                                    Safe.HoldAmount += v;
                                    acct.WithdrawFromSecure(User, v);
                                    Safe.AddHistory(String.Format("<basefont color=green>{0} +{1}", User.Name, v.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
                                }

                                from.SendGump(new WallSafeGump(User, Safe));
                            }
                        }
                        else
                            from.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                    },
                    (from, text, acct) =>
                    {
                        from.SendGump(new WallSafeGump(User, Safe));
                    }, account);
                    break;
                case 2:
                    User.SendLocalizedMessage(1155866); // Enter amount to withdraw:
                    User.BeginPrompt<Account>(
                    (from, text, acct) =>
                    {
                        int v = 0;

                        if (text != null && !String.IsNullOrEmpty(text))
                        {
                            v = Utility.ToInt32(text);

                            if (v <= 0 || v > Safe.HoldAmount)
                                from.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                            else if (acct != null)
                            {
                                int left = Account.MaxSecureAmount - secureAmount;

                                if (v > left)
                                {
                                    acct.DepositToSecure(User, left);
                                    Safe.HoldAmount -= left;
                                    Safe.AddHistory(String.Format("<basefont color=red>{0} -{1}", User.Name, left.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
                                }
                                else
                                {
                                    acct.DepositToSecure(User, v);
                                    Safe.HoldAmount -= v;
                                    Safe.AddHistory(String.Format("<basefont color=red>{0} -{1}", User.Name, v.ToString("N0", CultureInfo.GetCultureInfo("en-US"))));
                                }

                                from.SendGump(new WallSafeGump(User, Safe));
                            }
                        }
                        else
                            from.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                    },
                    (from, text, acct) =>
                    {
                        from.SendGump(new WallSafeGump(User, Safe));
                    }, account);
                    break;
            }
        }
    }
}