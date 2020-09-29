namespace Server.Engines.Quests.Necro
{
    public class LostCallingScrollConversation : QuestConversation
    {
        public LostCallingScrollConversation()
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
