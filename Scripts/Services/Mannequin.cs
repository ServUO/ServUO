using System;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public class Mannequin : BaseCreature
    {
        public override bool NoHouseRestrictions { get { return true; } }
        public override bool ClickTitle { get { return false; } }
        public override bool IsInvulnerable { get { return true; } }

        public Mobile Owner { get; set; }

        [Constructable]
        public Mannequin(Mobile owner)
            : base(AIType.AI_Use_Default, FightMode.None, 1, 1, 0.2, 0.2)
        {
            InitStats(100, 100, 25);

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

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (IsOwner(Owner))
            {
                if (from.Alive && from.InRange(this, 4))
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

        private class CustomizeBodyEntry : ContextMenuEntry
        {
            private Mobile _From;
            private Mobile _Mannequin;

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
            private Mobile _From;
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
            writer.Write((int)0); // version

            writer.Write(Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Owner = reader.ReadMobile();
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
