using Server;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bittiez.RegBag
{
    public class RegBag : Bag
    {
        [Constructable]
        public RegBag()
        {
            this.Name = "a regeant bag";
            this.Weight = 1;
            this.Hue = 673;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!(dropped is BaseReagent)){
                from.SendMessage("It seems you can't place that item in this bag.");
                return false;
            }

            return base.OnDragDrop(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!(item is BaseReagent))
            {
                from.SendMessage("It seems you can't place that item in this bag.");
                return false;
            }
            return base.OnDragDropInto(from, item, p);
        }

        public override int GetTotal(TotalType type)
        {
            if (type != TotalType.Weight)
                return base.GetTotal(type);
            else
            {
                return (int)(TotalItemWeights() * (0.1));
            }
        }

        public override void UpdateTotal(Item sender, TotalType type, int delta)
        {
            if (type != TotalType.Weight)
                base.UpdateTotal(sender, type, delta);
            else
                base.UpdateTotal(sender, type, (int)(delta * (0.1)));
        }

        private double TotalItemWeights()
        {
            double weight = 0.0;

            foreach (Item item in Items)
                weight += (item.Weight * (double)(item.Amount));

            return weight;
        }

        public RegBag(Serial serial)
            : base(serial)
        {
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
