namespace Server.Items
{
    public class BrassOrrery : Item
    {
        public override int LabelNumber => 1125363;  // orrery

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get; set; }

        [Constructable]
        public BrassOrrery()
            : base(0xA17C)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                ToggleActivation(m);
            }
        }

        public void ToggleActivation(Mobile m)
        {
            if (Active)
            {
                ItemID = 0xA17C;
                m.PlaySound(0x1E2);

                Active = false;
            }
            else
            {
                ItemID = 0xA17B;
                m.PlaySound(480);

                Active = true;
            }
        }

        public BrassOrrery(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(Active);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Active = reader.ReadBool();
        }
    }
}
