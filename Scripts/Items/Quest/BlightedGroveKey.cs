namespace Server.Items
{
    public class BlightedGroveKey : MasterKey
    {
        public BlightedGroveKey()
            : base(0x21C)
        {
        }

        public BlightedGroveKey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074346;// dryad's curse
        public override int Lifespan => 600;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override bool CanOfferConfirmation(Mobile from)
        {
            if (from.Region != null && from.Region.IsPartOf("Blighted Grove"))
            {
                return base.CanOfferConfirmation(from);
            }

            if (Altar == null)
            {
                return false;
            }

            Map map = Altar.Map;

            foreach (Rectangle2D rect in _EntryLocs)
            {
                if (rect.Contains(from.Location))
                {
                    return base.CanOfferConfirmation(from);
                }
            }

            return false;
        }

        private readonly Rectangle2D[] _EntryLocs =
        {
            new Rectangle2D(574, 1630, 20, 15),
            new Rectangle2D(579, 1645, 12, 4)
        };
    }
}