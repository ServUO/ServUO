using Server.Mobiles;

namespace Server.Items
{
    public class OracleOfTheSea : Spyglass
    {
        public static readonly int MaxUses = 5;

        public override int LabelNumber => 1150184;

        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining { get { return m_UsesRemaining; } set { m_UsesRemaining = value; InvalidateProperties(); } }

        [Constructable]
        public OracleOfTheSea()
        {
            Hue = 1265;
            Weight = 3.0;
            m_UsesRemaining = 5;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060728, "{0}\t{1}", m_UsesRemaining.ToString(), MaxUses.ToString());
            list.Add(1150207, "#{0}\t", 1150208);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_UsesRemaining <= 0 || !IsChildOf(from.Backpack))
                base.OnDoubleClick(from);

            if (CharydbisSpawner.SpawnInstance != null && CharydbisSpawner.SpawnInstance.TrySpawnCharybdis(from))
                UsesRemaining--;
        }

        public OracleOfTheSea(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_UsesRemaining = reader.ReadInt();
        }
    }
}