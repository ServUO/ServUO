namespace Server.Items
{
    class Tub : BaseWaterContainer
    {
        private static readonly int vItemID = 0xe83;
        private static readonly int fItemID = 0xe7b;
        [Constructable]
        public Tub()
            : this(false)
        {
        }

        [Constructable]
        public Tub(bool filled)
            : base((filled) ? Tub.fItemID : Tub.vItemID, filled)
        {
        }

        public Tub(Serial serial)
            : base(serial)
        {
        }

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
                return 50;
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