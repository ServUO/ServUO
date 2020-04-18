using Server.Items;

namespace Server.Engines.Quests
{
    public class KnowledgeoftheSoulforge : BaseQuest
    {
        public KnowledgeoftheSoulforge()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(EnchantedEssence), "Enchanted Essence", 50, 0x2DB2));

            AddReward(new BaseReward(typeof(ScrollBox), 1, "Knowledge"));
        }

        public override object Title => "Knowledge of the Soulforge";
        public override object Description => 1112526;
        public override object Refuse => 1112546;
        public override object Uncomplete => 1112547;
        public override object Complete => 1112527;
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