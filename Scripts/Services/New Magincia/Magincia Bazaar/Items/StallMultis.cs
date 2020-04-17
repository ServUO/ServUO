using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public class BaseBazaarMulti : BaseMulti
    {
        private List<Item> m_Fillers;

        public List<Item> Fillers => m_Fillers;

        public BaseBazaarMulti(int id) : base(id)
        {
            m_Fillers = new List<Item>();
        }

        public void AddComponent(Item item)
        {
            if (item != null)
                m_Fillers.Add(item);
        }

        public override void OnLocationChange(Point3D old)
        {
            foreach (Item item in m_Fillers)
            {
                if (item != null && !item.Deleted)
                    item.Location = new Point3D(X + (item.X - old.X), Y + (item.Y - old.Y), Z + (item.Z - old.Z));
            }
        }

        public override void OnMapChange()
        {
            foreach (Item item in m_Fillers)
            {
                if (item != null && !item.Deleted)
                    item.Map = Map; ;
            }
        }

        public override void OnAfterDelete()
        {
            foreach (Item item in m_Fillers)
            {
                if (item != null && !item.Deleted)
                    item.Delete();
            }

            base.OnAfterDelete();
        }

        /*public override int GetMaxUpdateRange()
        {
            return 18;
        }

        public override int GetUpdateRange(Mobile m)
        {
            return 18;
        }*/

        public BaseBazaarMulti(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Fillers.Count);
            foreach (Item item in m_Fillers)
                writer.Write(item);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Fillers = new List<Item>();

            int c = reader.ReadInt();
            for (int i = 0; i < c; i++)
            {
                Item item = reader.ReadItem();

                if (item != null)
                    AddComponent(item);
            }
        }
    }

    public class CommodityStyle1 : BaseBazaarMulti
    {
        [Constructable]
        public CommodityStyle1() : base(0x1772)
        {
            Item comp = new Static(1801);
            comp.MoveToWorld(new Point3D(X + 1, Y + 1, Z), Map);
            AddComponent(comp);
        }

        public CommodityStyle1(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CommodityStyle2 : BaseBazaarMulti
    {
        [Constructable]
        public CommodityStyle2() : base(0x1773)
        {
            Item comp = new Static(9272);
            comp.MoveToWorld(new Point3D(X + 1, Y, Z), Map);
            AddComponent(comp);
        }

        public CommodityStyle2(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CommodityStyle3 : BaseBazaarMulti
    {
        [Constructable]
        public CommodityStyle3() : base(0x1774)
        {
            Item comp = new Static(16527);
            comp.MoveToWorld(new Point3D(X, Y, Z), Map);
            AddComponent(comp);
        }

        public CommodityStyle3(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PetStyle1 : BaseBazaarMulti
    {
        [Constructable]
        public PetStyle1() : base(0x1775)
        {
            Item comp = new Static(1036);
            comp.MoveToWorld(new Point3D(X - 1, Y, Z), Map);
            AddComponent(comp);
        }

        public PetStyle1(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PetStyle2 : BaseBazaarMulti
    {
        [Constructable]
        public PetStyle2() : base(0x1777)
        {
            Item comp = new Static(6013);
            comp.MoveToWorld(new Point3D(X, Y - 1, Z), Map);
            AddComponent(comp);

            comp = new Static(6013);
            comp.MoveToWorld(new Point3D(X, Y + 1, Z), Map);
            AddComponent(comp);
        }

        public PetStyle2(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PetStyle3 : BaseBazaarMulti
    {
        [Constructable]
        public PetStyle3() : base(0x177B)
        {
            Item comp = new Static(2324);
            comp.MoveToWorld(new Point3D(X - 1, Y, Z), Map);
            AddComponent(comp);
        }

        public PetStyle3(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}