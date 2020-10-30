using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0xA323, 0xA324)]
    public class MagicMilkPail : Item, ISecurable
    {
        public override int LabelNumber => 1159047; // Magic Milk Pail

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextUse { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public MagicMilkPail()
            : base(0xA323)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public MagicMilkPail(Serial serial)
            : base(serial)
        {
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

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (house == null || !house.IsLockedDown(this))
            {
                from.SendLocalizedMessage(1114298); // This must be locked down in order to use it.
            }
            else if (CheckAccessible(from, this))
            {
                if (NextUse < DateTime.UtcNow)
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x47E, 1159049); // *You collect some magically delicious milk!*
                    NextUse = DateTime.UtcNow + TimeSpan.FromSeconds(30);
                    from.AddToBackpack(new GlassMug(BeverageType.Milk));
                }
                else
                {
                    from.SendLocalizedMessage(1159048); // You need to wait a few moments before collecting more milk.
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Level);
            writer.Write(NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Level = (SecureLevel)reader.ReadInt();
            NextUse = reader.ReadDateTime();
        }
    }
}
