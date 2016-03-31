namespace Server.Items
{
    class WaterBarrel : BaseWaterContainer
    {
        private static readonly int vItemID = 0xe77;
        private static readonly int fItemID = 0x154d;
        [Constructable]
        public WaterBarrel()
            : this(false)
        {
        }

        [Constructable]
        public WaterBarrel(bool filled)
            : base((filled) ? WaterBarrel.fItemID : WaterBarrel.vItemID, filled)
        {
        }

        public WaterBarrel(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1025453;
            }
        }/* water barrel */
        public override int voidItem_ID
        {
            get
            {
                return vItemID;
            }
        }
        public override int fullItem_ID
        {
            get
            {
                return fItemID;
            }
        }
        public override int MaxQuantity
        {
            get
            {
                return 100;
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