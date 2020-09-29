namespace Server.Engines.Quests.Haven
{
    public class KillHordeMinionsObjective : QuestObjective
    {
        public KillHordeMinionsObjective()
        { }

        public override object Message => 0;

        public override void ChildDeserialize(GenericReader reader)
        {
            reader.ReadEncodedInt();

            reader.ReadEncodedInt();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);

            writer.WriteEncodedInt(0);
        }
    }

    public class GetDaemonBloodObjective : QuestObjective
    {
        public GetDaemonBloodObjective()
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

    public class GetDaemonBoneObjective : QuestObjective
    {
        public GetDaemonBoneObjective()
        { }

        public override object Message => 0;

        public override void ChildDeserialize(GenericReader reader)
        {
            reader.ReadEncodedInt();

            reader.ReadInt();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);

            writer.Write(0);
        }
    }
}
