namespace Server.Items
{
    public class PeerlessKey : BaseDecayingItem
    {
        private Map m_Map;

        [CommandProperty(AccessLevel.GameMaster)]
        public Map PeerlessMap
        {
            get { return m_Map; }
            set
            {
                m_Map = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public PeerlessKey(int itemID)
            : base(itemID)
        {
            LootType = LootType.Blessed;
        }

        public PeerlessKey(Serial serial)
            : base(serial)
        {
        }

        public override int Lifespan => 604800;
        public override bool UseSeconds => false;

        public override void AddItemSocketProperties(ObjectPropertyList list)
        {
            if (PeerlessMap != null)
            {
                if (PeerlessMap == Map.Felucca)
                    list.Add(1012001); // Felucca
                else if (PeerlessMap == Map.Trammel)
                    list.Add(1012000); // Trammel
                else if (PeerlessMap == Map.Ilshenar)
                    list.Add(1012002); // Ilshenar
                else if (PeerlessMap == Map.Malas)
                    list.Add(1060643); // Malas
                else if (PeerlessMap == Map.Tokuno)
                    list.Add(1063258); // Tokuno Islands
            }

            base.AddItemSocketProperties(list);
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
