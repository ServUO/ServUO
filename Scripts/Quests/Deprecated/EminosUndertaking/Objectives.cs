namespace Server.Engines.Quests.Ninja
{
    public class SneakPastGuardiansObjective : QuestObjective
    {
        public SneakPastGuardiansObjective()
        { }

        public override object Message => 0;

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

    public class HallwayWalkObjective : QuestObjective
    {
        public HallwayWalkObjective()
        { }

        public override object Message => 0;

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
