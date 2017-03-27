using System;

namespace Server.Items
{
    public class EssenceBox : WoodenBox
    {
        [Constructable]
        public EssenceBox()
            : base()
        {
            this.Movable = true;
            this.Hue = 2306;

           this.DropItem(Loot.RandomEssence());                   
        }

        public EssenceBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113770;
            }
        }//Essence Box
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