using System;

namespace Server.Engines.Quests.Matriarch
{
    public class DontOfferConversation : QuestConversation
    {
        private bool m_Friend;
        public DontOfferConversation(bool friend)
        {
            this.m_Friend = friend;
        }

        public DontOfferConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.m_Friend)
                {
                    /* <I>The Solen Matriarch smiles as you greet her.</I><BR><BR>
                    * 
                    * It is good to see you again. I would offer to process some zoogi fungus for you,
                    * but you seem to be busy with another task at the moment. Perhaps you should
                    * finish whatever is occupying your attention at the moment and return to me once
                    * you're done.
                    */
                    return 1054081;
                }
                else
                {
                    /* <I>The Solen Matriarch smiles as she eats the seed you offered.</I><BR><BR>
                    * 
                    * Thank you for that seed. It was quite delicious.  <BR><BR>
                    * 
                    * I would offer to make you a friend of my colony, but you seem to be busy with
                    * another task at the moment. Perhaps you should finish whatever is occupying
                    * your attention at the moment and return to me once you're done.
                    */
                    return 1054079;
                }
            }
        }
        public override bool Logged
        {
            get
            {
                return false;
            }
        }
        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_Friend = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_Friend);
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
                /* <I>The Solen Matriarch looks pleased that you've accepted.</I><BR><BR>
                * 
                * Very good. Please start by hunting some infiltrators from the other solen
                * colony and eliminating them. Slay 7 of them and then return to me.<BR><BR>
                * 
                * Farewell for now and good hunting.
                */
                return 1054084;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new KillInfiltratorsObjective());
        }
    }

    public class DuringKillInfiltratorsConversation : QuestConversation
    {
        public DuringKillInfiltratorsConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Solen Matriarch looks up as you approach.</I><BR><BR>
                * 
                * You're back, but you have not yet eliminated 7 infiltrators from the enemy
                * colony. Return when you have completed this task.<BR><BR>
                * 
                * Carry on. I'll be waiting for your return.
                */
                return 1054089;
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

    public class GatherWaterConversation : QuestConversation
    {
        public GatherWaterConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Solen Matriarch nods favorably as you approach her.</I><BR><BR>
                * 
                * Marvelous! I'm impressed at your ability to hunt and kill enemies for me.
                * My colony is thankful.<BR><BR>
                * 
                * Now I must ask that you gather some water for me. A standard pitcher of water
                * holds approximately one gallon. Please decant 8 gallons of fresh water
                * into our water vats.<BR><BR>
                * 
                * Farewell for now.
                */
                return 1054091;
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new GatherWaterObjective());
        }
    }

    public class DuringWaterGatheringConversation : QuestConversation
    {
        public DuringWaterGatheringConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Solen Matriarch looks up as you approach.</I><BR><BR>
                * 
                * You're back, but you have not yet gathered 8 gallons of water. Return when
                * you have completed this task.<BR><BR>
                * 
                * Carry on. I'll be waiting for your return.
                */
                return 1054094;
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

    public class ProcessFungiConversation : QuestConversation
    {
        private bool m_Friend;
        public ProcessFungiConversation(bool friend)
        {
            this.m_Friend = friend;
        }

        public ProcessFungiConversation()
        {
        }

        public override object Message
        {
            get
            {
                if (this.m_Friend)
                {
                    /* <I>The Solen Matriarch listens as you report the completion of your
                    * tasks to her.</I><BR><BR>
                    * 
                    * I give you my thanks for your help, and I will gladly process some zoogi
                    * fungus into powder of translocation for you. Two of the zoogi fungi are
                    * required for each measure of the powder. I will process up to 200 zoogi fungi
                    * into 100 measures of powder of translocation.<BR><BR>
                    * 
                    * I will also give you some gold for assisting me and my colony, but first let's
                    * take care of your zoogi fungus.
                    */
                    return 1054097;
                }
                else
                {
                    /* <I>The Solen Matriarch listens as you report the completion of your
                    * tasks to her.</I><BR><BR>
                    * 
                    * I give you my thanks for your help, and I will gladly make you a friend of my
                    * solen colony. My warriors, workers, and queens will not longer look at you
                    * as an intruder and attack you when you enter our lair.<BR><BR>
                    * 
                    * I will also process some zoogi fungus into powder of translocation for you.
                    * Two of the zoogi fungi are required for each measure of the powder. I will
                    * process up to 200 zoogi fungi into 100 measures of powder of translocation.<BR><BR>
                    * 
                    * I will also give you some gold for assisting me and my colony, but first let's
                    * take care of your zoogi fungus.
                    */
                    return 1054096;
                }
            }
        }
        public override void OnRead()
        {
            this.System.AddObjective(new ProcessFungiObjective());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_Friend = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_Friend);
        }
    }

    public class DuringFungiProcessConversation : QuestConversation
    {
        public DuringFungiProcessConversation()
        {
        }

        public override object Message
        {
            get
            {
                /* <I>The Solen Matriarch smiles as you greet her.</I><BR><BR>
                * 
                * I will gladly process some zoogi fungus into powder of translocation for you.
                * Two of the zoogi fungi are required for each measure of the powder.
                * I will process up to 200 zoogi fungi into 100 measures of powder of translocation.
                */
                return 1054099;
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

    public class FullBackpackConversation : QuestConversation
    {
        private readonly bool m_Logged;
        public FullBackpackConversation(bool logged)
        {
            this.m_Logged = logged;
        }

        public FullBackpackConversation()
        {
            this.m_Logged = true;
        }

        public override object Message
        {
            get
            {
                /* <I>The Solen Matriarch looks at you with a smile.</I><BR><BR>
                * 
                * While I'd like to finish conducting our business, it seems that you're a
                * bit overloaded with equipment at the moment.<BR><BR>
                * 
                * Perhaps you should free some room in your backpack before we proceed.
                */
                return 1054102;
            }
        }
        public override bool Logged
        {
            get
            {
                return this.m_Logged;
            }
        }
        public override void OnRead()
        {
            if (this.m_Logged)
                this.System.AddObjective(new GetRewardObjective());
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
                /* <I>The Solen Matriarch smiles as you greet her.</I><BR><BR>
                * 
                * Ah good, you've returned. I will conclude our business by giving you
                * gold I owe you for aiding me.
                */
                return 1054101;
            }
        }
        public override void OnRead()
        {
            this.System.Complete();
        }
    }
}