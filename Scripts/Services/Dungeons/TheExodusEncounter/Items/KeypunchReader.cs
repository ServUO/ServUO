using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class KeypunchReader : OrnateWoodenChest
    {
        public override int LabelNumber => 1153868;  // Keypunch Reader  

        [Constructable]
        public KeypunchReader()
        {
            Weight = 0.0;
            Hue = 2500;
            Movable = false;
        }

        public KeypunchReader(Serial serial) : base(serial)
        {
        }

        public override bool IsDecoContainer => false;

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            if (dropped is PunchCard)
            {
                if (TotalItems >= 50)
                {
                    CheckItems(from);
                }
                else
                {
                    from.SendLocalizedMessage(1152375); // You feed the punch card into the machine but nothing happens. 
                }

                return true;
            }

            from.SendLocalizedMessage(1074836); // The container cannot hold that type of object.
            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            if (item is PunchCard)
            {
                if (TotalItems >= 50)
                {
                    CheckItems(from);
                }
                else
                {
                    from.SendLocalizedMessage(1152375); // You feed the punch card into the machine but nothing happens. 
                }

                return true;
            }

            from.SendLocalizedMessage(1074836); // The container cannot hold that type of object.
            return false;
        }

        public void CheckItems(Mobile from)
        {
            List<Item> items = Items;

            IEnumerable<Item> punch = items.Where(x => x is PunchCard);

            if (punch.Count() >= 50)
            {
                punch.ToList().ForEach(f => f.Delete());

                from.AddToBackpack(new NexusAddonDeed());
                from.SendLocalizedMessage(1152376); // As you feed the punch card into the machine it turns on! 
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
            reader.ReadInt();
        }
    }
}
