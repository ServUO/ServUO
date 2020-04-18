namespace Server.Items
{
    public class AcidWall1 : BaseWall
    {
        [Constructable]
        public AcidWall1()
            : base(578)
        {
        }

        public AcidWall1(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(1111659); // You try to examine the strange wall but the vines get in your way.
            }
            else if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(1019045); // I can't reach that.
            }
            base.OnDoubleClick(from);
        }

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
    }
}