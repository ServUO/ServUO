using System;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Targeting;

namespace Server.Mobiles
{
    public class Mannequin : BaseCreature
    {
        public override bool NoHouseRestrictions { get { return true; } }
        public override bool ClickTitle { get { return false; } }
        public override bool IsInvulnerable { get { return true; } }

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
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return m == Owner;
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

        public override void OnAosSingleClick(Mobile from)
        {
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

        private List<Layer> SameLayers = new List<Layer>()
        {
            Layer.FirstValid,
            Layer.OneHanded,
            Layer.TwoHanded,
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
            var ll = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
              .ToList().Where(r => r.FullName.Contains("MannequinProperty") && r.IsClass == true && r.IsAbstract == false).ToList();

            List<ValuedProperty> cat = new List<ValuedProperty>();

            ll.ForEach(x =>
            {
                var CI = Activator.CreateInstance(Type.GetType(x.FullName));

                if (CI is ValuedProperty)
                {
                    ValuedProperty p = CI as ValuedProperty;

                    if (p.Matches(item) || p.AlwaysVisible)
                        cat.Add(p);
                }
            });

            return cat;
        }

        public static List<ValuedProperty> FindItemProperty(Item item, bool visible = false)
        {
            var ll = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
              .ToList().Where(r => r.FullName.Contains("MannequinProperty") && r.IsClass == true && r.IsAbstract == false).ToList();

            List<ValuedProperty> cat = new List<ValuedProperty>();

            ll.ForEach(x =>
            {
                var CI = Activator.CreateInstance(Type.GetType(x.FullName));

                if (CI is ValuedProperty)
                {
                    ValuedProperty p = CI as ValuedProperty;

                    if (p.Matches(item) || visible && p.AlwaysVisible)
                        cat.Add(p);
                }
            });

            return cat.OrderByDescending(x => x.Hue).ToList();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (IsOwner(Owner))
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
                    if (from.Race == Race || (from.Race == Race.Elf && Race == Race.Human || from.Race == Race.Human && Race == Race.Elf))
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
                List<Item> toAdd = new List<Item>(mobile.Items.Where(i => IsEquipped(i)));

                if (mobile.Backpack != null)
                {
                    toAdd.AddRange(mobile.Backpack.Items);
                }

                foreach (var item in toAdd)
                {
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
            private Mobile _From;
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
            private Mobile _From;
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
                    from.SendGump(new MannequinCompareGump(_Mannequin, (Item)targeted));
                }
            }
        }

        private class ViewSuitsSelectItemEntry : ContextMenuEntry
        {
            private Mobile _From;
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
                    if (targeted is Item)
                        from.SendGump(new MannequinStatsGump(_Mannequin, (Item)targeted));
                }
            }
        }

        private class AddDescriptionEntry : ContextMenuEntry
        {
            private Mobile _From;
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
                private Mannequin _Mannequin;

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
            private Mobile _From;
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
            private Mannequin _Mannequin;

            public SwitchClothesEntry(Mobile from, Mannequin m)
                : base(1151606, 2)
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                _Mannequin.SwitchClothes(_From, _Mannequin);
            }
        }

        public static bool IsEquipped(Item item)
        {
            return item != null && item.Parent is Mobile && ((Mobile)item.Parent).FindItemOnLayer(item.Layer) == item &&
                   item.Layer != Layer.Mount && item.Layer != Layer.Bank &&
                   item.Layer != Layer.Invalid && item.Layer != Layer.Backpack && !(item is Backpack);
        }

        public void SwitchClothes(Mobile from, Mobile m)
        {
            List<Item> MobileItems = from.Items.Where(x => IsEquipped(x)).ToList();
            List<Item> MannequinItems = m.Items.Where(x => IsEquipped(x)).ToList();

            MannequinItems.ForEach(x => m.RemoveItem(x));
            MobileItems.ForEach(x => from.RemoveItem(x));

            List<Item> ExceptItems = new List<Item>();

            MannequinItems.ForEach(x =>
            {
                if (x.CanEquip(from))
                {
                    from.EquipItem(x);
                }
                else
                {
                    ExceptItems.Add(x);
                }
            });

            MobileItems.ForEach(x =>
            {
                if (x.CanEquip(m))
                {
                    m.EquipItem(x);
                }
                else
                {
                    ExceptItems.Add(x);
                }
            });

            if (ExceptItems.Count > 0)
            {
                ExceptItems.ForEach(x => from.AddToBackpack(x));
                from.SendLocalizedMessage(1151641, ExceptItems.Count.ToString(), 0x22); // ~1_COUNT~ items could not be swapped between you and the mannequin. These items are now in your backpack, or on the floor at your feet if your backpack is too full to hold them.
            }

            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1151607); // You quickly swap clothes with the mannequin.
        }

        private class RotateEntry : ContextMenuEntry
        {
            private Mobile _From;
            private Mobile _Mannequin;

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
            private Mobile _From;
            private Mobile _Mannequin;

            public RedeedEntry(Mobile from, Mobile m)
                : base(1151601, 2)
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                List<Item> mannequinItems = _Mannequin.Items.Where(x => IsEquipped(x)).ToList();
                mannequinItems.ForEach(x => _From.AddToBackpack(x));

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
            writer.Write((int)1); // version
            
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
        public override int LabelNumber { get { return 1151602; } } // Mannequin Deed

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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
