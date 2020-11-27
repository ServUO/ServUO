using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x9F1C, 0x9F1D)]
    public class JewelryBox : Container, IDyable, ISecurable
    {
        public override int LabelNumber => 1157694;  // Jewelry Box

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        public JewelryBoxFilter Filter { get; set; }

        public override int DefaultMaxItems => 500;

        public bool IsFull => DefaultMaxItems <= Items.Count;

        [Constructable]
        public JewelryBox()
            : base(0x9F1C)
        {
            Weight = 10.0;
            Filter = new JewelryBoxFilter();
            Level = SecureLevel.CoOwners;
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                base.OnDoubleClick(from);

            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (!IsLockedDown && !IsSecure)
            {
                from.SendLocalizedMessage(1157727); // The jewelry box must be secured before you can use it.
            }
            else
            {
                from.SendGump(new JewelryBoxGump(from, this));
            }
        }

        public bool CheckAccessible(Mobile from, Item item)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true; // Staff can access anything

            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null)
                return false;

            switch (Level)
            {
                case SecureLevel.Owner: return house.IsOwner(from);
                case SecureLevel.CoOwners: return house.IsCoOwner(from);
                case SecureLevel.Friends: return house.IsFriend(from);
                case SecureLevel.Anyone: return true;
                case SecureLevel.Guild: return house.IsGuildMember(from);
            }

            return false;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public bool IsAccept(Item item)
        {
            foreach (Type type in _AcceptList)
                if (item.GetType().IsSubclassOf(type))
                    return true;

            return false;
        }


        private readonly Type[] _AcceptList =
        {
            typeof(BaseRing), typeof(BaseBracelet), typeof(BaseNecklace), typeof(BaseEarrings), typeof(BaseTalisman)
        };

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!IsLockedDown && !IsSecure)
            {
                from.SendLocalizedMessage(1157727); // The jewelry box must be secured before you can use it.
                return false;
            }
            else if (!CheckAccessible(from, this))
            {
                PrivateOverheadMessage(MessageType.Regular, 946, 1010563, from.NetState); // This container is secure.
                return false;
            }
            else if (!IsAccept(dropped))
            {
                from.SendLocalizedMessage(1157724); // This is not a ring, bracelet, necklace, earring, or talisman.
                return false;
            }
            else if (IsFull)
            {
                from.SendLocalizedMessage(1157723); // The jewelry box is full.
                return false;
            }
            else
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), () => from.SendGump(new JewelryBoxGump(from, this)));
                return base.OnDragDrop(from, dropped);
            }
        }

        public override bool DisplaysContent => false;

        public override int GetTotal(TotalType type)
        {
            if (type == TotalType.Weight)
            {
                int weight = base.GetTotal(type);

                if (weight > 0)
                    return (int)Math.Max(1, (base.GetTotal(type) * 0.3));
            }

            return base.GetTotal(type);
        }

        public JewelryBox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Level);
            Filter.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Level = (SecureLevel)reader.ReadInt();
            Filter = new JewelryBoxFilter(reader);
        }
    }
}
