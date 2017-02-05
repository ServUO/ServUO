using System;

namespace Server.Items
{
    public class ScrollBox3 : WoodenBox // Secrets of the Soulforge Reward Item
    {
        [Constructable]	
        public ScrollBox3()
            : base()
        {
            this.Movable = true;
            this.Hue = 1159;

            if (Utility.RandomBool())
            {
                this.DropItem(new PowerScroll(SkillName.Imbuing, 110.0));
            }
            else
            {
                this.DropItem(new PowerScroll(SkillName.Imbuing, 105.0));
            }
        }

        public ScrollBox3(Serial serial)
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
    }
}
