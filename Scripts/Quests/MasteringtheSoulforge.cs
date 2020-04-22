using Server.Items;

namespace Server.Engines.Quests
{
    public class MasteringtheSoulforge : BaseQuest
    {
        public MasteringtheSoulforge()
        {
            AddObjective(new ObtainObjective(typeof(RelicFragment), "Relic Fragments", 50, 0x2DB3));

            AddReward(new BaseReward(typeof(ScrollBox2), "Knowledge"));
        }

        public override object Title => "Mastering the Soulforge";
        public override object Description => 1112529;
        public override object Refuse => 1112549;
        public override object Uncomplete => 1112550;
        public override object Complete => 1112551;
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