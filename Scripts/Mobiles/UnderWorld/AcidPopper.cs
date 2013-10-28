using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class AcidPopper : Item
    {
        [Constructable]
        public AcidPopper()
            : this(1)
        {
        }

        [Constructable]
        public AcidPopper(int amount)
            : base(0x44c1)
        {
            this.Hue = 68;
            this.Stackable = true;
            this.Amount = amount;
        }

        public AcidPopper(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1095058;
            }
        }// Acid Popper
        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1060640);   // The item must be in your backpack to use it.
                return;
            }

            List<NavreyParalyzingWeb> list = new List<NavreyParalyzingWeb>();
            foreach (Item item in this.Map.GetItemsInRange(this.GetWorldLocation(), 0))
            {
                if (item is NavreyParalyzingWeb)
                    list.Add((NavreyParalyzingWeb)item);
            }

            if (0 == list.Count)
                return;

            this.Consume();
            from.SendLocalizedMessage(1113240);   // The acid popper bursts and burns away the webbing.

            foreach (Item item in list)
                item.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}