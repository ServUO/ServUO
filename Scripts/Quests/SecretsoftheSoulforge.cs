using Server.Items;

namespace Server.Engines.Quests
{
    public class SecretsoftheSoulforge : BaseQuest
    {
        public SecretsoftheSoulforge()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(MagicalResidue), "Magical Residue", 50, 0x2DB1));

            AddReward(new BaseReward(typeof(ScrollBox3), 1, "Knowledge"));
        }

        public override object Title => "Secrets of the Soulforge";
        public override object Description => 1112522;
        public override object Refuse => 1112523;
        public override object Uncomplete => 1112547;
        public override object Complete => 1112524;
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