namespace Server.Engines.TombOfKings
{
    public class ChamberSwitch : Item
    {
        private readonly Chamber m_Chamber;

        public ChamberSwitch(Chamber chamber, Point3D loc, int itemId)
            : base(itemId)
        {
            m_Chamber = chamber;

            Movable = false;
            MoveToWorld(loc, Map.TerMur);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!m_Chamber.IsOpened() && from.InRange(this, 1))
            {
                Effects.PlaySound(Location, Map, 0x3E8);

                switch (ItemID)
                {
                    case 0x108F: ItemID = 0x1090; break;
                    case 0x1090: ItemID = 0x108F; break;
                    case 0x1091: ItemID = 0x1092; break;
                    case 0x1092: ItemID = 0x1091; break;
                }

                m_Chamber.Open();
            }
        }

        public ChamberSwitch(Serial serial)
            : base(serial)
        {
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

            Delete();
        }
    }
}