using System;

namespace Server.Engines.Quests.Ninja
{
    public class EminosUndertakingQuest : QuestSystem
    {
        public EminosUndertakingQuest()
        { }

        public override object Name => 1063173; // Emino's Undertaking
        public override object OfferMessage => 1063174;
        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;
        public override int Picture => 0x15D5;

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
