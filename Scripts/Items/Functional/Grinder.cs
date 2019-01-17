using System;
using Server.Gumps;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    [Flipable(0x9A97, 0x9A98)]
    public class Grinder : Item, ISecurable
    {
        public override int LabelNumber { get { return 1123599; } } // Grinder

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public Grinder()
            : base(0x9A97)
        {
            LootType = LootType.Blessed;
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

        public Grinder(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (CheckAccessible(from, this))
                from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private readonly Item Item;
            public InternalTarget(Item item)
                : base(-1, true, TargetFlags.None)
            {
                Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (Item.Deleted)
                    return;

                if (!Item.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    return;
                }

                if (Item is CoffeePod)
                {
                    from.AddToBackpack(new CoffeeGrounds(((CoffeePod)Item).Amount));
                }
                else
                {
                    from.SendLocalizedMessage(1155729); // That is not something that can be ground.
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Level = (SecureLevel)reader.ReadInt();
        }
    }
}
