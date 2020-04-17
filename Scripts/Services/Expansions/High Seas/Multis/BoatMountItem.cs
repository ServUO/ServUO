using Server.Mobiles;

namespace Server.Multis
{
    public class BoatMountItem : Item, IMountItem
    {
        private BaseBoat m_Mount;
        public IMount Mount => m_Mount;

        public BoatMountItem(BaseBoat mount) : base(0x3E96)
        {
            Layer = Layer.Mount;

            Movable = false;
            m_Mount = mount;
        }

        public BoatMountItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Mount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Mount = reader.ReadItem() as BaseBoat;

            if (m_Mount == null)
                Delete();
            else
                Internalize();
        }
    }
}
