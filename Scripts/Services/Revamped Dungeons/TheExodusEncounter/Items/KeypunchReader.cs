using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class KeypunchReader : OrnateWoodenChest
    {
        public override int LabelNumber => 1153868;  // Keypunch Reader  

        [Constructable]
        public KeypunchReader()
            : base()
        {
            Weight = 0.0;
            Hue = 2500;
            Movable = false;
        }

        public KeypunchReader(Serial serial) : base(serial)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            if (TotalItems >= 50)
            {
                CheckItems(from);
            }

            if (dropped is PunchCard)
            {
                from.SendLocalizedMessage(1152375); // You feed the punch card into the machine but nothing happens. 
            }

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            if (TotalItems >= 50)
            {
                CheckItems(from);
            }

            if (item is PunchCard)
            {
                from.SendLocalizedMessage(1152375); // You feed the punch card into the machine but nothing happens. 
            }

            return true;
        }

        public void CheckItems(Mobile m)
        {
            List<Item> items = Items;

            IEnumerable<Item> punch = items.Where(x => x is PunchCard);
            IEnumerable<Item> kit = items.Where(x => x is ExoticToolkit);

            if (punch.Count() >= 50 && kit.Count() >= 1)
            {
                punch.ToList().ForEach(f => f.Delete());

                DropItem(new NexusAddonDeed());
                m.SendLocalizedMessage(1152376); // As you feed the punch card into the machine it turns on! 
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
}