using Server.Items;

namespace Server.Engines.Quests
{
    public class ALittleSomething : BaseQuest
    {
        public ALittleSomething()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(BrilliantAmber), "Brilliant Amber", 1, 0x3199));

            AddReward(new BaseReward(typeof(MeagerImbuingBag), 1, "Meager Imbuing Bag"));
        }

        public override object Title => "A Little Something";
        public override object Description => 1113773;
        public override object Refuse => 1113774;
        public override object Uncomplete => 1113775;
        public override object Complete => 1113776;
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