using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x9A97, 0x9A98)]
    public class Grinder : Item, ISecurable
    {
        public override int LabelNumber => 1123599;  // Grinder

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public Grinder()
            : base(0x9A97)
        {
            LootType = LootType.Blessed;
        }

        /*public bool CheckAccessible(Mobile from, Item item)
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
        }*/

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public Grinder(Serial serial)
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
            else if (from.InRange(GetWorldLocation(), 2) /*&& CheckAccessible(from, this)*/)
            {
                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            public InternalTarget(Item item)
                : base(-1, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is CoffeePod)
                {
                    CoffeePod pod = (CoffeePod)targeted;

                    if (!pod.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    }
                    else
                    {
                        from.AddToBackpack(new CoffeeGrounds(pod.Amount));
                        pod.Delete();
                    }
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
            writer.Write(0); // version

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
