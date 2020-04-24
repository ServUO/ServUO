namespace Server.Engines.Quests.Ambitious
{
    public class DontOfferConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The Solen queen considers you for a moment then says,</I><BR><BR>
* 
* Hmmm... I could perhaps benefit from your assistance, but you seem to be
* busy with another task at the moment. Return to me when you complete whatever
* it is that you're working on and maybe I can still put you to good use.
*/
                1054059;
        public override bool Logged => false;
    }

    public class AcceptConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The Solen queen smiles as you decide to help her.</I><BR><BR>
* 
* Excellent. We'll worry about the zoogi fungus later - start by eliminating
* 5 queens from my colony.<BR><BR>That part's important, by the way; they must
* be queens from my colony. Killing queens from the other solen colony does
* little to help me become Matriarch of this colony and will not count
* toward your task.<BR><BR>
* 
* Oh, and none of those nasty infiltrator queens either. They perform a necessary
* duty, I suppose, spying on the other colony. I fail to see why that couldn't be
* left totally to the warriors, though. Nevertheless, they do not count as well.<BR><BR>
* 
* Very well. Carry on. I'll be waiting for your return.
*/
                1054061;
        public override void OnRead()
        {
            System.AddObjective(new KillQueensObjective());
        }
    }

    public class DuringKillQueensConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The Solen queen looks up as you approach.</I><BR><BR>
* 
* You're back, but you have not yet eliminated 5 queens from my colony.
* Return when you have completed this task.<BR><BR>
* 
* Remember, by the way, that queens from the other solen colony and
* infiltrator queens do not count toward your task.<BR><BR>
* 
* Very well. Carry on. I'll be waiting for your return.
*/
                1054066;
        public override bool Logged => false;
    }

    public class GatherFungiConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The Solen queen looks pleased to see you.</I><BR><BR>
* 
* Splendid! You've done quite well in reducing my competition to become
* the next Matriarch. Now I must ask that you gather some zoogi fungus for me.
* I must practice processing it into powder of translocation.<BR><BR>
* 
* I believe the amount we agreed upon earlier was 50. Please return when
* you have that amount and then give them to me.<BR><BR>
* 
* Farewell for now.
*/
                1054068;
        public override void OnRead()
        {
            System.AddObjective(new GatherFungiObjective());
        }
    }

    public class DuringFungiGatheringConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The Solen queen looks up as you approach.</I><BR><BR>
* 
* Do you have the zoogi fungus?<BR><BR>
* 
* If so, give them to me. Otherwise, go gather some and then return to me.
*/
                1054070;
        public override bool Logged => false;
    }

    public class EndConversation : QuestConversation
    {
        public override object Message =>
                /* <I>The Solen queen smiles as she takes the zoogi fungus from you.</I><BR><BR>
* 
* Wonderful! I greatly appreciate your help with these tasks. My plans are beginning
* to take shape ensuring that I will be the next Matriarch. But there is still
* much to be done until then.<BR><BR>
* 
* You've done what I've asked of you and for that I thank you. Please accept this
* bag of sending and some powder of translocation as a reward. Oh, and I suppose
* I should give you some gold as well. Yes, yes. Of course.
*/
                1054073;
        public override void OnRead()
        {
            bool bagOfSending = true;
            bool powderOfTranslocation = true;
            bool gold = true;

            AmbitiousQueenQuest.GiveRewardTo(System.From, ref bagOfSending, ref powderOfTranslocation, ref gold);

            if (!bagOfSending && !powderOfTranslocation && !gold)
            {
                System.Complete();
            }
            else
            {
                System.AddConversation(new FullBackpackConversation(true, bagOfSending, powderOfTranslocation, gold));
            }
        }
    }

    public class FullBackpackConversation : QuestConversation
    {
        private readonly bool m_Logged;
        private bool m_BagOfSending;
        private bool m_PowderOfTranslocation;
        private bool m_Gold;
        public FullBackpackConversation(bool logged, bool bagOfSending, bool powderOfTranslocation, bool gold)
        {
            m_Logged = logged;

            m_BagOfSending = bagOfSending;
            m_PowderOfTranslocation = powderOfTranslocation;
            m_Gold = gold;
        }

        public FullBackpackConversation()
        {
            m_Logged = true;
        }

        public override object Message =>
                /* <I>The Solen queen looks at you with a smile.</I><BR><BR>
* 
* While I'd like to finish conducting our business, it seems that you're a
* bit overloaded with equipment at the moment.<BR><BR>
* 
* Perhaps you should free some room in your backpack before we proceed.
*/
                1054077;
        public override bool Logged => m_Logged;
        public override void OnRead()
        {
            if (m_Logged)
                System.AddObjective(new GetRewardObjective(m_BagOfSending, m_PowderOfTranslocation, m_Gold));
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            m_BagOfSending = reader.ReadBool();
            m_PowderOfTranslocation = reader.ReadBool();
            m_Gold = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(m_BagOfSending);
            writer.Write(m_PowderOfTranslocation);
            writer.Write(m_Gold);
        }
    }

    public class End2Conversation : QuestConversation
    {
        public override object Message =>
                /* <I>The Solen queen looks up as you approach.</I><BR><BR>
* 
* Ah good, you've returned. I will conclude our business by giving you any
* remaining rewards I owe you for aiding me.
*/
                1054078;
        public override void OnRead()
        {
            System.Complete();
        }
    }
}
