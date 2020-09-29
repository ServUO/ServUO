using System;

namespace Server.Engines.Quests.Haven
{
    public class UzeraanTurmoilQuest : QuestSystem
    {
        public UzeraanTurmoilQuest()
        { }

        public override object Name => 1049007; // "Uzeraan's Turmoil"
        public override object OfferMessage => 1049008;
        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;
        public override int Picture => 0x15C9; // warrior

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
