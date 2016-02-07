using System;

namespace Server.Items
{
    public class WamapsBoneEarrings : GoldEarrings
    {
        [Constructable]
        public WamapsBoneEarrings()
        {
            Hue = 2955;
            //Myrmidex Drone protection; +40%
            //Durability 255/255
        }

        public WamapsBoneEarrings(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1156132;
            }
        }// Wamap's Bone Earrings

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