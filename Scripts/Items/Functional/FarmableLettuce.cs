using System;

namespace Server.Items
{
    public class FarmableLettuce : FarmableCrop
    {
        [Constructable]
        public FarmableLettuce()
            : base(GetCropID())
        {
        }

        public FarmableLettuce(Serial serial)
            : base(serial)
        {
        }

        public static int GetCropID()
        {
            return 3254;
        }

        public override Item GetCropObject()
        {
            Lettuce lettuce = new Lettuce();

            lettuce.ItemID = Utility.Random(3184, 2);

            return lettuce;
        }

        public override int GetPickedID()
        {
            return 3254;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}