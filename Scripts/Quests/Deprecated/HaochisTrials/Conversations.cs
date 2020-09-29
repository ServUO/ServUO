namespace Server.Engines.Quests.Samurai
{
    public class GainKarmaConversation : QuestConversation
    {
        public GainKarmaConversation()
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

    public class SecondTrialIntroConversation : QuestConversation
    {
        public SecondTrialIntroConversation()
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

    public class ThirdTrialIntroConversation : QuestConversation
    {
        public ThirdTrialIntroConversation()
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

    public class FifthTrialIntroConversation : QuestConversation
    {
        public FifthTrialIntroConversation()
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

    public class SixthTrialIntroConversation : QuestConversation
    {
        public SixthTrialIntroConversation()
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
