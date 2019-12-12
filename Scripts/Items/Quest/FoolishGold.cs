using System;
using Server;


namespace Server.Items
{
    public class NiporailemsTreasure : Item
    {
        public override int LabelNumber { get { return ItemID == 0x11EA ? 1112115 : 1112113; } }   //  Niporailem's Treasure : Treasure Sand

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Decays { get { return Link != null && !Link.Deleted ? base.Decays : true; } }

        [CommandProperty(AccessLevel.Decorator)]
        public override TimeSpan DecayTime { get { return TimeSpan.FromMinutes(15); } }

        [CommandProperty(AccessLevel.Decorator)]
        public Mobile Link { get; set; }

        public NiporailemsTreasure(Mobile link)
            : base(0xEEF)
        {
            Link = link;
            Weight = 100.0;
        }

        public NiporailemsTreasure(Serial serial) : base(serial)
        {
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool convert = base.DropToWorld(from, p);

            if (convert)
                ConvertItem(from);

            return convert;
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            bool convert = base.DropToMobile(from, target, p);

            if (convert)
                ConvertItem(from);

            return convert;
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            bool convert = base.DropToItem(from, target, p);

            if (convert && Parent != from.Backpack)
                ConvertItem(from);

            return convert;
        }

        public virtual void ConvertItem(Mobile from)
        {
            from.SendLocalizedMessage(1112112); // To carry the burden of greed!

            ItemID = 0x11EA;
            Weight = 25.0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write(Link);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Link = reader.ReadMobile();
        }
    }
}
