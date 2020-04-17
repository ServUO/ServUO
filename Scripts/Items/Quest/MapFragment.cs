using Server.Engines.Quests;

namespace Server.Items
{
    public class MapFragment : BaseQuestItem
    {
        [Constructable]
        public MapFragment()
            : base(0x14ED)
        {
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public MapFragment(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074533;// Fragment of a Map
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