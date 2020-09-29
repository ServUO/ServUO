namespace Server.Engines.Quests.Samurai
{
    public class FirstTrialKillObjective : QuestObjective
    {
        public FirstTrialKillObjective()
        { }

        public override object Message => 0;

        public override void ChildDeserialize(GenericReader reader)
        {
            reader.ReadEncodedInt();

            reader.ReadEncodedInt();
            reader.ReadEncodedInt();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0);

            writer.WriteEncodedInt(0);
            writer.WriteEncodedInt(0);
        }
    }

    public class FirstTrialReturnObjective : QuestObjective
    {
        public FirstTrialReturnObjective()
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

    public class SecondTrialReturnObjective : QuestObjective
    {
        public SecondTrialReturnObjective()
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

    public class FourthTrialReturnObjective : QuestObjective
    {
        public FourthTrialReturnObjective()
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

    public class FifthTrialIntroObjective : QuestObjective
    {
        public FifthTrialIntroObjective()
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
