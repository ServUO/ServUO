namespace Server.Engines.Quests.Necro
{
    public class FindCallingScrollObjective : QuestObjective
    {
        public FindCallingScrollObjective()
        { }

        public override object Message => 0;

        public override void ChildDeserialize(GenericReader reader)
        {
            reader.ReadEncodedInt();

            reader.ReadEncodedInt();
            reader.ReadBool();
            reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);

            writer.WriteEncodedInt(0);
            writer.Write(false);
            writer.Write(false);
        }
    }

    public class FindMardothEndObjective : QuestObjective
    {
        public FindMardothEndObjective()
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
