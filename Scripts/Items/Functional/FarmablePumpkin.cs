namespace Server.Items
{
    public class FarmablePumpkin : FarmableCrop
    {
        [Constructable]
        public FarmablePumpkin()
            : base(GetCropID())
        {
        }

        public FarmablePumpkin(Serial serial)
            : base(serial)
        {
        }

        public static int GetCropID()
        {
            return Utility.Random(3166, 3);
        }

        public override Item GetCropObject()
        {
            Pumpkin pumpkin = new Pumpkin
            {
                ItemID = Utility.Random(3178, 3)
            };

            return pumpkin;
        }

        public override int GetPickedID()
        {
            return Utility.Random(3166, 3);
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