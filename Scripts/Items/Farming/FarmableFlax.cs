using System;

namespace Server.Items
{
    public class FarmableFlax : FarmableCrop
    {
        [Constructable]
        public FarmableFlax()
            : base(GetCropID())
        {
        }

        public FarmableFlax(Serial serial)
            : base(serial)
        {
        }

        public static int GetCropID()
        {
            return Utility.Random(6809, 3);
        }

        public override Item GetCropObject()
        {
            Flax flax = new Flax();

            flax.ItemID = Utility.Random(6812, 2);

            return flax;
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