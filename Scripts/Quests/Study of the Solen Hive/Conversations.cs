using System;

namespace Server.Engines.Quests.Naturalist
{
    public class DontOfferConversation : QuestConversation
    {
        public DontOfferConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Naturalist looks up from his scribbled notes.</I><BR><BR>
                * 
                * Greetings!<BR><BR>
                * 
                * If you're interested in helping out a scholar of some repute, I do have some
                * work that I could use some assistance with.<BR><BR>
                * 
                * You seem a little preoccupied with another task right now, however. Perhaps
                * you should finish whatever it is that has your attention at the moment and
                * return to me once you're done.
                */
                return 1054052;
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
    }

    public class AcceptConversation : QuestConversation
    {
        public AcceptConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* Ah! This is splendid news! Each time an assistant travels into the
                * Solen Hive to gather information for me, I feel as if I am one step
                * closer to some grand discovery. Though I felt the same way when I was
                * certain that Terathans had the ability to change their shape to resemble
                * various fruits and vegetables - a point on which I am certain further
                * study of the beasts will prove correct.<BR><BR>
                * 
                * In any case, I cannot thank you enough! Please return to me when you have
                * studied all the Solen Egg Nests hidden within the Solen Hive.
                */
                return 1054043;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new StudyNestsObjective());
        }
    }

    public class NaturalistDuringStudyConversation : QuestConversation
    {
        public NaturalistDuringStudyConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Naturalist looks up from his notes with a frustrated look
                * on his face.</I><BR><BR>
                * 
                * Haven't you finished the task I appointed you yet? Gah! It's so
                * difficult to find a good apprentice these days.<BR><BR>
                * 
                * Remember, you must first find an entrance to the Solen Hive. Once inside,
                * I need you to examine the Solen Egg Nests for me. When you have studied
                * all four nests, you should have enough information to earn yourself a
                * reward.<BR><BR>
                * 
                * Now go on, away with you! I have piles of notes from other more helpful
                * apprentices that I still need to study!
                */
                return 1054049;
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
    }

    public class EndConversation : QuestConversation
    {
        public EndConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Naturalist looks up from his notes with a pleased expression
                * on his face.</I><BR><BR>
                * 
                * Ah! Thank you, my goodly apprentice! These notes you have gathered will
                * no doubt assist me in my understanding of these fascinating Solen creatures.<BR><BR>
                * 
                * Now, since you've done such a fine job, I feel that I should give you a little
                * reward.<BR><BR>
                * 
                * I have a botanist friend who has discovered a strange mutation in the plants
                * she has grown. Through science and sorcery, she has managed to produce a mutant
                * strain of colored seeds the like of which no gardener has laid eyes upon.<BR><BR>
                * 
                * As a reward for your fine efforts, I present you with this strange rare seed.
                * Which reminds me, I still need to compile my notes on Solen dietary habits. They
                * are voracious seed eaters, those Solen Matriarchs!<BR><BR>
                * 
                * In any case, I must get back to my notes now. I give you my thanks once more,
                * and bid a good day to you my little apprentice! If you wish to help me out again,
                * just say the word.
                */
                return 1054050;
            }
        }
        public override void OnRead()
        {
            this.System.Complete();
        }
    }

    public class SpecialEndConversation : QuestConversation
    {
        public SpecialEndConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Naturalist looks up from his notes with an ecstatic look upon
                * his face.</I><BR><BR>
                * 
                * Oh my! These notes you've brought me - they say you have information on the
                * Secret Solen Egg Nest? I've heard many tales of this secret store of special
                * Solen Eggs, but there are many missing gaps in my notes concerning it.<BR><BR>
                * 
                * The notes you've made here will most certainly advance my understanding of
                * this mysterious breed of creatures!<BR><BR>
                * 
                * Considering the amazing effort you put into your work, I feel as if I should
                * give you something extra special as a bonus. Hrmm...<BR><BR>
                * 
                * I have a botanist friend who has discovered a strange mutation in the plants
                * she has grown. Through science and sorcery, she has managed to produce a rare
                * mutant strain of colored seeds the like of which no gardener has laid
                * eyes upon.<BR><BR>
                * 
                * I've given a few of her seeds out to various apprentices - but I usually keep
                * her very rare stock all for myself. They're quite amazing looking! However,
                * since you've done such a fine job for me, I'll present you with one of
                * these rare fire-red seeds.<BR><BR>
                * 
                * Once again, my thanks to you! Now I really must get back to studying these notes!
                * Take care, my fine apprentice, and come back if you wish to help me further!
                */
                return 1054051;
            }
        }
        public override void OnRead()
        {
            this.System.Complete();
        }
    }

    public class FullBackpackConversation : QuestConversation
    {
        public FullBackpackConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Naturalist looks at you with a friendly expression.</I><BR><BR>
                * 
                * I see you've returned with information for me. While I'd like to finish
                * conducting our business, it seems that you're a bit overloaded with
                * equipment at the moment. Perhaps you'd better free up some room before
                * we get to discussing your reward.
                */
                return 1054053;
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
    }
}