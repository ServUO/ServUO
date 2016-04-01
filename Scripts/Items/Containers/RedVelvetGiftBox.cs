using System;

/*
* Simply add this box with param true to create the entire valentine's 2007 package.
* Adding it with no params or false will create an empty box.
*/
namespace Server.Items
{
    public class RedVelvetGiftBox : BaseContainer
    {
        [Constructable]
        public RedVelvetGiftBox()
            : this(false)
        {
        }

        [Constructable]
        public RedVelvetGiftBox(bool fill)
            : base(0xE7A)
        {
            this.Hue = 0x20;

            if (fill)
            {
                for (int i = 0; i < 5; i++)
                {
                    this.AddToBox(new ValentinesCardSouth(), new Point3D(60 + (i * 10), 47, 0));
                    this.AddToBox(new ValentinesCardEast(), new Point3D(20 + (i * 10), 72, 0));
                }
                this.AddToBox(new Bacon(), new Point3D(90, 85, 0));
                this.AddToBox(new RoseInAVase(), new Point3D(130, 55, 0));
            }
        }

        public RedVelvetGiftBox(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID
        {
            get
            {
                return 0x3f;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1077596;
            }
        }// A Red Velvet Box
        public virtual void AddToBox(Item item, Point3D loc)
        {
            this.DropItem(item);
            item.Location = loc;
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