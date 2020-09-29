using System;

namespace Server.Engines.Quests.Samurai
{
    public class HaochisTrialsQuest : QuestSystem
    {
        public HaochisTrialsQuest()
        { }

        public override object Name => 1063022; // Haochi's Trials
        public override object OfferMessage => 1063023;
        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;
        public override int Picture => 0x15D7;

        public override void ChildDeserialize(GenericReader reader)
        {
            reader.ReadEncodedInt();

            reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);

            writer.Write(false);
        }
    }
}
