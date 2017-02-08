using System;

namespace Server.Items
{
    public class PrismaticAmber : Amber
    {
        [Constructable]
        public PrismaticAmber()
            : base()
        {
            this.LootType = LootType.Blessed;
            this.Stackable = false;
            this.Weight = 1;
        }

        public PrismaticAmber(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075299;
            }
        }// Prismatic Amber
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            list.Add(1075269); // Destroyed when dropped
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool ret = base.DropToWorld(from, p);
				
            if (ret)
                this.DestroyItem(from);
				
            return ret;
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            bool ret = base.DropToMobile(from, target, p);
			
            if (ret)
                this.DestroyItem(from);
			
            return ret;
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            bool ret = base.DropToItem(from, target, p);

            if (ret && this.Parent != from.Backpack)
                this.DestroyItem(from);
			
            return ret;
        }

        public virtual void DestroyItem(Mobile from)
        {
            from.SendLocalizedMessage(500424); // You destroyed the item.
            this.Delete();			
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