using System;

namespace Server.Items
{
    public class ScrollBox2 : WoodenBox // Mastering the Soulforge Quest Item
    {
        [Constructable]	
        public ScrollBox2()
            : base()
        {
            this.Movable = true;
            this.Hue = 1266;

            DropItem(new PowerScroll(SkillName.Imbuing, 120.0));
        }

        public ScrollBox2(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Reward Scroll Box";
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

        private static void PlaceItemIn(Container parent, int x, int y, Item item) 
        { 
            parent.AddItem(item); 
            item.Location = new Point3D(x, y, 0); 
        }
    }
}
