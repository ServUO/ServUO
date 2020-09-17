namespace Server.Items
{
    public class FarmableTurnip : FarmableCrop
    {
        [Constructable]
        public FarmableTurnip()
            : base(GetCropID())
        {
        }

        public FarmableTurnip(Serial serial)
            : base(serial)
        {
        }

        public static int GetCropID()
        {
            return Utility.Random(3169, 3);
        }

        public override Item GetCropObject()
        {
            Turnip turnip = new Turnip
            {
                ItemID = Utility.Random(3385, 2)
            };

            return turnip;
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