using System;
using Server.Targeting;

namespace Server.Items
{
    [Flipable(0x9A97, 0x9A98)]
    public class Grinder : Item
    {
        [Constructable]
        public Grinder()
            : base(0x9A97)
        {
            LootType = LootType.Blessed;
        }

        public Grinder(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
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
                    from.AddToBackpack(new CoffeeGrounds());
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
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
