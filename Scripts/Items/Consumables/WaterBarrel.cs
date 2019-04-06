namespace Server.Items
{
    class WaterBarrel : BaseWaterContainer, IChopable
    {
        private static readonly int EmptyID = 0xE77;
        private static readonly int FilledID = 0x154D;

        [Constructable]
        public WaterBarrel()
            : this(false)
        {
        }

        [Constructable]
        public WaterBarrel(bool filled)
            : base(filled ? FilledID : EmptyID, filled)
        {
        }

        public WaterBarrel(Serial serial)
            : base(serial)
        {
        }

        public void OnChop(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2) && !IsLockedDown && !IsSecure && (RootParent == null || RootParent == from))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                Delete();
            }
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
                return EmptyID;
            }
        }
        public override int fullItem_ID
        {
            get
            {
                return FilledID;
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