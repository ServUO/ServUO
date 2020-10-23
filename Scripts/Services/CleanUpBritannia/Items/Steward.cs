using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Network;
using Server.Prompts;
using Server.Spells;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public class Steward : BaseCreature
    {
        private Dictionary<Mobile, DateTime> _Table;

        public override bool NoHouseRestrictions => true;
        public override bool ClickTitle => false;
        public override bool IsInvulnerable => true;

        private BaseHouse m_House;
        public Mobile Owner { get; set; }
        public string Keyword { get; set; }

        public BaseHouse House
        {
            get { return m_House; }
            set
            {
                if (m_House != null)
                    m_House.PlayerVendors.Remove(this);

                if (value != null)
                    value.PlayerVendors.Add(this);

                m_House = value;
            }
        }

        [Constructable]
        public Steward(Mobile owner, BaseHouse house)
            : base(AIType.AI_Use_Default, FightMode.None, 1, 1, 0.2, 0.2)
        {
            InitStats(100, 100, 25);

            Hits = HitsMax;
            Blessed = true;
            Frozen = true;

            Owner = owner;
            House = house;
            Body = 0x190;
            Race = Race.Human;
            Name = "a Steward";
            Hue = 1828;
            Direction = Direction.South;

            Keyword = "";
            _Table = new Dictionary<Mobile, DateTime>();

            Container pack = new StewardBackpack(Owner, this)
            {
                Movable = false
            };
            AddItem(pack);
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

        public override void OnDoubleClick(Mobile from)
        {
            DisplayPaperdollTo(from);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (IsOwner(from) && Backpack != null)
            {
                return AddToBackpack(dropped);
            }

            return false;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return from is PlayerMobile;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            PlayerMobile m = e.Mobile as PlayerMobile;

            if (m == null || Keyword == "")
                return;

            if (e.Speech.Contains(Keyword))
            {
                if (Backpack == null || Backpack.Items.Count <= 0 || IsInCooldown(m))
                {
                    return;
                }

                if (Backpack.Items.Count > 0 && m.AddToBackpack(Backpack.Items[Utility.Random(Backpack.Items.Count)]))
                {
                    m.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
                    _Table[m] = DateTime.UtcNow + TimeSpan.FromHours(24);
                }
            }
        }

        public bool IsInCooldown(Mobile from)
        {
            if (_Table.ContainsKey(from))
            {
                if (_Table[from] < DateTime.UtcNow)
                    _Table.Remove(from);
            }

            return _Table.ContainsKey(from);
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

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (IsOwner(from))
            {
                if (from.Alive && from.InRange(this, 4))
                    list.Add(new CustomizeBodyEntry(from, this));

                if (from.Alive && from.InRange(this, 2))
                {
                    list.Add(new RenameEntry(from, this));
                    list.Add(new SetKeywordEntry(from, this));
                    list.Add(new OpenBackpackEntry(from, this));

                    if (from.Race == Race || (from.Race == Race.Elf && Race == Race.Human || from.Race == Race.Human && Race == Race.Elf))
                    {
                        list.Add(new SwitchClothesEntry(from, this));
                    }

                    list.Add(new RotateEntry(from, this));
                    list.Add(new RedeedEntry(from, this));
                }
            }
        }

        private class RenameEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mobile _Mannequin;

            public RenameEntry(Mobile from, Mobile m)
                : base(1155203, 2)
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                _From.SendLocalizedMessage(1062494); // Enter a new name for your vendor (20 characters max):
                _From.Prompt = new RenamePrompt(_Mannequin);
            }
        }

        private class RenamePrompt : Prompt
        {
            public override int MessageCliloc => 1062433;
            private readonly Mobile _Mannequin;

            public RenamePrompt(Mobile m)
            {
                _Mannequin = m;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (text.Length > 20)
                {
                    from.SendLocalizedMessage(501143); // That name is too long.
                }
                else
                {
                    _Mannequin.Name = text.Trim() == "" ? "Pat" : text;
                    from.SendLocalizedMessage(1062496); // Your vendor has been renamed.
                }
            }
        }

        private class SetKeywordEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mobile _Mannequin;

            public SetKeywordEntry(Mobile from, Mobile m)
                : base(1153254, 4)
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                _From.SendLocalizedMessage(1153259); // Enter a new keyword for your npc (20 characters max):
                _From.Prompt = new SetKeywordPrompt(_Mannequin);
            }
        }

        private class SetKeywordPrompt : Prompt
        {
            private readonly Mobile _Mannequin;

            public SetKeywordPrompt(Mobile m)
            {
                _Mannequin = m;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (text.Length > 20)
                {
                    from.SendLocalizedMessage(1153255); // That keyword is too long.
                }
                else
                {
                    if (_Mannequin is Steward)
                    {
                        ((Steward)_Mannequin).Keyword = text;
                        from.SendLocalizedMessage(1153257); // The keyword has been set.
                    }
                }
            }
        }

        private static bool ContainsDisallowedSpeech(string text)
        {
            foreach (string word in ProfanityProtection.Disallowed)
            {
                if (text.Contains(word))
                    return true;
            }
            return false;
        }

        private class OpenBackpackEntry : ContextMenuEntry
        {
            private readonly Mobile _From;
            private readonly Mobile _Mannequin;

            public OpenBackpackEntry(Mobile from, Mobile m)
                : base(3006145, 4)
            {
                _From = from;
                _Mannequin = m;
            }

            public override void OnClick()
            {
                if (_Mannequin.Backpack != null)
                    _Mannequin.Backpack.DisplayTo(_From);
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
            private readonly Steward _Mannequin;

            public SwitchClothesEntry(Mobile from, Steward m)
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
                if (_Mannequin.Backpack != null && _Mannequin.Backpack.Items.Any())
                {
                    _From.SendLocalizedMessage(1153315); // You must empty the mannequin's backpack before re-deeding.
                    return;
                }

                List<Item> mannequinItems = _Mannequin.Items.Where(x => IsEquipped(x)).ToList();
                mannequinItems.ForEach(x => _From.AddToBackpack(x));

                _Mannequin.Delete();

                _From.AddToBackpack(new StewardDeed());
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            House = null;
        }

        public Steward(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(House);
            writer.Write(Owner);
            writer.Write(Keyword);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Table = new Dictionary<Mobile, DateTime>();

            switch (version)
            {
                case 1:
                    {
                        House = (BaseHouse)reader.ReadItem();
                        goto case 0;
                    }
                case 0:
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(15), () => { House = BaseHouse.FindHouseAt(this); });

                        Owner = reader.ReadMobile();
                        Keyword = reader.ReadString();

                        break;
                    }
            }
        }
    }

    public class StewardBackpack : Backpack
    {
        private Mobile _Owner;
        private Mobile _Mannequin;

        public StewardBackpack(Mobile from, Mobile m)
        {
            _Owner = from;
            _Mannequin = m;

            Layer = Layer.Backpack;
        }

        public override int DefaultMaxWeight => 400;

        public StewardBackpack(Serial serial)
            : base(serial)
        {
        }

        public virtual bool IsOwner(Mobile m)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            return m == _Owner;
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            return false;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return base.OnDragDrop(from, dropped);
        }

        public override bool IsAccessibleTo(Mobile m)
        {
            return IsOwner(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(_Mannequin);
            writer.Write(_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        _Mannequin = reader.ReadMobile();
                        goto case 0;
                    }
                case 0:
                    {
                        _Owner = reader.ReadMobile();

                        break;
                    }
            }
        }
    }

    public class MannequinGump : Gump
    {
        private readonly Mobile _From;
        private readonly Mobile _Mannequin;

        public MannequinGump(Mobile from, Mobile m)
            : base(50, 50)
        {
            _From = from;
            _Mannequin = m;

            AddPage(0);

            AddBackground(0, 0, 300, 130, 0x13BE);
            AddImageTiled(10, 10, 280, 20, 0xA40);
            AddImageTiled(10, 40, 280, 80, 0xA40);
            AddAlphaRegion(10, 10, 280, 110);
            AddHtmlLocalized(10, 12, 280, 18, 1151582, 0x7FFF, false, false); // <center>CUSTOMIZE BODY</center>

            AddHtmlLocalized(45, 52, 180, 18, 1072255, _Mannequin.Race == Race.Human ? 0x1CFF : 0x7FFF, false, false); // Human
            AddButton(10, 50, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 72, 180, 18, 1072256, _Mannequin.Race == Race.Elf ? 0x1CFF : 0x7FFF, false, false); // Elf
            AddButton(10, 70, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(45, 92, 180, 18, 1029613, _Mannequin.Race == Race.Gargoyle ? 0x1CFF : 0x7FFF, false, false); // Gargoyle 
            AddButton(10, 90, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(205, 52, 180, 18, 1015327, _Mannequin.Female ? 0x7FFF : 0x1CFF, false, false); // Male
            AddButton(170, 50, 0xFA6, 0xFA6, 4, GumpButtonType.Reply, 0);

            AddHtmlLocalized(205, 72, 180, 18, 1015328, _Mannequin.Female ? 0x1CFF : 0x7FFF, false, false); // Female
            AddButton(170, 70, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0);
        }

        public void ValidateItems(Mobile from, Mobile m)
        {
            List<Item> MannequinItems = m.Items.Where(x => Steward.IsEquipped(x)).ToList();
            MannequinItems.ForEach(x => _Mannequin.RemoveItem(x));

            List<Item> ExceptItems = new List<Item>();

            MannequinItems.ForEach(x =>
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
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (_Mannequin == null)
                return;

            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 1: // Human
                    {
                        if (_Mannequin.Female)
                        {
                            _Mannequin.Body = 0x190;
                        }
                        else
                        {
                            _Mannequin.Body = 0x191;
                        }

                        _Mannequin.Race = Race.Human;

                        ValidateItems(from, _Mannequin);

                        break;
                    }
                case 2: // Elf
                    {
                        if (_Mannequin.Female)
                        {
                            _Mannequin.Body = 0x25d;
                        }
                        else
                        {
                            _Mannequin.Body = 0x25e;
                        }

                        _Mannequin.Race = Race.Elf;

                        ValidateItems(from, _Mannequin);

                        break;
                    }
                case 3: // Gargoyle
                    {
                        if (_Mannequin.Female)
                        {
                            _Mannequin.Body = 0x29a;
                        }
                        else
                        {
                            _Mannequin.Body = 0x29b;
                        }

                        _Mannequin.Race = Race.Gargoyle;

                        ValidateItems(from, _Mannequin);

                        break;
                    }
                case 4: // Male
                    {
                        if (_Mannequin.Race == Race.Human)
                        {
                            _Mannequin.Body = 0x190;
                        }
                        else if (_Mannequin.Race == Race.Elf)
                        {
                            _Mannequin.Body = 0x25d;
                        }
                        else if (_Mannequin.Race == Race.Gargoyle)
                        {
                            _Mannequin.Body = 0x29a;
                        }

                        _Mannequin.Female = false;

                        ValidateItems(from, _Mannequin);

                        break;
                    }
                case 5: // Female
                    {
                        if (_Mannequin.Race == Race.Human)
                        {
                            _Mannequin.Body = 0x191;
                        }
                        else if (_Mannequin.Race == Race.Elf)
                        {
                            _Mannequin.Body = 0x25e;
                        }
                        else if (_Mannequin.Race == Race.Gargoyle)
                        {
                            _Mannequin.Body = 0x29b;
                        }

                        _Mannequin.Female = true;

                        ValidateItems(from, _Mannequin);

                        break;
                    }
            }
        }
    }

    [Flipable(0x14F0, 0x14EF)]
    public class StewardDeed : Item
    {
        public override int LabelNumber => 1153344;  // Steward Deed

        [Constructable]
        public StewardDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
        }

        public StewardDeed(Serial serial)
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
                        if (house.Public)
                        {
                            from.SendLocalizedMessage(1151657); // Where do you wish to place this?
                            from.Target = new PlaceTarget(this);
                        }
                        else
                        {
                            from.SendLocalizedMessage(1153304); // You cannot place this vendor, steward or barkeep. Make sure the house is public and has sufficient storage available.
                        }
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
            int version = reader.ReadInt();
        }
    }

    public class PlaceTarget : Target
    {
        private readonly Item _Deed;

        public PlaceTarget(Item deed)
            : base(-1, true, TargetFlags.None)
        {
            _Deed = deed;
        }

        public static AddonFitResult CouldFit(Point3D p, Map map, Mobile from, ref BaseHouse house)
        {
            if (!map.CanFit(p.X, p.Y, p.Z, 20, true, true, true))
                return AddonFitResult.Blocked;
            else if (!BaseAddon.CheckHouse(from, p, map, 20, ref house))
                return AddonFitResult.NotInHouse;
            else
                return CheckDoors(p, 20, house);
        }

        public static AddonFitResult CheckDoors(Point3D p, int height, BaseHouse house)
        {
            List<Item> doors = house.Doors;

            for (int i = 0; i < doors.Count; i++)
            {
                BaseDoor door = doors[i] as BaseDoor;

                Point3D doorLoc = door.GetWorldLocation();
                int doorHeight = door.ItemData.CalcHeight;

                if (Utility.InRange(doorLoc, p, 1) && (p.Z == doorLoc.Z || ((p.Z + height) > doorLoc.Z && (doorLoc.Z + doorHeight) > p.Z)))
                    return AddonFitResult.DoorTooClose;
            }

            return AddonFitResult.Valid;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            IPoint3D p = targeted as IPoint3D;
            Map map = from.Map;

            if (p == null || map == null || _Deed == null || _Deed.Deleted)
                return;

            if (_Deed.IsChildOf(from.Backpack))
            {
                SpellHelper.GetSurfaceTop(ref p);
                BaseHouse house = null;
                Point3D loc = new Point3D(p);

                if (targeted is Item && !((Item)targeted).IsLockedDown && !((Item)targeted).IsSecure && !(targeted is AddonComponent))
                {
                    from.SendLocalizedMessage(1151655); // The mannequin cannot fit there.
                    return;
                }

                AddonFitResult result = CouldFit(loc, map, from, ref house);

                if (result == AddonFitResult.Valid)
                {
                    Mobile mannequin;

                    if (_Deed is StewardDeed)
                    {
                        mannequin = new Steward(from, house);
                    }
                    else
                    {
                        mannequin = new Mannequin(from);
                    }

                    mannequin.MoveToWorld(loc, map);
                    _Deed.Delete();
                }
                else
                {
                    from.SendLocalizedMessage(1151655); // The mannequin cannot fit there.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }
    }
}
