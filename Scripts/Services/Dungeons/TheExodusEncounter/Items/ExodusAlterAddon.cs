namespace Server.Items
{
    public class ExodusAlterAddon : BaseAddon
    {
        [Constructable]
        public ExodusAlterAddon()
        {
            AddComponent(0x3F9, 0, 1, 5);
            AddComponent(0x3FA, 1, 0, 5);
            AddComponent(0x3F7, 0, 0, 5);
            AddComponent(0x3F8, 1, 1, 5);
        }

        public void AddComponent(int id, int x, int y, int z)
        {
            AddonComponent ac = new AddonComponent(id)
            {
                Hue = 2702
            };
            AddComponent(ac, x, y, z);
        }

        public ExodusAlterAddon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}