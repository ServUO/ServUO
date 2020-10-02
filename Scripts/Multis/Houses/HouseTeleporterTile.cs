using Server.ContextMenus;
using Server.Engines.CityLoyalty;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Multis
{
    public class HouseTeleporterTile : HouseTeleporter, IFlipable
    {
        public static void Initialize()
        {
            TileData.ItemTable[0x574A].Flags = TileFlag.None;
        }

        public static int MaxCharges = 1000;

        private int _Charges;

        public override int ItemID
        {
            get { return base.ItemID; }
            set
            {
                base.ItemID = value;

                HueChange();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public HouseTeleporterTile Link
        {
            get
            {
                if (Target != null && Target.Deleted)
                    Target = null;

                return Target as HouseTeleporterTile;
            }
            set
            {
                Target = value;
            }
        }

        private bool IsMoveOver => ItemID == 0x574A || ItemID == 0xA1CB || ItemID == 0xA1CC || ItemID == 0x40BB;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return _Charges; }
            set
            {
                _Charges = value;

                HueChange();

                InvalidateProperties();
            }
        }

        public void HueChange()
        {
            if (UsesCharges && ItemID == 0x574A)
            {
                if (_Charges == 0)
                {
                    Hue = 340;
                }
                else if (_Charges >= 1 && Hue != 541)
                {
                    Hue = 541;
                }
            }
            else
            {
                Hue = 0;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UsesCharges { get; set; }

        public override int LabelNumber => Link == null ? 1114916 : 1113917; // house teleporter (unlinked) -or- House Teleporter

        public HouseTeleporterTile(bool vetReward)
            : base(vetReward ? 0x40BB : 0x574A, null)
        {
            UsesCharges = !vetReward;
            Movable = true;
            Weight = 1.0;
            LootType = LootType.Blessed;

            if (vetReward)
            {
                UsesCharges = false;
            }
            else
            {
                UsesCharges = true;
                Charges = MaxCharges;
            }
        }

        public void OnFlip(Mobile from)
        {
            bool flip = false;

            switch (ItemID)
            {
                case 0x108C:
                    ItemID = 0x1093;
                    flip = true;
                    break;
                case 0x1093:
                    ItemID = 0x108C;
                    flip = true;
                    break;
                case 0x108D:
                    ItemID = 0x1094;
                    flip = true;
                    break;
                case 0x1094:
                    ItemID = 0x108D;
                    flip = true;
                    break;
                case 0x108E:
                    ItemID = 0x1095;
                    flip = true;
                    break;
                case 0x1095:
                    ItemID = 0x108E;
                    flip = true;
                    break;
                case 0x1090:
                case 0x108F:
                    ItemID = 0x1091;
                    flip = true;
                    break;
                case 0x1091:
                    ItemID = 0x1090;
                    flip = true;
                    break;
                case 0xA1CB:
                    ItemID = 0xA1CC;
                    flip = true;
                    break;
                case 0xA1CC:
                    ItemID = 0xA1CB;
                    flip = true;
                    break;
                case 0xA2BA:
                    ItemID = 0xA2BC;
                    flip = true;
                    break;
                case 0xA2BC:
                    ItemID = 0xA2BA;
                    flip = true;
                    break;
                case 0xA2BB:
                    ItemID = 0xA2BD;
                    flip = true;
                    break;
                case 0xA2BD:
                    ItemID = 0xA2BB;
                    flip = true;
                    break;
            }

            if (!flip)
                from.SendLocalizedMessage(1042273); // You cannot turn that.
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (IsMoveOver)
            {
                base.OnMoveOver(m);
            }

            return true;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.InRange(Location, 2) || IsChildOf(from.Backpack))
            {
                if (UsesCharges)
                {
                    list.Add(new RechargeEntry(from, this));
                    list.Add(new ChangeTypeEntry(from, this));
                }
            }

            base.GetContextMenuEntries(from, list);
        }

        private class RechargeEntry : ContextMenuEntry
        {
            private readonly Mobile Mobile;
            private readonly HouseTeleporterTile Item;

            public RechargeEntry(Mobile mobile, HouseTeleporterTile item)
                : base(1076197, 2)
            {
                Mobile = mobile;
                Item = item;

                BaseHouse house = BaseHouse.FindHouseAt(item);

                Enabled = Item.IsLockedDown && house != null && house.IsOwner(Mobile);
            }

            public override void OnClick()
            {
                if (Item == null || Item.Deleted)
                    return;

                Mobile.SendLocalizedMessage(1158897); // Target the gate scrolls you wish to recharge this item with...
                Mobile.Target = new InternalTarget(Item);
            }
        }

        private class InternalTarget : Target
        {
            private readonly HouseTeleporterTile Item;

            public InternalTarget(HouseTeleporterTile item)
                : base(2, false, TargetFlags.None)
            {
                Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || Item == null || Item.Deleted)
                {
                    return;
                }

                if (targeted is Item)
                {
                    Item item = targeted as Item;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                        return;
                    }

                    if (item is GateTravelScroll)
                    {
                        GateTravelScroll scroll = item as GateTravelScroll;

                        if (Item.Charges >= MaxCharges)
                        {
                            from.SendLocalizedMessage(1115126); // The House Teleporter cannot be charged any further.
                        }
                        else
                        {
                            int left = MaxCharges - Item.Charges;
                            int scrollsNeeded = Math.Max(1, left / 5);

                            if (scroll.Amount <= scrollsNeeded)
                            {
                                Item.Charges = Math.Min(MaxCharges, Item.Charges + (scroll.Amount * 5));
                                scroll.Delete();
                            }
                            else
                            {
                                scroll.Amount -= scrollsNeeded;
                                Item.Charges = MaxCharges;
                            }

                            from.SendLocalizedMessage(1115127); // The Gate Travel scroll crumbles to dust as it strengthens the House Teleporter.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1158898); // Target gate scroll to recharge the teleporter.
                    }
                }
            }
        }

        private class ChangeTypeEntry : ContextMenuEntry
        {
            private readonly Mobile Mobile;
            private readonly Item Item;

            public ChangeTypeEntry(Mobile mobile, Item item)
                : base(1158896, 2)
            {
                Mobile = mobile;
                Item = item;

                BaseHouse house = BaseHouse.FindHouseAt(item);

                Enabled = Item.IsLockedDown && house != null && house.IsOwner(Mobile);
            }

            public override void OnClick()
            {
                if (Item == null || Item.Deleted)
                    return;

                if (!Mobile.HasGump(typeof(HouseTeleporterTypeGump)))
                {
                    BaseGump.SendGump(new HouseTeleporterTypeGump((PlayerMobile)Mobile, Item));
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (UsesCharges)
            {
                list.Add(1060741, _Charges.ToString()); // charges: ~1_val~
            }
        }

        public bool CheckBaseAccess(Mobile m)
        {
            return base.CheckAccess(m);
        }

        public override bool CheckAccess(Mobile m)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);
            BaseHouse linkHouse = Link == null ? null : BaseHouse.FindHouseAt(Link);

            if (house == null || Link == null || !IsLockedDown || !Link.IsLockedDown || linkHouse == null) // TODO: Messages for these?
            {
                return false;
            }

            if (UsesCharges && _Charges == 0)
            {
                m.SendLocalizedMessage(1115121); // There are no charges left in this teleporter.
                return false;
            }

            if (UsesCharges && Link.Charges == 0)
            {
                m.SendLocalizedMessage(1115120); // There are no more charges left in the remote teleporter.
                return false;
            }

            if (CheckBaseAccess(m) && Link.CheckBaseAccess(m))
            {
                return CheckTravel(m, Link.Location, Link.Map);
            }

            return false;
        }

        public bool CheckTravel(Mobile from, Point3D dest, Map destMap)
        {
            if (from.Criminal)
            {
                from.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (SpellHelper.CheckCombat(from))
            {
                from.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else if (destMap == Map.Felucca && from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendLocalizedMessage(1049543); // You decide against traveling to Felucca while you are still young.
                return false;
            }
            else if (SpellHelper.RestrictRedTravel && from.Murderer && destMap.Rules != MapRules.FeluccaRules && !Siege.SiegeShard)
            {
                from.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                return false;
            }
            else if (Region.FindRegions(dest, destMap).Any(r => r.Name == "Abyss") && from is PlayerMobile && !((PlayerMobile)from).AbyssEntry)
            {
                from.SendLocalizedMessage(1112226); // Thou must be on a Sacred Quest to pass through.
                return false;
            }
            else if (CityTradeSystem.HasTrade(from))
            {
                from.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
                return false;
            }
            else if (from.Holding != null)
            {
                from.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
                return false;
            }
            else if (from.Target != null)
            {
                from.SendLocalizedMessage(500310); // You are too busy with something else.
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.SendLocalizedMessage(1114918); // Select a House Teleporter to link to.
                m.BeginTarget(-1, false, TargetFlags.None, (from, targeted) =>
                {
                    if (targeted is HouseTeleporterTile)
                    {
                        HouseTeleporterTile tile = targeted as HouseTeleporterTile;

                        if (tile.IsChildOf(m.Backpack))
                        {
                            tile.Link = this;
                            Link = tile;

                            if (UsesCharges && tile.UsesCharges) //TODO:  Can you link non-charged with charged?
                            {
                                from.SendLocalizedMessage(1115119); // The two House Teleporters are now linked and the charges remaining have been rebalanced.

                                if (!UsesCharges)
                                    UsesCharges = true;

                                if (!tile.UsesCharges)
                                    tile.UsesCharges = true;

                                int charges = _Charges + tile.Charges;
                                Charges = charges / 2;
                                tile.Charges = charges / 2;
                            }
                            else if (!UsesCharges && !tile.UsesCharges)
                            {
                                from.SendLocalizedMessage(1114919); // The two House Teleporters are now linked.
                            }
                            else
                            {
                                from.SendMessage("Those cannot be linked."); // TODO: Message?
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(1114917); // This must be in your backpack to link it.
                        }
                    }
                });
            }
            else if (!IsMoveOver)
            {
                if (Target != null && !Target.Deleted && InRange(m, 1))
                {
                    if (CheckAccess(m))
                    {
                        if (!m.Hidden || m.IsPlayer())
                            new EffectTimer(m.Location, m.Map, 2023, 0x1F0, TimeSpan.FromSeconds(0.4)).Start();

                        new DelayTimer(this, m).Start();
                    }
                }
            }
            else
            {
                m.SendLocalizedMessage(1114917); // This must be in your backpack to link it.
            }
        }

        private class DelayTimer : Timer
        {
            private readonly HouseTeleporter m_Teleporter;
            private readonly Mobile m_Mobile;

            public DelayTimer(HouseTeleporter tp, Mobile m)
                : base(TimeSpan.FromSeconds(1.0))
            {
                m_Teleporter = tp;
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                Item target = m_Teleporter.Target;

                if (target != null && !target.Deleted)
                {
                    Mobile m = m_Mobile;

                    Point3D p = target.GetWorldTop();
                    Map map = target.Map;

                    BaseCreature.TeleportPets(m, p, map);
                    m.MoveToWorld(p, map);                   

                    if (!m.Hidden || m.IsPlayer())
                    {
                        Effects.PlaySound(target.Location, target.Map, 0x1FE);

                        Effects.SendLocationParticles(EffectItem.Create(m_Teleporter.Location, m_Teleporter.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023, 0);
                        Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023, 0);

                        new EffectTimer(target.Location, target.Map, 2023, -1, TimeSpan.FromSeconds(0.4)).Start();
                    }

                    m_Teleporter.OnAfterTeleport(m);
                }
            }
        }

        public override void OnAfterTeleport(Mobile m)
        {
            if (UsesCharges)
            {
                Charges = Math.Max(0, _Charges - 1);

                if (Link != null)
                {
                    Link.Charges = Math.Max(0, Link.Charges - 1);

                    if (!Link.UsesCharges)
                    {
                        Link.UsesCharges = true;
                    }
                }
            }
        }

        public HouseTeleporterTile(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Charges);
            writer.Write(UsesCharges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Charges = reader.ReadInt();
            UsesCharges = reader.ReadBool();

            if (ItemID == 0x40B9)
                ItemID = 0x574A;
        }
    }

    public class HouseTeleporterTypeGump : BaseGump
    {
        public Item Teleporter { get; set; }

        public HouseTeleporterTypeGump(PlayerMobile pm, Item item)
            : base(pm, 100, 100)
        {
            Teleporter = item;
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 310, 400, 0x6DB);

            AddImage(54, 0, 0x6E4);
            AddHtmlLocalized(10, 10, 290, 18, 1114513, "#1113917", 0x0, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            AddItem(35, 80, 0x574A);
            AddButton(105, 80, 0x845, 0x846, 22346, GumpButtonType.Reply, 0);

            AddItem(35, 140, 0x108C);
            AddButton(105, 140, 0x845, 0x846, 4236, GumpButtonType.Reply, 0);

            AddItem(35, 200, 0x108D);
            AddButton(105, 200, 0x845, 0x846, 4237, GumpButtonType.Reply, 0);

            AddItem(35, 260, 0x108E);
            AddButton(105, 260, 0x845, 0x846, 4238, GumpButtonType.Reply, 0);

            AddItem(35, 320, 0x108F);
            AddButton(105, 320, 0x845, 0x846, 4239, GumpButtonType.Reply, 0);

            AddItem(235, 80, 0x1090);
            AddButton(195, 80, 0x845, 0x846, 4240, GumpButtonType.Reply, 0);

            AddItem(235, 140, 0x9CDE);
            AddButton(195, 140, 0x845, 0x846, 40158, GumpButtonType.Reply, 0);

            AddItem(235, 200, 0xA1CB);
            AddButton(195, 200, 0x845, 0x846, 41419, GumpButtonType.Reply, 0);

            AddItem(235, 260, 0xA2BA);
            AddButton(195, 260, 0x845, 0x846, 41658, GumpButtonType.Reply, 0);

            AddItem(235, 320, 0xA2BB);
            AddButton(195, 320, 0x845, 0x846, 41659, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID != 0)
            {
                Teleporter.ItemID = info.ButtonID;
                Refresh();
            }
        }
    }

    public class HouseTeleporterTileBag : Bag
    {
        public override int LabelNumber => 1113917;

        [Constructable]
        public HouseTeleporterTileBag()
            : this(false)
        {
        }

        [Constructable]
        public HouseTeleporterTileBag(bool reward)
        {
            Hue = 1336;

            HouseTeleporterTile tele1 = new HouseTeleporterTile(reward);
            HouseTeleporterTile tele2 = new HouseTeleporterTile(reward);

            tele1.Link = tele2;
            tele2.Link = tele1;

            DropItem(tele1);
            DropItem(tele2);
            DropItem(new HouseTeleporterInstructions(reward));
        }

        public HouseTeleporterTileBag(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HouseTeleporterInstructions : Item
    {
        public override int LabelNumber => 1115122; // Care Instructions

        public bool VetReward { get; set; }

        public HouseTeleporterInstructions(bool reward)
            : base(0xFF4)
        {
            VetReward = reward;
            Hue = 195;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1115123); // Congratulations on becoming the<br> owner of your very own house<br> teleporter set!
            list.Add(1115124); // To use them, lock one down in your<br> home then lock the other down in<br> the home of a trusted friend.

            if (!VetReward)
            {
                list.Add(1115125); // Drop Gate Travel scrolls onto these<br> to recharge them.
            }
        }

        public HouseTeleporterInstructions(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(VetReward);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            VetReward = reader.ReadBool();
        }
    }
}
