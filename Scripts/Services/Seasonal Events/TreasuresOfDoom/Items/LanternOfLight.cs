namespace Server.Items
{
    public class LanternOfLight : Lantern
    {
        private string _OwnerName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string OwnerName { get { return _OwnerName; } set { _OwnerName = value; InvalidateProperties(); } }

        [Constructable]
        public LanternOfLight()
        {
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1155597, _OwnerName); // ~1_NAME~'s Lantern of Light
        }

        public LanternOfLight(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_OwnerName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            _OwnerName = reader.ReadString();
        }
    }
}