namespace Server.Items
{
    public class FarmableCarrot : FarmableCrop
    {
        [Constructable]
        public FarmableCarrot()
            : base(GetCropID())
        {
        }

        public FarmableCarrot(Serial serial)
            : base(serial)
        {
        }

        public static int GetCropID()
        {
            return 3190;
        }

        public override Item GetCropObject()
        {
            Carrot carrot = new Carrot
            {
                ItemID = Utility.Random(3191, 2)
            };

            return carrot;
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