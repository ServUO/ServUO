using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    [Furniture]
    [Flipable(0xA50F, 0xA510)]
    public class CowStatue : Item, ISecurable
    {
        public override int LabelNumber => 1072494;  // Cow

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextResourceCount { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public CowStatue()
            : base(0xA50F)
        {
            LootType = LootType.Blessed;
            Weight = 1;
            NextResourceCount = DateTime.UtcNow + TimeSpan.FromDays(1);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
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

        public CowStatue(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null || !house.IsLockedDown(this))
            {
                from.SendLocalizedMessage(1114298); // This must be locked down in order to use it.
            }
            else if (from.InRange(GetWorldLocation(), 2) && CheckAccessible(from, this))
            {
                if (NextResourceCount < DateTime.UtcNow)
                {
                    NextResourceCount = DateTime.UtcNow + TimeSpan.FromDays(1);
                    from.AddToBackpack(new RawRibs(2));
                }
                else
                {
                    from.SendLocalizedMessage(1071539); // Sorry. You cannot receive another item at this time.
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write((int)Level);
            writer.Write(NextResourceCount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Level = (SecureLevel)reader.ReadInt();
            NextResourceCount = reader.ReadDateTime();
        }
    }
}
