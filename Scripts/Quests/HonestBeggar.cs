using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class HonestBeggarQuest : BaseQuest
    {
        public HonestBeggarQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(ReginasRing), "regina's ring", 1, typeof(Regina), "Regina"));

            AddReward(new BaseReward(1075394)); // Find the ring’s owner.
        }

        public override QuestChain ChainID => QuestChain.HonestBeggar;
        public override Type NextQuest => typeof(ReginasThanksQuest);
        public override bool DoneOnce => true;
        /* Honest Beggar */
        public override object Title => 1075392;
        /* Beg pardon, sir. I mean, madam. Uh, can I ask a favor of you? I found this jeweled ring.  Most people would sell 
        it and keep the money, but not me. I ain't never stole nothing, and I ain't about to start. I tried to take it over 
        to Brit castle, figgerin' it must belong to some highborn lady, but the guards threw me out. You look like they might 
        let you pass. Will you take the ring over there and see if you can find the owner? */
        public override object Description => 1075393;
        /* I see. Too good to help an honest beggar like me, eh? */
        public override object Refuse => 1075395;
        /* A jewel like this must be worth a lot, so it must belong to some noble or another. I would show it around the castle. 
        Someone’s bound to recognize it. */
        public override object Uncomplete => 1075396;
        /* Didst thou find my ring? I thank thee very much! It is an old ring, and a gift from my husband. I was most distraught 
        when I realized it was missing. */
        public override object Complete => 1075397;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ReginasThanksQuest : BaseQuest
    {
        public ReginasThanksQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(ReginasLetter), "regina's letter", 1, typeof(Evan), "Evan"));

            AddReward(new BaseReward(typeof(TransparentHeart), 1075400));
        }

        public override QuestChain ChainID => QuestChain.HonestBeggar;
        public override bool DoneOnce => true;
        /* Regina’s Thanks */
        public override object Title => 1075398;
        /* What’s that you say? It was a humble beggar that found my ring? Such honesty must be rewarded. Here, take this packet 
        and return it to him, and I will be in your debt. */
        public override object Description => 1075399;
        /* Hmph. Very well. What did you say his name was? */
        public override object Refuse => 1075401;
        /* Take the packet and return it to the beggar who found my ring. */
        public override object Uncomplete => 1075402;
        /* What? For me? Let me see . . . these sapphire earrings are for you, it says. Oh, she wants to offer me a job! This is 
        the most wonderful thing that ever happened to me!  */
        public override object Complete => 1075403;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}