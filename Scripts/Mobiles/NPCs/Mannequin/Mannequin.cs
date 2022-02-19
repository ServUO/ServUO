using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using Server.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public class Mannequin : BaseCreature
    {
        public override bool NoHouseRestrictions => true;
        public override bool ClickTitle => false;
        public override bool IsInvulnerable => true;

        public Mobile Owner { get; set; }
        public string Description { get; set; }

        [Constructable]
        public Mannequin(Mobile owner)
            : base(AIType.AI_Use_Default, FightMode.None, 1, 1, 0.2, 0.2)
        {
            InitStats(100, 100, 25);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Fire, 0);
            SetDamageType(ResistanceType.Cold, 0);
            SetDamageType(ResistanceType.Poison, 0);
            SetDamageType(ResistanceType.Energy, 0);

            Hits = HitsMax;
            Blessed = true;
            Frozen = true;

            Owner = owner;
            Body = 0x190;
            Race = Race.Human;
            Name = "a Mannequin";
            Hue = 1828;
            Direction = Direction.South;
        }

        public bool IsOwner(Mobile m)
        {
            if (m == Owner || m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return AccountHandler.CheckAccount(m, Owner);
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            return false;
        }

        public override bool AllowEquipFrom(Mobile from)
        {
            if (IsOwner(from))
                return true;

            return base.AllowEquipFrom(from);
        }

        public override bool CheckNonlocalLift(Mobile from, Item item)
        {
            if (IsOwner(from))
                return true;

            return base.CheckNonlocalLift(from, item);
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            if (IsOwner(from))
                return true;

            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(Description))
            {
                list.Add(1159410, Description); // Description: ~1_MESSAGE~
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            DisplayPaperdollTo(from);
        }

        public override void OnStatsQuery(Mobile from)
        {
            if (from.Map == Map && Utility.InUpdateRange(this, from) && from.CanSee(this))
            {
                from.Send(new MobileStatusCompact(false, this));

                if (Map != null)
                {
                    ProcessDelta();

                    Packet p = null;

                    IPooledEnumerable eable = Map.GetClientsInRange(Location);

                    foreach (NetState state in eable)
                    {
                        state.Mobile.ProcessDelta();

                        if (p == null)
                            p = Packet.Acquire(new UpdateStatueAnimation(this, 1, 4, 0));

                        state.Send(p);
                    }

                    Packet.Release(p);

                    eable.Free();
                }
            }
        }

        private readonly List<Layer> SameLayers = new List<Layer>
        {
            Layer.FirstValid,
            Layer.OneHanded,
            Layer.TwoHanded
        };

        public bool CheckSameLayer(Item i1, Item i2)
        {
            return i1 is BaseWeapon && i2 is BaseWeapon && SameLayers.Contains(i1.Layer) && SameLayers.Contains(i2.Layer);
        }

        public bool LayerValidation(Item i1, Item i2)
        {
            return i1.Layer == i2.Layer || CheckSameLayer(i1, i2);
        }

        public static List<ValuedProperty> GetProperty(Item item)
        {
            return FindItemProperty(item, true).Where(x => x.Catalog != Catalog.None).ToList();
        }

        public static List<ValuedProperty> GetProperty(List<Item> items)
        {
            return FindItemsProperty(items).Where(x => x.Catalog != Catalog.None).ToList();
        }

        public static List<ValuedProperty> FindItemsProperty(List<Item> item)
        {
            List<Type> ll = new List<Type>();

            var rs = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

            for (var index = 0; index < rs.Length; index++)
            {
                var r = rs[index];

                if (r.FullName != null && r.FullName.Contains("MannequinProperty") && r.IsClass && !r.IsAbstract)
                {
                    ll.Add(r);
                }
            }

            List<ValuedProperty> cat = new List<ValuedProperty>();

            for (var index = 0; index < ll.Count; index++)
            {
                var x = ll[index];

                object CI = Activator.CreateInstance(Type.GetType(x.FullName));

                if (CI is ValuedProperty p && (p.Matches(item) || p.AlwaysVisible))
                {
                    cat.Add(p);
                }
            }

            return cat;
        }

        public static List<ValuedProperty> FindItemProperty(Item item, bool visible = false)
        {
            List<Type> ll = new List<Type>();

            var rs = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

            for (var index = 0; index < rs.Length; index++)
            {
                var r = rs[index];

                if (r.FullName != null && r.FullName.Contains("MannequinProperty") && r.IsClass && !r.IsAbstract)
                {
                    ll.Add(r);
                }
            }

            List<ValuedProperty> cat = new List<ValuedProperty>();

            for (var index = 0; index < ll.Count; index++)
            {
                var x = ll[index];

                object CI = Activator.CreateInstance(Type.GetType(x.FullName));

                if (CI is ValuedProperty p && (p.Matches(item) || visible && p.AlwaysVisible))
                {
                    cat.Add(p);
                }
            }

            return cat.OrderByDescending(x => x.Hue).ToList();
        }

        public static List<ValuedProperty> FindMagicalItemProperty(Item item)
        {
            List<Type> ll = new List<Type>();

            var rs = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

            for (var index = 0; index < rs.Length; index++)
            {
                var r = rs[index];

                if (r.FullName != null && r.FullName.Contains("MannequinProperty") && r.IsClass && !r.IsAbstract)
                {
                    ll.Add(r);
                }
            }

            List<ValuedProperty> cat = new List<ValuedProperty>();

            for (var index = 0; index < ll.Count; index++)
            {
                var x = ll[index];

                object CI = Activator.CreateInstance(Type.GetType(x.FullName));

                if (CI is ValuedProperty p && p.Matches(item) && p.IsMagical)
                {
                    cat.Add(p);
                }
            }

            return cat;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (IsOwner(from))
            {
                if (from.Alive && from.InRange(this, 2))
                {
                    list.Add(new ViewSuitsEntry(from, this));
                    list.Add(new CompareWithItemInSlotEntry(from, this));
                    list.Add(new ViewSuitsSelectItemEntry(from, this));
                    list.Add(new AddDescriptionEntry(from, this));
                }

                if (from.InRange(this, 4))
                    list.Add(new CustomizeBodyEntry(from, this));

                if (from.Alive && from.InRange(this, 2))
                {
                    if (from.Race == Race || from.Race == Race.Elf && Race == Race.Human || from.Race == Race.Human && Race == Race.Elf)
                    {
                        list.Add(new SwitchClothesEntry(from, this));
                    }

                    list.Add(new RotateEntry(from, this));
                    list.Add(new RedeedEntry(from, this));
                }
            }
        }

        public static void ForceRedeed(Mobile mobile, BaseHouse house = null)
        {
            if (!(mobile is Mannequin) && !(mobile is Steward))
            {
                return;
            }

            if (house != null)
            {
                List<Item> toAdd = new List<Item>(mobile.Items.Where(IsEquipped));

                if (mobile.Backpack != null)
                {
                    toAdd.AddRange(mobile.Backpack.Items);
                }

                for (var index = 0; index < toAdd.Count; index++)
                {
                    Item item = toAdd[index];

                    house.DropToMovingCrate(item);
                }

                if (mobile is Mannequin)
                {
                    house.DropToMovingCrate(new MannequinDeed());
                }
                else
                {
                    house.DropToMovingCrate(new StewardDeed());
                }
            }

            mobile.Delete();
        }

        private class ViewSuitsEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mannequin _Mannequin;

            public ViewSuitsEntry(Mobile from, Mannequin m)
                : base(1159296, 3) // View Suit Stats
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                _From.SendGump(new MannequinStatsGump(_Mannequin));
            }
        }

        private class CompareWithItemInSlotEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mannequin _Mannequin;

            public CompareWithItemInSlotEntry(Mobile from, Mannequin m)
                : base(1159295, 3) // View Suit Stats With Selected Item
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                _From.SendLocalizedMessage(1159294); // Target the item you wish to compare.
                _From.Target = new InternalTarget(_Mannequin);
            }

            private class InternalTarget : Target
            {
                private readonly Mannequin _Mannequin;

                public InternalTarget(Mannequin m)
                    : base(-1, false, TargetFlags.None)
                {
                    _Mannequin = m;
                }

                protected override void OnTarget(Mobile from, object targeted)
                {
                    if (targeted is Item item)
                    {
                        from.SendGump(new MannequinCompareGump(_Mannequin, item));
                    }
                    else
                    {
                        from.SendLocalizedMessage(1149667); // Invalid target.
                    }

                }
            }
        }

        private class ViewSuitsSelectItemEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mannequin _Mannequin;

            public ViewSuitsSelectItemEntry(Mobile from, Mannequin m)
                : base(1159297, 3) // View Suit Stats With Selected Item
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                _From.SendLocalizedMessage(1159294); // Target the item you wish to compare.
                _From.Target = new InternalTarget(_Mannequin);
            }

            private class InternalTarget : Target
            {
                private readonly Mannequin _Mannequin;

                public InternalTarget(Mannequin m)
                    : base(-1, false, TargetFlags.None)
                {
                    _Mannequin = m;
                }

                protected override void OnTarget(Mobile from, object targeted)
                {
                    if (targeted is Item item)
                        from.SendGump(new MannequinStatsGump(_Mannequin, item));
                }
            }
        }

        private class AddDescriptionEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mannequin _Mannequin;

            public AddDescriptionEntry(Mobile from, Mannequin m)
                : base(1159411, 3) // Add Description
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                _From.SendGump(new DescriptionGump(_Mannequin));
            }

            private class DescriptionGump : Gump
            {
                private readonly Mannequin _Mannequin;

                public DescriptionGump(Mannequin mann)
                    : base(0, 0)
                {
                    _Mannequin = mann;

                    AddBackground(50, 50, 400, 300, 0xA28);

                    AddPage(0);

                    AddHtmlLocalized(50, 70, 400, 20, 1159409, 0x0, false, false); // <CENTER>Mannequin</CENTER>
                    AddHtmlLocalized(75, 95, 350, 145, 1159408, 0x0, true, true); // Enter the description to add to the mannequin. Leave the text area blank to remove any existing text.
                    AddButton(125, 300, 0x81A, 0x81B, 1, GumpButtonType.Reply, 0);
                    AddButton(320, 300, 0x819, 0x818, 0, GumpButtonType.Reply, 0);
                    AddImageTiled(75, 245, 350, 40, 0xDB0);
                    AddImageTiled(76, 245, 350, 2, 0x23C5);
                    AddImageTiled(75, 245, 2, 40, 0x23C3);
                    AddImageTiled(75, 285, 350, 2, 0x23C5);
                    AddImageTiled(425, 245, 2, 42, 0x23C3);
                    AddTextEntry(78, 246, 343, 37, 0x4FF, 0, "", 44);
                }

                public override void OnResponse(NetState sender, RelayInfo info)
                {
                    if (_Mannequin.Deleted)
                        return;

                    if (info.ButtonID == 1)
                    {
                        TextRelay text = info.GetTextEntry(0);
                        string s = text.Text;

                        if (s.Length > 44)
                            s = s.Substring(0, 44);

                        _Mannequin.Description = s;
                        _Mannequin.InvalidateProperties();

                        sender.Mobile.SendLocalizedMessage(1159412); // Updated
                    }
                }
            }
        }

        private class CustomizeBodyEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mobile _Mannequin;

            public CustomizeBodyEntry(Mobile from, Mobile m)
                : base(1151585, 4)
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                _From.SendGump(new MannequinGump(_From, _Mannequin));
            }
        }

        private class SwitchClothesEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mannequin _Mannequin;

            public SwitchClothesEntry(Mobile from, Mannequin m)
                : base(1151606, 2)
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                if (!_From.HasTrade)
                {
                    _Mannequin.SwitchClothes(_From, _Mannequin);
                }
                else
                {
                    _From.SendLocalizedMessage(1004041); // You can't do that while you have a trade pending.
                }
            }
        }

        public static bool IsEquipped(Item item)
        {
            return item != null && item.Parent is Mobile mobile && mobile.FindItemOnLayer(item.Layer) == item &&
                   item.Layer != Layer.Mount && item.Layer != Layer.Bank &&
                   item.Layer != Layer.Invalid && item.Layer != Layer.Backpack && !(item is Backpack);
        }

        public void SwitchClothes(Mobile from, Mobile m)
        {
            List<Item> MobileItems = new List<Item>();
            foreach (var item in from.Items.Where(IsEquipped))
            {
                MobileItems.Add(item);
            }

            List<Item> MannequinItems = new List<Item>();
            foreach (var item in m.Items.Where(IsEquipped))
            {
                MannequinItems.Add(item);
            }

            for (var index = 0; index < MannequinItems.Count; index++)
            {
                var mannequinItem = MannequinItems[index];

                m.RemoveItem(mannequinItem);
            }

            for (var index = 0; index < MobileItems.Count; index++)
            {
                var mobileItem = MobileItems[index];

                from.RemoveItem(mobileItem);
            }

            List<Item> ExceptItems = new List<Item>();

            for (var index = 0; index < MannequinItems.Count; index++)
            {
                var x = MannequinItems[index];

                if (x.CanEquip(from))
                {
                    from.EquipItem(x);
                }
                else
                {
                    ExceptItems.Add(x);
                }
            }

            for (var index = 0; index < MobileItems.Count; index++)
            {
                var x = MobileItems[index];

                if (x.CanEquip(m))
                {
                    m.EquipItem(x);
                }
                else
                {
                    ExceptItems.Add(x);
                }
            }

            if (ExceptItems.Count > 0)
            {
                for (var index = 0; index < ExceptItems.Count; index++)
                {
                    var x = ExceptItems[index];

                    from.AddToBackpack(x);
                }

                from.SendLocalizedMessage(1151641, ExceptItems.Count.ToString(), 0x22); // ~1_COUNT~ items could not be swapped between you and the mannequin. These items are now in your backpack, or on the floor at your feet if your backpack is too full to hold them.
            }

            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1151607); // You quickly swap clothes with the mannequin.
        }

        private class RotateEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mobile _Mannequin;

            public RotateEntry(Mobile from, Mobile m)
                : base(1151586, 2)
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                int direction = (int)_Mannequin.Direction;
                direction++;

                if (direction > 0x7)
                    direction = 0x0;

                _Mannequin.Direction = (Direction)direction;

                _From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1151587); // You rotate the mannequin a little bit.
            }
        }

        private class RedeedEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mobile _Mannequin;

            public RedeedEntry(Mobile from, Mobile m)
                : base(1151601, 2)
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                List<Item> mannequinItems = new List<Item>();

                foreach (var item in _Mannequin.Items.Where(IsEquipped))
                {
                    mannequinItems.Add(item);
                }

                for (var index = 0; index < mannequinItems.Count; index++)
                {
                    var x = mannequinItems[index];

                    _From.AddToBackpack(x);
                }

                _Mannequin.Delete();

                _From.AddToBackpack(new MannequinDeed());
            }
        }

        public Mannequin(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(Description);
            writer.Write(Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        Description = reader.ReadString();
                        goto case 0;
                    }
                case 0:
                    {
                        Owner = reader.ReadMobile();
                        break;
                    }
            }
        }
    }



    [Flipable(0x14F0, 0x14EF)]
    public class MannequinDeed : Item
    {
        public override int LabelNumber => 1151602;  // Mannequin Deed

        [Constructable]
        public MannequinDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
        }

        public MannequinDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null)
                {
                    if (house.Owner == from || house.IsCoOwner(from))
                    {
                        from.SendLocalizedMessage(1151657); // Where do you wish to place this?
                        from.Target = new PlaceTarget(this);
                    }
                    else
                    {
                        from.SendLocalizedMessage(502096); // You must own the house to do this.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
