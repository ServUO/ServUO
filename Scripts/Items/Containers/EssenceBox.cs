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

            switch (Utility.Random(11))
            {
                case 0:
                    this.DropItem(new EssencePrecision());
                    break;
                case 1:
                    this.DropItem(new EssenceAchievement());
                    break;
                case 2:
                    this.DropItem(new EssenceBalance());
                    break;
                case 3:
                    this.DropItem(new EssenceControl());
                    break;
                case 4:
                    this.DropItem(new EssenceDiligence());
                    break;
                case 5:
                    this.DropItem(new EssenceDirection());
                    break;
                case 6:
                    this.DropItem(new EssenceFeeling());
                    break;
                case 7:
                    this.DropItem(new EssenceOrder());
                    break;
                case 8:
                    this.DropItem(new EssencePassion());
                    break;
                case 9:
                    this.DropItem(new EssencePersistence());
                    break;
                case 10:
                    this.DropItem(new EssenceSingularity());
                    break;
            }
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